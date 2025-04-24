using System;
using System.Collections;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform target;
    [SerializeField] private Transform camStart;
    [SerializeField] private TMP_InputField lobbyIdText;
    [SerializeField] private Toggle useSteam;
    [SerializeField] private float zoomTime;
    [SerializeField] private float breathingOffset;
    [SerializeField] private float breathingSpeed;
    
    protected Callback<LobbyEnter_t> LobbyEnter;
    protected Callback<GameLobbyJoinRequested_t> LobbyJoinRequested;

    private float _initialFOV;
    
    private bool joining;

    private bool _hosting;

    private DateTime initTime;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _initialFOV = cam.fieldOfView;
        
        LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
        LobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
    }

    private void Start()
    {
        LoadingScreen.Loaded();
    }

    private void Update()
    {
        if (joining)
        {
            float progress = (float)(DateTime.Now - initTime).TotalSeconds / zoomTime;
            
            cam.transform.position = Vector3.Lerp(camStart.position, target.position, progress);
            canvasGroup.alpha = Mathf.Lerp(1, 0, progress);
        }
        
        BreathingEffect();
    }
    
    float _fovOffset = 0;
    private bool _forwards;

    private void BreathingEffect()
    {
        if (_fovOffset >= breathingOffset)
        {
            _forwards = false;
        }
        else if (_fovOffset <= -breathingOffset)
        {
            _forwards = true;
        }

        if (_forwards)
        {
            _fovOffset += breathingSpeed * Time.deltaTime;
        }
        else
        {
            _fovOffset -= breathingSpeed * Time.deltaTime;
        }

        cam.fieldOfView = _initialFOV + _fovOffset;
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        if (NetworkManager.Instance.Server == null)
        {
            NetworkManager.Instance.SetLobbyId((CSteamID)callback.m_ulSteamIDLobby);

            StartMoving();
        }
    }

    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        if (NetworkManager.Instance.Server == null)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }
    }

    private void StartMoving()
    {
        canvasGroup.interactable = false;
        initTime = DateTime.Now;
        StartCoroutine(MovingJoinTimer());
        joining = true;
    }

    private IEnumerator MovingJoinTimer()
    {
        yield return new WaitForSeconds(zoomTime);
        JoinGame();
    }

    private Scene _currentScene;

    private void JoinGame()
    {
        Debug.Log($"Hosting game: {_hosting}");
        NetworkManager.Instance.RegisterInit(_hosting, useSteam.isOn);
        StartCoroutine(LoadAsynchronously("Game"));

        _currentScene = SceneManager.GetActiveScene();
    }

    IEnumerator LoadAsynchronously(string levelToLoad)
    {
        // Set the loading of the level as an async operation
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Single);

        while(!operation.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync(_currentScene);
    }

    public void StartHost()
    {
        if (useSteam.isOn)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
        }
        else
        {
            StartMoving();
        }
        _hosting = true;
    }

    public void StartJoin()
    {
        if (useSteam.isOn)
        {
            SteamMatchmaking.JoinLobby((CSteamID)ulong.Parse(lobbyIdText.text));
        }
        else
        {
            StartMoving();
        }
        _hosting = false;
    }

    private void OnDisable()
    {
        LobbyEnter.Dispose();
        LobbyJoinRequested.Dispose();
    }

    public void Quit()
    {
        Application.Quit();
    }
}

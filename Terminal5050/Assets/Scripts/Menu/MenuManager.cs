using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private Transform target;
    [SerializeField] private Transform camStart;
    [SerializeField] private float zoomTime;
    
    private bool joining;

    private DateTime initTime;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (joining)
        {
            float progress = (float)(DateTime.Now - initTime).TotalSeconds / zoomTime;
            
            cam.position = Vector3.Lerp(camStart.position, target.position, progress);
        }
    }

    private void StartMoving()
    {
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
        NetworkManager.Instance.RegisterInit(true);
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
        StartMoving();
    }

    public void StartJoin()
    {
        StartMoving();
    }
}

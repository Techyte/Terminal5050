using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPauseManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private CameraController camController;
    [SerializeField] private GameObject pausedUI;

    private bool _paused;

    public bool Paused => _paused;

    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Start()
    {
        pausedUI.SetActive(false);
    }

    private void Update()
    {
        if (!_player.local)
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePaused();
        }
        
        pausedUI.SetActive(_paused);
    }

    private void TogglePaused()
    {
        SetPaused(!_paused);
    }

    public void SetPaused(bool newPaused)
    {
        _paused = newPaused;

        if (_paused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            movement.cancel = true;
            camController.cancel = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            movement.cancel = false;
            camController.cancel = false;
        }
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SetPaused(true);
        }
    }
}

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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

    private void JoinGame()
    {
        Debug.Log("loading game");
        SceneManager.LoadScene("Game");
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

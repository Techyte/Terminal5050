using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    [SerializeField] private List<GameObject> dontDestroyObjects;
    [SerializeField] private string sceneToLoad;

    private void Awake()
    {
        foreach (var obj in dontDestroyObjects)
        {
            DontDestroyOnLoad(obj);
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}

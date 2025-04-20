using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    private static LoadingScreen Instance;

    [SerializeField] private GameObject obj;
    
    private void Awake()
    {
        Instance = this;
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Instance.obj.SetActive(true);
    }

    public static void Loaded()
    {
        Instance.obj.SetActive(false);
    }
}

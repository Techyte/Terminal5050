using System;
using UnityEngine;

public class DirectoryManager : MonoBehaviour
{
    public static DirectoryManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CMDManager.Instance.OnChoiceSelected += OnChoiceSelected;
    }

    private void OnChoiceSelected(object sender, int e)
    {
        // were we the one that called for the choice
        if (sender is DirectoryManager)
        {
            Debug.Log($"Director manager called the choice of {e}");
        }
    }

    public void DirCommand()
    {
        string[] dirs = new[]
        {
            "Documents",
            "Work",
            "Music",
            "Pictures",
            "Images"
        };
        CMDManager.Instance.OutputChoice(dirs, this);
    }
}

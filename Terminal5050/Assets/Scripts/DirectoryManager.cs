using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DirectoryManager : MonoBehaviour
{
    [SerializeField] private AudioSource beep;
    [SerializeField] private AudioSource unhappyBeep;
    [SerializeField] private string backString = "<color=yellow>BACK</color>";
    [SerializeField] private string lockedString = "#locked";
    [SerializeField] private AudioSource error;
    [SerializeField] private AudioClip errorClip;

    private string _currentPath;
    private string CurrentPathTrunc => _currentPath.Substring(_currentPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
    private string _root;

    private void Awake()
    {
        _currentPath = Application.persistentDataPath+"/Directories";
        _root = Application.persistentDataPath+"/Directories";
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
            string selected = dirs[e];

            if (selected != backString && !selected.EndsWith(".txt") && !selected.Contains("CORE files"))
            {
                _currentPath += @"\"+dirs[e];
                OpenDirectoryScreen();
            }
            else if (selected.EndsWith(".txt"))
            {
                DisplayTxtFile(_currentPath + @"\"+dirs[e], dirs[e]);
            }
            else if (selected.Contains("CORE files"))
            {
                CMDManager.Instance.Output("<color=red>ACCESS DENIED</color>");
                OpenDirectoryScreen();
            }
            else
            {
                _currentPath = _currentPath.Remove(_currentPath.LastIndexOf(Path.DirectorySeparatorChar)); // removes the last file location
                OpenDirectoryScreen();
            }
            
            beep.Play();
        }
    }

    private void DisplayTxtFile(string path, string name)
    {
        CMDManager.Instance.StartProcess();
        CMDManager.Instance.Output($"\nReading {name}...");
        StreamReader sr = new StreamReader(path);

        string output = sr.ReadToEnd();

        if (output == lockedString)
        {
            StartCoroutine(LockedFile());
        }
        else
        {
            CMDManager.Instance.Output(output);
        }
    }

    private IEnumerator LockedFile()
    {
        CMDManager.Instance.StartProcess();
        yield return new WaitForSeconds(1);
        CMDManager.Instance.Output("<color=yellow>Retrieving file info is taking longer than expected</color>");
        yield return new WaitForSeconds(2);
        CMDManager.Instance.Output("<color=red>ERROR READING TEXT FILE</color>");
        error.PlayOneShot(errorClip);
        yield return new WaitForSeconds(1);
        CMDManager.Instance.Output("Exiting file read");
        unhappyBeep.Play();

        CMDManager.Instance.StopProcess();
        CMDManager.Instance.tBehaviour.Wake();
    }

    public void StopAll()
    {
        StopAllCoroutines();
    }

    private List<string> dirs;
    private List<string> files = new List<string>();

    public void OpenDirectoryScreen()
    {
        dirs = new List<string>(Directory.EnumerateDirectories(_currentPath));
        files = new List<string>(Directory.EnumerateFiles(_currentPath));

        List<string> f = files.ToList();
        files.Clear();

        foreach (var file in f)
        {
            if (file.EndsWith(".txt"))
            {
                files.Add(file);
            }
        }
        
        dirs.AddRange(files);
        
        for (int i = 0; i < dirs.Count; i++)
        {
            dirs[i] = dirs[i].Substring(dirs[i].LastIndexOf(Path.DirectorySeparatorChar) + 1); // removes the absolute location from the string
        }

        if (_currentPath != _root)
        {
            dirs.Add(backString);
        }

        string prompt;

        if (_currentPath == _root)
        {
            prompt = "Root";
        }
        else
        {
            prompt = CurrentPathTrunc;
        }
        
        CMDManager.Instance.OutputChoice(dirs.ToArray(), prompt, this);
    }
}

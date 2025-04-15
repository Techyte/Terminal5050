using System;
using System.Collections;
using None;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CMDManager : MonoBehaviour
{
    public static CMDManager Instance;
    
    [SerializeField] private TMP_InputField input;
    [SerializeField] private AltTMP_InputField text;
    [SerializeField] private AudioSource[] computerWhispers;
    [SerializeField] private AudioSource fadeWhisper;
    [SerializeField] private AudioSource click;
    [SerializeField] private AudioSource beep;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private Image fade;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private AudioSource[] allSources;
    public TerminalBehaviour tBehaviour;

    public string terminalName = "Terminal5050";

    [HideInInspector] public float tfWaitTime = 0.1f;

    public EventHandler<int> OnChoiceSelected;
    public EventHandler CreepyStart;
    public EventHandler CreepyEnd;

    public bool creepyOutputting;

    private int attempts;

    public bool Outputting { get; private set; }
    private bool fadeOut;

    private int currentChoiceIndex;
    private bool choosing;

    private void Awake()
    {
        Instance = this;

        text.text = $"<color=white>Welcome to {terminalName}, please enter 'help' for assistance</color>";
    }

    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            if (!Outputting && !string.IsNullOrEmpty(input.text) && !choosing)
            {
                Input();
            }
            else if (choosing)
            {
                OptionSelected(currentChoiceIndex);
            }
            input.Select();
        }

        if (UnityEngine.Input.GetKey(KeyCode.LeftControl) && UnityEngine.Input.GetKeyDown(KeyCode.C) && (Outputting || choosing))
        {
            StopAll();
            beep.Play();
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) && choosing)
        {
            currentChoiceIndex--;
            ChoiceChanged();
            UpdateChoice();
            input.Select();
            click.PlayOneShot(clickClip);
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) && choosing)
        {
            currentChoiceIndex++;
            ChoiceChanged();
            UpdateChoice();
            input.Select();
            click.PlayOneShot(clickClip);
        }

        if (fadeOut)
        {
            fade.color = Color.Lerp(fade.color, new Color(0, 0, 0, 1), fadeSpeed * Time.deltaTime);
            foreach (var source in allSources)
            {
                source.volume = Mathf.Lerp(source.volume, 0, fadeSpeed * Time.deltaTime);
            }

            if (fade.color.a > 0.99)
            {
                Application.Quit();
            }
        }
    }

    public void StopAll()
    {
        choosing = false;
        
        StopAllCoroutines();
        tBehaviour.StopAll();
        
        Output("Canceled");
        StopProcess();
    }

    public void TypedLetter()
    {
        click.PlayOneShot(clickClip);
    }

    private void Input()
    {
        string command = input.text;
        
        input.text = string.Empty;

        StartProcess();
        Output(command, true);
        StopProcess();

        StartCoroutine(TastefulWait(command));
        
        input.ActivateInputField();
        input.Select();
    }

    private IEnumerator TastefulWait(string command)
    {
        yield return new WaitForSeconds(tfWaitTime);
        tBehaviour.HandleCommand(command);
    }

    private string _originalText;
    private string _promptText;

    private string[] _options;

    private object _sender;

    public void OutputChoice(string[] options, string prompt, object sender)
    {
        StartProcess();
        
        choosing = true;
        
        currentChoiceIndex = 0;
        _options = options;
        _sender = sender;
        _promptText = prompt;

        Output("");
        
        _originalText = text.text;
        
        UpdateChoice();
        
        StopProcess();
    }

    public void OptionSelected(int index)
    {
        choosing = false;
        StopProcess();
        
        OnChoiceSelected.Invoke(_sender, index);
    }

    private void ChoiceChanged()
    {
        if (currentChoiceIndex >= _options.Length)
        {
            currentChoiceIndex = 0;
        }
        else if (currentChoiceIndex < 0)
        {
            currentChoiceIndex = _options.Length - 1;
        }
    }

    private void UpdateChoice()
    {
        text.text = _originalText;
        text.text += "\n"+_promptText;
        for (int i = 0; i < _options.Length; i++)
        {
            if (currentChoiceIndex == i)
            {
                text.text += $"\n<font=\"Terminus SDF\"><mark=#007304><color=#000000>[{i}] - {_options[i]}</color></mark></font>";
            }
            else
            {
                text.text += $"\n[{i}] - {_options[i]}";
            }
        }
    }

    public void StartProcess()
    {
        Outputting = true;
    }

    public void StopProcess()
    {
        Outputting = false;
    }

    public void Output(string output, bool arrow = false)
    {
        bool wasOutputting = Outputting;

        if (!Outputting)
        {
            StartProcess();
        }

        if (arrow == false)
        {
            output = "<color=grey>" + output + "</color>";
        }

        if (text.text == string.Empty)
        {
            text.text = arrow ? "> " + output : output;
        }
        else
        {
            text.text = arrow ? text.text + "\n" + "> " + output : text.text + "\n" + output;
        }

        if (!wasOutputting)
        {
            StopProcess();
        }
    }

    public void Output(string output, Color color)
    {
        bool wasOutputting = Outputting;

        if (!Outputting)
        {
            StartProcess();
        }

        output = $"<color=#{color.ToHexString()}>" + output + "</color>";

        if (text.text == string.Empty)
        {
            text.text = output;
        }
        else
        {
            text.text = text.text + "\n" + output;
        }

        if (!wasOutputting)
        {
            StopProcess();
        }
    }
}

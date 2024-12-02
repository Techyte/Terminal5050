using System;
using System.Collections;
using None;
using TMPro;
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
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private Image fade;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private AudioSource[] allSources;
    [SerializeField] private TerminalBehaviour tBehaviour;

    public string terminalName = "Terminal5050";

    [HideInInspector] public float tfWaitTime = 0.1f;

    public EventHandler<int> OnChoiceSelected;

    private int attempts;

    [HideInInspector] public bool outputting;
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
            if (!outputting && !string.IsNullOrEmpty(input.text))
            {
                Input();
            }
            else if (outputting && choosing)
            {
                OptionSelected(currentChoiceIndex);
            }
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) && choosing)
        {
            currentChoiceIndex--;
            ChoiceChanged();
            UpdateChoice();
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) && choosing)
        {
            currentChoiceIndex++;
            ChoiceChanged();
            UpdateChoice();
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

    public void TypedLetter()
    {
        Debug.Log("typed");
        click.PlayOneShot(clickClip);
    }

    private void Input()
    {
        string command = input.text;
        
        input.text = string.Empty;

        Output(command, true);

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

    private string[] _options;

    private object _sender;

    public void OutputChoice(string[] options, object sender)
    {
        choosing = true;
        outputting = true;
        
        currentChoiceIndex = 0;
        _options = options;
        _sender = sender;

        _originalText = text.text;
        
        UpdateChoice();
    }

    public void OptionSelected(int index)
    {
        Output($"Selected option {index} - {_options[index]}");
        
        choosing = false;
        outputting = false;
        
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
        for (int i = 0; i < _options.Length; i++)
        {
            if (currentChoiceIndex == i)
            {
                text.text += $"<font=\"Terminus\"><mark=#007304><color=#000000>\n[{i}] - {_options[i]}</color></mark></font>";
            }
            else
            {
                text.text += $"\n[{i}] - {_options[i]}";
            }
        }
    }

    public void Output(string output, bool arrow = false)
    {
        outputting = true;
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
    }

    public IEnumerator OutputCreepy(string output)
    {
        outputting = true;
        AudioSource whisper = computerWhispers[Random.Range(0, computerWhispers.Length)];
        
        whisper.Play();
        text.text += "<color=white>";
        for (int i = 0; i < output.Length; i++)
        {
            if (i == 0)
            {
                text.text += "\n";
            }

            yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
            text.text += output[i];
        }
        text.text += "</color>";
        whisper.Stop();
        outputting = false;
    }

    public void StartFading()
    {
        fade.enabled = true;
        fadeOut = true;
        fadeWhisper.Play();
    }
}

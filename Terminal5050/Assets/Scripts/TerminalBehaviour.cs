using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerminalBehaviour : MonoBehaviour
{
    [SerializeField] private AudioSource beep;
    [SerializeField] private AudioSource error;
    [SerializeField] private AudioClip errorClip;
    
    private int attempts;

    private void Start()
    {
        CMDManager.Instance.OnChoiceSelected += OnChoiceSelected;
    }

    private void OnChoiceSelected(object sender, int e)
    {
        if (sender is TerminalBehaviour)
        {
            Debug.Log($"Terminal behaviour called the choice of {e}");
        }
    }

    public void Output(string output, bool arrow = false)
    {
        CMDManager.Instance.Output(output, arrow);
    }
    
    public void OutputCreepy(string output)
    {
        CMDManager.Instance.StartCoroutine(CMDManager.Instance.OutputCreepy(output));
    }
    
    public void HandleCommand(string command)
    {
        string[] parts = command.Split(' ');
        string head = parts[0];

        switch (attempts)
        {
            case 3:
                OutputCreepy("Hello there");
                break;
            case 4:
                OutputCreepy("Now aren't you just something");
                break;
            case 5:
                OutputCreepy("Tapping away");
                break;
            case 6:
                OutputCreepy("Without a care in the world");
                break;
            case 7:
                OutputCreepy("No idea what is to come");
                break;
            case 8:
                OutputCreepy("You will see");
                break;
            case 9:
                OutputCreepy("You will see...");;
                CMDManager.Instance.StartFading();
                break;
        }
        
        attempts += 1;

        if (attempts <= 3)
        {
            switch (head)
            {
                case "help":
                    Output($"Welcome to {CMDManager.Instance.terminalName}, the purpose of this interface is to <b><color=red>[REDACTED]</color></b>, please enjoy!\n\n" +
                           $"<b>Useful commands:</b>\n" +
                           "tName {newName} - Change the terminal name\n" +
                           "tColour {newColour} - Change the terminal colour\n" +
                           "diog - Perform system diagnostics" +
                           "control + C - Force stop current process" +
                           "cd - Browse system files");
                    beep.Play();
                    CMDManager.Instance.outputting = false;
                    break;
                case "tName":
                    Output($"Attempting to change the terminal name to {parts[1]}");
                    StartCoroutine(ChangeError());
                    break;
                case "tColour":
                    Output($"Attempting to change the terminal color to {parts[1]}");
                    StartCoroutine(ChangeError());
                    break;
                case "diog":
                    Output($"Performing system diagnostics...");
                    StartCoroutine(Diagnostics());
                    break;
                case "cd":
                    DirectoryManager.Instance.OpenDirectoryScreen();
                    beep.Play();
                    break;
                case "tetst":
                    string[] e = new[]
                    {
                        "a",
                        "b",
                        "c"
                    };
                    CMDManager.Instance.OutputChoice(e, this);
                    break;
                case "hardwarestore":
                    Output($"Tire gauges, hamster cages, thermostats and bug deflectors\nTrailer hitch demagnetizers, automatic circumcisers");
                    CMDManager.Instance.outputting = false;
                    beep.Play();
                    break;
                default:
                    Output("Unknown command please try again");
                    CMDManager.Instance.outputting = false;
                    error.PlayOneShot(errorClip);
                    break;
            }
        }
    }

    private IEnumerator ChangeError()
    {
        yield return new WaitForSeconds(1);
        
        Output("<color=red><b>CRITICAL SYSTEM ERROR</b></color>");
        error.PlayOneShot(errorClip);
        
        yield return new WaitForSeconds(0.3f);
        
        Output("Unable to change terminal information, please try again later");
        
        CMDManager.Instance.outputting = false;
    }

    private IEnumerator Diagnostics()
    {
        beep.Play();
        yield return new WaitForSeconds(1);
        Output("Time remaining: 3");
        yield return new WaitForSeconds(1);
        Output("Time remaining: 2");
        yield return new WaitForSeconds(1);
        Output("Time remaining: 1");
        yield return new WaitForSeconds(1);
        Output("Time remaining: 0\n");
        
        beep.Play();
        
        Output("Diagnostics results:\n" +
               $"Peripheral Response Time: {Random.Range(0, 6)}ms\n" +
               $"Core Integrity: {Random.Range(40, 61)}%\n" +
               $"TfWaitTime: {CMDManager.Instance.tfWaitTime}\n" +
               $"TFC Score: {Random.Range(7, 10)}/10\n" +
               $"Core Tempt: {Random.Range(50, 81)}c\n");
        
        Output("Attempting to retrieve critical core info...\n");
        yield return new WaitForSeconds(1);
        OutputCreepy("Not today...");

        yield return new WaitUntil((() =>
        {
            return !CMDManager.Instance.outputting;
        }));
        
        Output("<color=red>ERROR</color>");
        error.PlayOneShot(errorClip);
        Output("Could not retrieve core info, please try again later");
        CMDManager.Instance.outputting = false;
    }
}

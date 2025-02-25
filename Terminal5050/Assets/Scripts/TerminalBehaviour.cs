using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerminalBehaviour : MonoBehaviour
{
    [SerializeField] private AudioSource beep;
    [SerializeField] private AudioSource error;
    [SerializeField] private AudioClip errorClip;
    
    private WaitUntil _spookyFinish = new WaitUntil((() =>
    {
        return !CMDManager.Instance.creepyOutputting;
    }));

    private bool _computerAwake;

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

    public void Wake()
    {
        _computerAwake = true;
    }

    public void Output(string output, bool arrow = false)
    {
        CMDManager.Instance.Output(output, arrow);
    }
    
    public void HandleCommand(string command)
    {
        string[] parts = command.Split(' ');
        string head = parts[0];

        switch (head)
        {
            case "help":
                CMDManager.Instance.StartProcess();
                Output($"Welcome to {CMDManager.Instance.terminalName}, the purpose of this interface is to <b><color=red>[REDACTED]</color></b>.\n\n" +
                       $"<b>Useful commands:</b>\n\n" +
                       "tName {newName} - Change the terminal name\n" +
                       "tColour {newColour} - Change the terminal colour\n" +
                       "dig - Perform system diagnostics\n" +
                       "cd - Browse system files\n\n" + 
                       "stopspeaker - Stop any sound comming from speakers\n\n" + 
                        "control+C - Force stop current process");
                beep.Play();
                CMDManager.Instance.StopProcess();
                break;
            case "tName":
                CMDManager.Instance.StartProcess();
                Output($"Attempting to change the terminal name to {parts[1]}");
                StartCoroutine(ChangeError());
                CMDManager.Instance.StopProcess();
                break;
            case "tColour":
                CMDManager.Instance.StartProcess();
                Output($"Attempting to change the terminal color to {parts[1]}");
                StartCoroutine(ChangeError());
                break;
            case "dig":
                CMDManager.Instance.StartProcess();
                Output($"Performing system diagnostics...");
                StartCoroutine(Diagnostics());
                break;
            case "speaker":
                CMDManager.Instance.StartProcess();
                Output("\n</color>\u2191 \u2193 and \u21B2 to browse <color=grey>");
                SpeakerManager.Instance.OpenSpeakerScreen();
                beep.Play();
                CMDManager.Instance.StopProcess();
                break;
            case "hardwarestore":
                CMDManager.Instance.StartProcess();
                Output($"Tire gauges, hamster cages, thermostats and bug deflectors\nTrailer hitch demagnetizers, automatic circumcisers");
                beep.Play();
                CMDManager.Instance.StopProcess();
                break;
            case "door":
                CMDManager.Instance.StartProcess();
                if (parts.Length > 1)
                {
                    Output($"Toggling door {parts[1]}");
                    DoorManager.Instance.ToggleDoor(parts[1]);
                    beep.Play();
                }
                else
                {
                    Output($"Please provide a door to toggle");
                }
                CMDManager.Instance.StopProcess();
                break;
            case "stopspeaker":
                CMDManager.Instance.StartProcess();
                Output($"Stopping all speakers");
                SpeakerManager.Instance.StopPlaying();
                CMDManager.Instance.StopProcess();
                break;
            default:
                CMDManager.Instance.StartProcess();
                Output("Unknown command please try again");
                error.PlayOneShot(errorClip);
                CMDManager.Instance.StopProcess();
                break;
        }
    }

    public void StopAll()
    {
        StopAllCoroutines();
    }

    private IEnumerator ChangeError()
    {
        yield return new WaitForSeconds(1);
        
        Output("<color=red><b>CRITICAL SYSTEM ERROR</b></color>");
        error.PlayOneShot(errorClip);
        
        yield return new WaitForSeconds(0.3f);
        
        Output("Unable to change terminal information, please try again later");
        
        CMDManager.Instance.StopProcess();
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

        yield return _spookyFinish;
        
        Output("<color=red>ERROR</color>");
        error.PlayOneShot(errorClip);
        Output("Could not retrieve core info, please try again later");
        CMDManager.Instance.StopProcess();
    }
}

using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ActionBar : MonoBehaviour
{
    public static ActionBar Instance;
    [SerializeField] private TextMeshProUGUI actionBarText;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float openDuration;

    private bool _on;

    private void Update()
    {
        if (_on)
        {
            actionBarText.color = Color.white;
        }
        else
        {
            actionBarText.color = new Color(actionBarText.color.r, actionBarText.color.g, actionBarText.color.b,
                Mathf.Lerp(actionBarText.color.a, 0, fadeSpeed * Time.deltaTime));
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void NewOutput(string content, Color color)
    {
        _on = true;
        actionBarText.text += $"<color=#{color.ToHexString()}>\n" + content + "</color>";
    }

    public void NewOutput(string content)
    {
        NewOutput(content+"\n", Color.white);
        StopAllCoroutines();
        _on = true;
        StartCoroutine(FadeCountdown());
    }

    private IEnumerator FadeCountdown()
    {
        yield return new WaitForSeconds(openDuration);
        _on = false;
    }
}

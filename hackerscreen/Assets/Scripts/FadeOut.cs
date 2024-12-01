using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private AudioSource source;

    private bool begin;

    private void Start()
    {
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(1);
        begin = true;
    }

    private void Update()
    {
        if (!begin)
        {
            return;
        }
        background.color = Color.Lerp(background.color, new Color(0, 0, 0, 0), speed * Time.deltaTime);
        text.color = Color.Lerp(text.color, new Color(0, 0, 0, 0), speed * Time.deltaTime);
        source.volume = Mathf.Lerp(source.volume, 0, speed * Time.deltaTime);
    }
}

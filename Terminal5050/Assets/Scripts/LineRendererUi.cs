using UnityEngine;
using UnityEngine.UI;

public class LineRendererUi : MonoBehaviour
{
    [SerializeField] private RectTransform origin;
    [SerializeField] private RectTransform target;
    [SerializeField] private RectTransform worldTarget;
    
    [SerializeField] private Image image;
    [SerializeField] private RectTransform rectTransform;

    private void Update()
    {
        DrawLine();
    }

    public void DrawLine()
    {
        CreateLine();
    }

    public void CreateLine()
    {
        Debug.Log("CreateLine");
        if (origin.gameObject.activeSelf && this.gameObject.activeSelf)
        {
            Debug.Log("active");

            worldTarget.position = target.position;

            Vector2 obj1Pos = origin.anchoredPosition;
            Vector2 obj2Pos = worldTarget.anchoredPosition;

            Vector2 midPoint = (obj1Pos + obj2Pos) / 2;

            rectTransform.anchoredPosition = new Vector2(midPoint.x, midPoint.y);
            Vector2 dif = obj1Pos - obj2Pos;
            rectTransform.sizeDelta = new Vector2(dif.magnitude, 5);
            rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
        }
    }
}
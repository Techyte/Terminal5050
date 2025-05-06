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
            
            // Vector2 obj1Pos = new Vector2(object1.localPosition.x + 960, object1.localPosition.y + 540);
            // Vector2 obj2Pos = new Vector2(object2.localPosition.x + 960, object2.localPosition.y + 540);

            Vector2 obj1Pos = origin.anchoredPosition;
            Vector2 obj2Pos = worldTarget.anchoredPosition;

            Vector2 midPoint = (obj1Pos + obj2Pos) / 2;

            rectTransform.anchoredPosition = new Vector2(midPoint.x, midPoint.y);
            Vector2 dif = obj1Pos - obj2Pos;
            Debug.Log(obj1Pos);
            Debug.Log(obj2Pos);
            Debug.Log(dif);
            Debug.Log(dif.magnitude);
            rectTransform.sizeDelta = new Vector2(dif.magnitude, 5);
            rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
        }
        
        // positionOne = canvas.InverseTransformPoint(positionOne);
        // positionTwo = canvas.InverseTransformPoint(positionTwo);
        
        // m_image.color = color;
        //
        // Vector2 point1 = new Vector2(positionTwo.x, positionTwo.y);
        // Vector2 point2 = new Vector2(positionOne.x, positionOne.y);
        // Vector2 midpoint = (point1 + point2) / 2f;
        //
        // m_myTransform.position = midpoint;
        //
        // Vector2 dir = point2 - point1;
        // m_myTransform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        // m_myTransform.localScale = new Vector3(dir.magnitude, width, 1f);
    }
}
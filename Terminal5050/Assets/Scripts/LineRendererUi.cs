using UnityEngine;
using UnityEngine.UI;

public class LineRendererUi : MonoBehaviour
{
    [SerializeField] private RectTransform m_myTransform;
    [SerializeField] private Image m_image;
    [SerializeField] private RectTransform target;
    [SerializeField] private Transform canvasObject;
    [SerializeField] private float width;
    [SerializeField] private Transform canvas;
    [SerializeField] private RectTransform debugObj;

    public Vector2 originPosition;

    private void Update()
    {
        DrawLine();
    }

    public void DrawLine()
    {
        // Debug.Log(target.position);
        CreateLine(originPosition, target.position, m_image.color);
    }

    public void CreateLine(Vector3 positionOne, Vector3 positionTwo, Color color)
    {
        // positionOne = canvas.InverseTransformPoint(positionOne);
        // positionTwo = canvas.InverseTransformPoint(positionTwo);
        
        m_image.color = color;

        Vector2 point1 = new Vector2(positionTwo.x, positionTwo.y);
        Vector2 point2 = new Vector2(positionOne.x, positionOne.y);
        Vector2 midpoint = (point1 + point2) / 2f;
        
        // Debug.Log(midpoint);

        m_myTransform.position = midpoint;

        Vector2 dir = point1 - point2;
        m_myTransform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        m_myTransform.localScale = new Vector3(dir.magnitude, width, 1f);
    }
}
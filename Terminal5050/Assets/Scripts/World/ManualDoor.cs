using UnityEngine;

public class ManualDoor : MonoBehaviour
{
    public string id;
    [SerializeField] private Transform closedPos;
    [SerializeField] private Transform openPos;
    [SerializeField] private Transform doorObject;
    [SerializeField] private float speed;
    public bool open;

    public void Toggle()
    {
        open = !open;
    }

    private void Update()
    {
        if (open)
        {
            doorObject.position = Vector3.Lerp(doorObject.position, openPos.position, speed * Time.deltaTime);
        }
        else
        {
            doorObject.position = Vector3.Lerp(doorObject.position, closedPos.position, speed * Time.deltaTime);
        }
    }
}
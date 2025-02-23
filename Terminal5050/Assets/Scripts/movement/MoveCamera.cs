using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public bool cancel;
    [SerializeField] private Transform cameraPosition;

    private void Update()
    {
        if (!cancel)
        {
            transform.position = cameraPosition.position;
        }
    }
}

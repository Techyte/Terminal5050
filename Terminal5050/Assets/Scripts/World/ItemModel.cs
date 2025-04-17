using UnityEngine;

public class ItemModel : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        transform.parent.GetComponent<WorldItem>().DestroyRigidbody();
    }
}

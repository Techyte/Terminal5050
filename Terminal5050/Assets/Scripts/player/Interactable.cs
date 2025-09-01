using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool hoverable = false;
    
    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public virtual void Interact(Player player)
    {
        
    }

    public virtual string GetHoverText(Player player)
    {
        return "";
    }

    public virtual Vector3 GetHoverBounds()
    {
        return Vector3.zero;
    }
}

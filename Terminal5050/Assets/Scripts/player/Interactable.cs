using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public virtual void Interact(Player player)
    {
        
    }
}

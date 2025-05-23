using UnityEngine;
using UnityEngine.Events;

public class Button : Interactable
{
    public UnityEvent clickEvent;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource clickSound;
    
    public override void Interact(Player player)
    {
        clickEvent?.Invoke();
        Clicked();
    }

    private void Clicked()
    {
        animator.SetTrigger("Clicked");
        clickSound.Play();
    }
}

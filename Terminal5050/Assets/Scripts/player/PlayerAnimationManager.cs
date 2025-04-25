using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    public Animator animator;

    private Vector3 _oldPos;

    private void Update()
    {
        if (_oldPos != transform.position)
        {
            animator.SetBool("Running", true);
        }
        else
        {
            animator.SetBool("Running", false);
        }
        
        _oldPos = transform.position;
    }
}

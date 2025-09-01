using System;
using TMPro;
using UnityEngine;

public class MovementUI : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI velText;

    private void Update()
    {
        speedText.text = $"Speed: {Math.Round(rb.linearVelocity.magnitude, 2)}";
        velText.text = $"Velocity: {rb.linearVelocity}";
    }
}

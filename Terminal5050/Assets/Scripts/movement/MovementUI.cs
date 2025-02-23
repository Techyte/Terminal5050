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
        speedText.text = $"Speed: {Math.Round(rb.velocity.magnitude, 2)}";
        velText.text = $"Velocity: {rb.velocity}";
    }
}

using System;
using UnityEngine;

public class PlayerRotationManager : MonoBehaviour
{
    [SerializeField] private Transform cam;

    private void Update()
    {
        Vector3 camEuler = cam.eulerAngles;
        Vector3 selfEuler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(selfEuler.x, camEuler.y, selfEuler.z);
    }
}

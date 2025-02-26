using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationRange;
    [SerializeField] private SecurityCamera defaultCam;
    
    private List<SecurityCamera> _cams = new List<SecurityCamera>();

    private SecurityCamera _activeCam;
    private int _activeIndex;

    private float _rotation = 0;
    private bool _rotDirection;

    private void Awake()
    {
        AddCam(defaultCam);
        SwitchToCam(0);
    }

    public void AddCam(SecurityCamera newCam)
    {
        _cams.Add(newCam);
    }

    public void SwitchToCam(int index)
    {
        foreach (var cam in _cams)
        {
            cam.Disable();
        }
        _cams[index].Enable();
        _activeCam = _cams[index];
        _activeIndex = index;
    }

    private void Update()
    {
        if (!_activeCam)
        {
            return;
        }
        
        if (_rotDirection)
        {
            _rotation += rotationSpeed * Time.deltaTime;
        }
        else
        {
            _rotation -= rotationSpeed * Time.deltaTime;
        }

        if (_rotation >= rotationRange)
        {
            _rotDirection = false;
        }
        else if (_rotation <= -rotationRange)
        {
            _rotDirection = true;
        }
        
        _activeCam.Rotation(_rotation);
    }
}

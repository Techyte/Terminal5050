using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationRange;
    [SerializeField] private SecurityCamera[] defaultCam;
    [SerializeField] private float cameraPowerDrain;
    [SerializeField] private RawImage image;
    
    private List<SecurityCamera> _cams = new List<SecurityCamera>();

    private SecurityCamera _activeCam;
    private int _activeIndex;

    private float _rotation = 0;
    private bool _rotDirection;

    private bool _on;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (var cam in defaultCam)
        {
            cam.Trigger();
        }
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

    public void Cycle()
    {
        int newIndex = (_activeIndex + 1) % _cams.Count;
        SwitchToCam(newIndex);
    }

    public void ToggleCameras()
    {
        _on = !_on;

        if (_on)
        {
            image.color = Color.white;
            PowerManager.Instance.NewDrain("Cameras", cameraPowerDrain);
        }
        else
        {
            PowerManager.Instance.RemoveDrain("Cameras");
            image.color = Color.black;
        }
    }

    private void Update()
    {
        if (!_activeCam || !_on)
        {
            return;
        }
        else
        {
            PowerManager.Instance.ChangeCharge(-cameraPowerDrain * Time.deltaTime);
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

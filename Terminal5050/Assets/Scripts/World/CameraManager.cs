using System;
using System.Collections.Generic;
using Riptide;
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

    public bool on => _on;
    public float rotation => _rotation;

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
        SwitchToCam("0");
    }

    public void AddCam(SecurityCamera newCam)
    {
        _cams.Add(newCam);
    }

    public void SwitchToCam(string id)
    {
        foreach (var cam in _cams)
        {
            cam.Disable();
        }

        SecurityCamera newCam = _cams[_activeIndex];

        int index = 0;

        for (int i = 0; i < _cams.Count; i++)
        {
            if (_cams[i].id == id)
            {
                newCam = _cams[i];
                index = i;
            }
        }
        
        newCam.Enable();
        _activeCam = newCam;
        _activeIndex = index;
    }

    public void Cycle()
    {
        int newIndex = (_activeIndex + 1) % _cams.Count;
        SwitchToCam(_cams[newIndex].id);
    }

    public void ToggleCameras(bool on)
    {
        _on = on;
        
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

    public void ClientReceivedCameraRotation(float rotation)
    {
        _rotation = rotation;
    }

    #region ToggleCameraHandling

    public void ToggleCameraButton()
    {
        SendCamerasToggleMessage(!on);
    }

    public void SendCamerasToggleMessage(bool on)
    {
        // host
        if (NetworkManager.Instance.Server != null)
        {
            ToggleCameras(on);
            
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.CameraToggled);
            message.AddBool(on);

            NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
        }
        else // client wants to notify the server
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientToServerMessageId.ToggleCamera);
            message.AddBool(on);

            NetworkManager.Instance.Client.Send(message);
        }
    }

    public void ServerReceivedToggleCameras(bool on)
    {
        SendCamerasToggleMessage(on);
    }

    public void ClientReceivedCameraToggled(bool on)
    {
        ToggleCameras(on);
    }

    #endregion

    #region SwitchCameraHandling

    public void SwitchCameraButton()
    {
        SendCamerasSwitchMessage();
    }

    public void SendCamerasSwitchMessage()
    {
        // host
        if (NetworkManager.Instance.Server != null)
        {
            Cycle();
            
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.CameraChanged);
            message.AddString(_activeCam.id);

            NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
        }
        else // client wants to notify the server
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientToServerMessageId.ChangedCamera);

            NetworkManager.Instance.Client.Send(message);
        }
    }

    public void ServerReceivedSwitchCameras()
    {
        SendCamerasSwitchMessage();
    }

    public void ClientReceivedCameraSwitched(string newId)
    {
        SwitchToCam(newId);
    }

    #endregion

    #region CameraActivatingHandling

    public void ActivateCameraButton(SecurityCamera cam)
    {
        SendCameraActivateMessage(cam);
    }

    public void SendCameraActivateMessage(SecurityCamera cam)
    {
        // host
        if (NetworkManager.Instance.Server != null)
        {
            cam.Trigger();
            
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.CameraActivated);
            message.AddString(cam.id);

            NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
        }
        else // client wants to notify the server
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientToServerMessageId.ActivateCamera);
            message.AddString(cam.id);

            NetworkManager.Instance.Client.Send(message);
        }
    }

    public void ServerReceivedActivateCamera(string id)
    {
        SendCameraActivateMessage(GetCamFromId(id));
    }

    public void ClientReceivedCameraActivated(string id)
    {
        Debug.Log($"Client received camera {id} activated");
        GetCamFromId(id).Trigger();
    }

    private SecurityCamera GetCamFromId(string id)
    {
        SecurityCamera[] cams = FindObjectsOfType<SecurityCamera>();

        foreach (var cam in cams)
        {
            if (cam.id == id)
            {
                return cam;
            }
        }

        return null;
    }

    #endregion
}

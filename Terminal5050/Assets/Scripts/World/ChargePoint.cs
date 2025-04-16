using System;
using UnityEngine;

public class ChargePoint : Interactable
{
    [SerializeField] private Transform battery;
    [SerializeField] private Transform camViewLoc;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float zoomTime = 2;
    [SerializeField] private AudioSource click;
    [SerializeField] private AudioSource humming;
    [SerializeField] private AudioSource error;

    private bool _interacting = false;

    private PersonalPowerManager _pManager;
    private PlayerMovement _movement;
    private CameraController _cam;

    private void Awake()
    {
        humming.volume = 0;
    }

    public override void Interact(PersonalPowerManager pManager)
    {
        if (_interacting)
        {
            GoBack();
            initTime = DateTime.Now;
        }
        else
        {
            if (pManager.charge == pManager.maxCharge)
            {
                ActionBar.Instance.NewOutput("Power Pack Full");
                error.Play();
                return;
            }

            _cam = pManager.transform.parent.GetComponentInChildren<CameraController>();
            _movement = pManager.GetComponent<PlayerMovement>();
            
            PowerManager.Instance.NewDrain("Charging", chargeSpeed);
            _interacting = true;
            initTime = DateTime.Now;
            
            _pManager = pManager;
            _clicked = false;
        }
    }

    private void Update()
    {
        if (_interacting)
        {
            Slotted();
            
            float amountToGain = chargeSpeed * Time.deltaTime;
            
            if (_pManager.charge + amountToGain >= _pManager.maxCharge)
            {
                PowerManager.Instance.ChangeCharge(-(_pManager.maxCharge - _pManager.charge));
                _pManager.charge = _pManager.maxCharge;
                GoBack();
                ActionBar.Instance.NewOutput("Finished Charging");
            }
            else if (PowerManager.Instance.CurrentCharge - amountToGain <= 0)
            {
                GoBack();
                ActionBar.Instance.NewOutput("Ran out of power", Color.red);
            }
            else
            {
                _pManager.charge += amountToGain;
                PowerManager.Instance.ChangeCharge(-amountToGain);
            }
        }
    }
    
    private DateTime initTime;
    private bool _clicked;

    private void Slotted()
    {
        _movement.cancel = true;
        _cam.cancel = true;
        
        battery.transform.position = Vector3.Lerp(_pManager.batteryLocation.position, camViewLoc.position,
            ((float)(DateTime.Now - initTime).TotalSeconds / zoomTime));
        battery.transform.rotation = Quaternion.Lerp(_pManager.batteryLocation.rotation, camViewLoc.rotation,
            (float)(DateTime.Now - initTime).TotalSeconds / zoomTime);

        if ((DateTime.Now - initTime).TotalSeconds >= zoomTime && !_clicked)
        {
            click.Play();
            humming.volume = 1;
            _clicked = true;
        }
    }
    
    private void Return()
    {
        battery.transform.position = _pManager.batteryLocation.position;
        battery.transform.rotation = _pManager.batteryLocation.rotation;
        click.Play();
    }

    private void GoBack()
    {
        Return();
        _interacting = false;
        _movement.cancel = false;
        _cam.cancel = false;
        humming.volume = 0;
        PowerManager.Instance.RemoveDrain("Charging");
    }
}
using System;
using UnityEngine;

public class BankManager : MonoBehaviour
{
    public static BankManager Instance;

    private int _value;

    public int Value
    {
        get
        {
            return _value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void SetNewValue(int newValue)
    {
        _value = newValue;
    }
}

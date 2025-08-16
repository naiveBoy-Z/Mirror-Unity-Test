using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Net;
using System.Net.Sockets;

public class Test : MonoBehaviour
{
    InputAction pressLeftGripAction, pressRightGripAction, action1, action2;
    public TextMeshProUGUI _text;

    private void Start()
    {
        _text.text = $"{GetLocalIPAddress()}";
        action1 = new InputAction("action", InputActionType.Button, "<ViveWaveController>{LeftHand}/triggerPressed");
        action1.Enable();
        action2 = new InputAction("action2", InputActionType.Value, "<ViveWaveController>{LeftHand}/devicePosition");
        action2.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (action1.WasPressedThisFrame())
        {
            _text.text = action2.ReadValue<Vector3>().y.ToString();
        }
    }

    string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "Không tìm thấy IP";
    }
}

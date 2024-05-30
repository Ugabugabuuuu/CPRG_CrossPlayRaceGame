using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;

public class FinishLineTriggerCheck : MonoBehaviour
{
    public static FinishLineTriggerCheck Instance { get; private set; }
    public event EventHandler OnLocalPlayerFinsihedChanged;
    private void Awake()
    {
        Instance = this;
    }
    public void InvokeEvent(MyEventArgs a)
    {
        OnLocalPlayerFinsihedChanged?.Invoke(this, a);
    }
}

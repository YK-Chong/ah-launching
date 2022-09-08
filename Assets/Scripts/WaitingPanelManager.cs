using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingPanelManager : PanelManager<WaitingPanelManager>
{
    protected override Manager.State DisplayState { get; } = Manager.State.Waiting;

    public Text text;

    private void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        Manager.Instance.OnSignalReceivedCallback += (state) => text.text = "Waiting others to join";
        OnHide += Reset;
    }

    private void Reset()
    {
        text.text = "Loading";
    }
}


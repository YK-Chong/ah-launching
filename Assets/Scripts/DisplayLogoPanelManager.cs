using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayLogoPanelManager : PanelManager<DisplayLogoPanelManager>
{
    protected override Manager.State DisplayState { get; } = Manager.State.End;

    private void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
    }
}


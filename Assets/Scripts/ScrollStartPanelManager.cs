using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollStartPanelManager : PanelManager<ScrollStartPanelManager>
{
    protected override Manager.State DisplayState { get; } = Manager.State.Start;

    private void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
    }
}


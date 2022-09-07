using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollEndPanelManager : PanelManager<ScrollEndPanelManager>
{
    protected override Manager.State DisplayState { get; } = Manager.State.ScrollEnd;

    private void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
    }

    private void Reset()
    {

    }
}


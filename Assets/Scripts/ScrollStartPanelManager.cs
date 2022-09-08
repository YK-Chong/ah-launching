using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cloth2D;

public class ScrollStartPanelManager : PanelManager<ScrollStartPanelManager>
{
    protected override Manager.State DisplayState { get; } = Manager.State.Start;
    private void Awake()
    {
        Instance = this;
        OnDisplay += () => ScrollEndPanelManager.Instance.SetCanvasGroupAlpha(0);
    }

    protected override void Start()
    {
        base.Start();
    }
}
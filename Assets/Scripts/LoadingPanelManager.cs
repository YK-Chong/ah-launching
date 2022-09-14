using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanelManager : PanelManager<LoadingPanelManager>
{
    protected override Manager.State DisplayState { get; } = Manager.State.Loading;

    private void Awake()
    {
        Instance = this;
    }
}


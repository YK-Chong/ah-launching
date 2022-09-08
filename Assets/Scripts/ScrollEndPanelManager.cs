using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollEndPanelManager : PanelManager<ScrollEndPanelManager>
{
    protected override Manager.State DisplayState { get; } = Manager.State.ScrollEnd;

    public Transform gif;

    private void Awake()
    {
        Instance = this;
    }

    protected override void Update()
    {
        base.Update();
        gif.localScale = Vector3.one * CanvasGroup.alpha;
    }
}


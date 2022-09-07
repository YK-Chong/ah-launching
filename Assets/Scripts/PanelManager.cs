using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PanelManager<T> : MonoBehaviour where T : PanelManager<T>
{
    #region Private Variables
    private Transform _child;
    #endregion

    public static T Instance { get; protected set; }
    protected abstract Manager.State DisplayState { get; }
    protected bool IsDisplay { get; private set; }
    protected Action OnDisplay { get; set; }
    protected Action OnHide { get; set; }

    private CanvasGroup _canvasGroup;

    protected virtual void Start()
    {
        Manager.Instance.OnStateChanged += OnStateChanged;
        _child = transform.GetChild(0);
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnStateChanged(Manager.State state)
    {
        //_child.gameObject.SetActive(IsDisplay = state == DisplayState);
        IsDisplay = state == DisplayState;
        _canvasGroup.alpha = (IsDisplay ? 1 : 0);
        if (IsDisplay)
            OnDisplay?.Invoke();
        else
            OnHide?.Invoke();
    }
}

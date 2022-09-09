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
    public CanvasGroup CanvasGroup { get => _canvasGroup; }

    protected virtual void Start()
    {
        Manager.Instance.OnStateChanged += OnStateChanged;
        _child = transform.GetChild(0);
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    protected virtual void Update()
    {
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha,  (IsDisplay ? 1 : 0), Time.deltaTime * 4);
    }

    public void SetCanvasGroupAlpha(float alpha)
    {
        _canvasGroup.alpha = alpha;
    }

    private void OnStateChanged(Manager.State state)
    {
        //_child.gameObject.SetActive(IsDisplay = state == DisplayState);
        IsDisplay = state == DisplayState;
        
        if (IsDisplay)
            OnDisplay?.Invoke();
        else
            OnHide?.Invoke();
    }
}

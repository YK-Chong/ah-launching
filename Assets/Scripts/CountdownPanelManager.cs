using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownPanelManager : PanelManager<CountdownPanelManager>
{
    protected override Manager.State DisplayState { get; } = Manager.State.Countdown;
    private float _timer;
    private float _countdownTimer = 3f;
    public Text txt_Countdown;

    private void Awake()
    {
        Instance = this;
        OnHide += Reset;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if(IsDisplay)
        {
            _timer += Time.deltaTime;
            int countdown = (int)_countdownTimer - (int)_timer;
            txt_Countdown.text = countdown.ToString();
            txt_Countdown.gameObject.SetActive(countdown > 0);
            if (_timer >= _countdownTimer)
            {
                Manager.Instance.ChangeState(Manager.State.Start);
            }
        }
    }

    private void Reset()
    {
        _timer = 0;
    }
}


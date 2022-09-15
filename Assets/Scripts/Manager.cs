using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; private set; }
    public Action<State> OnStateChanged { get; set; }
    [SerializeField]
    private State _currentState;
    public State CurrentState
    {
        set
        {
            if(_currentState != value)
            {
                _currentState = value;
                OnStateChanged?.Invoke(_currentState);
            }
        }
        get
        {
            return _currentState;
        }
    }
    public GameObject cloth;
    public MeshRenderer meshRenderer;
    public Action<int> OnSignalReceivedCallback { get; set; }
    public bool ScrollEnded { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
    }

    IEnumerator Start()
    {
        WebsocketManager.Connect(OnSignalReceived);
        yield return new WaitForEndOfFrame();
        //yield return new WaitUntil(() => MyGifPlayer.Instance.IsReady);
        ChangeState(State.Loading);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ChangeState((State)0);
        }
        else if (Input.GetKey(KeyCode.Alpha1))
        {
            ChangeState((State)1);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            ChangeState((State)2);
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            ChangeState((State)3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeState((State)4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeState((State)5);
        }
#endif
    }

    void OnSignalReceived(int state)
    {
        ChangeState((State)state);
        OnSignalReceivedCallback?.Invoke(state);
    }

    public void ChangeState(State state)
    {
        if (state != State.Countdown && state != State.Start && state != State.ScrollEnd)
            ScrollEnded = false;

        if (state == State.Countdown)
        {
            if (ScrollEnded)
                CurrentState = State.ScrollEnd;
            else
                CurrentState = State.Start;
        }
        else
            CurrentState = state;
        
        //cloth.SetActive(CurrentState == State.Start);
        meshRenderer.enabled = (CurrentState == State.Start);
    }

    public void EndScroll()
    {
        ScrollEnded = true;
    }

    public enum State
    {
        Waiting = 0,
        Countdown = 1,
        End = 2,
        ScrollEnd = 3,
        Start = 4,
        Loading = 5
    }
}

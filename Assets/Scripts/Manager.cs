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
    public State CurrentState { get; private set; }
    public GameObject cloth;
    public Action<int> OnSignalReceivedCallback { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    IEnumerator Start()
    {
        WebsocketManager.Connect(OnSignalReceived);
        yield return new WaitForEndOfFrame();
        //yield return new WaitUntil(() => MyGifPlayer.Instance.IsReady);
        ChangeState(State.Waiting);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    ChangeState((State)0);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    ChangeState((State)1);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    ChangeState((State)2);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    ChangeState((State)3);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    ChangeState((State)4);
        //}
    }

    void OnSignalReceived(int state)
    {
        ChangeState((State)state);
        OnSignalReceivedCallback?.Invoke(state);
    }

    public void ChangeState(State state)
    {
        CurrentState = state;
        OnStateChanged?.Invoke(CurrentState);
        cloth.SetActive(state == State.Start);
    }

    public enum State
    {
        Waiting = 0,
        Countdown = 1,
        End = 2,
        ScrollEnd = 3,
        Start = 4
    }
}

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

    private void Awake()
    {
        Instance = this;
    }

    IEnumerator Start()
    {
        WebsocketManager.Connect(OnSignalReceived);
        for (int i = 0; i < 98; i++)
        {
            Application.OpenURL("https://192.168.1.26:8080/");
        }
        yield return new WaitForEndOfFrame();
        //yield return new WaitUntil(() => MyGifPlayer.Instance.IsReady);
        ChangeState(State.Waiting);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ChangeState(State.Waiting);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeState(State.Countdown);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeState(State.End);
        }
    }

    void OnSignalReceived(int state)
    {
        ChangeState((State)state);
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

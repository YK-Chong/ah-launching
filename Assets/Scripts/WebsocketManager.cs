using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WebsocketManager
{
    [DllImport("__Internal", EntryPoint = "WebGL_ConnectWebSocket")]
    private static extern void WebGL_ConnectWebSocket(Action<int> callback);

	private static event Action<int> CallbackEvent;

	[MonoPInvokeCallback(typeof(Action<int>))]
	private static void Callback(int state)
	{
		CallbackEvent?.Invoke(state);
		//CallbackEvent = null;
	}

	public static void Connect(Action<int> callback = null)
	{
		#if UNITY_WEBGL && !UNITY_EDITOR
			if (callback != null) {
			CallbackEvent += callback;
			WebGL_ConnectWebSocket(Callback);
			} else {
			WebGL_ConnectWebSocket(null);
			}
		#endif
	}
}

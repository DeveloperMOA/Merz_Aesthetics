using System.Runtime.InteropServices;
using UnityEngine;

public static class MobileMessage
{
#if UNITY_IOS

	[DllImport("__Internal")]
	private static extern void _ShowAlert(string title, string message);
#endif

	public static void ShowAlert(string title,string message)
	{
#if UNITY_EDITOR
		Debug.Log(title + ": " + message);
#elif UNITY_IOS
		_ShowAlert(title,message);
#elif UNITY_ANDROID
		Debug.Log(title + ": " + message);
		AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
		object[] toastParams = new object[3];
		AndroidJavaClass unityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		toastParams[0] = unityActivity.GetStatic<AndroidJavaObject>("currentActivity");
		toastParams[1] = title + ": " + message;
		toastParams[2] = toastClass.GetStatic<int>("LENGTH_LONG");
		AndroidJavaObject toastObject =	toastClass.CallStatic<AndroidJavaObject>("makeText", toastParams);
		toastObject.Call("show");
#endif
	}
}

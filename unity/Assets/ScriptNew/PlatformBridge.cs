using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBridge : MonoBehaviour
{

	public delegate void WeChatLoginCallBackEvent(string result);

	public WeChatLoginCallBackEvent WeChatLoginListener;

	public void WeChatLoginCallBack(string result)
	{
		if (WeChatLoginListener != null)
			WeChatLoginListener(result);
	}

	public void doWeChatLogin()
	{
		#if UNITY_ANDROID
		currentActivity.Call("wechatLogin");
		#elif UNITY_IPHONE
		#else 
		#endif
	}

	public void shareAchievementToWeChat()
	{
	}

	public void inviteFriend()
	{
	}

	public void shareZhanji(int id)
	{
	}

	//==============================================================================
	//==============================================================================

	private static PlatformBridge _instance;

	public static PlatformBridge Instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType(typeof(PlatformBridge)) as PlatformBridge;
				if (_instance == null) {
					_instance = new GameObject("Platform").AddComponent<PlatformBridge>();
					_instance.Awake();
					DontDestroyOnLoad(_instance.gameObject);
				}
			}
			return _instance;
		}
	}

	void Awake()
	{
		if (_instance == null) {
			_instance = this;
			DontDestroyOnLoad(gameObject);

		} else if (_instance != this) {
			Destroy(this.gameObject);
			return;     
		}
		Init();
	}
	//===========================================================================

	#if UNITY_ANDROID
	public static AndroidJavaClass jc;
	public static AndroidJavaObject currentActivity;
	#endif

	void Init()
	{
		#if UNITY_ANDROID
		if (currentActivity == null) {
			jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
		}
		#endif

		Debug.Log("platform bridge inited!");
	}
}

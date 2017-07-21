using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using LitJson;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;


public class InitializationConfigScritp : MonoBehaviour
{
	
	int num = 0;
	bool hasPaused = false;
	private bool updateLocation = false;
	private AmapLocation amap = new AmapLocation();

	void Awake()
	{
		SocketEventHandle.getInstance().disConnetNotice += disConnetNotice;
		SocketEventHandle.getInstance().otherTeleLogin += otherTeleLogin;
	}

	void Start()
	{
		MicroPhoneInput.getInstance();
		GlobalDataScript.getInstance();
		TipsManagerScript.getInstance().parent = gameObject.transform;
		SoundCtrl.getInstance();

		//UpdateScript update = new UpdateScript ();
		//StartCoroutine (update.updateCheck ());
		ServiceErrorListener seriveError = new ServiceErrorListener();
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		//heartBeatThread();
		heartBeatTimer();

		amap.StartLocation();
	}

	private void  disConnetNotice()
	{
		if (GlobalDataScript.isonLoginPage) {
		} else {
			cleaListener();
			PrefabManage.loadPerfab("Prefab/Panel_Start");
		}
	}

	private void otherTeleLogin(ClientResponse response)
	{
		//	TipsManagerScript.getInstance ().setTips ("你的账号在其他设备登录");
		disConnetNotice();
	}

	private void cleaListener()
	{
		/*
		if (SocketEventHandle.getInstance ().LoginCallBack != null) {
			SocketEventHandle.getInstance ().LoginCallBack = null;
		}
*/
		if (SocketEventHandle.getInstance().CreateRoomCallBack != null) {
			SocketEventHandle.getInstance().CreateRoomCallBack = null;
		}

		if (SocketEventHandle.getInstance().JoinRoomCallBack != null) {
			SocketEventHandle.getInstance().JoinRoomCallBack = null;
		}

		if (SocketEventHandle.getInstance().StartGameNotice != null) {
			SocketEventHandle.getInstance().StartGameNotice = null;
		}

		if (SocketEventHandle.getInstance().pickCardCallBack != null) {
			SocketEventHandle.getInstance().pickCardCallBack = null;
		}

		if (SocketEventHandle.getInstance().otherPickCardCallBack != null) {
			SocketEventHandle.getInstance().otherPickCardCallBack = null;
		}

		if (SocketEventHandle.getInstance().putOutCardCallBack != null) {
			SocketEventHandle.getInstance().putOutCardCallBack = null;
		}

		if (SocketEventHandle.getInstance().PengCardCallBack != null) {
			SocketEventHandle.getInstance().PengCardCallBack = null;
		}

		if (SocketEventHandle.getInstance().GangCardCallBack != null) {
			SocketEventHandle.getInstance().GangCardCallBack = null;
		}

		if (SocketEventHandle.getInstance().HupaiCallBack != null) {
			SocketEventHandle.getInstance().HupaiCallBack = null;
		}

	
		if (SocketEventHandle.getInstance().gangCardNotice != null) {
			SocketEventHandle.getInstance().gangCardNotice = null;
		}



		if (SocketEventHandle.getInstance().btnActionShow != null) {
			SocketEventHandle.getInstance().btnActionShow = null;
		}

		if (SocketEventHandle.getInstance().outRoomCallback != null) {
			SocketEventHandle.getInstance().outRoomCallback = null;
		}

		if (SocketEventHandle.getInstance().dissoliveRoomResponse != null) {
			SocketEventHandle.getInstance().dissoliveRoomResponse = null;
		}

		if (SocketEventHandle.getInstance().gameReadyNotice != null) {
			SocketEventHandle.getInstance().gameReadyNotice = null;
		}

		if (SocketEventHandle.getInstance().messageBoxNotice != null) {
			SocketEventHandle.getInstance().messageBoxNotice = null;
		}

		if (SocketEventHandle.getInstance().backLoginNotice != null) {
			SocketEventHandle.getInstance().backLoginNotice = null;
		}
		/*
		if (SocketEventHandle.getInstance ().RoomBackResponse != null) {
			SocketEventHandle.getInstance ().RoomBackResponse = null;
		}
		*/

		if (SocketEventHandle.getInstance().cardChangeNotice != null) {
			SocketEventHandle.getInstance().cardChangeNotice = null;
		}

		if (SocketEventHandle.getInstance().offlineNotice != null) {
			SocketEventHandle.getInstance().offlineNotice = null;
		}

		if (SocketEventHandle.getInstance().onlineNotice != null) {
			SocketEventHandle.getInstance().onlineNotice = null;
		}

		if (SocketEventHandle.getInstance().giftResponse != null) {
			SocketEventHandle.getInstance().giftResponse = null;
		}

		if (SocketEventHandle.getInstance().returnGameResponse != null) {
			SocketEventHandle.getInstance().returnGameResponse = null;
		}

		if (SocketEventHandle.getInstance().gameFollowBanderNotice != null) {
			SocketEventHandle.getInstance().gameFollowBanderNotice = null;
		}

		if (SocketEventHandle.getInstance().contactInfoResponse != null) {
			SocketEventHandle.getInstance().contactInfoResponse = null;
		}

		if (SocketEventHandle.getInstance().zhanjiResponse != null) {
			SocketEventHandle.getInstance().zhanjiResponse = null;
		}

		if (SocketEventHandle.getInstance().zhanjiDetailResponse != null) {
			SocketEventHandle.getInstance().zhanjiDetailResponse = null;
		}

		if (SocketEventHandle.getInstance().gameBackPlayResponse != null) {
			SocketEventHandle.getInstance().gameBackPlayResponse = null;
		}

	}

	void Update()
	{
		if (amap.hasLocation && GlobalDataScript.loginResponseData != null && GlobalDataScript.loginResponseData.isOnLine == true &&
		    (updateLocation == false || GlobalDataScript.loginResponseData.address.Equals(amap.address) == false)) {
			CustomSocket.getInstance().sendMsg(new LocationRequest(amap.longitude, amap.latitude, amap.address));
			GlobalDataScript.loginResponseData.latitude = amap.latitude;
			GlobalDataScript.loginResponseData.longitude = amap.longitude;
			GlobalDataScript.loginResponseData.address = amap.address;
			updateLocation = true;
		}
	}

	System.Timers.Timer t;

	private  void heartBeatTimer()
	{
		t = new System.Timers.Timer(20000);   //实例化Timer类，设置间隔时间为10000毫秒；   
		t.Elapsed += new System.Timers.ElapsedEventHandler(doSendHeartbeat); //到达时间的时候执行事件；   
		t.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)；   
		t.Enabled = true;     //是否执行System.Timers.Timer.Elapsed事件；   
	}

	public  void doSendHeartbeat(object source, System.Timers.ElapsedEventArgs e)
	{
//		CustomSocket.getInstance().SendHeartBeatData();
	}

	//	private void heartBeatThread()
	//	{
	//		Thread thread = new Thread(sendHeartbeat);
	//		thread.IsBackground = true;
	//		thread.Start();
	//	}
	//
	//
	//	private static void sendHeartbeat()
	//	{
	//		CustomSocket.getInstance().sendHeadData();
	//		Thread.Sleep(20000);
	//		sendHeartbeat();
	//	}
}

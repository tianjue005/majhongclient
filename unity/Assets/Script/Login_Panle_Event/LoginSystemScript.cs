using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AssemblyCSharp;
using LitJson;
using System.Collections.Generic;
using cn.sharesdk.unity3d;
using tutorial;


public class LoginSystemScript : MonoBehaviour
{
	//public ShareSDK shareSdk;
	private GameObject panelCreateDialog;

	public Toggle agreeProtocol;

	public Text versionText;

	//点击次数
	private int tapCount = 0;

	public GameObject watingPanel;

	void Start ()
	{
		CustomSocket.getInstance ().hasStartTimer = false;
		CustomSocket.getInstance ().Connect ();
		ChatSocket.getInstance ().Connect ();
		GlobalDataScript.isonLoginPage = true;
		SocketEventHandle.getInstance ().LoginCallBack += LoginCallBack;
		SocketEventHandle.getInstance ().RoomBackResponse += RoomBackResponse;
		PlatformBridge.Instance.WeChatLoginListener += WeChatLoginCallBack;
		versionText.text = "版本号：" + Application.version;
	}

	private bool isAutoLoginWithSessionId = true;
	
	// Update is called once per frame
	void Update ()
	{

		if (isAutoLoginWithSessionId && CustomSocket.getInstance ().isConnected) {
			isAutoLoginWithSessionId = false;
			LoginWithSessionId ();
		}

		if (Input.GetKey (KeyCode.Escape)) { //Android系统监听返回键，由于只有Android和ios系统所以无需对系统做判断
			if (panelCreateDialog == null) {
				panelCreateDialog = Instantiate (Resources.Load ("Prefab/Panel_Exit")) as GameObject;
				panelCreateDialog.transform.parent = gameObject.transform;
				panelCreateDialog.transform.localScale = Vector3.one;
				//panelCreateDialog.transform.localPosition = new Vector3 (200f,150f);
				panelCreateDialog.GetComponent<RectTransform> ().offsetMax = new Vector2 (0f, 0f);
				panelCreateDialog.GetComponent<RectTransform> ().offsetMin = new Vector2 (0f, 0f);
			}
		}
	}

	public void loginWechat ()
	{
		SoundCtrl.getInstance ().playSoundUI (true);
		
		if (!CustomSocket.getInstance ().isConnected) {
			CustomSocket.getInstance ().Connect ();
			ChatSocket.getInstance ().Connect ();
			tapCount = 0;
			return;
		}

		GlobalDataScript.reinitData ();//初始化界面数据
		if (agreeProtocol.isOn) {
//			doLogin();
			if (Application.platform == RuntimePlatform.OSXEditor ||
			    Application.platform == RuntimePlatform.WindowsEditor) {
				WeChatLoginCallBack ("{\"access_token\":" +
				"\"BYl9EIcjwyVFdP3F8ZIo8_OrDlMi4MNKFRDGdl_3KkmtU5YLl7Vgi4dw17I24tS2af2CqhBX6X3p9R5emnZOen8SvuFkNn5_wd4042L3d2w\"," +
				"\"expires_in\":7200," +
				"\"refresh_token\":" +
				"\"BQhqiKHK_cUnED8a1CfRgUcXuS4EyGcq9XQAdj-N-3ctMUoG6VxzTROBIe7m1Q2Ws17br6MJxk85tIbMz4ecsI1VEXsfslh9jyWN1Xjt6vc\"," +
				"\"openid\":\"o3LILj-8xTgvce2P_QBjMrgDZdQg\"," +
				"\"scope\":\"snsapi_userinfo\"," +
				"\"unionid\":\"oHRAHuO86ExIL6tLdyAkFSFlEoYw\"}");
			} else {
				PlatformBridge.Instance.doWeChatLogin ();
				watingPanel.SetActive (true);
			}
		} else {
			MyDebug.Log ("请先同意用户使用协议");
			TipsManagerScript.getInstance ().setTips ("请先同意用户使用协议");
		}

		tapCount += 1;
		Invoke ("resetClickNum", 10f);
	}

	public void WeChatLoginCallBack (string result)
	{
		try {
			Hashtable map = (Hashtable)MiniJSON.jsonDecode (result);
			string accessToken = (string)map ["access_token"];
			string openId = (string)map ["openid"];
			Debug.Log (" tag unity result: " + result + "  token:" + accessToken + " openid:" + openId);

			app_login__weixin_api login = new app_login__weixin_api ();
			login.access_token = accessToken;
			login.openid = openId;
			byte[] content = ClientRequest.Serialize<app_login__weixin_api> (login);

			app_login__weixin_api test = ClientRequest.DeSerialize<app_login__weixin_api> (content);
			Debug.Log (content.Length +"test: " + test.access_token + " openid:  " + test.openid);

			CustomSocket.getInstance ().sendMsg (new ClientRequest (ApiCode.LoginWeChatRequest).SetContent<app_login__weixin_api> (login));

		} catch (System.Exception e) {
			Debug.Log (e.StackTrace);
		}
	}

	public void LoginWithSessionId ()
	{
		string sessionId = GamePreferences.Instance.SessionId;
		if (Utils.IsNull (sessionId)) {
			Debug.Log ("login with sessionid=[" + sessionId + "]");
			return;
		}
		profile_api login = new profile_api ();
		login.SESSIONID = sessionId;
		CustomSocket.getInstance ().sendMsg (new ClientRequest (ApiCode.LoginSessionRequest).SetContent<profile_api> (login));
	}

	public void LoginCallBack (ClientResponse response)
	{
		if (watingPanel != null) {
			watingPanel.SetActive (false);
		}
	
		SoundCtrl.getInstance ().stopBGM ();
		SoundCtrl.getInstance ().playBGM ();
		if (response.status == 1) {
			if (GlobalDataScript.homePanel != null) {
				GlobalDataScript.homePanel.GetComponent<HomePanelScript> ().removeListener ();
				Destroy (GlobalDataScript.homePanel);
			}

			if (GlobalDataScript.gamePlayPanel != null) {
				GlobalDataScript.gamePlayPanel.GetComponent<MyMahjongScript> ().exitOrDissoliveRoom ();
			}

			GlobalDataScript.loginResponseData = JsonMapper.ToObject<AvatarVO> (response.message);
			ChatSocket.getInstance ().sendMsg (new LoginChatRequest (GlobalDataScript.loginResponseData.account.uuid));
			panelCreateDialog = Instantiate (Resources.Load ("Prefab/Panel_Home")) as GameObject;
			panelCreateDialog.transform.parent = GlobalDataScript.getInstance ().canvsTransfrom;
			panelCreateDialog.transform.localScale = Vector3.one;
			panelCreateDialog.GetComponent<RectTransform> ().offsetMax = new Vector2 (0f, 0f);
			panelCreateDialog.GetComponent<RectTransform> ().offsetMin = new Vector2 (0f, 0f);
			GlobalDataScript.homePanel = panelCreateDialog;
			removeListener ();
			Destroy (this);
			Destroy (gameObject);
		}
	}

	private void removeListener ()
	{
		SocketEventHandle.getInstance ().LoginCallBack -= LoginCallBack;
		SocketEventHandle.getInstance ().RoomBackResponse -= RoomBackResponse;
	}

	private void RoomBackResponse (ClientResponse response)
	{
		watingPanel.SetActive (false);

		if (GlobalDataScript.homePanel != null) {
			GlobalDataScript.homePanel.GetComponent<HomePanelScript> ().removeListener ();
			Destroy (GlobalDataScript.homePanel);
		}

		if (GlobalDataScript.gamePlayPanel != null) {
			GlobalDataScript.gamePlayPanel.GetComponent<MyMahjongScript> ().exitOrDissoliveRoom ();
		}
		GlobalDataScript.reEnterRoomData = JsonMapper.ToObject<RoomJoinResponseVo> (response.message);

		for (int i = 0; i < GlobalDataScript.reEnterRoomData.playerList.Count; i++) {
			AvatarVO itemData =	GlobalDataScript.reEnterRoomData.playerList [i];
			if (itemData.account.openid == GlobalDataScript.loginResponseData.account.openid) {
				GlobalDataScript.loginResponseData.account.uuid = itemData.account.uuid;
				GlobalDataScript.loginResponseData.isOnLine = true;
				ChatSocket.getInstance ().sendMsg (new LoginChatRequest (GlobalDataScript.loginResponseData.account.uuid));
				break;
			}
		}

		GlobalDataScript.gamePlayPanel = PrefabManage.loadPerfab ("Prefab/Panel_GamePlay");
		removeListener ();
		Destroy (this);
		Destroy (gameObject);
	}

	private void resetClickNum ()
	{
		tapCount = 0;
	}

}

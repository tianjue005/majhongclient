using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AssemblyCSharp;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using LitJson;
using tutorial;

public class EnterRoomScript : MonoBehaviour
{

	public Button button_sure, button_delete;
	//确认删除按钮

	private List<String> inputChars;
	//输入的字符
	public List<Text> inputTexts;

	public List<GameObject> btnList;

	// Use this for initialization
	void Start()
	{
		SocketEventHandle.getInstance().CreateRoomCallBack += onJoinRoomCallBack;
		inputChars = new List<String>();
		for (int i = 0; i < btnList.Count; i++) {
			GameObject gobj = btnList [i];
			btnList [i].GetComponent<Button>().onClick.AddListener(delegate() {
				this.OnClickHandle(gobj); 
			});
		}
	}

	public void OnClickHandle(GameObject gobj)
	{
		//if(eventData.button)
		Debug.Log(gobj);
		clickNumber(gobj.GetComponentInChildren<Text>().text);
	}

	private void clickNumber(string number)
	{
		SoundCtrl.getInstance().playSoundUI();
		if (inputChars.Count >= 6) {
			return;
		}
		inputChars.Add(number);
		int index = inputChars.Count;
		inputTexts [index - 1].text = number.ToString();

		if (inputChars.Count == 6)
			enterRoom();
	}

	public void deleteNumber()
	{
		SoundCtrl.getInstance().playSoundUI();
		if (inputChars != null && inputChars.Count > 0) {
			inputChars.RemoveAt(inputChars.Count - 1);
			inputTexts [inputChars.Count].text = "";
		}
	}

	public void closeDialog()
	{
		SoundCtrl.getInstance().playSoundUI();
		Debug.Log("closeDialog");
		//GlobalDataScript.homePanel.SetActive (false);
		removeListener();
		Destroy(this);
		Destroy(gameObject);
	}

	private void removeListener()
	{
		SocketEventHandle.getInstance().CreateRoomCallBack -= onJoinRoomCallBack;
	}

	private void enterRoom()
	{
		String roomNumber = inputChars [0] + inputChars [1] + inputChars [2] + inputChars [3] + inputChars [4] + inputChars [5];
		staticc__room_opreation_api request = new staticc__room_opreation_api();
		request.SESSIONID = GamePreferences.Instance.SessionId;
		request.operation = 2;// 1 means create, 2 means join
		request.rtype = ApiCode.CODE_ZERO;
		request.room_num = int.Parse(roomNumber);
		CustomSocket.getInstance().sendMsg(new ClientRequest(ApiCode.CreateRoomRequest).SetContent<staticc__room_opreation_api>(request));
	}

	public void sureRoomNumber()
	{
		SoundCtrl.getInstance().playSoundUI();
		if (inputChars.Count != 6) {
			Debug.Log("请先完整输入房间号码！");
			TipsManagerScript.getInstance().setTips("请先完整输入房间号码！");
			return;
		}

		enterRoom();
	}

	public void onJoinRoomCallBack(ClientResponse response)
	{
		if (response.handleCode == StatusCode.SESSION_expire ||
		    response.handleCode == StatusCode.SESSION_invalid || response.bytes == null) {
			Debug.Log("onJoinRoomCallBack error " + response.handleCode);
			return;
		}
		Debug.Log("join room success!");
		if (response.status == 1) {
//			GlobalDataScript.roomJoinResponseData = JsonMapper.ToObject<RoomJoinResponseVo>(response.message);
//			GlobalDataScript.roomVo.addWordCard = GlobalDataScript.roomJoinResponseData.addWordCard;
//			GlobalDataScript.roomVo.hong = GlobalDataScript.roomJoinResponseData.hong;
//			GlobalDataScript.roomVo.ma = GlobalDataScript.roomJoinResponseData.ma;
//			GlobalDataScript.roomVo.name = GlobalDataScript.roomJoinResponseData.name;
//			GlobalDataScript.roomVo.roomId = GlobalDataScript.roomJoinResponseData.roomId;
//			GlobalDataScript.roomVo.roomType = GlobalDataScript.roomJoinResponseData.roomType;
//			GlobalDataScript.roomVo.roundNumber = GlobalDataScript.roomJoinResponseData.roundNumber;
//			GlobalDataScript.roomVo.sevenDouble = GlobalDataScript.roomJoinResponseData.sevenDouble;
//			GlobalDataScript.roomVo.xiaYu = GlobalDataScript.roomJoinResponseData.xiaYu;
//			GlobalDataScript.roomVo.ziMo = GlobalDataScript.roomJoinResponseData.ziMo;
//			GlobalDataScript.roomVo.magnification = GlobalDataScript.roomJoinResponseData.magnification;
//
//			GlobalDataScript.roomVo.chengbei = GlobalDataScript.roomJoinResponseData.chengbei;
//			GlobalDataScript.roomVo.aa = GlobalDataScript.roomJoinResponseData.aa;
//			GlobalDataScript.roomVo.zashu = GlobalDataScript.roomJoinResponseData.zashu;
//			GlobalDataScript.roomVo.paofen = GlobalDataScript.roomJoinResponseData.paofen;
//			GlobalDataScript.roomVo.roundtype = GlobalDataScript.roomJoinResponseData.roundtype;
//			GlobalDataScript.roomVo.yuanzishu = GlobalDataScript.roomJoinResponseData.yuanzishu;
//			GlobalDataScript.roomVo.yuanzijiesu = GlobalDataScript.roomJoinResponseData.yuanzijiesu;
//			GlobalDataScript.roomVo.zhanzhuangbi = GlobalDataScript.roomJoinResponseData.zhanzhuangbi;
//			GlobalDataScript.roomVo.guozhuangbi = GlobalDataScript.roomJoinResponseData.guozhuangbi;
//			GlobalDataScript.roomVo.fengfa = GlobalDataScript.roomJoinResponseData.fengfa;
//
//			GlobalDataScript.surplusTimes = GlobalDataScript.roomJoinResponseData.realRoundNumber();
			GlobalDataScript.gameOver = false;

//			GlobalDataScript.loginResponseData.roomId = GlobalDataScript.roomJoinResponseData.roomId;
			GlobalDataScript.gamePlayPanel = PrefabManage.loadPerfab("Prefab/Panel_GamePlay");
//			GlobalDataScript.gamePlayPanel.GetComponent<MyMahjongScript>().joinToRoom(GlobalDataScript.roomJoinResponseData.playerList);
			closeDialog();
		} else {
			TipsManagerScript.getInstance().setTips(response.message);
		}
	}

}

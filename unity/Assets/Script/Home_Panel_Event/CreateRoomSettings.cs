using UnityEngine;
using UnityEngine.UI;
using AssemblyCSharp;
using System.Collections.Generic;
using System;
using LitJson;

using tutorial;

[Serializable]
public class ChoiceLine
{
	public Text title;
	public List<Toggle> choiceList;
	public List<Text> choiceTextList;
}

public class CreateRoomSettings : MonoBehaviour
{
	public Transform[] choiceParent;
	public ChoiceLine[] choiceArray;
	private int[] itemChooseIndex;

	public Text nanjingeXiaohaoText;

	//房卡数
	private int roomCardCount;
	private GameObject gameSence;

	//创建房间的信息
	private RoomCreateVo sendVo;

	void Awake()
	{
		int choiceCount = choiceParent.Length;
		choiceArray = new ChoiceLine[choiceCount];
		itemChooseIndex = new int[choiceCount];
		for (int i = 0; i < choiceCount; i++) {
			choiceArray [i] = new ChoiceLine();
			choiceArray [i].title = choiceParent [i].Find("Text_Title").GetComponent<Text>();
			choiceArray [i].choiceList = new List<Toggle>();
			choiceArray [i].choiceTextList = new List<Text>();
			for (int j = 0; j < 3; j++) {
				Toggle toggle = choiceParent [i].Find("Layout/Toggle" + (j + 1)).GetComponent<Toggle>();
				choiceArray [i].choiceList.Add(toggle);
				choiceArray [i].choiceTextList.Add(toggle.transform.Find("Label").GetComponent<Text>());
			}
		}
	}


	void Start()
	{
		SocketEventHandle.getInstance().CreateRoomCallBack += onCreateRoomCallback;

		initToggle();
	}

	private void initToggle()
	{
		RestoreDefaultSetting();

		for (int i = 0; i < choiceArray.Length; i++) {
			for (int j = 0; j < choiceArray [i].choiceList.Count; j++) {
				choiceArray [i].choiceList [j].onValueChanged.AddListener(onValueChanged);
			}
		}
	}

	public void closeDialog()
	{
		SoundCtrl.getInstance().playSoundUI();
		SocketEventHandle.getInstance().CreateRoomCallBack -= onCreateRoomCallback;
		Destroy(this);
		Destroy(gameObject);
	}

	private Color32 selectTextColor = new Color32(239, 76, 74, 255);
	private Color32 noselectTextColor = new Color32(165, 92, 50, 255);

	private void onValueChanged(bool value)
	{
		for (int i = 0; i < choiceArray.Length; i++) {
			for (int j = 0; j < choiceArray [i].choiceList.Count; j++) {
				if (choiceArray [i].choiceList [j].isOn) {
					itemChooseIndex [i] = j;
					choiceArray [i].choiceTextList [j].color = selectTextColor;
				} else {
					choiceArray [i].choiceTextList [j].color = noselectTextColor;
				}
			}
		}

		int xiaohao = 0;
		nanjingeXiaohaoText.text = "-消耗" + xiaohao + "钻石-";
	}


	public void createNanjingRoom()
	{
		staticc__room_opreation_api request = new staticc__room_opreation_api();
		request.SESSIONID = GamePreferences.Instance.SessionId;
		request.operation = 1;// 1 means create, 2 means join
		request.rtype = ApiCode.CODE_ZERO;
		staticc__ChoiceR_response res = GlobalDataScript.roomParameters;
		for (int i = 0; i < res.choice_info.Count; i++) {
			play_config_unit item = new play_config_unit();
			item.name = res.choice_info [i].name;
			item.choice = res.choice_info [i].choice [itemChooseIndex [i]];
			item.choice_order = itemChooseIndex [i];
			request.play_config.Add(item);
		}
		CustomSocket.getInstance().sendMsg(new ClientRequest(ApiCode.CreateRoomRequest).SetContent<staticc__room_opreation_api>(request));
	}

	public void clickDefault()
	{
		SoundCtrl.getInstance().playSoundUI();
		RestoreDefaultSetting();
	}

	public void RestoreDefaultSetting()
	{
		if (GlobalDataScript.roomParameters == null)
			return;
		staticc__ChoiceR_response res = GlobalDataScript.roomParameters;
		for (int i = 0; i < res.choice_info.Count; i++) {
			choice_info item = res.choice_info [i];
			choiceArray [i].title.text = item.name;
			for (int j = 0; j < item.choice.Count; j++) {
				//choiceArray[i].choiceList[j].isOn = (j==item.index);
				choiceArray [i].choiceTextList [j].text = item.choice [j];
			}
			for (int j = item.choice.Count; j < choiceArray [i].choiceList.Count; j++) {
				choiceArray [i].choiceList [j].gameObject.SetActive(false);
			}
		}
		for (int i = res.choice_info.Count; i < choiceArray.Length; i++) {
			choiceParent [i].gameObject.SetActive(false);
		}
	}

	public void onCreateRoomCallback(ClientResponse response)
	{
		if (response.handleCode == StatusCode.SESSION_expire ||
		    response.handleCode == StatusCode.SESSION_invalid || response.bytes == null) {
			TipsManagerScript.getInstance().setTips("错误：" + response.handleCode);
			return;
		}

		staticc__room_opreation_response res = ClientRequest.DeSerialize<staticc__room_opreation_response>(response.bytes);
		Debug.Log("room number: " + res.data.room_num);

		//进入游戏中等待其他玩家加入
		GlobalDataScript.roomInfo = res.data;
		int roomid = Int32.Parse(response.message);
		sendVo.roomId = roomid;
		//			GlobalDataScript.roomVo = sendVo;
		GlobalDataScript.loginResponseData.roomId = roomid;
		//GlobalDataScript.loginResponseData.isReady = true;
		GlobalDataScript.loginResponseData.main = true;
		GlobalDataScript.loginResponseData.isOnLine = true;

		GlobalDataScript.gamePlayPanel = PrefabManage.loadPerfab("Prefab/Panel_GamePlay");
		GlobalDataScript.gamePlayPanel.GetComponent<MyMahjongScript>().createRoomAddAvatarVO(GlobalDataScript.loginResponseData);
		closeDialog();
	}
}

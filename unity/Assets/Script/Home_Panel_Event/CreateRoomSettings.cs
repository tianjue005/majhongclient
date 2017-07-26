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

	public Text nanjingeXiaohaoText;

	public GameObject panelYuanzi;
	public GameObject panelChangkaitou;

	//房卡数
	private int roomCardCount;
	private GameObject gameSence;

	//创建房间的信息
	private RoomCreateVo sendVo;

	void Awake ()
	{
		int choiceCount = choiceParent.Length;
		choiceArray = new ChoiceLine[choiceCount];
		for (int i = 0; i < choiceCount; i++) {
			choiceArray [i] = new ChoiceLine ();
			choiceArray [i].title = choiceParent [i].Find ("Text_Title").GetComponent<Text> ();
			choiceArray [i].choiceList = new List<Toggle> ();
			choiceArray [i].choiceTextList = new List<Text> ();
			for (int j = 0; j < 3; j++) {
				Toggle toggle = choiceParent [i].Find ("Layout/Toggle" + (j + 1)).GetComponent<Toggle> ();
				choiceArray [i].choiceList.Add (toggle);
				choiceArray [i].choiceTextList.Add (toggle.transform.Find ("Label").GetComponent<Text> ());
			}
		}
	}


	void Start ()
	{
		SocketEventHandle.getInstance ().CreateRoomCallBack += onCreateRoomCallback;

		initToggle ();
	}

	private void initToggle ()
	{
		RestoreDefaultSetting ();

		for (int i = 0; i < choiceArray.Length; i++) {
			for (int j = 0; j < choiceArray [i].choiceList.Count; j++) {
				choiceArray [i].choiceList [j].onValueChanged.AddListener (onValueChanged);
			}
		}
	}

	public void closeDialog ()
	{
		SoundCtrl.getInstance ().playSoundUI ();
		SocketEventHandle.getInstance ().CreateRoomCallBack -= onCreateRoomCallback;
		Destroy (this);
		Destroy (gameObject);
	}

	private Color32 selectTextColor = new Color32 (239, 76, 74, 255);
	private Color32 noselectTextColor = new Color32 (165, 92, 50, 255);

	private void onValueChanged (bool value)
	{
		for (int i = 0; i < choiceArray.Length; i++) {
			for (int j = 0; j < choiceArray [i].choiceList.Count; j++) {
				if (choiceArray [i].choiceList [j].isOn) {
					choiceArray [i].choiceTextList [j].color = selectTextColor;
				} else {
					choiceArray [i].choiceTextList [j].color = noselectTextColor;
				}
			}
		}

		int xiaohao = 0;
		nanjingeXiaohaoText.text = "-消耗" + xiaohao + "钻石-";
	}


	public void createNanjingRoom ()
	{
		SoundCtrl.getInstance ().playSoundUI ();
		int paofen = 1;
		int roundtype = 0; 
		int yuanzishu = 0;
		int yuanzijiesu = 1;
		int roundNumber = 0;
		int roomcard = 4;
		int zashu = 1;
		bool zhanzhuangbi = false;
		bool guozhuangbi = false;
		bool fengfa = false;

		sendVo = new RoomCreateVo ();
		sendVo.roomType = GameConfig.GAME_TYPE_NANJING;
		sendVo.paofen = paofen;
		sendVo.roundtype = roundtype;
		sendVo.yuanzishu = yuanzishu;
		sendVo.yuanzijiesu = yuanzijiesu;
		sendVo.roundNumber = roundNumber;
		sendVo.zashu = zashu;
		sendVo.zhanzhuangbi = zhanzhuangbi;
		sendVo.guozhuangbi = guozhuangbi;
		sendVo.fengfa = fengfa;

		sendVo.chengbei = false;
		sendVo.aa = false;

		string sendmsgstr = JsonMapper.ToJson (sendVo);
		if (GlobalDataScript.loginResponseData.account.roomcard >= roomcard) {
			CustomSocket.getInstance ().sendMsg (new CreateRoomRequest (sendmsgstr));
		} else {
			TipsManagerScript.getInstance ().setTips ("你的砖石数量不足，不能创建房间");
		}
	}

	public void clickDefault ()
	{
		SoundCtrl.getInstance ().playSoundUI ();
		RestoreDefaultSetting ();
	}

	public void RestoreDefaultSetting ()
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
				choiceArray [i].choiceList [j].gameObject.SetActive (false);
			}
		}
	}

	public void onCreateRoomCallback (ClientResponse response)
	{
		Debug.Log (response.message);
		if (response.status == 1) {
			
			//RoomCreateResponseVo responseVO = JsonMapper.ToObject<RoomCreateResponseVo> (response.message);
			int roomid = Int32.Parse (response.message);
			sendVo.roomId = roomid;
			GlobalDataScript.roomVo = sendVo;
			GlobalDataScript.loginResponseData.roomId = roomid;
			//GlobalDataScript.loginResponseData.isReady = true;
			GlobalDataScript.loginResponseData.main = true;
			GlobalDataScript.loginResponseData.isOnLine = true;

			GlobalDataScript.gamePlayPanel = PrefabManage.loadPerfab ("Prefab/Panel_GamePlay");
			GlobalDataScript.gamePlayPanel.GetComponent<MyMahjongScript> ().createRoomAddAvatarVO (GlobalDataScript.loginResponseData);
			closeDialog ();
		} else {
			TipsManagerScript.getInstance ().setTips (response.message);
		}
	}
}

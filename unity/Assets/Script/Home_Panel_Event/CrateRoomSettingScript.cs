using UnityEngine;
using UnityEngine.UI;
using AssemblyCSharp;
using System.Collections.Generic;
using System;
using LitJson;

public class CrateRoomSettingScript : MonoBehaviour
{
	public GameObject panelNanjingSetting;
	public List<Toggle> nanjingPaofen;
	public List<Toggle> nanjingRoundRule;
	public List<Toggle> nanjingYuanziShu;
	public List<Toggle> nanjingYuanziRule;
	public List<Toggle> nanjingChangkaitou;
	public Toggle nanjingeZashu;
	public Toggle nanjingeZhanzhuangbi;
	public Toggle nanjingeGuozhuangbi;
	public Toggle nanjingeFafen;

	public List<Text> nanjingPaofenText;
	public List<Text> nanjingRoundRuleText;
	public List<Text> nanjingYuanziShuText;
	public List<Text> nanjingYuanziRuleText;
	public List<Text> nanjingChangkaitouText;
	public Text nanjingeZashuText;
	public Text nanjingeZhanzhuangbiText;
	public Text nanjingeGuozhuangbiText;
	public Text nanjingeFafenText;

	public Text nanjingeXiaohaoText;

	public GameObject panelYuanzi;
	public GameObject panelChangkaitou;

	//房卡数
	private int roomCardCount;
	private GameObject gameSence;

	//创建房间的信息
	private RoomCreateVo sendVo;

	void Start()
	{
		SocketEventHandle.getInstance().CreateRoomCallBack += onCreateRoomCallback;

		initToggle();
	}

	private void initToggle()
	{
		RestoreDefaultSetting();

		for (int i = 0; i < nanjingPaofen.Count; i++) {
			nanjingPaofen [i].onValueChanged.AddListener(onValueChanged);
		}

		for (int i = 0; i < nanjingRoundRule.Count; i++) {
			nanjingRoundRule [i].onValueChanged.AddListener(onValueChanged);
		}

		for (int i = 0; i < nanjingYuanziShu.Count; i++) {
			nanjingYuanziShu [i].onValueChanged.AddListener(onValueChanged);
		}

		for (int i = 0; i < nanjingYuanziRule.Count; i++) {
			nanjingYuanziRule [i].onValueChanged.AddListener(onValueChanged);
		}

		for (int i = 0; i < nanjingChangkaitou.Count; i++) {
			nanjingChangkaitou [i].onValueChanged.AddListener(onValueChanged);
		}

		nanjingeZhanzhuangbi.onValueChanged.AddListener(onValueChanged);
		nanjingeGuozhuangbi.onValueChanged.AddListener(onValueChanged);
		nanjingeFafen.onValueChanged.AddListener(onValueChanged);
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
		int roundtype = 0;
		for (int i = 0; i < nanjingPaofen.Count; i++) {
			if (nanjingPaofen [i].isOn) {
				nanjingPaofenText [i].color = selectTextColor;
			} else {
				nanjingPaofenText [i].color = noselectTextColor;
			}
		}

		for (int i = 0; i < nanjingRoundRule.Count; i++) {
			if (nanjingRoundRule [i].isOn) {
				nanjingRoundRuleText [i].color = selectTextColor;
				if (i == 0) {
					if (panelYuanzi.activeSelf == false)
						panelYuanzi.SetActive(true);
				} else if (i == 1) {
					if (panelChangkaitou.activeSelf == false)
						panelChangkaitou.SetActive(true);
				}
				roundtype = i;
			} else {
				nanjingRoundRuleText [i].color = noselectTextColor;
				if (i == 0) {
					if (panelYuanzi.activeSelf)
						panelYuanzi.SetActive(false);
				} else if (i == 1) {
					if (panelChangkaitou.activeSelf)
						panelChangkaitou.SetActive(false);
				}
			}
		}

		for (int i = 0; i < nanjingYuanziShu.Count; i++) {
			if (nanjingYuanziShu [i].isOn) {
				nanjingYuanziShuText [i].color = selectTextColor;
			} else {
				nanjingYuanziShuText [i].color = noselectTextColor;
			}
		}

		for (int i = 0; i < nanjingYuanziRule.Count; i++) {
			if (nanjingYuanziRule [i].isOn) {
				nanjingYuanziRuleText [i].color = selectTextColor;
			} else {
				nanjingYuanziRuleText [i].color = noselectTextColor;
			}
		}

		for (int i = 0; i < nanjingChangkaitou.Count; i++) {
			if (nanjingChangkaitou [i].isOn) {
				nanjingChangkaitouText [i].color = selectTextColor;
			} else {
				nanjingChangkaitouText [i].color = noselectTextColor;
			}
		}

		nanjingeZashuText.color = selectTextColor;

		if (nanjingeZhanzhuangbi.isOn) {
			nanjingeZhanzhuangbiText.color = selectTextColor;
		} else {
			nanjingeZhanzhuangbiText.color = noselectTextColor;
		}

		if (nanjingeGuozhuangbi.isOn) {
			nanjingeGuozhuangbiText.color = selectTextColor;
		} else {
			nanjingeGuozhuangbiText.color = noselectTextColor;
		}

		if (nanjingeFafen.isOn) {
			nanjingeFafenText.color = selectTextColor;
		} else {
			nanjingeFafenText.color = noselectTextColor;
		}

		int xiaohao = 0;
		if (roundtype == 0) {
			for (int i = 0; i < nanjingYuanziShu.Count; i++) {
				if (nanjingYuanziShu [i].isOn) {
					xiaohao = NanjingConfig.YUANZI_ROOMCARD_SET [i];
					break;
				}
			}
		} else {
			for (int i = 0; i < nanjingChangkaitou.Count; i++) {
				if (nanjingChangkaitou [i].isOn) {
					xiaohao = NanjingConfig.ROOMCARD_SET [i];
					break;
				}
			}
		}

		nanjingeXiaohaoText.text = "-消耗" + xiaohao + "钻石-";
	}


	public void createNanjingRoom()
	{
		SoundCtrl.getInstance().playSoundUI();
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

		for (int i = 0; i < nanjingRoundRule.Count; i++) {
			if (nanjingRoundRule [i].isOn) {
				roundtype = i;
				break;
			}
		}

		if (roundtype == 0) {
			for (int i = 0; i < nanjingYuanziShu.Count; i++) {
				if (nanjingYuanziShu [i].isOn) {
					yuanzishu = i;
					roomcard = NanjingConfig.YUANZI_ROOMCARD_SET [i];
					break;
				}
			}

		} else {
			for (int i = 0; i < nanjingChangkaitou.Count; i++) {
				if (nanjingChangkaitou [i].isOn) {
					roundNumber = i;
					roomcard = NanjingConfig.ROOMCARD_SET [i];
					break;
				}
			}
		}


		zashu = 1;
		if (nanjingeZhanzhuangbi.isOn) {
			zhanzhuangbi = true;
			guozhuangbi = true;
		}

		if (nanjingeFafen.isOn) {
			fengfa = true;
		}

		sendVo = new RoomCreateVo();
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

		string sendmsgstr = JsonMapper.ToJson(sendVo);
		if (GlobalDataScript.loginResponseData.account.roomcard >= roomcard) {
			CustomSocket.getInstance().sendMsg(new CreateRoomRequest(sendmsgstr));
		} else {
			TipsManagerScript.getInstance().setTips("你的砖石数量不足，不能创建房间");
		}
	}

	public void clickDefault()
	{
		SoundCtrl.getInstance().playSoundUI();
		RestoreDefaultSetting();
	}

	public void RestoreDefaultSetting()
	{
		for (int i = 0; i < nanjingPaofen.Count; i++) {
			nanjingPaofen [i].isOn = (i == 1);
		}

		for (int i = 0; i < nanjingRoundRule.Count; i++) {
			nanjingRoundRule [i].isOn = (i == 0);
		}

		for (int i = 0; i < nanjingYuanziShu.Count; i++) {
			nanjingYuanziShu [i].isOn = (i == 0);
		}

		for (int i = 0; i < nanjingYuanziRule.Count; i++) {
			nanjingYuanziRule [i].isOn = (i == 1);
		}

		for (int i = 0; i < nanjingChangkaitou.Count; i++) {
			nanjingChangkaitou [i].isOn = (i == 0);
		}

		nanjingeZashu.isOn = true;
		nanjingeZhanzhuangbi.isOn = false;
		nanjingeGuozhuangbi.isOn = false;
		nanjingeFafen.isOn = false;
	}

	public void onCreateRoomCallback(ClientResponse response)
	{
		Debug.Log(response.message);
		if (response.status == 1) {
			
			//RoomCreateResponseVo responseVO = JsonMapper.ToObject<RoomCreateResponseVo> (response.message);
			int roomid = Int32.Parse(response.message);
			sendVo.roomId = roomid;
			GlobalDataScript.roomVo = sendVo;
			GlobalDataScript.loginResponseData.roomId = roomid;
			//GlobalDataScript.loginResponseData.isReady = true;
			GlobalDataScript.loginResponseData.main = true;
			GlobalDataScript.loginResponseData.isOnLine = true;

			GlobalDataScript.gamePlayPanel = PrefabManage.loadPerfab("Prefab/Panel_GamePlay");
			GlobalDataScript.gamePlayPanel.GetComponent<MyMahjongScript>().createRoomAddAvatarVO(GlobalDataScript.loginResponseData);
			closeDialog();
		} else {
			TipsManagerScript.getInstance().setTips(response.message);
		}
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using AssemblyCSharp;
using cn.sharesdk.unity3d;
using System.IO;
using System.Collections.Generic;

public class GameOverScript : MonoBehaviour
{
	/**时间显示条**/
	public Text timeText;

	/**房间号**/
	public Text roomNoText;

	/**局数**/
	public Text jushuText;

	/**标题显示**/
	//public Text TitleText;

	/***单局面板**/
	public GameObject singlePanel;
	public GameObject signalEndPanel;

	/***全局面板**/
	public GameObject finalEndPanel;
	public GameObject finalPanel;

	/**分享单局结束战绩按钮**/
	public GameObject shareSiganlButton;

	/**继续游戏按钮**/
	public GameObject continueGame;

	/**分享全局结束战绩按钮**/

	public Button closeButton;

	private List<AvatarVO> mAvatarvoList;

	/**0表示打开单局结束模板，1表示全局结束模板**/
	private int mDispalyFlag;

	private string picPath;
	//图片存储路径


	/**
	 * 设置面板的显示内容
	 * dispalyFlag:0------>表示单据结束； 1--------->全局结束
	 */
	public void setDisplaContent(int dispalyFlag, List<AvatarVO> personList)
	{
		mAvatarvoList = personList;
		mDispalyFlag = dispalyFlag;
		initRoomBaseInfo();
		jushuText.text = "局数：" + (GlobalDataScript.totalTimes - GlobalDataScript.surplusTimes) + "/" + GlobalDataScript.totalTimes;
		if (dispalyFlag == 0) {
			singlePanel.SetActive(true);
			finalPanel.SetActive(false);
			continueGame.SetActive(true);
			closeButton.transform.gameObject.SetActive(false);
			if (GlobalDataScript.surplusTimes == 0 || GlobalDataScript.isOverByPlayer || GlobalDataScript.gameOver) {
				shareSiganlButton.SetActive(true);
				continueGame.SetActive(false);
			} else {
				shareSiganlButton.SetActive(false);
				continueGame.SetActive(true);
			}

			setSignalContent();
		} else if (dispalyFlag == 1) {
			singlePanel.SetActive(false);
			finalPanel.SetActive(true);
			shareSiganlButton.SetActive(false);
			continueGame.SetActive(false);
			closeButton.transform.gameObject.SetActive(true);
			setFinalContent();
		}


	}

	private int getIndex(int uuid)
	{
		if (mAvatarvoList != null) {
			for (int i = 0; i < mAvatarvoList.Count; i++) {
				if (mAvatarvoList [i].account.uuid == uuid) {
					return i;
				}
			}
		}
		return 0;
	}

	private void initRoomBaseInfo()
	{
		timeText.text = DateTime.Now.ToString("yyyy-MM-dd");
		roomNoText.text = "房间号：" + GlobalDataScript.roomVo.roomId;
	}

	private Account getAcount(int uuid)
	{
		if (mAvatarvoList != null && mAvatarvoList.Count > 0) {
			for (int i = 0; i < mAvatarvoList.Count; i++) {
				if (mAvatarvoList [i].account.uuid == uuid) {
					return mAvatarvoList [i].account;
				}
			}
		}
		return null;
	}

	private void setFinalContent()
	{
		GlobalDataScript.finalGameEndVo.totalInfo [0].setIsWiner(true);
		GlobalDataScript.finalGameEndVo.totalInfo [0].setIsPaoshou(true);
		int topScore = GlobalDataScript.finalGameEndVo.totalInfo [0].scores;
		int topPaoshou = GlobalDataScript.finalGameEndVo.totalInfo [0].dianpao;

		int uuid0 = GlobalDataScript.finalGameEndVo.totalInfo [0].uuid;
		int owerUuid = GlobalDataScript.finalGameEndVo.theowner;

		Account account0 = getAcount(uuid0);

		//AvatarVO avatarVO0 = getAvatar (uuid0);

		string iconstr = account0.headicon;
		string nickName = account0.nickname;
		GlobalDataScript.finalGameEndVo.totalInfo [0].setIcon(iconstr);
		GlobalDataScript.finalGameEndVo.totalInfo [0].setNickname(nickName);
		if (owerUuid == uuid0) {
			GlobalDataScript.finalGameEndVo.totalInfo [0].setIsMain(true);
		} else {
			GlobalDataScript.finalGameEndVo.totalInfo [0].setIsMain(false);
		}
		//	GlobalDataScript.finalGameEndVo.totalInfo [0].setIsMain (avatarVO0.main);
		int lastTopIndex = 0;
		int lastPaoshouIndex = 0;
		if (GlobalDataScript.finalGameEndVo != null && GlobalDataScript.finalGameEndVo.totalInfo.Count > 0) {

			for (int i = 1; i < GlobalDataScript.finalGameEndVo.totalInfo.Count; i++) {
				if (topScore < GlobalDataScript.finalGameEndVo.totalInfo [i].scores) {
					GlobalDataScript.finalGameEndVo.totalInfo [lastTopIndex].setIsWiner(false);
					GlobalDataScript.finalGameEndVo.totalInfo [i].setIsWiner(true);
					lastTopIndex = i;
					topScore = GlobalDataScript.finalGameEndVo.totalInfo [i].scores;
				}
				if (topPaoshou < GlobalDataScript.finalGameEndVo.totalInfo [i].dianpao && !GlobalDataScript.finalGameEndVo.totalInfo [i].getIsWiner()) {
					topPaoshou = GlobalDataScript.finalGameEndVo.totalInfo [i].dianpao;
					GlobalDataScript.finalGameEndVo.totalInfo [i].setIsPaoshou(true);
					GlobalDataScript.finalGameEndVo.totalInfo [lastPaoshouIndex].setIsPaoshou(false);
					lastPaoshouIndex = i;
				}


				int uuid = GlobalDataScript.finalGameEndVo.totalInfo [i].uuid;
				Account account = getAcount(uuid);
				if (account != null) {
					GlobalDataScript.finalGameEndVo.totalInfo [i].setIcon(account.headicon);
					GlobalDataScript.finalGameEndVo.totalInfo [i].setNickname(account.nickname);

				}
				if (owerUuid == uuid) {
					GlobalDataScript.finalGameEndVo.totalInfo [i].setIsMain(true);
				} else {
					GlobalDataScript.finalGameEndVo.totalInfo [i].setIsMain(false);
				}


			}

			for (int i = 0; i < GlobalDataScript.finalGameEndVo.totalInfo.Count; i++) {
				FinalGameEndItemVo itemdata = GlobalDataScript.finalGameEndVo.totalInfo [i];
				GameObject itemTemp = Instantiate(Resources.Load("Prefab/Panel_Final_Item")) as GameObject;
				itemTemp.transform.parent = finalEndPanel.transform;
				itemTemp.transform.localScale = Vector3.one;
				itemTemp.GetComponent<finalOverItem>().setUI(itemdata);
			}

		}



	}

	private void setSignalContent()
	{


		if (GlobalDataScript.hupaiResponseVo != null && GlobalDataScript.hupaiResponseVo.avatarList.Count > 0) {
			for (int i = 0; i < GlobalDataScript.hupaiResponseVo.avatarList.Count; i++) {
				HupaiResponseItem itemdata = GlobalDataScript.hupaiResponseVo.avatarList [i];
				GameObject itemTemp = Instantiate(Resources.Load("Prefab/Panel_Current_Item")) as GameObject;
				itemTemp.transform.parent = signalEndPanel.transform;
				itemTemp.transform.localScale = Vector3.one;
				itemTemp.GetComponent<SignalOverItemScript>().setUI(itemdata, getMainuuid());
			}
		}
	}

	private int getMainuuid()
	{
		for (int i = 0; i < mAvatarvoList.Count; i++) {
			if (mAvatarvoList [i].main) {
				return mAvatarvoList [i].account.uuid;
			}
		}
		return 0;
	}

	public void reStratGame()
	{
		SoundCtrl.getInstance().playSoundUI();
		if (GlobalDataScript.isOverByPlayer) {
			TipsManagerScript.getInstance().setTips("房间已解散，不能重新开始游戏");
			return;
		}

		if (GlobalDataScript.surplusTimes > 0 && GlobalDataScript.gameOver == false) {
			CustomSocket.getInstance().sendMsg(new GameReadyRequest());
			CommonEvent.getInstance().readyGame();
			closeDialog();

		} else {
			TipsManagerScript.getInstance().setTips("游戏已经结束，无法重新开始游戏");
		}

	}


	public void closeDialog()
	{
		SoundCtrl.getInstance().playSoundUI();
		GameOverScript self = GetComponent<GameOverScript>();
		Destroy(self.continueGame);
		Destroy(self.jushuText);
		Destroy(self.singlePanel);
		Destroy(self.finalPanel);
		Destroy(self.shareSiganlButton);
		Destroy(self.continueGame);

		if (GlobalDataScript.singalGameOverList != null && GlobalDataScript.singalGameOverList.Count > 0) {
			for (int i = 0; i < GlobalDataScript.singalGameOverList.Count; i++) {
				//GlobalDataScript.singalGameOverList [i].GetComponent<GameOverScript> ().closeDialog ();
				Destroy(GlobalDataScript.singalGameOverList [i].GetComponent<GameOverScript>());
				Destroy(GlobalDataScript.singalGameOverList [i]);
			}
			int count = GlobalDataScript.singalGameOverList.Count;
			for (int i = 0; i < count; i++) {
				GlobalDataScript.singalGameOverList.RemoveAt(0);
			}
		}

		Destroy(this);
		Destroy(gameObject);

	}

	public void doShare()
	{
		SoundCtrl.getInstance().playSoundUI();
//		GlobalDataScript.getInstance().wechatOperate.shareAchievementToWeChat(PlatformType.WeChat);
//		GlobalDataScript.getInstance().wechatOperate.shareAchievementToWeChat();
		PlatformBridge.Instance.shareAchievementToWeChat();
	}

	public void openFinalOverPanl()
	{
		SoundCtrl.getInstance().playSoundUI();
		if (GlobalDataScript.finalGameEndVo != null && GlobalDataScript.finalGameEndVo.totalInfo != null && GlobalDataScript.finalGameEndVo.totalInfo.Count > 0) {
			GameObject obj = PrefabManage.loadPerfab("prefab/Panel_Game_Over");
			obj.GetComponent<GameOverScript>().setDisplaContent(1, GlobalDataScript.roomAvatarVoList);
			obj.transform.SetSiblingIndex(2);

			if (GlobalDataScript.singalGameOverList.Count > 0) {
				for (int i = 0; i < GlobalDataScript.singalGameOverList.Count; i++) {
					//GlobalDataScript.singalGameOverList [i].GetComponent<GameOverScript> ().closeDialog ();
					Destroy(GlobalDataScript.singalGameOverList [i].GetComponent<GameOverScript>());
					Destroy(GlobalDataScript.singalGameOverList [i]);
				}
				//int count = GlobalDataScript.singalGameOverList.Count;
				//for (int i = 0; i < count; i++) {
				//	GlobalDataScript.singalGameOverList.RemoveAt (0);
				//}
				GlobalDataScript.singalGameOverList.Clear();
			}
			if (CommonEvent.getInstance().closeGamePanel != null) {
				CommonEvent.getInstance().closeGamePanel();
			}

		}



	}


}

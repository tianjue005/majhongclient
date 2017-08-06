using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AssemblyCSharp;
using DG.Tweening;
using UnityEngine.UI;
using LitJson;
using System.Runtime.InteropServices;

public class MyMahjongScript : MonoBehaviour
{
	public double lastTime;
	public Text Number;
	public Text roomNumber;
	public Text roomPaofen;
	public Text roomRule;

	public GameObject pengEffectGame;
	//碰牌的特效

	public GameObject gangEffectGame;
	//杠牌的特效

	public GameObject huEffectGame;
	//胡牌的特效

	public GameObject liujuEffectGame;
	//流局的特效

	public GameObject huagangEffectGame;
	//public GameObject buhuaEffectGame;
	public int otherPengCard;
	public int otherGangCard;
	public ButtonActionScript btnActionScript;
	public List<Transform> parentList;
	public List<Transform> outparentList;
	public List<GameObject> dirGameList;
	public List<PlayerItemScript> playerItems;
	public Text LeavedCastNumText;
	//剩余牌的张数
	public Text LeavedRoundNumText;
	//剩余局数
	//public int StartRoundNum;
	public Transform pengGangParenTransformB;
	public Transform pengGangParenTransformL;
	public Transform pengGangParenTransformR;
	public Transform pengGangParenTransformT;
	public List<AvatarVO> avatarList;
	public Button inviteFriendButton;
	public GameObject tableShow;
	public GameObject startGameShow;
	public Slider dianliangSlider;
	public Image dianliangImage;

	public Image live1;
	public Image live2;
	public Image centerImage;
	public GameObject noticeGameObject;
	public Text noticeText;
	public GameObject genZhuang;
	public Text versionText;

	public Text buhuaTextB;
	public Text buhuaTextL;
	public Text buhuaTextR;
	public Text buhuaTextT;
	public GameObject buhuaPanel;
	public GameObject bixiahuText;
	public GameObject messageBox;
	public Text messageBoxText;

	public List<Text> scoreList;

	private Color32 winTextColor = new Color32(252, 197, 5, 255);
	private Color32 loseTextColor = new Color32(90, 178, 224, 255);

	//======================================
	private int uuid;
	private float timer = 0;
	private int LeavedCardsNum;
	private int MoPaiCardPoint;
	private List<List<GameObject>> PengGangCardList;
	//碰杠牌组
	private List<List<GameObject>> PengGangList_L;
	private List<List<GameObject>> PengGangList_T;
	private List<List<GameObject>> PengGangList_R;
	private List<GameObject> buhuaListB;
	//buhua
	private List<GameObject> buhuaList_L;
	private List<GameObject> buhuaList_T;
	private List<GameObject> buhuaList_R;
	private string effectType;
	private List<List<int>> mineList;
	private int gangKind;
	private int otherGangType;
	private GameObject cardOnTable;
	/// <summary>
	/// 
	/// </summary>
	private int useForGangOrPengOrChi;
	private int selfGangCardPoint;
	/// <summary>
	/// 庄家的索引
	/// </summary>
	private int bankerId;
	private int curDirIndex;
	/// <summary>
	/// 打出来的牌
	/// </summary>
	private GameObject putOutCard;

	private int otherMoCardPoint;
	private GameObject Pointertemp;

	private int putOutCardPoint = -1;
	//打出的牌
	private int putOutCardPointAvarIndex = -1;
	//最后一个打出牌的人的index

	private string outDir;
	private int SelfAndOtherPutoutCard = -1;
	/// <summary>
	/// 当前摸的牌
	/// </summary>
	private GameObject pickCardItem;
	private GameObject otherPickCardItem;
	/// <summary>
	/// 当前的方向字符串
	/// </summary>
	private string curDirString = "B";
	/// <summary>
	/// 普通胡牌算法
	/// </summary>
	private NormalHuScript norHu;
	/// <summary>
	/// 赖子胡牌算法
	/// </summary>
	private NaiziHuScript naiziHu;

	// Use this for initialization
	private GameToolScript gameTool;
	/**游戏单局结束动态面板**/
	//private GameObject singalEndPanel;
	//private List<int> GameOverPlayerCoins;


	private int showTimeNumber = 0;
	private int showNoticeNumber = 0;
	private bool timeFlag = false;
	/// <summary>
	/// 手牌数组，0自己，1-右边。2-上边。3-左边
	/// </summary>
	public List<List<GameObject>> handerCardList;
	/// <summary>
	/// 打在桌子上的牌
	/// </summary>
	public List<List<GameObject>> tableCardList;
	public List<GameObject> huaParent;
	/**后台传过来的杠牌**/
	private string[] gangPaiList;

	private bool isFirstOpen = true;

	/**是否为抢胡 游戏结束时需置为false**/
	private bool isQiangHu = false;
	/**更否申请退出房间申请**/
	private bool canClickButtonFlag = false;

	private string passType = "";

	private bool buhuaBegin = false;

	public GameObject tingPanel;
	public Transform tingPosition;
	public List<GameObject> tingCardList;
	public bool isWillTing = false;
	public Transform pointerParent;

	//private bool isSelfPickCard = false;

	void Start()
	{
		randShowTime();
		timeFlag = true;
		SoundCtrl.getInstance().stopBGM();
		SoundCtrl.getInstance().playGameBGM();
		//===========================================================================================
		norHu = new NormalHuScript();
		naiziHu = new NaiziHuScript();
		gameTool = new GameToolScript();
		versionText.text = "V" + Application.version;
		//===========================================================================================
		btnActionScript = gameObject.GetComponent<ButtonActionScript>();
		addListener(); //监听麻将的各种可能,(开始游戏,碰,杠,胡)
		initPanel();
		initArrayList();
		//initPerson ();//初始化每个成员1000分

		GlobalDataScript.isonLoginPage = false;
		if (GlobalDataScript.reEnterRoomData != null) {
			GlobalDataScript.loginResponseData.roomId = GlobalDataScript.reEnterRoomData.roomId;
			reEnterRoom();
		} else {
			//readyGame();
			//markselfReadyGame ();
		}
		GlobalDataScript.reEnterRoomData = null;

#if UNITY_ANDROID || UNITY_IPHONE
		StartCoroutine(UpdataBattery());
#endif
	}

	void randShowTime()
	{
		showTimeNumber = (int)(UnityEngine.Random.Range(5000, 10000));
	}

	void initPanel()
	{
		clean();
		btnActionScript.cleanBtnShow();
		hideTingCard();
		//masContaner.SetActive (false);
		ShowBuhuaInit();
	}

	string _battery = string.Empty;
	#if UNITY_ANDROID
	private IEnumerator UpdataBattery()
	{
		while (true) {
			int battery = GetBatteryLevel();
			if (battery > 10)
				dianliangImage.sprite = Resources.Load("Image/youxizhong_icon3", typeof(Sprite)) as Sprite;
			else
				dianliangImage.sprite = Resources.Load("Image/youxizhong_icon4", typeof(Sprite)) as Sprite;
			dianliangSlider.value = battery * 0.01f;

			yield return new WaitForSeconds(300f);
		}
	}


	private int GetBatteryLevel()
	{
		try {
			string CapacityString = System.IO.File.ReadAllText("/sys/class/power_supply/battery/capacity");
			return int.Parse(CapacityString);
		} catch (Exception e) {
			Debug.Log("Failed to read battery power; " + e.Message);
		}
		return 0;
	}
	#elif UNITY_IPHONE
  [DllImport("__Internal")]
  private static extern float _getiOSBatteryLevel();

  private IEnumerator UpdataBattery() {
    while (true) {
	  float battery = _getiOSBatteryLevel();
      if (battery > 0.1f) dianliangImage.sprite = Resources.Load("Image/youxizhong_icon3", typeof(Sprite)) as Sprite;
      else dianliangImage.sprite = Resources.Load("Image/youxizhong_icon4", typeof(Sprite)) as Sprite;
      dianliangSlider.value = battery;

      yield return new WaitForSeconds(300f);
    }
  }
#endif

	public void ShowBuhuaPanel()
	{
		buhuaPanel.SetActive(true);
	}

	public void CloseBuhuaPanel()
	{
		buhuaPanel.SetActive(false);
	}

	public void addListener()
	{
		SocketEventHandle.getInstance().StartGameNotice += startGame;
		SocketEventHandle.getInstance().pickCardCallBack += pickCard;
		SocketEventHandle.getInstance().otherPickCardCallBack += otherPickCard;
		SocketEventHandle.getInstance().putOutCardCallBack += otherPutOutCard;
		SocketEventHandle.getInstance().otherUserJointRoomCallBack += otherUserJointRoom;
		SocketEventHandle.getInstance().PengCardCallBack += otherPeng;
		SocketEventHandle.getInstance().GangCardCallBack += gangResponse;
		SocketEventHandle.getInstance().gangCardNotice += otherGang;
		SocketEventHandle.getInstance().btnActionShow += actionBtnShow;
		SocketEventHandle.getInstance().HupaiCallBack += hupaiCallBack;
		SocketEventHandle.getInstance().FinalGameOverCallBack += finalGameOverCallBack;
		SocketEventHandle.getInstance().outRoomCallback += outRoomCallbak;
		SocketEventHandle.getInstance().dissoliveRoomResponse += dissoliveRoomResponse;
		SocketEventHandle.getInstance().gameReadyNotice += gameReadyNotice;
		SocketEventHandle.getInstance().offlineNotice += offlineNotice;
		SocketEventHandle.getInstance().messageBoxNotice += messageBoxNotice;
		SocketEventHandle.getInstance().returnGameResponse += returnGameResponse;
		SocketEventHandle.getInstance().onlineNotice += onlineNotice;
		CommonEvent.getInstance().readyGame += markselfReadyGame;
		CommonEvent.getInstance().closeGamePanel += exitOrDissoliveRoom;
		SocketEventHandle.getInstance().gameFollowBanderNotice += gameFollowBanderNotice;

		SocketEventHandle.getInstance().buhuaResponse += buhuaResponse;
		SocketEventHandle.getInstance().huaGangResponse += huaGangResponse;
		SocketEventHandle.getInstance().scoreResponse += scoreResponse;
		SocketEventHandle.getInstance().buhuaBeginResponse += buhuaBeginResponse;
	}

	private void removeListener()
	{
		SocketEventHandle.getInstance().StartGameNotice -= startGame;
		SocketEventHandle.getInstance().pickCardCallBack -= pickCard;
		SocketEventHandle.getInstance().otherPickCardCallBack -= otherPickCard;
		SocketEventHandle.getInstance().putOutCardCallBack -= otherPutOutCard;
		SocketEventHandle.getInstance().otherUserJointRoomCallBack -= otherUserJointRoom;
		SocketEventHandle.getInstance().PengCardCallBack -= otherPeng;
		SocketEventHandle.getInstance().GangCardCallBack -= gangResponse;
		SocketEventHandle.getInstance().gangCardNotice -= otherGang;
		SocketEventHandle.getInstance().btnActionShow -= actionBtnShow;
		SocketEventHandle.getInstance().HupaiCallBack -= hupaiCallBack;
		SocketEventHandle.getInstance().FinalGameOverCallBack -= finalGameOverCallBack;
		SocketEventHandle.getInstance().outRoomCallback -= outRoomCallbak;
		SocketEventHandle.getInstance().dissoliveRoomResponse -= dissoliveRoomResponse;
		SocketEventHandle.getInstance().gameReadyNotice -= gameReadyNotice;
		SocketEventHandle.getInstance().offlineNotice -= offlineNotice;
		SocketEventHandle.getInstance().onlineNotice -= onlineNotice;
		SocketEventHandle.getInstance().messageBoxNotice -= messageBoxNotice;
		SocketEventHandle.getInstance().returnGameResponse -= returnGameResponse;
		CommonEvent.getInstance().readyGame -= markselfReadyGame;
		CommonEvent.getInstance().closeGamePanel -= exitOrDissoliveRoom;
		SocketEventHandle.getInstance().gameFollowBanderNotice -= gameFollowBanderNotice;

		SocketEventHandle.getInstance().buhuaResponse -= buhuaResponse;
		SocketEventHandle.getInstance().huaGangResponse -= huaGangResponse;
		SocketEventHandle.getInstance().scoreResponse -= scoreResponse;
		SocketEventHandle.getInstance().buhuaBeginResponse -= buhuaBeginResponse;
	}


	private void initArrayList()
	{
		mineList = new List<List<int>>();
		handerCardList = new List<List<GameObject>>(); //手牌的链表
		tableCardList = new List<List<GameObject>>(); //桌牌的链表

		for (int i = 0; i < 4; i++) {
			handerCardList.Add(new List<GameObject>());
			tableCardList.Add(new List<GameObject>());
		}

		PengGangList_L = new List<List<GameObject>>(); //左边的碰杠链表
		PengGangList_R = new List<List<GameObject>>(); //右边的碰杠链表
		PengGangList_T = new List<List<GameObject>>(); //顶部的碰杠链表
		PengGangCardList = new List<List<GameObject>>();

		/**
     	* 补花的链表
     	* */
		buhuaList_L = new List<GameObject>();
		buhuaList_R = new List<GameObject>();
		buhuaList_T = new List<GameObject>(); 
		buhuaListB = new List<GameObject>();

		tingCardList = new List<GameObject>(); //听牌的链表
	}

	/// <summary>
	/// Cards the select.
	/// </summary>
	/// <param name="obj">Object.</param>
	public void cardSelect(GameObject obj)
	{
		for (int i = 0; i < handerCardList [0].Count; i++) {
			if (handerCardList [0] [i] == null) {
				handerCardList [0].RemoveAt(i);
				i--;
			} else {
				handerCardList [0] [i].transform.localPosition = new Vector3(handerCardList [0] [i].transform.localPosition.x, 0); //从右到左依次对齐
				handerCardList [0] [i].transform.GetComponent<bottomScript>().selected = false;
			}
		}
		if (obj != null) {
			obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, 20);
			var card = obj.transform.GetComponent<bottomScript>();
			card.selected = true;
			SoundCtrl.getInstance().playSoundOther("sort");
			if (GlobalDataScript.isDrag) {
				onCardSelected(card.getPoint());
			}
		}
	}

	private void soundStartGame()
	{
		SoundCtrl.getInstance().playSoundOther("open");
	}

	/// <summary>
	/// 开始游戏
	/// </summary>
	/// <param name="response">Response.</param>
	public void startGame(ClientResponse response)
	{
		GlobalDataScript.roomAvatarVoList = avatarList;
		//GlobalDataScript.surplusTimes -= 1;
		StartGameVO sgvo = JsonMapper.ToObject<StartGameVO>(response.message);
		bankerId = sgvo.bankerId;
		cleanGameplayUI();
		GlobalDataScript.surplusTimes--;
		curDirString = getDirection(bankerId);

		LeavedRoundNumText.text = (GlobalDataScript.totalTimes - GlobalDataScript.surplusTimes)
		+ "/" + GlobalDataScript.totalTimes;//刷新剩余圈数
		
		if (!isFirstOpen) {
			btnActionScript = gameObject.GetComponent<ButtonActionScript>();
			initPanel();
			initArrayList();
			avatarList [bankerId].main = true;
		}

		if (sgvo.bixiahu)
			bixiahuText.SetActive(true);

		GlobalDataScript.finalGameEndVo = null;
		GlobalDataScript.mainUuid = avatarList [bankerId].account.uuid;
		initArrayList();
		curDirString = getDirection(bankerId);
		playerItems [curDirIndex].setbankImgEnable(true);
		SetDirGameObjectAction();
		isFirstOpen = false;
		GlobalDataScript.isOverByPlayer = false;

		mineList = sgvo.paiArray;

		UpateTimeReStart();

		setAllPlayerReadImgVisbleToFalse();
		initMyCardListAndOtherCard(13, 13, 13);

		ShowLeavedCardsNumForInit();
		ShowBuhuaInit();

		if (curDirString == DirectionEnum.Bottom) {
			GlobalDataScript.isDrag = true;
			onCardChanged();
		} else {
			GlobalDataScript.isDrag = false;
		}
	}

	private void cleanGameplayUI()
	{
		canClickButtonFlag = true;
		buhuaBegin = false;
		inviteFriendButton.transform.gameObject.SetActive(false);
		live1.transform.gameObject.SetActive(true);
		live2.transform.gameObject.SetActive(true);
		centerImage.transform.gameObject.SetActive(true);
		liujuEffectGame.SetActive(false);
		tableShow.SetActive(true);
		bixiahuText.SetActive(false);
		if (GlobalDataScript.surplusTimes == GlobalDataScript.totalTimes) {
			startGameShow.SetActive(true);
			soundStartGame();
		}

		tingPanel.SetActive(false);

		ShowBuhuaInit();
	}

	private void ShowBuhuaInit()
	{
		buhuaTextB.text = "0";
		buhuaTextT.text = "0";
		buhuaTextL.text = "0";
		buhuaTextR.text = "0";
	}

	public void ShowLeavedCardsNumForInit()
	{
//		RoomCreateVo roomCreateVo = GlobalDataScript.roomVo;

		LeavedCardsNum = 144; //麻将的总数
		LeavedCardsNum = LeavedCardsNum - 53; //除去玩家的手牌剩余的牌数
		LeavedCastNumText.text = (LeavedCardsNum) + ""; //在界面上显示剩余的总牌数
	}

	public void CardsNumChange()
	{
		LeavedCardsNum--;
		if (LeavedCardsNum < 0) {
			LeavedCardsNum = 0;
		}
		LeavedCastNumText.text = LeavedCardsNum + "";
	}

	/// <summary>
	/// 别人摸牌通知
	/// </summary>
	/// <param name="response">Response.</param>
	public void otherPickCard(ClientResponse response)
	{
		UpateTimeReStart();
		JsonData json = JsonMapper.ToObject(response.message);
		//下一个摸牌人的索引
		int avatarIndex = (int)json ["avatarIndex"];
		Debug.Log("otherPickCard avatarIndex = " + avatarIndex);
		otherPickCardAndCreate(avatarIndex);
		SetDirGameObjectAction();
		CardsNumChange();
	}

	private void otherPickCardAndCreate(int avatarIndex)
	{
		//getDirection (avatarIndex);
		int myIndex = getMyIndexFromList();
		int seatIndex = avatarIndex - myIndex;
		if (seatIndex < 0) {
			seatIndex = 4 + seatIndex;
		}
		curDirString = playerItems [seatIndex].dir;
		//SetDirGameObjectAction ();
		otherMoPaiCreateGameObject(curDirString);
	}

	public void otherMoPaiCreateGameObject(string dir)
	{
		Vector3 tempVector3 = new Vector3(0, 0);
		//Transform tempParent = null;
		switch (dir) {
			case DirectionEnum.Top://上
                             //tempParent = topParent.transform;
				tempVector3 = new Vector3(-273, 0f);
				break;
			case DirectionEnum.Left://左
                              //tempParent = leftParent.transform;
				tempVector3 = new Vector3(0, -173f);

				break;
			case DirectionEnum.Right://右
                               //tempParent = rightParent.transform;
				tempVector3 = new Vector3(0, 183f);
				break;
		}

		String path = "prefab/card/Bottom_" + dir;
		Debug.Log("path  = " + path);
		otherPickCardItem = createGameObjectAndReturn(path, parentList [getIndexByDir(dir)], tempVector3);//实例化当前摸的牌
		otherPickCardItem.transform.localScale = Vector3.one;//原大小

	}

	/// <summary>
	/// 自己摸牌
	/// </summary>
	/// <param name="response">Response.</param>
	public void pickCard(ClientResponse response)
	{

		UpateTimeReStart();
		CardVO cardvo = JsonMapper.ToObject<CardVO>(response.message);
		MoPaiCardPoint = cardvo.cardPoint;
		Debug.Log("摸牌" + MoPaiCardPoint);
		SelfAndOtherPutoutCard = MoPaiCardPoint;
		useForGangOrPengOrChi = cardvo.cardPoint;
		putCardIntoMineList(MoPaiCardPoint);
		moPai();
		curDirString = DirectionEnum.Bottom;
		SetDirGameObjectAction();
		CardsNumChange();
		//checkHuOrGangOrPengOrChi (MoPaiCardPoint,2);
		GlobalDataScript.isDrag = true;
		onCardChanged();
	}

	/// <summary>
	/// 胡，杠，碰，吃，pass按钮显示.
	/// </summary>
	/// <param name="response">Response.</param>
	public void actionBtnShow(ClientResponse response)
	{
		GlobalDataScript.isDrag = false;
		string[] strs = response.message.Split(new char[1] { ',' });
		if (curDirString == DirectionEnum.Bottom) {
			passType = "selfPickCard";
		} else {
			passType = "otherPickCard";
		}

		for (int i = 0; i < strs.Length; i++) {
			if (strs [i].Equals("hu")) {
				btnActionScript.showBtn(1);

			} else if (strs [i].Contains("qianghu")) {

				try {
					SelfAndOtherPutoutCard = int.Parse(strs [i].Split(new char[1] { ':' }) [1]);
				} catch (Exception e) {
				}

				btnActionScript.showBtn(1);
				isQiangHu = true;
			} else if (strs [i].Contains("peng")) {
				btnActionScript.showBtn(3);
				putOutCardPoint = int.Parse(strs [i].Split(new char[1] { ':' }) [2]);


			} else if (strs [i].Equals("chi")) {
				//btnActionScript.showBtn (3);
			}
			if (strs [i].Contains("gang")) {

				btnActionScript.showBtn(2);
				gangPaiList = strs [i].Split(new char[1] { ':' });
				List<string> gangPaiListTemp = gangPaiList.ToList();
				gangPaiListTemp.RemoveAt(0);
				gangPaiList = gangPaiListTemp.ToArray();
			}
		}
	}

	/// <summary>
	/// 初始化其它玩家的手牌
	/// </summary>
	/// <param name="topCount"></param>
	/// <param name="leftCount"></param>
	/// <param name="rightCount"></param>
	private void initMyCardListAndOtherCard(int topCount, int leftCount, int rightCount)
	{
		for (int a = 0; a < mineList [0].Count; a++) {//我的牌13张
			Debug.Log("mineList[0].Count:" + mineList [0].Count);
			if (mineList [0] [a] > 0) {
				for (int b = 0; b < mineList [0] [a]; b++) {
					Debug.Log("mineList[0][a]:" + mineList [0] [a]);
					GameObject gob = Instantiate(Resources.Load("prefab/card/Bottom_B")) as GameObject;
					//GameObject.Instantiate ("");
					if (gob != null) {//
						gob.transform.SetParent(parentList [0]);//设置父节点
						gob.transform.localScale = new Vector3(1.1f, 1.1f, 1);
						gob.GetComponent<bottomScript>().onSendMessage += cardChange;//发送消息fd
						gob.GetComponent<bottomScript>().reSetPoisiton += cardSelect; //选择的牌
						gob.GetComponent<bottomScript>().setPoint(a);//设置指针  每张牌的索引 
						Debug.Log("aaaa:" + a);
						SetPosition(false);
						handerCardList [0].Add(gob);//增加游戏对象
					} else {
						Debug.Log("--> gob is null");//游戏对象为空
					}
				}

			}
		}

		initOtherCardList(DirectionEnum.Left, leftCount);
		initOtherCardList(DirectionEnum.Right, rightCount);
		initOtherCardList(DirectionEnum.Top, topCount);

		if (bankerId == getMyIndexFromList()) {
			SetPosition(true);//设置位置
			Debug.Log("初始化数据自己为庄家");
			//	checkHuPai();
		} else {
			SetPosition(false);
			otherPickCardAndCreate(bankerId);
		}
	}


	private void setAllPlayerReadImgVisbleToFalse()
	{
		for (int i = 0; i < playerItems.Count; i++) {
			playerItems [i].readyImg.SetActive(false);
		}
	}

	private void setAllPlayerHuImgVisbleToFalse()
	{
		for (int i = 0; i < playerItems.Count; i++) {
			playerItems [i].setHuFlagHidde();
		}
	}

	/// <summary>
	/// Gets the index by dir.
	/// </summary>
	/// <returns>The index by dir.</returns>
	/// <param name="dir">Dir.</param>
	private int getIndexByDir(string dir)
	{
		int result = 0;
		switch (dir) {
			case DirectionEnum.Top: //上
				result = 2;
				break;
			case DirectionEnum.Left: //左
				result = 3;
				break;
			case DirectionEnum.Right: //右
				result = 1;
				break;
			case DirectionEnum.Bottom: //下
				result = 0;
				break;
		}
		return result;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="initDirection"></param>
	private void initOtherCardList(string initDiretion, int count) //初始化
	{
		for (int i = 0; i < count; i++) {
			GameObject temp = Instantiate(Resources.Load("Prefab/card/Bottom_" + initDiretion)) as GameObject; //实例化当前牌
			if (temp != null) { //有可能没牌了
				temp.transform.SetParent(parentList [getIndexByDir(initDiretion)]); //父节点
				temp.transform.localScale = Vector3.one;
				switch (initDiretion) {

					case DirectionEnum.Top: //上
						temp.transform.localPosition = new Vector3(-204 + 38 * i, 0); //位置   
						handerCardList [2].Add(temp);
						temp.transform.localScale = Vector3.one; //原大小
						break;

					case DirectionEnum.Left: //左
						temp.transform.localPosition = new Vector3(0, -100 + i * 23); //位置   
						temp.transform.SetSiblingIndex(0);
						handerCardList [3].Add(temp);
						break;

					case DirectionEnum.Right: //右
						temp.transform.localPosition = new Vector3(0, 119 - i * 23); //位置     
						handerCardList [1].Add(temp);
						break;
				}
			}

		}
	}

	/// <summary>
	/// 
	/// </summary>
	public void moPai() //摸牌
	{
		pickCardItem = Instantiate(Resources.Load("prefab/card/Bottom_B")) as GameObject; //实例化当前摸的牌
		Debug.Log("摸牌 === >> " + MoPaiCardPoint);
		if (pickCardItem != null) { //有可能没牌了
			pickCardItem.name = "pickCardItem";
			pickCardItem.transform.SetParent(parentList [0]); //父节点
			pickCardItem.transform.localScale = new Vector3(1.1f, 1.1f, 1);//原大小
			pickCardItem.transform.localPosition = new Vector3(550f, 0); //位置
			pickCardItem.GetComponent<bottomScript>().onSendMessage += cardChange; //发送消息
			pickCardItem.GetComponent<bottomScript>().reSetPoisiton += cardSelect;
			pickCardItem.GetComponent<bottomScript>().setPoint(MoPaiCardPoint); //得到索引
			insertCardIntoList(pickCardItem);
		}
		Debug.Log("moPai  goblist count === >> " + handerCardList [0].Count);

	}

	public void putCardIntoMineList(int cardPoint)
	{
		if (mineList [0] [cardPoint] < 4) {
			mineList [0] [cardPoint]++;

		}
	}

	public void pushOutFromMineList(int cardPoint)
	{

		if (mineList [0] [cardPoint] > 0) {
			mineList [0] [cardPoint]--;
		}
	}

	/// <summary>
	/// 接收到其它人的出牌通知
	/// </summary>
	/// <param name="response">Response.</param>
	public void otherPutOutCard(ClientResponse response)
	{

		JsonData json = JsonMapper.ToObject(response.message);
		int cardPoint = (int)json ["cardIndex"];
		int curAvatarIndex = (int)json ["curAvatarIndex"];
		putOutCardPointAvarIndex = getIndexByDir(getDirection(curAvatarIndex));
		Debug.Log("otherPickCard avatarIndex = " + curAvatarIndex);
		useForGangOrPengOrChi = cardPoint;
		if (otherPickCardItem != null) {
			int dirIndex = getIndexByDir(getDirection(curAvatarIndex));
			Destroy(otherPickCardItem);
			otherPickCardItem = null;

		} else {
			int dirIndex = getIndexByDir(getDirection(curAvatarIndex));
			GameObject obj = handerCardList [dirIndex] [0];
			handerCardList [dirIndex].RemoveAt(0);
			Destroy(obj);

		}
		createPutOutCardAndPlayAction(cardPoint, curAvatarIndex);
	}

	/// <summary>
	/// 创建打来的的牌对象，并且开始播放动画
	/// </summary>
	/// <param name="cardPoint">Card point.</param>
	/// <param name="curAvatarIndex">Current avatar index.</param>
	private void createPutOutCardAndPlayAction(int cardPoint, int curAvatarIndex)
	{
		outDir = getDirection(curAvatarIndex);
		SoundCtrl.getInstance().playSound(cardPoint, avatarList [curAvatarIndex].account.sex, outDir);
		SoundCtrl.getInstance().playSoundGame("tileout");
		Vector3 tempVector3 = new Vector3(0, 0);

		switch (outDir) {
			case DirectionEnum.Top: //上
				tempVector3 = new Vector3(0, 140f);
				break;
			case DirectionEnum.Left: //左
				tempVector3 = new Vector3(-370, 0f);
				break;
			case DirectionEnum.Right: //右
				tempVector3 = new Vector3(370, 0f);
				break;
			case DirectionEnum.Bottom:
				tempVector3 = new Vector3(0, -140f);
				break;
		}

		GameObject tempGameObject = createGameObjectAndReturn("Prefab/card/PutOutCard", parentList [4], tempVector3);
		tempGameObject.name = "putOutCard";
		tempGameObject.transform.localScale = Vector3.one;
		tempGameObject.GetComponent<TopAndBottomCardScript>().setPoint(cardPoint);
		putOutCardPoint = cardPoint;
		SelfAndOtherPutoutCard = cardPoint;
		putOutCard = tempGameObject;
		destroyPutOutCard(cardPoint);
		if (putOutCard != null) {
			Destroy(putOutCard, 1f);
		}
	}


	/// <summary>
	/// 根据一个人在数组里的索引，得到这个人所在的方位，L-左，T-上,R-右，B-下（自己的方位永远都是在下方）
	/// </summary>
	/// <returns>The direction.</returns>
	/// <param name="avatarIndex">Avatar index.</param>
	private String getDirection(int avatarIndex)
	{
		String result = DirectionEnum.Bottom;
		int myselfIndex = getMyIndexFromList();
		if (myselfIndex == avatarIndex) {
			Debug.Log("getDirection == B");
			curDirIndex = 0;
			return result;
		}
		//从自己开始计算，下一位的索引
		for (int i = 0; i < 4; i++) {
			myselfIndex++;
			if (myselfIndex >= 4) {
				myselfIndex = 0;
			}
			if (myselfIndex == avatarIndex) {
				if (i == 0) {
					Debug.Log("getDirection == R");
					curDirIndex = 1;
					return DirectionEnum.Right;
				} else if (i == 1) {
					Debug.Log("getDirection == T");
					curDirIndex = 2;
					return DirectionEnum.Top;
				} else {
					Debug.Log("getDirection == L");
					curDirIndex = 3;
					return DirectionEnum.Left;
				}
			}
		}
		Debug.Log("getDirection == B");
		curDirIndex = 0;
		return DirectionEnum.Bottom;
	}

	/// <summary>
	/// 设置红色箭头的显示方向
	/// </summary>
	public void SetDirGameObjectAction() //设置方向
	{
		//UpateTimeReStart();
		for (int i = 0; i < dirGameList.Count; i++) {
			dirGameList [i].SetActive(false);
		}
		dirGameList [getIndexByDir(curDirString)].SetActive(true);
	}

	public void ThrowBottom(int index)//
	{
		GameObject temp = null;
		String path = "";
		Vector3 poisVector3 = Vector3.one;

		if (outDir == DirectionEnum.Bottom) {
			path = "Prefab/ThrowCard/TopAndBottomCard";
			poisVector3 = new Vector3(-261 + tableCardList [0].Count % 11 * 37, (int)(tableCardList [0].Count / 11) * 60f);
			GlobalDataScript.isDrag = false;
		} else if (outDir == DirectionEnum.Right) {
			path = "Prefab/ThrowCard/ThrowCard_R";
			poisVector3 = new Vector3((int)(-tableCardList [1].Count / 11 * 54f), -180f + tableCardList [1].Count % 11 * 28);
		} else if (outDir == DirectionEnum.Top) {
			path = "Prefab/ThrowCard/TopAndBottomCard";
			poisVector3 = new Vector3(289f - tableCardList [2].Count % 11 * 37, -(int)(tableCardList [2].Count / 11) * 60f);
		} else if (outDir == DirectionEnum.Left) {
			path = "Prefab/ThrowCard/ThrowCard_L";
			poisVector3 = new Vector3(tableCardList [3].Count / 11 * 54f, 152f - tableCardList [3].Count % 11 * 28);
			//     parenTransform = leftOutParent;
		}

		temp = createGameObjectAndReturn(path, outparentList [curDirIndex], poisVector3);
		temp.transform.localScale = Vector3.one;
		if (outDir == DirectionEnum.Right || outDir == DirectionEnum.Left) {
			temp.GetComponent<TopAndBottomCardScript>().setLefAndRightPoint(index);
		} else {
			temp.GetComponent<TopAndBottomCardScript>().setPoint(index);
		}

		cardOnTable = temp;
		//temp.transform.SetAsLastSibling();
		tableCardList [getIndexByDir(outDir)].Add(temp);
		if (outDir == DirectionEnum.Right) {
			temp.transform.SetSiblingIndex(0);
		}
		//丢牌上
		//顶针下
		setPointGameObject(temp);
	}

	public void otherPeng(ClientResponse response)//其他人碰牌
	{
		UpateTimeReStart();
		OtherPengGangBackVO cardVo = JsonMapper.ToObject<OtherPengGangBackVO>(response.message);
		otherPengCard = cardVo.cardPoint;
		curDirString = getDirection(cardVo.avatarId);
		SetDirGameObjectAction();
		effectType = "peng";
		//pengGangHuEffectCtrl();
		SoundCtrl.getInstance().playSoundByAction("peng", avatarList [cardVo.avatarId].account.sex, curDirString);
		SoundCtrl.getInstance().playSoundGame("peng");
		if (cardOnTable != null) {
			reSetOutOnTabelCardPosition(cardOnTable);
			Destroy(cardOnTable);
		}


		if (curDirString == DirectionEnum.Bottom) {  //==============================================自己碰牌
			mineList [0] [putOutCardPoint]++;
			mineList [1] [putOutCardPoint] = 2;
			int removeCount = 0;
			for (int i = 0; i < handerCardList [0].Count; i++) {
				GameObject temp = handerCardList [0] [i];
				int tempCardPoint = temp.GetComponent<bottomScript>().getPoint();
				if (tempCardPoint == putOutCardPoint) {

					handerCardList [0].RemoveAt(i);
					Destroy(temp);
					i--;
					removeCount++;
					if (removeCount == 2) {
						break;
					}
				}
			}
			SetPosition(true);
			bottomPeng();
			pengGangHuEffectCtrl();
		} else {//==============================================其他人碰牌
			List<GameObject> tempCardList = handerCardList [getIndexByDir(curDirString)];
			string path = "Prefab/PengGangCard/PengGangCard_" + curDirString;
			if (tempCardList != null) {
				Debug.Log("tempCardList.count======前" + tempCardList.Count);
				for (int i = 0; i < 2; i++) {//消除其他的人牌碰牌长度
					GameObject temp = tempCardList [0];
					Destroy(temp);
					tempCardList.RemoveAt(0);

				}
				Debug.Log("tempCardList.count======前" + tempCardList.Count);

				otherPickCardItem = tempCardList [0];
				gameTool.setOtherCardObjPosition(tempCardList, curDirString, 1);
				//Destroy (tempCardList [0]);
				tempCardList.RemoveAt(0);
			}
			Vector3 tempvector3 = new Vector3(0, 0, 0);
			List<GameObject> tempList = new List<GameObject>();

			switch (curDirString) {
				case DirectionEnum.Right:
					{
						int flagTurn = -1;
						for (int i = 0; i < 3; i++) {
							GameObject obj;
							TopAndBottomCardScript card;
							if ((putOutCardPointAvarIndex + i) == 4 || (putOutCardPointAvarIndex + i) == 0) {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T", pengGangParenTransformR.transform,
									new Vector3(7, -122 + PengGangList_R.Count * 112 + i * 26f + 6));
								obj.transform.SetSiblingIndex(0);
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setPoint(cardVo.cardPoint);
								flagTurn = i;
								card.isTurn = true;
							} else {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_R", pengGangParenTransformR.transform,
									new Vector3(0, -122 + PengGangList_R.Count * 112 + i * 26f + (flagTurn > -1 ? 12 : 0)));
								obj.transform.SetSiblingIndex(0);
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setLefAndRightPoint(cardVo.cardPoint);
							}
							tempList.Add(obj);
						}
					}
					break;
				case DirectionEnum.Top:
					{
						int flagTurn = -1;
						for (int i = 0; i < 3; i++) {
							GameObject obj;
							TopAndBottomCardScript card;
							if ((i == 0 && putOutCardPointAvarIndex == 3) || (i == 1 && putOutCardPointAvarIndex == 0) || (i == 2 && putOutCardPointAvarIndex == 1)) {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L", pengGangParenTransformT.transform,
									new Vector3(251 - PengGangList_T.Count * 133f + i * 37 + 6.5f, 7, 0));
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setLefAndRightPoint(cardVo.cardPoint);
								flagTurn = i;
								card.isTurn = true;
							} else {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T", pengGangParenTransformT.transform,
									new Vector3(251 - PengGangList_T.Count * 133f + i * 37 + (flagTurn > -1 ? 13 : 0), 0, 0));
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setPoint(cardVo.cardPoint);
							}
							tempList.Add(obj);
						}
					}
					break;
				case DirectionEnum.Left:
					{
						int flagTurn = -1;
						for (int i = 0; i < 3; i++) {
							GameObject obj;
							TopAndBottomCardScript card;
							if ((putOutCardPointAvarIndex + i) == 2) {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T", pengGangParenTransformL.transform,
									new Vector3(-7, 122 - PengGangList_L.Count * 112 - i * 26f - 6, 0));
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setPoint(cardVo.cardPoint);
								flagTurn = i;
								card.isTurn = true;
							} else {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L", pengGangParenTransformL.transform,
									new Vector3(0, 122 - PengGangList_L.Count * 112 - i * 26f - (flagTurn > -1 ? 12 : 0), 0));
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setLefAndRightPoint(cardVo.cardPoint);
							}
							tempList.Add(obj);
						}
					}
					break;
			}
			addListToPengGangList(curDirString, tempList);
		}
	}

	private void bottomPeng()
	{
		List<GameObject> templist = new List<GameObject>();
		int flagTurn = -1;
		for (int i = 0; i < 3; i++) {
			GameObject obj;
			TopAndBottomCardScript card;
			if (i + putOutCardPointAvarIndex == 3) {
				obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L",
					pengGangParenTransformB.transform,
					new Vector3(-370 + PengGangCardList.Count * 210 + i * 60f + 10, -12));
				obj.transform.localScale = new Vector3(1.6f, 1.6f, 1);
				card = obj.GetComponent<TopAndBottomCardScript>();
				card.setLefAndRightPoint(putOutCardPoint);
				flagTurn = i;
				card.isTurn = true;
			} else {
				obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_B",
					pengGangParenTransformB.transform,
					new Vector3(-370 + PengGangCardList.Count * 210 + i * 60f + (flagTurn > -1 ? 20 : 0), 0));
				obj.transform.localScale = Vector3.one;
				card = obj.GetComponent<TopAndBottomCardScript>();
				card.setPoint(putOutCardPoint);
			}
			templist.Add(obj);
		}
		PengGangCardList.Add(templist);
		GlobalDataScript.isDrag = true;
		onCardChanged();
	}

	private float buhuaTime = 0;

	private void buhuaEffect(int sex)
	{
		if (buhuaTime > 0)
			return;
		effectType = "buhua";
		pengGangHuEffectCtrl();
		SoundCtrl.getInstance().playSoundByAction("buhua", sex, "");
		buhuaTime = 1;
	}

	public void buhuaResponse(ClientResponse response)
	{
		UpateTimeReStart();
		OtherPengGangBackVO cardVo = JsonMapper.ToObject<OtherPengGangBackVO>(response.message);
		int cardPoint = cardVo.cardPoint;
		curDirString = getDirection(cardVo.avatarId);
		SetDirGameObjectAction();
		buhuaEffect(avatarList [cardVo.avatarId].account.sex);

		if (curDirString == DirectionEnum.Bottom) {
			mineList [0] [cardPoint]++;
			mineList [1] [cardPoint] = 11;
			for (int i = 0; i < handerCardList [0].Count; i++) {
				GameObject temp = handerCardList [0] [i];
				int tempCardPoint = temp.GetComponent<bottomScript>().getPoint();
				if (tempCardPoint == cardPoint) {
					handerCardList [0].RemoveAt(i);
					Destroy(temp);
					i--;
					break;
				}
			}
			SetPosition(false);
		} else {
			if (otherPickCardItem != null) {
				int dirIndex = getIndexByDir(curDirString);
				Destroy(otherPickCardItem);
				otherPickCardItem = null;
			} else {
				List<GameObject> tempCardList = handerCardList [getIndexByDir(curDirString)];
				if (tempCardList != null) {
					GameObject temp = tempCardList [0];
					Destroy(temp);
					tempCardList.RemoveAt(0);
					gameTool.setOtherCardObjPosition(tempCardList, curDirString, 1);
				}
			}
		}
		addBuhua(curDirString, cardPoint);
	}

	public void buhuaBeginResponse(ClientResponse response)
	{
		JsonData data = JsonMapper.ToObject(response.message);
		int cardPoint = Int32.Parse(data ["cardPoint"].ToString());
		int pickPoint = Int32.Parse(data ["pickPoint"].ToString());
		int avatarIndex = Int32.Parse(data ["avatarId"].ToString());
		string dir = getDirection(avatarIndex);

		if (buhuaBegin == false) {
			SoundCtrl.getInstance().playSoundByAction("buhua", avatarList [avatarIndex].account.sex, "");
			buhuaBegin = true;
		}

		if (dir == DirectionEnum.Bottom) {
			mineList [0] [cardPoint]++;
			mineList [1] [cardPoint] = 11;
			for (int i = 0; i < handerCardList [0].Count; i++) {
				GameObject obj = handerCardList [0] [i];
				int tempCardPoint = obj.GetComponent<bottomScript>().getPoint();
				if (tempCardPoint == cardPoint) {
					handerCardList [0].RemoveAt(i);
					Destroy(obj);
					i--;
					break;
				}
			}
			addBuhua(dir, cardPoint);

			putCardIntoMineList(pickPoint);
			GameObject card = createGameObjectAndReturn("prefab/card/Bottom_B", parentList [0], new Vector3(550f, 0));
			card.transform.localScale = new Vector3(1.1f, 1.1f, 1);
			card.GetComponent<bottomScript>().onSendMessage += cardChange;
			card.GetComponent<bottomScript>().reSetPoisiton += cardSelect;
			card.GetComponent<bottomScript>().setPoint(pickPoint);
			insertCardIntoList(card);
			CardsNumChange();

			SetPosition(handerCardList [0].Count == 14);
		} else {
			addBuhua(dir, cardPoint);
			CardsNumChange();
		}
	}

	public void huaGangResponse(ClientResponse response)
	{
		OtherPengGangBackVO cardVo = JsonMapper.ToObject<OtherPengGangBackVO>(response.message);
		int cardPoint = cardVo.cardPoint;
		//curDirString = getDirection(cardVo.avatarId);
		effectType = "huagang";
		pengGangHuEffectCtrl();

		// sound
	}

	private void buhuaListInsert(List<GameObject> buhuaList, GameObject card, int cardPoint)
	{
		bool insertFlag = false;
		for (int i = 0; i < buhuaList.Count; i++) {
			int currentPoint = buhuaList [i].GetComponent<TopAndBottomCardScript>().getPoint();
			if (insertFlag == false && cardPoint < currentPoint) {
				buhuaList.Insert(i, card);
				card.transform.SetSiblingIndex(i);
				insertFlag = true;
				break;
			}
		}
		if (insertFlag == false) {
			buhuaList.Add(card);
		}
	}

	private void addBuhua(string dirString, int cardPoint)
	{
		string path;
		switch (dirString) {
			case DirectionEnum.Bottom:
				{
					path = "Prefab/PengGangCard/PengGangCard_B";
					GameObject obj = createGameObjectAndReturn(path,
						                 huaParent [1].transform, Vector3.zero);
					obj.GetComponent<TopAndBottomCardScript>().setPoint(cardPoint);
					obj.transform.localScale = new Vector3(0.8f, 0.8f, 1);

					buhuaListInsert(buhuaListB, obj, cardPoint);
					buhuaTextB.text = buhuaListB.Count + "";
				}
				break;
			case DirectionEnum.Right:
				{
					path = "Prefab/PengGangCard/PengGangCard_R";
					GameObject obj = createGameObjectAndReturn(path,
						                 huaParent [3].transform, Vector3.zero);
					obj.GetComponent<TopAndBottomCardScript>().setLefAndRightPoint(cardPoint);
					obj.transform.localScale = new Vector3(1.4f, 1.4f, 1);

					buhuaListInsert(buhuaList_R, obj, cardPoint);
					buhuaTextR.text = buhuaList_R.Count + "";
				}
				break;
			case DirectionEnum.Top:
				{
					path = "Prefab/PengGangCard/PengGangCard_B";
					GameObject obj = createGameObjectAndReturn(path,
						                 huaParent [2].transform, Vector3.zero);
					obj.GetComponent<TopAndBottomCardScript>().setPoint(cardPoint);
					obj.transform.localScale = new Vector3(0.8f, 0.8f, 1);

					buhuaListInsert(buhuaList_T, obj, cardPoint);
					buhuaTextT.text = buhuaList_T.Count + "";
				}
				break;
			case DirectionEnum.Left:
				{
					path = "Prefab/PengGangCard/PengGangCard_L";
					GameObject obj = createGameObjectAndReturn(path,
						                 huaParent [0].transform, Vector3.zero);
					obj.GetComponent<TopAndBottomCardScript>().setLefAndRightPoint(cardPoint);
					obj.transform.localScale = new Vector3(1.4f, 1.4f, 1);

					buhuaListInsert(buhuaList_L, obj, cardPoint);
					buhuaTextL.text = buhuaList_L.Count + "";
				}
				break;
		}
	}

	private void pengGangHuEffectCtrl()
	{
		if (effectType == "peng") {
			pengEffectGame.SetActive(true);
			// pengEffectGameList[getIndexByDir(curDirString)].SetActive(true);
		} else if (effectType == "gang") {
			gangEffectGame.SetActive(true);
			// gangEffectGameList[getIndexByDir(curDirString)].SetActive(true);
		} else if (effectType == "hu") {
			huEffectGame.SetActive(true);
			// huEffectGameList[getIndexByDir(curDirString)].SetActive(true);
		} else if (effectType == "liuju") {
			liujuEffectGame.SetActive(true);
		} else if (effectType == "buhua") {
			//buhuaEffectGame.SetActive(true);
		} else if (effectType == "huagang") {
			huagangEffectGame.SetActive(true);
		}
		invokeHidePengGangHuEff();
	}

	private void invokeHidePengGangHuEff()
	{
		Invoke("HidePengGangHuEff", 1f);
	}

	private void HidePengGangHuEff()
	{
		//   pengEffectGameList[getIndexByDir(curDirString)].SetActive(false);
		// gangEffectGameList[getIndexByDir(curDirString)].SetActive(false);
		// huEffectGameList[getIndexByDir(curDirString)].SetActive(false);
		pengEffectGame.SetActive(false);
		gangEffectGame.SetActive(false);
		huEffectGame.SetActive(false);
		huagangEffectGame.SetActive(false);
		//buhuaEffectGame.SetActive(false);
	}

	private void otherGang(ClientResponse response) //其他人杠牌
	{

		GangNoticeVO gangNotice = JsonMapper.ToObject<GangNoticeVO>(response.message);
		otherGangCard = gangNotice.cardPoint;
		otherGangType = gangNotice.type;
		string path = "";
		string path2 = "";
		Vector3 tempvector3 = new Vector3(0, 0, 0);
		curDirString = getDirection(gangNotice.avatarId);
		effectType = "gang";
		//pengGangHuEffectCtrl();
		SetDirGameObjectAction();
		SoundCtrl.getInstance().playSoundByAction("gang", avatarList [gangNotice.avatarId].account.sex, curDirString);
		SoundCtrl.getInstance().playSoundGame("peng");
		List<GameObject> tempCardList = null;


		//确定牌背景（明杠，暗杠）
		switch (curDirString) {
			case DirectionEnum.Right:
				tempCardList = handerCardList [1];
				path = "Prefab/PengGangCard/PengGangCard_R";
				path2 = "Prefab/PengGangCard/GangBack_L&R";
				break;
			case DirectionEnum.Top:
				tempCardList = handerCardList [2];
				path = "Prefab/PengGangCard/PengGangCard_T";
				path2 = "Prefab/PengGangCard/GangBack_T";
				break;
			case DirectionEnum.Left:
				tempCardList = handerCardList [3];
				path = "Prefab/PengGangCard/PengGangCard_L";
				path2 = "Prefab/PengGangCard/GangBack_L&R";
				break;
		}


		List<GameObject> tempList = new List<GameObject>();
		if (getPaiInpeng(otherGangCard, curDirString) == -1) {
			//删除玩家手牌，当玩家碰牌牌组里面的有碰牌时，不用删除手牌
			for (int i = 0; i < 3; i++) {
				GameObject temp = tempCardList [0];
				tempCardList.RemoveAt(0);
				Destroy(temp);
			}
			SetPosition(false);

			if (tempCardList != null) {
				gameTool.setOtherCardObjPosition(tempCardList, curDirString, 2);
			}

			//创建杠牌，当玩家碰牌牌组里面的无碰牌，才创建
			if (otherGangType == 0) {
				if (cardOnTable != null) {
					reSetOutOnTabelCardPosition(cardOnTable);
					Destroy(cardOnTable);
				}
				int flagTurn = -1;
				for (int i = 0; i < 4; i++) { //实例化其他人杠牌
					GameObject obj;
					TopAndBottomCardScript card;
					switch (curDirString) {
						case DirectionEnum.Right:
							if (i != 3) {
								if ((putOutCardPointAvarIndex + i) == 4 || (putOutCardPointAvarIndex + i) == 0) {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T", pengGangParenTransformR.transform,
										new Vector3(7, -122 + PengGangList_R.Count * 112 + i * 26f + 6));
									obj.transform.SetSiblingIndex(0);
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setPoint(otherGangCard);
									flagTurn = i;
									card.isTurn = true;
								} else {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_R", pengGangParenTransformR.transform,
										new Vector3(0, -122 + PengGangList_R.Count * 112 + i * 26f + (flagTurn > -1 ? 12 : 0)));
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setLefAndRightPoint(otherGangCard);
									obj.transform.SetSiblingIndex(0);
								}
							} else {
								if (flagTurn == 1) {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T",
										pengGangParenTransformR.transform, new Vector3(7f, -122 + PengGangList_R.Count * 112 + 48));
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setPoint(otherGangCard);
									card.isTurn = true;
								} else {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_R",
										pengGangParenTransformR.transform, new Vector3(0f, -122 + PengGangList_R.Count * 112 + 42 + (flagTurn == 0 ? 12 : 0)));
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setLefAndRightPoint(otherGangCard);
								}
							}
							tempList.Add(obj);
							break;
						case DirectionEnum.Top:
							if (i != 3) {
								if ((i == 0 && putOutCardPointAvarIndex == 3) || (i == 1 && putOutCardPointAvarIndex == 0) || (i == 2 && putOutCardPointAvarIndex == 1)) {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L", pengGangParenTransformT.transform,
										new Vector3(251 - PengGangList_T.Count * 133f + i * 37 + 6.5f, 7));
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setLefAndRightPoint(otherGangCard);
									flagTurn = i;
									card.isTurn = true;
								} else {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T", pengGangParenTransformT.transform,
										new Vector3(251 - PengGangList_T.Count * 133f + i * 37 + (flagTurn > -1 ? 13 : 0), 0f));
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setPoint(otherGangCard);
								}
							} else {
								if (flagTurn == 1) {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L",
										pengGangParenTransformT.transform, new Vector3(251 - PengGangList_T.Count * 133 + 37f + 6.5f, 20f));
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setLefAndRightPoint(otherGangCard);
									card.isTurn = true;
								} else {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T",
										pengGangParenTransformT.transform, new Vector3(251 - PengGangList_T.Count * 133 + 37f + (flagTurn == 0 ? 13 : 0), 16f));
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setPoint(otherGangCard);
								}
							}
							tempList.Add(obj);
							break;
						case DirectionEnum.Left:
							if (i != 3) {
								if ((putOutCardPointAvarIndex + i) == 2) {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T", pengGangParenTransformL.transform,
										new Vector3(-7f, 122 - PengGangList_L.Count * 112 - i * 26f - 6));
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setPoint(otherGangCard);
									flagTurn = i;
									card.isTurn = true;
								} else {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L", pengGangParenTransformL.transform,
										new Vector3(0f, 122 - PengGangList_L.Count * 112 - i * 26f - (flagTurn > -1 ? 12 : 0)));
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setLefAndRightPoint(otherGangCard);
								}
							} else {
								if (flagTurn == 1) {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T",
										pengGangParenTransformL.transform, new Vector3(-7f, 122 - PengGangList_L.Count * 112 - 16));
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setPoint(otherGangCard);
									card.isTurn = true;
								} else {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L",
										pengGangParenTransformL.transform, new Vector3(0f, 122 - PengGangList_L.Count * 112 - 10 - (flagTurn == 0 ? 12 : 0)));
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setLefAndRightPoint(otherGangCard);
								}
							}
							tempList.Add(obj);
							break;
					}
				}
			} else if (otherGangType == 1) {
				Destroy(otherPickCardItem);
				for (int j = 0; j < 4; j++) {
					GameObject obj2;
					if (j == 3) {
						obj2 = Instantiate(Resources.Load(path)) as GameObject;
					} else {
						obj2 = Instantiate(Resources.Load(path2)) as GameObject;
					}

					switch (curDirString) {
						case DirectionEnum.Right:
							obj2.transform.parent = pengGangParenTransformR.transform;
							if (j == 3) {
								tempvector3 = new Vector3(0f, -122 + PengGangList_R.Count * 112 + 26);
								obj2.GetComponent<TopAndBottomCardScript>().setLefAndRightPoint(otherGangCard);

							} else {
								tempvector3 = new Vector3(0, -122 + PengGangList_R.Count * 112 + j * 26);
								obj2.transform.SetSiblingIndex(0);
							}

							break;
						case DirectionEnum.Top:
							obj2.transform.parent = pengGangParenTransformT.transform;
							if (j == 3) {
								tempvector3 = new Vector3(251 - PengGangList_T.Count * 133f + 37f, 10f);
								obj2.GetComponent<TopAndBottomCardScript>().setPoint(otherGangCard);
							} else {
								tempvector3 = new Vector3(251 - PengGangList_T.Count * 133f + j * 37, 0f);
							}

							break;
						case DirectionEnum.Left:
							obj2.transform.parent = pengGangParenTransformL.transform;
							if (j == 3) {
								tempvector3 = new Vector3(0f, 122 - PengGangList_L.Count * 112 - 26, 0);
								obj2.GetComponent<TopAndBottomCardScript>().setLefAndRightPoint(otherGangCard);
							} else {
								tempvector3 = new Vector3(0, 122 - PengGangList_L.Count * 112 - j * 26f, 0);
							}

							break;
					}

					obj2.transform.localScale = Vector3.one;
					obj2.transform.localPosition = tempvector3;
					tempList.Add(obj2);
				}


			}
			addListToPengGangList(curDirString, tempList);
			//Destroy (otherPickCardItem);

		} else if (getPaiInpeng(otherGangCard, curDirString) != -1) {/////////end of if(getPaiInpeng(otherGangCard,curDirString) == -1)

			int gangIndex = getPaiInpeng(otherGangCard, curDirString);

			if (otherPickCardItem != null) {
				Destroy(otherPickCardItem);
			}

			GameObject obj;
			TopAndBottomCardScript card;
			switch (curDirString) {
				case DirectionEnum.Top:
					{
						int turnIndex = -1;
						for (int i = 0; i < 3; i++) {
							if (PengGangList_T [gangIndex] [i].GetComponent<TopAndBottomCardScript>().isTurn) {
								turnIndex = i;
								break;
							}
						}

						if (turnIndex == 1) {
							obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L",
								pengGangParenTransformT.transform, new Vector3(251 - gangIndex * 133 + 37f + 6.5f, 20f));
							card = obj.GetComponent<TopAndBottomCardScript>();
							card.setLefAndRightPoint(otherGangCard);
							card.isTurn = true;
						} else {
							obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T",
								pengGangParenTransformT.transform, new Vector3(251 - gangIndex * 133 + 37f + (turnIndex == 0 ? 13 : 0), 16f));
							card = obj.GetComponent<TopAndBottomCardScript>();
							card.setPoint(otherGangCard);
						}
						PengGangList_T [gangIndex].Add(obj);
					}
					break;
				case DirectionEnum.Left:
					{
						int turnIndex = -1;
						for (int i = 0; i < 3; i++) {
							if (PengGangList_L [gangIndex] [i].GetComponent<TopAndBottomCardScript>().isTurn) {
								turnIndex = i;
								break;
							}
						}
						if (turnIndex == 1) {
							obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T",
								pengGangParenTransformL.transform, new Vector3(-7f, 122 - gangIndex * 112 - 16));
							card = obj.GetComponent<TopAndBottomCardScript>();
							card.setPoint(otherGangCard);
							card.isTurn = true;
						} else {
							obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L",
								pengGangParenTransformL.transform, new Vector3(0f, 122 - gangIndex * 112 - 10 - (turnIndex == 0 ? 12 : 0)));
							card = obj.GetComponent<TopAndBottomCardScript>();
							card.setLefAndRightPoint(otherGangCard);
						}

						PengGangList_L [gangIndex].Add(obj);
					}
					break;
				case DirectionEnum.Right:
					{
						int turnIndex = -1;
						for (int i = 0; i < 3; i++) {
							if (PengGangList_R [gangIndex] [i].GetComponent<TopAndBottomCardScript>().isTurn) {
								turnIndex = i;
								break;
							}
						}

						if (turnIndex == 1) {
							obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T",
								pengGangParenTransformR.transform, new Vector3(7f, -122 + gangIndex * 112 + 48));
							card = obj.GetComponent<TopAndBottomCardScript>();
							card.setPoint(otherGangCard);
							card.isTurn = true;
						} else {
							obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_R",
								pengGangParenTransformR.transform, new Vector3(0f, -122 + gangIndex * 112 + 42 + (turnIndex == 0 ? 12 : 0)));
							card = obj.GetComponent<TopAndBottomCardScript>();
							card.setLefAndRightPoint(otherGangCard);
						}

						PengGangList_R [gangIndex].Add(obj);
					}
					break;
			}
		}
	}


	private void addListToPengGangList(string dirString, List<GameObject> tempList)
	{
		switch (dirString) {
			case DirectionEnum.Right:
				PengGangList_R.Add(tempList);
				break;
			case DirectionEnum.Top:
				PengGangList_T.Add(tempList);
				break;
			case DirectionEnum.Left:
				PengGangList_L.Add(tempList);
				break;
		}
	}

	/**
	 * 
	 * 判断碰牌的牌组里面是否包含某个牌，用于判断是否实例化一张牌还是三张牌
	 * cardpoint：牌点
	 * direction：方向
	 * 返回-1  代表没有牌
	 * 其余牌在list的位置
	 */
	private int getPaiInpeng(int cardPoint, string direction)
	{
		List<List<GameObject>> jugeList = new List<List<GameObject>>();
		switch (direction) {
			case DirectionEnum.Bottom://自己
				jugeList = PengGangCardList;
				break;
			case DirectionEnum.Right:
				jugeList = PengGangList_R;
				break;
			case DirectionEnum.Left:
				jugeList = PengGangList_L;
				break;
			case DirectionEnum.Top:
				jugeList = PengGangList_T;
				break;
		}

		if (jugeList == null || jugeList.Count == 0) {

			return -1;
		}

		//循环遍历比对点数
		for (int i = 0; i < jugeList.Count; i++) {

			try {
				if (jugeList [i] [0].GetComponent<TopAndBottomCardScript>().getPoint() == cardPoint) {
					return i;
				}
			} catch (Exception e) {
				return -1;
			}

		}

		return -1;
	}


	private void setPointGameObject(GameObject parent)
	{
		if (parent != null) {
			if (Pointertemp == null) {
				Pointertemp = Instantiate(Resources.Load("Prefab/Pointer")) as GameObject;
			}
			Pointertemp.transform.SetParent(parent.transform);
			Pointertemp.transform.localScale = Vector3.one;
			Pointertemp.transform.localPosition = new Vector3(0f, parent.transform.GetComponent<RectTransform>().sizeDelta.y / 2 + 10);
		}
	}
	//顶针实现

	public void onCardChanged()
	{
		for (int i = 0; i < handerCardList [0].Count; i++) {
			var card = handerCardList [0] [i].GetComponent<bottomScript>();
			if (card.getPoint() >= NanjingConfig.HUA_INDEX)
				return;
		}

		hideTingCard();
		isWillTing = false;

		int count = handerCardList [0].Count;
		if (count == 14 || count == 11 || count == 8 || count == 5 || count == 2) {
			List<int> tingCard = gameTool.CheckTingPickCard(handerCardList [0], PengGangCardList,
				                     (LeavedCardsNum <= 16 ? GameToolScript.Rule.Hu_Flag_Haidilao : 0) | buhuaListB.Count << 8);
			for (int i = 0; i < handerCardList [0].Count; i++) {
				var card = handerCardList [0] [i].GetComponent<bottomScript>();
				if (tingCard.Contains(card.getPoint())) {
					card.ting = true;
					isWillTing = true;
				}
			}
		} else {
			for (int i = 0; i < handerCardList [0].Count; i++) {
				handerCardList [0] [i].GetComponent<bottomScript>().ting = false;
			}
			int result = gameTool.CheckTing(handerCardList [0], PengGangCardList,
				             (LeavedCardsNum <= 16 ? GameToolScript.Rule.Hu_Flag_Haidilao : 0) | (buhuaListB.Count << 8));
			if (result > 0) {
				var cardList = gameTool.CheckTingCard(handerCardList [0], PengGangCardList,
					               (LeavedCardsNum <= 16 ? GameToolScript.Rule.Hu_Flag_Haidilao : 0) | (buhuaListB.Count << 8));
				if (cardList.Count > 0) {
					showTingCard(cardList);
				}
			}
		}
	}

	private void showTingCard(List<int> cardList)
	{
		if (cardList.Count > 0) {
			tingPanel.SetActive(true);
			cleanList(tingCardList);
			for (int i = 0; i < cardList.Count; i++) {
				GameObject card = createGameObjectAndReturn("prefab/card/Bottom_B", tingPosition, new Vector3(i * 30f, 0));
				card.transform.localScale = new Vector3(0.4f, 0.4f, 1);
				card.GetComponent<bottomScript>().setPoint(cardList [i]);
				tingCardList.Add(card);
			}
			tingPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(40f + cardList.Count * 30f, 50f);
		} else {
			hideTingCard();
		}
	}

	private void hideTingCard()
	{
		cleanList(tingCardList);
		tingPanel.SetActive(false);
	}

	public void onCardSelected(int card)
	{
		if (isWillTing) {
			var cardList = gameTool.CheckTingCard(handerCardList [0], card, PengGangCardList,
				               (LeavedCardsNum <= 16 ? GameToolScript.Rule.Hu_Flag_Haidilao : 0) | (buhuaListB.Count << 8));
			showTingCard(cardList);
		} else {
			hideTingCard();
		}
	}

	/// <summary>
	/// 自己打出来的牌
	/// </summary>
	/// <param name="obj">Object.</param>
	public void cardChange(GameObject obj)//
	{
		int cardPoint = obj.GetComponent<bottomScript>().getPoint();//将当期打出牌的点数传出
		if (cardPoint >= NanjingConfig.HUA_INDEX)
			return;
		if (SocketEventHandle.getInstance().isWaiting)
			return;
		int handCardCount = handerCardList [0].Count - 1;
		// check hua
		for (int i = 0; i < handerCardList [0].Count; i++) {
			if (handerCardList [0] [i].GetComponent<bottomScript>().getPoint() >= NanjingConfig.HUA_INDEX)
				return;
		}
		if (handCardCount == 13 || handCardCount == 10 || handCardCount == 7 || handCardCount == 4 || handCardCount == 1) {
			GlobalDataScript.isDrag = false;
			obj.GetComponent<bottomScript>().onSendMessage -= cardChange;
			obj.GetComponent<bottomScript>().reSetPoisiton -= cardSelect;

			pushOutFromMineList(cardPoint);                         //将牌的索引从minelist里面去掉
			handerCardList [0].Remove(obj);
			Debug.Log("cardchange  goblist count = > " + handerCardList [0].Count);
			Destroy(obj);
			SetPosition(false);
			createPutOutCardAndPlayAction(cardPoint, getMyIndexFromList());//讲拖出牌进行第一段动画的播放
			outDir = DirectionEnum.Bottom;
			//========================================================================
			CardVO cardvo = new CardVO();
			cardvo.cardPoint = cardPoint;
			putOutCardPointAvarIndex = getIndexByDir(getDirection(getMyIndexFromList()));
			CustomSocket.getInstance().sendMsg(new PutOutCardRequest(cardvo));

			onCardChanged();
		}

	}

	private void cardGotoTable() //动画第二段
	{
		Debug.Log("==cardGotoTable=Invoke=====>");

		if (outDir == DirectionEnum.Bottom) {
			if (putOutCard != null) {
				putOutCard.transform.DOLocalMove(new Vector3(-261f + tableCardList [0].Count * 39, -133f), 0.4f);
				putOutCard.transform.DOScale(new Vector3(0.5f, 0.5f), 0.4f);
			}
		} else if (outDir == DirectionEnum.Right) {
			if (putOutCard != null) {
				putOutCard.transform.DOLocalRotate(new Vector3(0, 0, 95), 0.4f);
				putOutCard.transform.DOLocalMove(new Vector3(448f, -140f + tableCardList [1].Count * 28), 0.4f);
				putOutCard.transform.DOScale(new Vector3(0.5f, 0.5f), 0.4f);
			}
		} else if (outDir == DirectionEnum.Top) {
			if (putOutCard != null) {
				putOutCard.transform.DOLocalMove(new Vector3(250f - tableCardList [2].Count * 39, 173f), 0.4f);
				putOutCard.transform.DOScale(new Vector3(0.5f, 0.5f), 0.4f);
			}
		} else if (outDir == DirectionEnum.Left) {
			if (putOutCard != null) {
				putOutCard.transform.DOLocalRotate(new Vector3(0, 0, -95), 0.4f);
				putOutCard.transform.DOLocalMove(new Vector3(-364f, 160f - tableCardList [3].Count * 28), 0.4f);
				putOutCard.transform.DOScale(new Vector3(0.5f, 0.5f), 0.4f);
			}
		}
		Invoke("destroyPutOutCard", 0.5f);
	}

	public void insertCardIntoList(GameObject item)//插入牌的方法
	{
		if (item != null) {
			int curCardPoint = item.GetComponent<bottomScript>().getPoint();//得到当前牌指针
			for (int i = 0; i < handerCardList [0].Count; i++) {//i<游戏物体个数 自增
				int cardPoint = handerCardList [0] [i].GetComponent<bottomScript>().getPoint();//得到所有牌指针
				if (cardPoint >= curCardPoint) {//牌指针>=当前牌的时候插入
					handerCardList [0].Insert(i, item);//在
					return;
				}
			}
			handerCardList [0].Add(item);//游戏对象列表添加当前牌
		}
		item = null;
	}

	public void SetPosition(bool flag)//设置位置
	{
		int count = handerCardList [0].Count;
		//int startX = 594 - count*79;
		int startX = 594 - count * 80;
		if (flag) {
			for (int i = 0; i < count - 1; i++) {
				handerCardList [0] [i].transform.localPosition = new Vector3(startX + i * 80f, 0); //从左到右依次对齐
			}
			handerCardList [0] [count - 1].transform.localPosition = new Vector3(550f, 0); //从左到右依次对齐

		} else {
			for (int i = 0; i < count; i++) {
				handerCardList [0] [i].transform.localPosition = new Vector3(startX + i * 80f - 80f, 0); //从左到右依次对齐
			}
		}
	}

	/// <summary>
	/// 销毁出的牌，并且检测是否可以碰
	/// </summary>
	private void destroyPutOutCard(int cardPoint)
	{
		ThrowBottom(cardPoint);

		if (outDir != DirectionEnum.Bottom) {
			gangKind = 0;
			//checkHuOrGangOrPengOrChi (Point,1);
		}

	}

	void Update()
	{
		if (buhuaTime > 0) {
			buhuaTime -= Time.deltaTime;
		}
		timer -= Time.deltaTime;
		if (timer < 0) {
			timer = 0;
			//UpateTimeReStart();
		}
		Number.text = Math.Floor(timer) + "";

		if (timeFlag) {
			showTimeNumber--;
			if (showTimeNumber < 0) {
				timeFlag = false;
				showTimeNumber = 0;
				playNoticeAction();
			}
		}
	}

	private void playNoticeAction()
	{
		noticeGameObject.SetActive(true);


		if (GlobalDataScript.noticeMegs != null && GlobalDataScript.noticeMegs.Count != 0) {
			noticeText.transform.localPosition = new Vector3(500, noticeText.transform.localPosition.y);
			noticeText.text = GlobalDataScript.noticeMegs [showNoticeNumber];
			float time = noticeText.text.Length * 0.5f + 422f / 56f;

			Tweener tweener = noticeText.transform.DOLocalMove(
				                  new Vector3(-noticeText.text.Length * 28, noticeText.transform.localPosition.y), time)
        .OnComplete(moveCompleted);
			tweener.SetEase(Ease.Linear);
			//tweener.SetLoops(-1);
		}
	}

	void moveCompleted()
	{
		showNoticeNumber++;
		if (showNoticeNumber == GlobalDataScript.noticeMegs.Count) {
			showNoticeNumber = 0;
		}
		noticeGameObject.SetActive(false);
		randShowTime();
		timeFlag = true;
	}

	/// <summary>
	/// 重新开始计时
	/// </summary>
	void UpateTimeReStart()
	{
		timer = 16;
	}

	/// <summary>
	/// 点击放弃按钮
	/// </summary>
	public void myPassBtnClick()
	{
		btnActionScript.cleanBtnShow();
		if (passType == "selfPickCard") {
			GlobalDataScript.isDrag = true;
			onCardChanged();
		}
		passType = "";
		CustomSocket.getInstance().sendMsg(new GaveUpRequest());
	}

	public void myPengBtnClick()
	{
		GlobalDataScript.isDrag = true;
		UpateTimeReStart();
		CardVO cardvo = new CardVO();
		cardvo.cardPoint = putOutCardPoint;
		CustomSocket.getInstance().sendMsg(new PengCardRequest(cardvo));
		btnActionScript.cleanBtnShow();
		onCardChanged();
	}



	public void gangResponse(ClientResponse response)
	{
		UpateTimeReStart();
		GangBackVO gangBackVo = JsonMapper.ToObject<GangBackVO>(response.message);
		gangKind = gangBackVo.type;
		int Num = 0;
		bool pengOrNot = false;

		if (gangBackVo.cardList.Count == 0) {
			if (gangKind == 0) {//明杠
				mineList [1] [selfGangCardPoint] = 3;
				/**杠牌点数**/
				//int gangpaiPonitTemp = gangBackVo.cardList [0];
				if (getPaiInpeng(selfGangCardPoint, DirectionEnum.Bottom) == -1) {//杠牌不在碰牌数组以内，一定为别人打得牌

					//销毁别人打的牌
					if (putOutCard != null) {
						Destroy(putOutCard);
					}
					if (cardOnTable != null) {
						reSetOutOnTabelCardPosition(cardOnTable);
						Destroy(cardOnTable);

					}

					//销毁手牌中的三张牌
					int removeCount = 0;
					for (int i = 0; i < handerCardList [0].Count; i++) {
						GameObject temp = handerCardList [0] [i];
						int tempCardPoint = handerCardList [0] [i].GetComponent<bottomScript>().getPoint();
						if (selfGangCardPoint == tempCardPoint) {
							handerCardList [0].RemoveAt(i);
							Destroy(temp);
							i--;
							removeCount++;
							if (removeCount == 3) {
								break;
							}
						}
					}

					//创建杠牌序列

					List<GameObject> gangTempList = new List<GameObject>();
					int flagTurn = -1;
					for (int i = 0; i < 4; i++) {
						GameObject obj;
						TopAndBottomCardScript card;
						if (i != 3) {
							if ((i + putOutCardPointAvarIndex) == 3) {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L",
									pengGangParenTransformB.transform, new Vector3(-370f + PengGangCardList.Count * 210 + i * 60f + 10, -12));
								obj.transform.localScale = new Vector3(1.6f, 1.6f, 1);
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setLefAndRightPoint(selfGangCardPoint);
								flagTurn = i;
								card.isTurn = true;
							} else {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_B",
									pengGangParenTransformB.transform, new Vector3(-370f + PengGangCardList.Count * 210 + i * 60f + (flagTurn > -1 ? 20 : 0), 0));
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setPoint(selfGangCardPoint);
							}

						} else {
							if (flagTurn == 1) {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L",
									pengGangParenTransformB.transform, new Vector3(-370f + PengGangCardList.Count * 210f + 70f, 14f));
								obj.transform.localScale = new Vector3(1.6f, 1.6f, 1);
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setLefAndRightPoint(selfGangCardPoint);
								card.isTurn = true;
							} else {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_B",
									pengGangParenTransformB.transform, new Vector3(-370f + PengGangCardList.Count * 210f + 60f + (flagTurn == 0 ? 20 : 0), 24f));
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setPoint(selfGangCardPoint);
							}
						}
						gangTempList.Add(obj);
					}

					//添加到杠牌数组里面
					PengGangCardList.Add(gangTempList);

				} else {//在碰牌数组以内，则一定是自摸的牌

					for (int i = 0; i < handerCardList [0].Count; i++) {
						if (handerCardList [0] [i].GetComponent<bottomScript>().getPoint() == selfGangCardPoint) {
							GameObject temp = handerCardList [0] [i];
							handerCardList [0].RemoveAt(i);
							Destroy(temp);
							break;
						}

					}

					int index = getPaiInpeng(selfGangCardPoint, DirectionEnum.Bottom);

					int turnIndex = -1;
					for (int i = 0; i < 3; i++) {
						if (PengGangCardList [index] [i].GetComponent<TopAndBottomCardScript>().isTurn) {
							turnIndex = i;
							break;
						}
					}

					GameObject obj;
					TopAndBottomCardScript card;
					if (turnIndex == 1) {
						obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L",
							pengGangParenTransformB.transform, new Vector3(-370f + index * 210f + 70f, 14f));
						obj.transform.localScale = new Vector3(1.6f, 1.6f, 1);
						card = obj.GetComponent<TopAndBottomCardScript>();
						card.setLefAndRightPoint(selfGangCardPoint);
						card.isTurn = true;
					} else {
						obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_B",
							pengGangParenTransformB.transform, new Vector3(-370f + index * 210f + 60f + (turnIndex == 0 ? 20 : 0), 24f));
						card = obj.GetComponent<TopAndBottomCardScript>();
						card.setPoint(selfGangCardPoint);
					}

					PengGangCardList [index].Add(obj);
				}
				//MoPaiCardPoint = gangBackVo.cardList [0];
				//putCardIntoMineList (gangBackVo.cardList [0]);


			} else if (gangKind == 1) { //===================================================================================暗杠

				mineList [1] [selfGangCardPoint] = 4;
				int removeCount = 0;

				for (int i = 0; i < handerCardList [0].Count; i++) {
					GameObject temp = handerCardList [0] [i];
					int tempCardPoint = handerCardList [0] [i].GetComponent<bottomScript>().getPoint();
					if (selfGangCardPoint == tempCardPoint) {
						handerCardList [0].RemoveAt(i);
						Destroy(temp);
						i--;
						removeCount++;
						if (removeCount == 4) {
							break;
						}
					}
				}
				List<GameObject> tempGangList = new List<GameObject>();
				for (int i = 0; i < 4; i++) {

					if (i < 3) {
						GameObject obj = createGameObjectAndReturn("Prefab/PengGangCard/gangBack",
							                 pengGangParenTransformB.transform, new Vector3(-370 + PengGangCardList.Count * 210 + i * 60, 0));
						tempGangList.Add(obj);
					} else if (i == 3) {
						GameObject obj1 = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_B",
							                  pengGangParenTransformB.transform, new Vector3(-310f + PengGangCardList.Count * 210, 24f));
						obj1.GetComponent<TopAndBottomCardScript>().setPoint(selfGangCardPoint);
						tempGangList.Add(obj1);
					}

				}

				PengGangCardList.Add(tempGangList);
			}
		} else if (gangBackVo.cardList.Count == 2) {

		}
		SetPosition(false);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="path"></param>
	/// <param name="parent"></param>
	/// <param name="position"></param>
	/// <returns></returns>
	private GameObject createGameObjectAndReturn(string path, Transform parent, Vector3 position)
	{
		GameObject obj = Instantiate(Resources.Load(path)) as GameObject;
		obj.transform.SetParent(parent);
		//  obj.transform.parent = parent;
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = position;
		return obj;
	}

	public void myGangBtnClick()
	{
		//useForGangOrPengOrChi = int.Parse (gangPaiList [0]);
		GlobalDataScript.isDrag = true;
		if (gangPaiList.Length == 1) {
			useForGangOrPengOrChi = int.Parse(gangPaiList [0]);
			selfGangCardPoint = useForGangOrPengOrChi;

		} else {//多张牌
			useForGangOrPengOrChi = int.Parse(gangPaiList [0]);
			selfGangCardPoint = useForGangOrPengOrChi;
		}

		CustomSocket.getInstance().sendMsg(new GangCardRequest(useForGangOrPengOrChi, 0));
		SoundCtrl.getInstance().playSoundByAction("gang", GlobalDataScript.loginResponseData.account.sex, curDirString);
		SoundCtrl.getInstance().playSoundGame("peng");
		btnActionScript.cleanBtnShow();
		effectType = "gang";
		pengGangHuEffectCtrl();
		gangPaiList = null;
		return;
	}

	/// <summary>
	/// 清理桌面
	/// </summary>
	public void clean()
	{
		cleanArrayList(handerCardList);
		cleanArrayList(tableCardList);
		cleanArrayList(PengGangList_L);
		cleanArrayList(PengGangCardList);
		cleanArrayList(PengGangList_R);
		cleanArrayList(PengGangList_T);
		cleanList(buhuaListB);
		cleanList(buhuaList_L);
		cleanList(buhuaList_T);
		cleanList(buhuaList_R);

		cleanList(tingCardList);

		if (mineList != null) {
			mineList.Clear();
		}


		if (putOutCard != null) {
			Destroy(putOutCard);
		}

		if (pickCardItem != null) {
			Destroy(pickCardItem);
		}

		if (otherPickCardItem != null) {
			Destroy(otherPickCardItem);
		}

	}

	private void cleanArrayList(List<List<GameObject>> list)
	{
		if (list != null) {
			while (list.Count > 0) {
				List<GameObject> tempList = list [0];
				list.RemoveAt(0);
				cleanList(tempList);
			}
		}
	}

	private void cleanList(List<GameObject> tempList)
	{
		if (tempList != null) {
			while (tempList.Count > 0) {
				GameObject temp = tempList [0];
				tempList.RemoveAt(0);
				GameObject.Destroy(temp);
			}
		}
	}

	public void setRoomRemark()
	{
//		RoomCreateVo roomvo = GlobalDataScript.roomVo;
//		GlobalDataScript.totalTimes = roomvo.realRoundNumber();
//		GlobalDataScript.surplusTimes = roomvo.realRoundNumber();
//		GlobalDataScript.gameOver = false;
//		roomNumber.text = "" + roomvo.roomId;
//		roomPaofen.text = "" + roomvo.realPaofen();
//		string str = "";
//		if (roomvo.roundtype == 0) {
//			str += "进园子 ";
//			str += roomvo.realYuanzishu() + "\n";
//			str += roomvo.realYuanziRule() + "家干\n";
//		} else {
//			str += "敞开头 ";
//			str += roomvo.realRoundNumber() + "局\n";
//		}
//		if (roomvo.zashu > 0) {
//			str += "砸" + roomvo.realZaShu() + " ";
//		}
//		if (roomvo.zhanzhuangbi) {
//			str += "比下胡 ";
//		}
//		roomRule.text = str;
		roomRule.text = "比下胡 ";
	}

	private void addAvatarVOToList(AvatarVO avatar)
	{
		if (avatarList == null) {
			avatarList = new List<AvatarVO>();
		}
		avatarList.Add(avatar);
		setSeat(avatar);
	}

	public void createRoomAddAvatarVO(AvatarVO avatar)
	{
//		avatar.scores = GlobalDataScript.roomVo.realScore();
		addAvatarVOToList(avatar);
		setRoomRemark();
		readyGame();
		markselfReadyGame();
	}

	public void joinToRoom(List<AvatarVO> avatars)
	{
		avatarList = avatars;
		for (int i = 0; i < avatars.Count; i++) {
//			avatars [i].scores = GlobalDataScript.roomVo.realScore();
			setSeat(avatars [i]);
		}
		setRoomRemark();
		readyGame();
		markselfReadyGame();
	}

	/// <summary>
	/// 设置当前角色的座位
	/// </summary>
	/// <param name="avatar">Avatar.</param>
	private void setSeat(AvatarVO avatar)
	{
		//游戏结束后用的数据，勿删！！！

		//GlobalDataScript.palyerBaseInfo.Add (avatar.account.uuid, avatar.account);

		if (avatar.account.uuid == GlobalDataScript.loginResponseData.account.uuid) {
			playerItems [0].setAvatarVo(avatar);
		} else {
			int myIndex = getMyIndexFromList();
			int curAvaIndex = avatarList.IndexOf(avatar);
			int seatIndex = curAvaIndex - myIndex;
			if (seatIndex < 0) {
				seatIndex = 4 + seatIndex;
			}
			playerItems [seatIndex].setAvatarVo(avatar);
		}

	}

	/// <summary>
	/// Gets my index from list.
	/// </summary>
	/// <returns>The my index from list.</returns>
	private int getMyIndexFromList()
	{
		if (avatarList != null) {
			for (int i = 0; i < avatarList.Count; i++) {
				if (avatarList [i].account.uuid == GlobalDataScript.loginResponseData.account.uuid
				    || avatarList [i].account.openid == GlobalDataScript.loginResponseData.account.openid) {
					GlobalDataScript.loginResponseData.account.uuid = avatarList [i].account.uuid;
					Debug.Log("数据正常返回" + i);
					return i;
				}
			}
		}

		Debug.Log("数据异常返回0");
		return 0;
	}

	private int getIndex(int uuid)
	{
		if (avatarList != null) {
			for (int i = 0; i < avatarList.Count; i++) {
				if (avatarList [i].account != null) {
					if (avatarList [i].account.uuid == uuid) {
						return i;
					}
				}
			}
		}
		return 0;
	}

	public void otherUserJointRoom(ClientResponse response)
	{
		AvatarVO avatar = JsonMapper.ToObject<AvatarVO>(response.message);
		addAvatarVOToList(avatar);
	}


	/**
	 * 胡牌请求
	 */
	public void hupaiRequest()
	{

		if (SelfAndOtherPutoutCard != -1) {
			int cardPoint = SelfAndOtherPutoutCard;//需修改成正确的胡牌cardpoint
			CardVO requestVo = new CardVO();
			requestVo.cardPoint = cardPoint;
			if (isQiangHu) {
				requestVo.type = "qianghu";
				isQiangHu = false;
			}
			string sendMsg = JsonMapper.ToJson(requestVo);
			CustomSocket.getInstance().sendMsg(new HupaiRequest(sendMsg));
			btnActionScript.cleanBtnShow();
		}


		//模拟胡牌操作
		//ClientResponse response = new ClientResponse();
		//HupaiResponseItem itemData = new HupaiResponseItem();
		//itemData.cardlist = new int[2][27]{{},{}}
	}



	/**
	 * 胡牌请求回调
	 */
	private void hupaiCallBack(ClientResponse response)
	{
		//删除这句，未区分胡家是谁
		GlobalDataScript.hupaiResponseVo = new HupaiResponseVo();
		GlobalDataScript.hupaiResponseVo = JsonMapper.ToObject<HupaiResponseVo>(response.message);

		string scores = GlobalDataScript.hupaiResponseVo.currentScore;
		hupaiCoinChange(scores);

		if (GlobalDataScript.hupaiResponseVo.type == "0") {
			effectType = "hu";
			pengGangHuEffectCtrl();
			for (int i = 0; i < GlobalDataScript.hupaiResponseVo.avatarList.Count; i++) {
				if (checkAvarHupai(GlobalDataScript.hupaiResponseVo.avatarList [i]) == 1) {//胡
					playerItems [getIndexByDir(getDirection(i))].setHuFlagDisplay();
					SoundCtrl.getInstance().playSoundByAction("hu", avatarList [i].account.sex, getDirection(i));
					SoundCtrl.getInstance().playSoundGame("hu");
				} else if (checkAvarHupai(GlobalDataScript.hupaiResponseVo.avatarList [i]) == 2) { //自摸
					playerItems [getIndexByDir(getDirection(i))].setHuFlagDisplay();
					SoundCtrl.getInstance().playSoundByAction("zimo", avatarList [i].account.sex, getDirection(i));
					SoundCtrl.getInstance().playSoundGame("hu");
				} else {
					playerItems [getIndexByDir(getDirection(i))].setHuFlagHidde();
				}
			}
			Invoke("openGameOverPanelSignal", 3);
		} else if (GlobalDataScript.hupaiResponseVo.type == "1") {

			SoundCtrl.getInstance().playSoundByAction("liuju", GlobalDataScript.loginResponseData.account.sex, "");
			effectType = "liuju";
			pengGangHuEffectCtrl();
			Invoke("openGameOverPanelSignal", 3);
		} else {
			Invoke("openGameOverPanelSignal", 3);
		}
	}

	private void finalGameOverCallBack(ClientResponse response)
	{
		GlobalDataScript.finalGameEndVo = JsonMapper.ToObject<FinalGameEndVo>(response.message);
		GlobalDataScript.gameOver = true;
	}

	/**
	 *检测某人是否胡牌 
	 */
	public int checkAvarHupai(HupaiResponseItem itemData)
	{
		string hupaiStr = itemData.totalInfo.hu;
		HuipaiObj hupaiObj = new HuipaiObj();
		if (hupaiStr != null && hupaiStr.Length > 0) {
			hupaiObj.uuid = hupaiStr.Split(new char[1] { ':' }) [0];
			hupaiObj.cardPiont = int.Parse(hupaiStr.Split(new char[1] { ':' }) [1]);
			hupaiObj.type = hupaiStr.Split(new char[1] { ':' }) [2];
			//增加判断是否是自己胡牌的判断

			if (hupaiStr.Contains("d_other")) {//排除一炮多响的情况
				return 0;
			} else if (hupaiObj.type == "zi_common") {
				return 2;

			} else if (hupaiObj.type == "d_self") {
				return 1;
			} else if (hupaiObj.type == "qiyise") {
				return 1;
			} else if (hupaiObj.type == "zi_qingyise") {
				return 2;
			} else if (hupaiObj.type == "qixiaodui") {
				return 1;
			} else if (hupaiObj.type == "self_qixiaodui") {
				return 2;
			} else if (hupaiObj.type == "gangshangpao") {
				return 1;
			} else if (hupaiObj.type == "gangshanghua") {
				return 2;
			}


		}
		return 0;
	}



	private void hupaiCoinChange(string scores)
	{
		string[] scoreList = scores.Split(new char[1] { ',' });
		if (scoreList != null && scoreList.Length > 0) {
			for (int i = 0; i < scoreList.Length - 1; i++) {
				string itemstr = scoreList [i];
				int uuid = int.Parse(itemstr.Split(new char[1] { ':' }) [0]);
				int score = int.Parse(itemstr.Split(new char[1] { ':' }) [1]);
				playerItems [getIndexByDir(getDirection(getIndex(uuid)))].scoreText.text = score + "";
				avatarList [getIndex(uuid)].scores = score;
			}
		}
	}


	private void openGameOverPanelSignal()
	{//单局结算
		liujuEffectGame.SetActive(false);
		setAllPlayerHuImgVisbleToFalse();

		//GlobalDataScript.singalGameOver = PrefabManage.loadPerfab("prefab/Panel_Game_Over");
		GameObject obj = PrefabManage.loadPerfab("Prefab/Panel_Game_Over");
		avatarList [bankerId].main = false;
		getDirection(bankerId);
		playerItems [curDirIndex].setbankImgEnable(false);
		if (handerCardList != null && handerCardList.Count > 0 && handerCardList [0].Count > 0) {
			for (int i = 0; i < handerCardList [0].Count; i++) {
				handerCardList [0] [i].GetComponent<bottomScript>().onSendMessage -= cardChange;
				handerCardList [0] [i].GetComponent<bottomScript>().reSetPoisiton -= cardSelect;
			}
		}

		initPanel();
		obj.GetComponent<GameOverScript>().setDisplaContent(0, avatarList);
		GlobalDataScript.singalGameOverList.Add(obj);
		//GlobalDataScript.singalGameOver.GetComponent<GameOverScript> ().setDisplaContent (0,avatarList,allMas,GlobalDataScript.hupaiResponseVo.validMas);	
	}

	private void loadPerfab(string perfabName, int openFlag)
	{
		GameObject obj = PrefabManage.loadPerfab(perfabName);
		obj.GetComponent<GameOverScript>().setDisplaContent(openFlag, avatarList);
	}

	private void reSetOutOnTabelCardPosition(GameObject cardOnTable)
	{
		Debug.Log("putOutCardPointAvarIndex===========:" + putOutCardPointAvarIndex);
		if (putOutCardPointAvarIndex != -1) {
			int objIndex = tableCardList [putOutCardPointAvarIndex].IndexOf(cardOnTable);
			if (objIndex != -1) {
				tableCardList [putOutCardPointAvarIndex].RemoveAt(objIndex);
				return;
			}
		}

	}

	/***
	 * 退出房间请求
	 */
	public void quiteRoom()
	{
		SoundCtrl.getInstance().playSoundUI();
		OutRoomRequestVo vo = new OutRoomRequestVo();
//		vo.roomId = GlobalDataScript.roomVo.roomId;
		string sendMsg = JsonMapper.ToJson(vo);
		CustomSocket.getInstance().sendMsg(new OutRoomRequest(sendMsg));
	}

	public void outRoomCallbak(ClientResponse response)
	{
		OutRoomResponseVo responseMsg = JsonMapper.ToObject<OutRoomResponseVo>(response.message);
		if (responseMsg.status_code == "0") {
			if (responseMsg.type == "0") {

				int uuid = responseMsg.uuid;
				if (uuid != GlobalDataScript.loginResponseData.account.uuid) {
					int index = getIndex(uuid);
					avatarList.RemoveAt(index);

					for (int i = 0; i < playerItems.Count; i++) {
						playerItems [i].setAvatarVo(null);
					}

					if (avatarList != null) {
						for (int i = 0; i < avatarList.Count; i++) {
							setSeat(avatarList [i]);
						}
						markselfReadyGame();
					}
				} else {
					exitOrDissoliveRoom();
				}

			} else {
				exitOrDissoliveRoom();
			}

		} else {
			TipsManagerScript.getInstance().setTips("退出房间失败：" + responseMsg.error);
		}
	}


	private string dissoliveRoomType = "0";

	public void dissoliveRoomRequest()
	{
		SoundCtrl.getInstance().playSoundUI();
		if (canClickButtonFlag) {
			dissoliveRoomType = "0";
			TipsManagerScript.getInstance().loadDialog("申请解散房间", "你确定要申请解散房间？", doDissoliveRoomRequest, cancle);
		} else {
			TipsManagerScript.getInstance().setTips("还没有开始游戏，不能申请退出房间");
		}

	}

	/***
	 * 申请解散房间回调
	 */
	GameObject dissoDialog;

	public void dissoliveRoomResponse(ClientResponse response)
	{
		Debug.Log("dissoliveRoomResponse" + response.message);
		DissoliveRoomResponseVo dissoliveRoomResponseVo = JsonMapper.ToObject<DissoliveRoomResponseVo>(response.message);
		string plyerName = dissoliveRoomResponseVo.accountName;
		if (dissoliveRoomResponseVo.type == "0") {
			GlobalDataScript.isonApplayExitRoomstatus = true;
			dissoliveRoomType = "1";
			dissoDialog = PrefabManage.loadPerfab("Prefab/Panel_Apply_Exit");
			dissoDialog.GetComponent<VoteScript>().iniUI(plyerName, avatarList);
		} else if (dissoliveRoomResponseVo.type == "3") {
			GlobalDataScript.isonApplayExitRoomstatus = false;
			if (dissoDialog != null) {
				GlobalDataScript.isOverByPlayer = true;
				dissoDialog.GetComponent<VoteScript>().removeListener();
				Destroy(dissoDialog.GetComponent<VoteScript>());
				Destroy(dissoDialog);
			}
		}
	}


	/**
	 * 申请或同意解散房间请求
	 * 
	 */
	public void doDissoliveRoomRequest()
	{
		DissoliveRoomRequestVo dissoliveRoomRequestVo = new DissoliveRoomRequestVo();
		dissoliveRoomRequestVo.roomId = GlobalDataScript.loginResponseData.roomId;
		dissoliveRoomRequestVo.type = dissoliveRoomType;
		string sendMsg = JsonMapper.ToJson(dissoliveRoomRequestVo);
		CustomSocket.getInstance().sendMsg(new DissoliveRoomRequest(sendMsg));
		GlobalDataScript.isonApplayExitRoomstatus = true;
	}

	private void cancle()
	{

	}

	private void cancle1()
	{
		dissoliveRoomType = "2";
		doDissoliveRoomRequest();
	}

	public void exitOrDissoliveRoom()
	{
		GlobalDataScript.loginResponseData.resetData();//复位房间数据
		GlobalDataScript.loginResponseData.roomId = 0;//复位房间数据
//		GlobalDataScript.roomVo.roomId = 0;
		GlobalDataScript.soundToggle = true;
		clean();
		removeListener();

		SoundCtrl.getInstance().stopBGM();
		SoundCtrl.getInstance().playBGM();
		if (GlobalDataScript.homePanel != null) {
			GlobalDataScript.homePanel.SetActive(true);
			GlobalDataScript.homePanel.transform.SetSiblingIndex(1);
		} else {
			GlobalDataScript.homePanel = PrefabManage.loadPerfab("Prefab/Panel_Home");
			GlobalDataScript.homePanel.transform.SetSiblingIndex(1);
		}

		while (playerItems.Count > 0) {
			PlayerItemScript item = playerItems [0];
			playerItems.RemoveAt(0);
			item.clean();
			Destroy(item.gameObject);
			Destroy(item);
		}
		Destroy(this);
		Destroy(gameObject);
	}

	public void gameReadyNotice(ClientResponse response)
	{

		//===============================================
		JsonData json = JsonMapper.ToObject(response.message);
		int avatarIndex = Int32.Parse(json ["avatarIndex"].ToString());
		int myIndex = getMyIndexFromList();
		int seatIndex = avatarIndex - myIndex;
		if (seatIndex < 0) {
			seatIndex = 4 + seatIndex;
		}
		playerItems [seatIndex].readyImg.SetActive(true);
		avatarList [avatarIndex].isReady = true;
		SoundCtrl.getInstance().playSoundOther("ready");
	}


	private void gameFollowBanderNotice(ClientResponse response)
	{
		genZhuang.SetActive(true);
		Invoke("hideGenzhuang", 2f);
	}

	private void hideGenzhuang()
	{
		genZhuang.SetActive(false);
	}

	/*************************断线重连*********************************/
	private void reEnterRoom()
	{
		if (GlobalDataScript.reEnterRoomData != null) {
			//显示房间基本信息
//			GlobalDataScript.roomVo.addWordCard = GlobalDataScript.reEnterRoomData.addWordCard;
//			GlobalDataScript.roomVo.hong = GlobalDataScript.reEnterRoomData.hong;
//			GlobalDataScript.roomVo.name = GlobalDataScript.reEnterRoomData.name;
//			GlobalDataScript.roomVo.roomId = GlobalDataScript.reEnterRoomData.roomId;
//			GlobalDataScript.roomVo.roomType = GlobalDataScript.reEnterRoomData.roomType;
//			GlobalDataScript.roomVo.roundNumber = GlobalDataScript.reEnterRoomData.roundNumber;
//			GlobalDataScript.roomVo.sevenDouble = GlobalDataScript.reEnterRoomData.sevenDouble;
//			GlobalDataScript.roomVo.xiaYu = GlobalDataScript.reEnterRoomData.xiaYu;
//			GlobalDataScript.roomVo.ziMo = GlobalDataScript.reEnterRoomData.ziMo;
//			GlobalDataScript.roomVo.magnification = GlobalDataScript.reEnterRoomData.magnification;
//			GlobalDataScript.roomVo.chengbei = GlobalDataScript.reEnterRoomData.chengbei;
//			GlobalDataScript.roomVo.zashu = GlobalDataScript.reEnterRoomData.zashu;
//			GlobalDataScript.roomVo.aa = GlobalDataScript.reEnterRoomData.aa;
//			GlobalDataScript.roomVo.paofen = GlobalDataScript.reEnterRoomData.paofen;
//			GlobalDataScript.roomVo.roundtype = GlobalDataScript.reEnterRoomData.roundtype;
//			GlobalDataScript.roomVo.yuanzishu = GlobalDataScript.reEnterRoomData.yuanzishu;
//			GlobalDataScript.roomVo.yuanzijiesu = GlobalDataScript.reEnterRoomData.yuanzijiesu;
//			GlobalDataScript.roomVo.zhanzhuangbi = GlobalDataScript.reEnterRoomData.zhanzhuangbi;
//			GlobalDataScript.roomVo.guozhuangbi = GlobalDataScript.reEnterRoomData.guozhuangbi;
//			GlobalDataScript.roomVo.fengfa = GlobalDataScript.reEnterRoomData.fengfa;

			setRoomRemark();
			//设置座位

			avatarList = GlobalDataScript.reEnterRoomData.playerList;
			GlobalDataScript.roomAvatarVoList = GlobalDataScript.reEnterRoomData.playerList;
			for (int i = 0; i < avatarList.Count; i++) {
				setSeat(avatarList [i]);
			}

			recoverOtherGlobalData();
			int[][] selfPaiArray = GlobalDataScript.reEnterRoomData.playerList [getMyIndexFromList()].paiArray;
			if (selfPaiArray == null || selfPaiArray.Length == 0) {//游戏还没有开始
				readyGame();
				markselfReadyGame();
			} else {//牌局已开始
				setAllPlayerReadImgVisbleToFalse();
				cleanGameplayUI();
				//显示打牌数据
				displayTableCards();
				//显示碰牌
				displayOtherHandercard(); //显示其他玩家的手牌
				displayallGangCard(); //显示杠牌
				displayPengCard(); //显示碰牌
				displayHua(); //显示花牌
				dispalySelfhanderCard();//显示自己的手牌
				CustomSocket.getInstance().sendMsg(new CurrentStatusRequest());
			}
		}

	}




	//恢复其他全局数据
	private void recoverOtherGlobalData()
	{
		int selfIndex = getMyIndexFromList();
		GlobalDataScript.loginResponseData.account.roomcard = GlobalDataScript.reEnterRoomData.playerList [selfIndex].account.roomcard;//恢复房卡数据，此时主界面还没有load所以无需操作界面显示

	}



	/// <summary>
	/// 显示自己自己手牌 
	/// </summary>
	private void dispalySelfhanderCard()
	{
		mineList = ToList(GlobalDataScript.reEnterRoomData.playerList [getMyIndexFromList()].paiArray);
		for (int i = 0; i < mineList [0].Count; i++) {
			if (mineList [0] [i] > 0) {
				for (int j = 0; j < mineList [0] [i]; j++) {
					GameObject gob = Instantiate(Resources.Load("prefab/card/Bottom_B")) as GameObject;
					//GameObject.Instantiate ("");

					if (gob != null) {//
						gob.transform.SetParent(parentList [0]);//设置父节点
						gob.transform.localScale = new Vector3(1.1f, 1.1f, 1);
						gob.GetComponent<bottomScript>().onSendMessage += cardChange;//发送消息fd
						gob.GetComponent<bottomScript>().reSetPoisiton += cardSelect;
						gob.GetComponent<bottomScript>().setPoint(i);//设置指针                                                                                         
						handerCardList [0].Add(gob);//增加游戏对象
					}
				}

			}
		}
		SetPosition(false);
	}

	private List<List<int>> ToList(int[][] param)
	{
		List<List<int>> returnData = new List<List<int>>();
		for (int i = 0; i < param.Length; i++) {
			List<int> temp = new List<int>();
			for (int j = 0; j < param [i].Length; j++) {
				temp.Add(param [i] [j]);
			}
			returnData.Add(temp);
		}
		return returnData;
	}

	public void soundActionPlay(float time)
	{
		playerItems [0].showChatAction(time);
	}

	public void soundActionStop()
	{
		playerItems [0].hideChatAction();
	}

	/**显示打牌数据在桌面**/
	private void displayTableCards()
	{
		//List<int[]> chupaiList = new List<int[]> ();
		for (int i = 0; i < GlobalDataScript.reEnterRoomData.playerList.Count; i++) {
			int[] chupai = GlobalDataScript.reEnterRoomData.playerList [i].chupais;
			outDir = getDirection(getIndex(GlobalDataScript.reEnterRoomData.playerList [i].account.uuid));
			if (chupai != null && chupai.Length > 0) {
				for (int j = 0; j < chupai.Length; j++) {
					ThrowBottom(chupai [j]);
				}
			}

		}
	}

	/**显示其他人的手牌**/
	private void displayOtherHandercard()
	{
		for (int i = 0; i < GlobalDataScript.reEnterRoomData.playerList.Count; i++) {
			string dir = getDirection(getIndex(GlobalDataScript.reEnterRoomData.playerList [i].account.uuid));
			int count = GlobalDataScript.reEnterRoomData.playerList [i].commonCards;
			if (dir != DirectionEnum.Bottom) {
				initOtherCardList(dir, count);
			}

		}
	}

	/**显示杠牌**/
	private void displayallGangCard()
	{
		for (int i = 0; i < GlobalDataScript.reEnterRoomData.playerList.Count; i++) {
			int[] paiArrayType = GlobalDataScript.reEnterRoomData.playerList [i].paiArray [1];
			string dirstr = getDirection(getIndex(GlobalDataScript.reEnterRoomData.playerList [i].account.uuid));
			if (paiArrayType.Contains<int>(2)) {
				string gangString = GlobalDataScript.reEnterRoomData.playerList [i].huReturnObjectVO.totalInfo.gang;
				if (gangString != null) {
					string[] gangtemps = gangString.Split(new char[1] { ',' });
					for (int j = 0; j < gangtemps.Length; j++) {
						string item = gangtemps [j];
						GangpaiObj gangpaiObj = new GangpaiObj();
						gangpaiObj.uuid = item.Split(new char[1] { ':' }) [0];
						gangpaiObj.cardPiont = int.Parse(item.Split(new char[1] { ':' }) [1]);
						gangpaiObj.type = item.Split(new char[1] { ':' }) [2];
						//增加判断是否为自己的杠牌的操作
						GlobalDataScript.reEnterRoomData.playerList [i].paiArray [0] [gangpaiObj.cardPiont] -= 4;
						if (gangpaiObj.type == "an") {
							doDisplayPengGangCard(dirstr, gangpaiObj.cardPiont, 4, 1);

						} else {
							int pengIndex = GlobalDataScript.reEnterRoomData.playerList [i].paiArray [2] [gangpaiObj.cardPiont];
							if (pengIndex == -1)
								pengIndex = GlobalDataScript.reEnterRoomData.playerList [i].paiArray [3] [gangpaiObj.cardPiont];
							int pengRealIndex = -1;
							if (pengIndex > -1) {
								pengRealIndex = getIndex(GlobalDataScript.reEnterRoomData.playerList [pengIndex].account.uuid);
							}
							doDisplayPengGangCard(dirstr, gangpaiObj.cardPiont, 4, 0, pengRealIndex);

						}
					}
				}
			}

		}
	}

	private void displayPengCard()
	{
		for (int i = 0; i < GlobalDataScript.reEnterRoomData.playerList.Count; i++) {
			int[] paiArrayType = GlobalDataScript.reEnterRoomData.playerList [i].paiArray [1];
			string dirstr = getDirection(getIndex(GlobalDataScript.reEnterRoomData.playerList [i].account.uuid));
			if (paiArrayType.Contains<int>(1)) {
				for (int j = 0; j < paiArrayType.Length; j++) {
					if (paiArrayType [j] == 1 && GlobalDataScript.reEnterRoomData.playerList [i].paiArray [0] [j] > 0) {
						GlobalDataScript.reEnterRoomData.playerList [i].paiArray [0] [j] -= 3;
						int pengIndex = GlobalDataScript.reEnterRoomData.playerList [i].paiArray [2] [j];
						int pengRealIndex = -1;
						if (pengIndex > -1) {
							pengRealIndex = getIndex(GlobalDataScript.reEnterRoomData.playerList [pengIndex].account.uuid);
						}
						//doDisplayPengGangCard(dirstr, j, 3, 2, pengRealIndex);
						doDisplayPengGangCard(dirstr, j, 4, 0, pengRealIndex);
					}
				}
			}
		}
	}

	private void displayHua()
	{
		for (int i = 0; i < GlobalDataScript.reEnterRoomData.playerList.Count; i++) {
			int[] paiArrayType = GlobalDataScript.reEnterRoomData.playerList [i].paiArray [1];
			string dirstr = getDirection(getIndex(GlobalDataScript.reEnterRoomData.playerList [i].account.uuid));
			for (int j = 0; j < paiArrayType.Length; j++) {
				int count = GlobalDataScript.reEnterRoomData.playerList [i].paiArray [0] [j];
				if (paiArrayType [j] >= 11 && count > 0) {
					GlobalDataScript.reEnterRoomData.playerList [i].paiArray [0] [j] = 0;
					for (int add = 0; add < count; add++) {
						addBuhua(dirstr, j);
					}
					//doDisplayPengGangCard(dirstr, j, count, 2);
				}
			}
		}
	}


	/**
	 * 显示杠碰牌
	 * cloneCount 代表clone的次数  若为3则表示碰   若为4则表示杠
	 */
	private void doDisplayPengGangCard(string dirstr, int point, int cloneCount, int flag, int fromIndex = -1)
	{
		List<GameObject> gangTempList;
		switch (dirstr) {
			case DirectionEnum.Bottom:
				{
					gangTempList = new List<GameObject>();
					int flagTurn = -1;
					for (int i = 0; i < cloneCount; i++) {
						GameObject obj;
						TopAndBottomCardScript card;
						if (i < 3) {
							if (flag != 1) {
								if (fromIndex == -1 || (fromIndex + i) != 3) {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_B",
										pengGangParenTransformB.transform, new Vector3(-370f + PengGangCardList.Count * 210f + i * 60f + (flagTurn > -1 ? 20 : 0), 0));
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setPoint(point);
								} else {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L",
										pengGangParenTransformB.transform, new Vector3(-370f + PengGangCardList.Count * 210f + i * 60f + 10f, -12));
									obj.transform.localScale = new Vector3(1.6f, 1.6f, 1);
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setLefAndRightPoint(point);
									flagTurn = i;
									card.isTurn = true;
								}
							} else {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/gangBack",
									pengGangParenTransformB.transform, new Vector3(-370f + PengGangCardList.Count * 210f + i * 60f, 0));
								obj.transform.localScale = Vector3.one;
							}
						} else {
							if (flagTurn == 1) {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L",
									pengGangParenTransformB.transform, new Vector3(-370f + PengGangCardList.Count * 210f + 70f, 14f));
								obj.transform.localScale = new Vector3(1.6f, 1.6f, 1);
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setLefAndRightPoint(point);
								card.isTurn = true;
							} else {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_B",
									pengGangParenTransformB.transform, new Vector3(-370f + PengGangCardList.Count * 210f + 60f + (flagTurn == 0 ? 20 : 0), 24f));
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setPoint(point);
							}
						}


						gangTempList.Add(obj);
					}
					PengGangCardList.Add(gangTempList);
				}
				break;
			case DirectionEnum.Top:
				{
					gangTempList = new List<GameObject>();
					int flagTurn = -1;
					for (int i = 0; i < cloneCount; i++) {
						GameObject obj;
						TopAndBottomCardScript card;
						if (i < 3) {
							if (flag != 1) {
								if (fromIndex != -1 && ((i == 0 && fromIndex == 3) || (i == 1 && fromIndex == 0) || (i == 2 && fromIndex == 1))) {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L",
										pengGangParenTransformT.transform, new Vector3(251 - PengGangList_T.Count * 133 + i * 37 + 6.5f, 7));
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setLefAndRightPoint(point);
									flagTurn = i;
									card.isTurn = true;
								} else {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T",
										pengGangParenTransformT.transform, new Vector3(251 - PengGangList_T.Count * 133 + i * 37 + (flagTurn > -1 ? 13 : 0), 0));
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setPoint(point);
								}
							} else {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/GangBack_T",
									pengGangParenTransformT.transform, new Vector3(251 - PengGangList_T.Count * 133 + i * 37, 0));
							}
						} else {
							if (flagTurn == 1) {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L",
									pengGangParenTransformT.transform, new Vector3(251 - PengGangList_T.Count * 133 + 37f + 6.5f, 20f));
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setLefAndRightPoint(point);
								card.isTurn = true;
							} else {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T",
									pengGangParenTransformT.transform, new Vector3(251 - PengGangList_T.Count * 133 + 37f + (flagTurn == 0 ? 13 : 0), 16f));
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setPoint(point);
							}
						}
						gangTempList.Add(obj);
					}
					PengGangList_T.Add(gangTempList);
				}
				break;
			case DirectionEnum.Left:
				{
					gangTempList = new List<GameObject>();
					int flagTurn = -1;
					for (int i = 0; i < cloneCount; i++) {
						GameObject obj;
						TopAndBottomCardScript card;
						if (i < 3) {
							if (flag != 1) {
								if (fromIndex == -1 || (fromIndex + i) != 2) {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L",
										pengGangParenTransformL.transform, new Vector3(0f, 122 - PengGangList_L.Count * 112 - i * 26f - (flagTurn > -1 ? 12 : 0)));
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setLefAndRightPoint(point);
								} else {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T",
										pengGangParenTransformL.transform, new Vector3(-7f, 122 - PengGangList_L.Count * 112 - i * 26f - 6));
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setPoint(point);
									flagTurn = i;
									card.isTurn = true;
								}
							} else {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/GangBack_L&R",
									pengGangParenTransformL.transform, new Vector3(0f, 122 - PengGangList_L.Count * 112 - i * 26f));
							}
						} else {
							if (flagTurn == 1) {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T",
									pengGangParenTransformL.transform, new Vector3(-7f, 122 - PengGangList_L.Count * 112 - 16));
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setPoint(point);
								card.isTurn = true;
							} else {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_L",
									pengGangParenTransformL.transform, new Vector3(0f, 122 - PengGangList_L.Count * 112 - 10 - (flagTurn == 0 ? 12 : 0)));
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setLefAndRightPoint(point);
							}
						}
						gangTempList.Add(obj);
					}
					PengGangList_L.Add(gangTempList);
				}
				break;
			case DirectionEnum.Right:
				{
					gangTempList = new List<GameObject>();
					int flagTurn = -1;
					for (int i = 0; i < cloneCount; i++) {
						GameObject obj;
						TopAndBottomCardScript card;
						if (i < 3) {
							if (flag != 1) {
								if (fromIndex == -1 || ((fromIndex + i) != 4 && (fromIndex + i) != 0)) {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_R",
										pengGangParenTransformR.transform, new Vector3(0, -122 + PengGangList_R.Count * 112 + i * 26f + (flagTurn > -1 ? 12 : 0)));
									obj.transform.SetSiblingIndex(0);
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setLefAndRightPoint(point);
								} else {
									obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T",
										pengGangParenTransformR.transform, new Vector3(7, -122 + PengGangList_R.Count * 112 + i * 26f + 6));
									obj.transform.SetSiblingIndex(0);
									card = obj.GetComponent<TopAndBottomCardScript>();
									card.setPoint(point);
									flagTurn = i;
									card.isTurn = true;
								}
							} else {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/GangBack_L&R",
									pengGangParenTransformR.transform, new Vector3(0, -122 + PengGangList_R.Count * 112 + i * 26f));
							}
						} else {
							if (flagTurn == 1) {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_T",
									pengGangParenTransformR.transform, new Vector3(7f, -122 + PengGangList_R.Count * 112 + 48));
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setPoint(point);
								card.isTurn = true;
							} else {
								obj = createGameObjectAndReturn("Prefab/PengGangCard/PengGangCard_R",
									pengGangParenTransformR.transform, new Vector3(0f, -122 + PengGangList_R.Count * 112 + 42 + (flagTurn == 0 ? 12 : 0)));
								card = obj.GetComponent<TopAndBottomCardScript>();
								card.setLefAndRightPoint(point);
							}
						}

						gangTempList.Add(obj);
					}
					PengGangList_R.Add(gangTempList);
				}
				break;
		}
	}

	public void inviteFriend()
	{
		SoundCtrl.getInstance().playSoundUI();
//		GlobalDataScript.getInstance().wechatOperate.inviteFriend();
		PlatformBridge.Instance.inviteFriend();
	}

	public void chatClick()
	{
		SoundCtrl.getInstance().playSoundUI();
	}

	/**用户离线回调**/
	public void offlineNotice(ClientResponse response)
	{
		int uuid = int.Parse(response.message);
		int index = getIndex(uuid);
		string dirstr = getDirection(index);
		switch (dirstr) {
			case DirectionEnum.Bottom:
				playerItems [0].GetComponent<PlayerItemScript>().setPlayerOffline();
				break;
			case DirectionEnum.Right:
				playerItems [1].GetComponent<PlayerItemScript>().setPlayerOffline();
				break;
			case DirectionEnum.Top:
				playerItems [2].GetComponent<PlayerItemScript>().setPlayerOffline();
				break;
			case DirectionEnum.Left:
				playerItems [3].GetComponent<PlayerItemScript>().setPlayerOffline();
				break;
		}

		//申请解散房间过程中，有人掉线，直接不能解散房间
		if (GlobalDataScript.isonApplayExitRoomstatus) {
			if (dissoDialog != null) {
				dissoDialog.GetComponent<VoteScript>().removeListener();
				Destroy(dissoDialog.GetComponent<VoteScript>());
				Destroy(dissoDialog);
			}
			TipsManagerScript.getInstance().setTips("由于" + avatarList [index].account.nickname + "离线，系统不能解散房间。");

		}
	}

	/**用户上线提醒**/
	public void onlineNotice(ClientResponse response)
	{
		int uuid = int.Parse(response.message);
		int index = getIndex(uuid);
		string dirstr = getDirection(index);
		switch (dirstr) {
			case DirectionEnum.Bottom:
				playerItems [0].GetComponent<PlayerItemScript>().setPlayerOnline();
				break;
			case DirectionEnum.Right:
				playerItems [1].GetComponent<PlayerItemScript>().setPlayerOnline();
				break;
			case DirectionEnum.Top:
				playerItems [2].GetComponent<PlayerItemScript>().setPlayerOnline();
				break;
			case DirectionEnum.Left:
				playerItems [3].GetComponent<PlayerItemScript>().setPlayerOnline();
				break;

		}
	}


	public void messageBoxNotice(ClientResponse response)
	{
		MessageRequestVo vo = JsonMapper.ToObject<MessageRequestVo>(response.message);

		int myIndex = getMyIndexFromList();
		int curAvaIndex = getIndex(vo.uuid);
		int seatIndex = curAvaIndex - myIndex;
		if (seatIndex < 0) {
			seatIndex = 4 + seatIndex;
		}
		switch (vo.type) {
			case 0:
				playerItems [seatIndex].showMessage(vo.message);
				break;
			case 1:
				playerItems [seatIndex].showChatMessage(int.Parse(vo.message));
				int code = int.Parse(vo.message);
				SoundCtrl.getInstance().playMessageBoxSound(code);
				break;
			case 2:
				playerItems [seatIndex].showBiaoqing(vo.message);
				break;

			case 3:
				messageBox.SetActive(true);
				messageBoxText.text = vo.message;
				break;
		}
	}


	/*显示自己准备*/
	private void markselfReadyGame()
	{
		playerItems [0].readyImg.transform.gameObject.SetActive(true);
		SoundCtrl.getInstance().playSoundOther("ready");
	}

	/**
    *准备游戏
	*/
	public void readyGame()
	{
		CustomSocket.getInstance().sendMsg(new GameReadyRequest());
	}

	public void returnGameResponse(ClientResponse response)
	{
		string returnstr = response.message;
		//JsonData returnJsonData = new JsonData (returnstr);
		//1.显示剩余牌的张数和圈数
		JsonData returnJsonData = JsonMapper.ToObject(response.message);
		string surplusCards = returnJsonData ["surplusCards"].ToString();
		LeavedCastNumText.text = surplusCards;
		LeavedCardsNum = int.Parse(surplusCards);
		GlobalDataScript.surplusTimes = int.Parse(returnJsonData ["gameRound"].ToString());
		LeavedRoundNumText.text = (GlobalDataScript.totalTimes - GlobalDataScript.surplusTimes) + "/" + GlobalDataScript.totalTimes;
		GlobalDataScript.gameOver = false;


		int curAvatarIndexTemp = -1;//当前出牌人的索引
		int pickAvatarIndexTemp = -1; //当前摸牌人的索引
		int putOffCardPointTemp = -1;//当前打得点数
		int currentCardPointTemp = -1;//当前摸的点数


		//不是自己摸牌
		try {

			curAvatarIndexTemp = int.Parse(returnJsonData ["curAvatarIndex"].ToString());//当前打牌人的索引
			putOffCardPointTemp = int.Parse(returnJsonData ["putOffCardPoint"].ToString());//当前打得点数

			putOutCardPointAvarIndex = getIndexByDir(getDirection(curAvatarIndexTemp));

			putOutCardPoint = putOffCardPointTemp;//碰
			//useForGangOrPengOrChi = putOutCardPoint;//杠
			//	selfGangCardPoint = useForGangOrPengOrChi;
			SelfAndOtherPutoutCard = putOutCardPoint;
			pickAvatarIndexTemp = int.Parse(returnJsonData ["pickAvatarIndex"].ToString()); //当前摸牌牌人的索引

			/**这句代码有可能引发catch  所以后面的 SelfAndOtherPutoutCard = currentCardPointTemp; 可能不执行**/
			currentCardPointTemp = int.Parse(returnJsonData ["currentCardPoint"].ToString());//当前摸得的点数  
			SelfAndOtherPutoutCard = currentCardPointTemp;
		} catch (Exception ex) {
		}

		if (pickAvatarIndexTemp == getMyIndexFromList()) {//自己摸牌
			if (currentCardPointTemp == -2) {
				MoPaiCardPoint = handerCardList [0] [handerCardList [0].Count - 1].GetComponent<bottomScript>().getPoint();
				SelfAndOtherPutoutCard = MoPaiCardPoint;
				useForGangOrPengOrChi = curAvatarIndexTemp;
				Destroy(handerCardList [0] [handerCardList [0].Count - 1]);
				handerCardList [0].Remove(handerCardList [0] [handerCardList [0].Count - 1]);
				SetPosition(false);
				putCardIntoMineList(MoPaiCardPoint);
				moPai();
				curDirString = DirectionEnum.Bottom;
				SetDirGameObjectAction();
				GlobalDataScript.isDrag = true;
			} else {
				if ((handerCardList [0].Count) % 3 != 1) {
					MoPaiCardPoint = currentCardPointTemp;
					SelfAndOtherPutoutCard = MoPaiCardPoint;
					useForGangOrPengOrChi = curAvatarIndexTemp;
					for (int i = 0; i < handerCardList [0].Count; i++) {
						if (handerCardList [0] [i].GetComponent<bottomScript>().getPoint() == currentCardPointTemp) {
							Destroy(handerCardList [0] [i]);
							handerCardList [0].Remove(handerCardList [0] [i]);
							break;
						}
					}
					SetPosition(false);
					putCardIntoMineList(MoPaiCardPoint);
					moPai();
					curDirString = DirectionEnum.Bottom;
					SetDirGameObjectAction();
					GlobalDataScript.isDrag = true;
				}
			}
			onCardChanged();
		} else { //别人摸牌
			curDirString = getDirection(pickAvatarIndexTemp);
			//	otherMoPaiCreateGameObject (curDirString);
			SetDirGameObjectAction();
		}

		//光标指向打牌人
		int dirindex = getIndexByDir(getDirection(curAvatarIndexTemp));
		if (tableCardList [dirindex] == null || tableCardList [dirindex].Count == 0) {//刚启动
		} else {
			cardOnTable = tableCardList [dirindex] [tableCardList [dirindex].Count - 1];
			GameObject temp = tableCardList [dirindex] [tableCardList [dirindex].Count - 1];
			setPointGameObject(temp);
		}
	}

	public void scoreResponse(ClientResponse response)
	{
		JsonData json = JsonMapper.ToObject(response.message);
		int avatarIndex = Int32.Parse(json ["avatarIndex"].ToString());
		int score = Int32.Parse(json ["score"].ToString());


		if (score == 0)
			return;

		int myIndex = getMyIndexFromList();
		int seatIndex = avatarIndex - myIndex;
		if (seatIndex < 0) {
			seatIndex = 4 + seatIndex;
		}

		avatarList [avatarIndex].scores += score;
		playerItems [seatIndex].scoreText.text = avatarList [avatarIndex].scores + "";

		scoreList [seatIndex].gameObject.SetActive(true);
		if (score >= 0) {
			scoreList [seatIndex].text = "+" + score;
			scoreList [seatIndex].color = winTextColor;
		} else {
			scoreList [seatIndex].text = "" + score;
			scoreList [seatIndex].color = loseTextColor;
		}
	}
}
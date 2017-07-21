using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AssemblyCSharp;
using System.Collections.Generic;

public class GangpaiObj
{
	public int cardPiont;
	//出牌的下标
	public string uuid;
	//出牌的玩家
	public string type;
}

public class HuipaiObj
{
	public int cardPiont;
	//出牌的下标
	public string uuid;
	public string type;
}

public class ChipaiObj
{
	public string[] cardPionts;
	//出牌的下标
}

public class SignalOverItemScript : MonoBehaviour
{

	public Text nickName;
	public Text resultDes;
	public GameObject huFlagImg;
	public Text totalScroe;
	public Text gangScore;
	public GameObject paiArrayPanel;
	private Color32 winTextColor = new Color32(252, 197, 5, 255);
	private Color32 loseTextColor = new Color32(48, 232, 97, 255);
	//public GameObject GenzhuangFlag;


	//public GameObject subContaner ;
	//public GameObject chiContaner;
	//public GameObject pengContaner;
	//public GameObject gangContaner;
	//public GameObject huContaner;

	private List<GangpaiObj> gangPaiList = new List<GangpaiObj>();
	//杠牌列表

	private string[] pengpaiList;
	//碰牌列表

	private List<ChipaiObj> chipaiList = new List<ChipaiObj>();
	//吃牌列表

	private List<int> maPais;
	//码牌数组

	private HuipaiObj hupaiObj = new HuipaiObj();
	//胡牌列表


	private string mdesCribe = "";
	//对结果展示字符串

	private int[] paiArray;
	//牌列表

	private HupaiResponseItem mHupaiResponseItemData;


	public void setUI(HupaiResponseItem itemData, int mainuuid)
	{
		mHupaiResponseItemData = itemData;
		nickName.text = itemData.nickname;
		if (itemData.huScore == 0) {
			totalScroe.text = itemData.huScore + "";
			totalScroe.color = winTextColor;
		} else if (itemData.huScore > 0) {
			totalScroe.text = "+" + itemData.huScore;
			totalScroe.color = winTextColor;
		} else {
			totalScroe.text = "" + itemData.huScore;
			totalScroe.color = loseTextColor;
		}
		gangScore.text = itemData.gangScore + "";

		paiArray = itemData.paiArray;
		huFlagImg.SetActive(false);
		if (itemData.totalInfo.genzhuang == "1" && itemData.uuid == GlobalDataScript.mainUuid) {
			//GenzhuangFlag.SetActive (true);
		} else {
			//GenzhuangFlag.SetActive (false);
		}
			
		/*
		if (GlobalDataScript.isGenzhuang && mainuuid == itemData.uuid) {
			GenzhuangFlag.SetActive(true);
		} else {
			GenzhuangFlag.SetActive(false);
		}
*/
		analysisPaiInfo(itemData);

	}


	TotalInfo itemData;

	private void analysisPaiInfo(HupaiResponseItem parms)
	{
		itemData = parms.totalInfo;
		string gangpaiStr = itemData.gang;
		if (gangpaiStr != null && gangpaiStr.Length > 0) {
			string[] gangtemps = gangpaiStr.Split(new char[1]{ ',' });
			for (int i = 0; i < gangtemps.Length; i++) {
				string item = gangtemps [i];
				GangpaiObj gangpaiObj = new GangpaiObj();
				gangpaiObj.uuid = item.Split(new char[1]{ ':' }) [0];
				gangpaiObj.cardPiont = int.Parse(item.Split(new char[1]{ ':' }) [1]);
				gangpaiObj.type = item.Split(new char[1]{ ':' }) [2];
				//增加判断是否为自己的杠牌的操作

				paiArray [gangpaiObj.cardPiont] -= 4;
				gangPaiList.Add(gangpaiObj);

				/*
				if (gangpaiObj.type == "an") {
					mdesCribe += "暗杠  ";
				} else {
					mdesCribe += "明杠  ";
				}
        */
			}
		}


		string pengpaiStr = itemData.peng;
		if (pengpaiStr != null && pengpaiStr.Length > 0) {
			
			pengpaiList = pengpaiStr.Split(new char[1]{ ',' });


			//string[] pengpaiListTTT = pengpaiList;
			List<string> pengpaiListTTT = new List<string>();
			for (int i = 0; i < pengpaiList.Length; i++) {
				if (paiArray [int.Parse(pengpaiList [i])] >= 3) {
					paiArray [int.Parse(pengpaiList [i])] -= 3;
					pengpaiListTTT.Add(pengpaiList [i]);
				}

			}
			pengpaiList = pengpaiListTTT.ToArray();
		}


		string chipaiStr = itemData.chi;
		if (chipaiStr != null && chipaiStr.Length > 0) {
			string[] chitemps = chipaiStr.Split(new char[1]{ ',' });
			for (int i = 0; i < chitemps.Length; i++) {
				string item = chitemps [i];
				ChipaiObj chipaiObj = new ChipaiObj();
				string[] pointStr = item.Split(new char[1]{ ':' }); 
				chipaiObj.cardPionts = pointStr;
				chipaiList.Add(chipaiObj);
				paiArray [int.Parse(chipaiObj.cardPionts [0])] -= 1;
				paiArray [int.Parse(chipaiObj.cardPionts [1])] -= 1;
				paiArray [int.Parse(chipaiObj.cardPionts [2])] -= 1;
			}

		}



		string hupaiStr = itemData.hu;
		if (hupaiStr != null && hupaiStr.Length > 0) {
			hupaiObj.uuid = hupaiStr.Split(new char[1]{ ':' }) [0];
			hupaiObj.cardPiont = int.Parse(hupaiStr.Split(new char[1]{ ':' }) [1]);
			hupaiObj.type = hupaiStr.Split(new char[1]{ ':' }) [2];
			//增加判断是否是自己胡牌的判断

			if (hupaiStr.Contains("d_other")) {//排除一炮多响的情况
				//mdesCribe += "点炮";
			} else if (hupaiObj.type == "zi_common") {
				//mdesCribe += "自摸      ";
				huFlagImg.SetActive(true);
				paiArray [hupaiObj.cardPiont] -= 1;

			} else if (hupaiObj.type == "d_self") {
				//mdesCribe += "接炮      ";
				huFlagImg.SetActive(true);
				paiArray [hupaiObj.cardPiont] -= 1;
			}
		}

		if (mHupaiResponseItemData.huType != null) {
			mdesCribe += mHupaiResponseItemData.huType;
		}
			
		resultDes.text = mdesCribe;
		maPais = parms.getMaPoints();
		arrangePai();
	}


	/**整理牌**/
	private void arrangePai()
	{
		
		float startPosition = 30f;
		GameObject itemTemp;

		int subPaiConut = 0;
		if (gangPaiList != null) {
			for (int i = 0; i < gangPaiList.Count; i++) {
				GangpaiObj itemgangData = gangPaiList [i];
				for (int j = 0; j < 4; j++) {

					itemTemp = Instantiate(Resources.Load("Prefab/ThrowCard/TopAndBottomCard")) as GameObject;
					itemTemp.transform.parent = paiArrayPanel.transform;
					itemTemp.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
					itemTemp.GetComponent<TopAndBottomCardScript>().setPoint(itemgangData.cardPiont);
					itemTemp.transform.localPosition = new Vector3(startPosition + ((i * 4) + j) * 44f, 0, 0);

				}
			}
			startPosition = startPosition + (gangPaiList.Count > 0 ? (gangPaiList.Count * 4 * 44f + 8f) : 0f);
		}



		if (pengpaiList != null) {
			for (int i = 0; i < pengpaiList.Length; i++) {
				string cardPoint = pengpaiList [i];
				for (int j = 0; j < 3; j++) {

					itemTemp = Instantiate(Resources.Load("Prefab/ThrowCard/TopAndBottomCard")) as GameObject;
					itemTemp.transform.parent = paiArrayPanel.transform;
					itemTemp.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
					itemTemp.GetComponent<TopAndBottomCardScript>().setPoint(int.Parse(cardPoint));

					itemTemp.transform.localPosition = new Vector3(startPosition + ((i * 3) + j) * 44f, 0, 0);


				}
			}
			startPosition = startPosition + (pengpaiList.Length > 0 ? (pengpaiList.Length * 3 * 44f + 8f) : 0f);

		}



		if (chipaiList != null) {
			for (int i = 0; i < chipaiList.Count; i++) {
				ChipaiObj itemgangData = chipaiList [i];
				for (int j = 0; j < 3; j++) {

					itemTemp = Instantiate(Resources.Load("Prefab/ThrowCard/TopAndBottomCard")) as GameObject;
					itemTemp.transform.parent = paiArrayPanel.transform;
					itemTemp.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
					itemTemp.GetComponent<TopAndBottomCardScript>().setPoint(int.Parse(itemgangData.cardPionts [j]));

					itemTemp.transform.localPosition = new Vector3(startPosition + ((i * 3) + j) * 44f, 0, 0);


				}
			}

			startPosition = startPosition + (chipaiList.Count > 0 ? (chipaiList.Count * 3 * 44f + 8f) : 0f);
		}


		bool hasHua = false;
		float huaPosition = 0;
		for (int i = 0; i < paiArray.Length; i++) {
			if (paiArray [i] > 0) {
				if (i >= NanjingConfig.HUA_INDEX) {
					continue;
					hasHua = true;
					huaPosition = 8;
				}

				for (int j = 0; j < paiArray [i]; j++) {
					itemTemp = Instantiate(Resources.Load("Prefab/ThrowCard/TopAndBottomCard")) as GameObject;
					itemTemp.transform.parent = paiArrayPanel.transform;
					itemTemp.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
					itemTemp.GetComponent<TopAndBottomCardScript>().setPoint(i);

					itemTemp.transform.localPosition = new Vector3(startPosition + subPaiConut * 44f + huaPosition, 0, 0);

					subPaiConut += 1;
				}
			}
		}

		startPosition = startPosition + (subPaiConut * 44f + huaPosition + 8f) + 40f;
		if (hupaiObj != null) {
			if (hupaiObj.type == "zi_common" || hupaiObj.type == "d_self") {
				itemTemp = Instantiate(Resources.Load("Prefab/ThrowCard/TopAndBottomCardHu")) as GameObject;
				itemTemp.transform.parent = paiArrayPanel.transform;
				itemTemp.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
				itemTemp.GetComponent<TopAndBottomCardScript>().setPoint(hupaiObj.cardPiont);
				itemTemp.transform.localPosition = new Vector3(startPosition, 0, 0);
			}
			startPosition = startPosition + 44f + 52f;
		} else {
			startPosition = startPosition + 52f;
		}
	}
}

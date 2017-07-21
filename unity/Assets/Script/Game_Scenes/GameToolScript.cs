using System;
using System.Collections.Generic;
using UnityEngine;

public class GameToolScript
{
	public GameToolScript()
	{
	}

	/// <summary>
	/// Sets the other card object position.
	/// </summary>
	/// <param name="tempList">Temp list.</param>
	/// <param name="initDiretion">Init diretion.</param>
	/// <param name="Type">Type.</param> 1- 碰，2-杠
	public void setOtherCardObjPosition(List<GameObject> tempList, String initDiretion, int type)
	{
		if (type == 1) {
			switch (initDiretion) {
				case DirectionEnum.Top: //上
					tempList [0].transform.localPosition = new Vector3(-273f, 0f); //位置                      
					break;
				case DirectionEnum.Left: //左
					tempList [0].transform.localPosition = new Vector3(0, -173); //位置              
					break;
				case DirectionEnum.Right: //右
					tempList [0].transform.localPosition = new Vector3(0, 180f); //位置                  
					break;
			}


			for (int i = 1; i < tempList.Count; i++) {

				switch (initDiretion) {
					case DirectionEnum.Top: //上
						tempList [i].transform.localPosition = new Vector3(-204f + 37 * (i - 1), 0); //位置                      
						break;
					case DirectionEnum.Left: //左
						tempList [i].transform.localPosition = new Vector3(0, -105 + (i - 1) * 23); //位置              
						break;
					case DirectionEnum.Right: //右
						tempList [i].transform.localPosition = new Vector3(0, 119 - (i - 1) * 23); //位置                  
						break;
				}
			}
		} else {

			for (int i = 0; i < tempList.Count; i++) {

				switch (initDiretion) {
					case DirectionEnum.Top: //上
						tempList [i].transform.localPosition = new Vector3(-204 + 37 * i, 0); //位置                      
						break;
					case DirectionEnum.Left: //左
						tempList [i].transform.localPosition = new Vector3(0, -105 + i * 23); //位置              
						break;
					case DirectionEnum.Right: //右
						tempList [i].transform.localPosition = new Vector3(0, 119 - i * 23); //位置                  
						break;
				}
			}
		}
	}

	public void ResetCardList(int[] cardList)
	{
		for (int i = 0; i < cardList.Length; i++) {
			cardList [i] = 0;
		}
	}

	private int jangHu = 0;

	private int remainHu(int[] pai)
	{
		int num = 0;
		for (int i = 0; i < pai.Length; i++) {
			num += pai [i];
		}
		return num;
	}

	private bool checkHu(int[] pai)
	{
		if (remainHu(pai) == 0)
			return true;

		for (int i = 0; i < pai.Length; i++) {
			if (pai [i] != 0) {
				if (pai [i] == 4) {
					pai [i] = 0;
					if (checkHu(pai))
						return true;
					pai [i] = 4;
				}

				if (pai [i] >= 3) {
					pai [i] -= 3;
					if (checkHu(pai)) {
						return true;
					}
					pai [i] += 3;
				}

				if (jangHu == 0 && pai [i] >= 2) {
					jangHu = 1;
					pai [i] -= 2;
					if (checkHu(pai))
						return true;
					pai [i] += 2;
					jangHu = 0;
				}

				if (i < 27 && (i % 9 != 7) && (i % 9 != 8) && pai [i + 1] != 0 && pai [i + 2] != 0) {
					pai [i]--;
					pai [i + 1]--;
					pai [i + 2]--;
					if (checkHu(pai))
						return true;
					pai [i]++;
					pai [i + 1]++;
					pai [i + 2]++;
				}
			}
		}
		return false;
	}

	List<int> getHuCount(int[] pai, int cardIndex = -1)
	{
		int[] paiCheck = new int[NanjingConfig.HUA_INDEX];
		for (int i = 0; i < paiCheck.Length; i++) {
			paiCheck [i] = 0;
		}

		List<int> huList = new List<int>();
		for (int i = 0; i < paiCheck.Length; i++) {
			if (pai [i] > 0) {
				if (i >= 27) {
					paiCheck [i] = 1;
				} else {
					paiCheck [i] = 1;
					int index = i % 9;
					if (index > 0)
						paiCheck [i - 1] = 1;
					if (index < 8)
						paiCheck [i + 1] = 1;
				}
			}
		}

		for (int i = 0; i < paiCheck.Length; i++) {
			if (paiCheck [i] > 0) {
				int[] cloneList = (int[])pai.Clone();
				if (cardIndex > -1)
					cloneList [cardIndex] -= 1;
				if (cloneList [i] > 3)
					continue;
				cloneList [i] += 1;
				jangHu = 0;
				if (checkHu(cloneList))
					huList.Add(i);
			}
		}
		return huList;
	}

	public int CheckHu(int[] cardList, int cardIndex, Dictionary<int, int> pengGangList, int flag)
	{
		jangHu = 0;
		int oneCardCount = 0;
		int twoCardCount = 0;
		int threeCardCount = 0;
		int fourCardCount = 0;

		int oneFengCount = 0;
		int twoFengCount = 0;
		int threeFengCount = 0;
		int fourFengCount = 0;

		int wanCount = 0;
		int tiaoCount = 0;
		int tongCount = 0;
		int fengCount = 0;

		int result = 0;
		int count = 0;
		for (int i = 0; i < cardList.Length; i++) {
			int paiCount = cardList [i];
			if (paiCount > 0) {
				if (paiCount == 1) {
					oneCardCount++;
					if (i >= 27) {
						oneFengCount++;
						return 0;
					}
				} else if (paiCount == 2) {
					twoCardCount++;
					if (i >= 27) {
						twoFengCount++;
						if (jangHu == 0) {
						}
					}
				} else if (paiCount == 3) {
					threeCardCount++;
					if (i >= 27) {
						threeFengCount++;
						cardList [i] = 0;
					}
				} else if (paiCount == 4) {
					fourCardCount++;
					if (i >= 27) {
						fourFengCount++;
					}
				}
				if (i >= 0 && i <= 8)
					wanCount += paiCount;
				if (i >= 9 && i <= 17)
					tiaoCount += paiCount;
				if (i >= 18 && i <= 26)
					tongCount += paiCount;
				if (i >= 27 && i <= 30)
					fengCount += paiCount;

				count += paiCount;
			}
		}

		// check qidui
		if (count == 14) {
			if (twoCardCount == 7)
				result += Rule.Hu_Type_Qidui;
			else if (fourCardCount == 1 && twoCardCount == 5)
				result += Rule.Hu_Type_Haohuaqidui;
			else if (fourCardCount == 2 && twoCardCount == 3)
				result += Rule.Hu_Type_Chaohaohuaqidui;
			else if (fourCardCount == 3 && twoCardCount == 1)
				result += Rule.Hu_Type_Chaochaohaohuaqidui;
		}

		bool isHu = false;
		if (result > 0)
			isHu = true;
		if (isHu == false) {
			if (twoFengCount > 1)
				return 0;
			if (fourFengCount > 0)
				return 0;
			int[] cloneList = (int[])cardList.Clone();
			isHu = checkHu(cloneList);
		}
		if (isHu == false)
			return 0;

		if (result == 0 && count == 14)
			result += Rule.Hu_Type_Menqing;

		if ((flag & Rule.Hu_Flag_Tianhu) > 0)
			result += Rule.Hu_Type_Tianhu;
		if ((flag & Rule.Hu_Flag_Dihu) > 0)
			result += Rule.Hu_Type_Dihu;
		if ((flag & Rule.Hu_Flag_Haidilao) > 0)
			result += Rule.Hu_Type_Haidilao;

		int wanPengGangCount = 0;
		int tiaoPengGangCount = 0;
		int tongPengGangCount = 0;
		int fengPengGangCount = 0;

		foreach (var i in pengGangList) {
			if (i.Key >= 0 && i.Key <= 8)
				wanPengGangCount++;
			if (i.Key >= 9 && i.Key <= 17)
				tiaoPengGangCount++;
			if (i.Key >= 18 && i.Key <= 26)
				tongPengGangCount++;
			if (i.Key >= 27 && i.Key <= 30)
				fengPengGangCount++;
		}

		bool yise = false;
		if (wanCount > 0 && (tiaoCount + tiaoPengGangCount + tongCount + tongPengGangCount) == 0)
			yise = true;
		if (tiaoCount > 0 && (wanCount + wanPengGangCount + tongCount + tongPengGangCount) == 0)
			yise = true;
		if (tongCount > 0 && (tiaoCount + tiaoPengGangCount + wanCount + wanPengGangCount) == 0)
			yise = true;

		if (yise && (fengCount + fengPengGangCount) > 0)
			result += Rule.Hu_Type_Hunyise;
		else if (yise && (fengCount + fengPengGangCount) == 0)
			result += Rule.Hu_Type_Qingyise;

		if (oneCardCount == 0 && fourCardCount == 0 && threeCardCount == 0 && twoCardCount == 1)
			result += Rule.Hu_Type_Quanqiududiao;
		else if (oneCardCount == 0 && fourCardCount == 0 && twoCardCount == 1)
			result += Rule.Hu_Type_DuiDuihu;

		if ((flag & Rule.Hu_Flag_XiaoGang) > 0)
			result += Rule.Hu_Type_Xiaogangkaihua;
		if ((flag & Rule.Hu_Flag_DaGang) > 0)
			result += Rule.Hu_Type_Dagangkaihua;

		// yajue
		if (pengGangList.ContainsKey(cardIndex)) {
			bool yadang = false;
			bool bianzhi = false;
			int huCount = getHuCount(cardList, cardIndex).Count;
			if (huCount == 1) {
				if (cardIndex < 27) {
					int card = cardIndex % 9;
					// yadang
					if (card > 0 && card < 8) {
						if (cardList [cardIndex] <= 2 && cardList [cardIndex - 1] == cardList [cardIndex] && cardList [cardIndex + 1] == cardList [cardIndex])
							yadang = true;
					}

					// bianzhi
					if (card == 2) {
						if (cardList [cardIndex] <= 2 && cardList [cardIndex - 2] == cardList [cardIndex] && cardList [cardIndex - 1] == cardList [cardIndex])
							bianzhi = true;
					}
					if (card == 6) {
						if (cardList [cardIndex] <= 2 && cardList [cardIndex + 1] == cardList [cardIndex] && cardList [cardIndex + 2] == cardList [cardIndex])
							bianzhi = true;
					}
				}
			}

			if (yadang || bianzhi)
				result += Rule.Hu_Type_Yajue;
		}

		if (result > 0 || (flag >> 8) >= 4)
			result += Rule.Hu_Type_Xiaohu;
		return result;
	}

	public void cleanTingSet()
	{
		tingSet.Clear();
	}

	private Dictionary<int, List<int>> tingSet = new Dictionary<int, List<int>>();
	private int jangTing = 0;

	private bool remainTing(int[] pai)
	{
		int oneCard = 0;
		int twoCard = 0;
		int count = 0;
		int[] card = { 0, 0 };
		for (int i = 0; i < pai.Length; i++) {
			if (pai [i] > 0) {
				if (pai [i] == 1)
					oneCard++;
				else if (pai [i] == 2)
					twoCard++;
				if (count < 2)
					card [count] = i;
				count++;
			}
		}
		if (count == 1)
			return true;
		if (count == 2) {
			if (twoCard > 0)
				return true;
			if ((card [1] - card [0]) > 2)
				return false;
			if (card [0] >= 27)
				return false;
			if (card [1] >= 27)
				return false;
			if (card [0] <= 8 && card [1] > 8)
				return false;
			if (card [0] <= 17 && card [1] > 17)
				return false;
			return true;
		}
		return false;
	}

	private bool checkTing(int[] pai)
	{
		var remain = remainTing(pai);
		if (remain)
			return true;

		for (int i = 0; i < pai.Length; i++) {
			if (pai [i] != 0) {
				if (pai [i] == 4) {
					pai [i] = 0;
					if (checkTing(pai))
						return true;
					pai [i] = 4;
				}

				if (pai [i] >= 3) {
					pai [i] -= 3;
					if (checkTing(pai)) {
						return true;
					}
					pai [i] += 3;
				}

				if (jangTing == 0 && pai [i] >= 2) {
					jangTing = 1;
					pai [i] -= 2;
					if (checkTing(pai))
						return true;
					pai [i] += 2;
					jangTing = 0;
				}

				if (i < 27 && (i % 9 != 7) && (i % 9 != 8) && pai [i + 1] != 0 && pai [i + 2] != 0) {
					pai [i]--;
					pai [i + 1]--;
					pai [i + 2]--;
					if (checkTing(pai))
						return true;
					pai [i]++;
					pai [i + 1]++;
					pai [i + 2]++;
				}
			}
		}
		return false;
	}

	private int CheckTing(int[] cardList, Dictionary<int, int> pengGangList, int flag)
	{
		jangTing = 0;
		int oneCardCount = 0;
		int twoCardCount = 0;
		int threeCardCount = 0;
		int fourCardCount = 0;

		int oneFengCount = 0;
		int twoFengCount = 0;
		int threeFengCount = 0;
		int fourFengCount = 0;

		int wanCount = 0;
		int tiaoCount = 0;
		int tongCount = 0;
		int fengCount = 0;

		int result = 0;
		int count = 0;
		for (int i = 0; i < cardList.Length; i++) {
			int paiCount = cardList [i];
			if (paiCount > 0) {
				if (paiCount == 1) {
					oneCardCount++;
					if (i >= 27) {
						oneFengCount++;
					}
				} else if (paiCount == 2) {
					twoCardCount++;
					if (i >= 27) {
						twoFengCount++;
					}
				} else if (paiCount == 3) {
					threeCardCount++;
					if (i >= 27) {
						threeFengCount++;
					}
				} else if (paiCount == 4) {
					fourCardCount++;
					if (i >= 27) {
						fourFengCount++;
					}
				}
				if (i >= 0 && i <= 8)
					wanCount += paiCount;
				if (i >= 9 && i <= 17)
					tiaoCount += paiCount;
				if (i >= 18 && i <= 26)
					tongCount += paiCount;
				if (i >= 27 && i <= 30)
					fengCount += paiCount;

				count += paiCount;
			}
		}

		// check qidui
		if (count == 13) {
			if (twoCardCount == 6)
				result += Rule.Hu_Type_Qidui;
			else if (fourCardCount == 1 && twoCardCount == 4)
				result += Rule.Hu_Type_Haohuaqidui;
			else if (fourCardCount == 2 && twoCardCount == 2)
				result += Rule.Hu_Type_Chaohaohuaqidui;
			else if (fourCardCount == 3)
				result += Rule.Hu_Type_Chaochaohaohuaqidui;
		}

		bool isTing = false;
		if (result > 0)
			isTing = true;
		if (isTing == false) {
			if (twoFengCount > 2)
				return 0;
			if (fourFengCount > 0)
				return 0;
			int[] cloneList = (int[])cardList.Clone();
			isTing = checkTing(cloneList);
		}
		if (isTing == false)
			return 0;

		if (result == 0 && count == 13)
			result += Rule.Hu_Type_Menqing;

		if ((flag & Rule.Hu_Flag_Tianhu) > 0)
			result += Rule.Hu_Type_Tianhu;
		if ((flag & Rule.Hu_Flag_Dihu) > 0)
			result += Rule.Hu_Type_Dihu;
		if ((flag & Rule.Hu_Flag_Haidilao) > 0)
			result += Rule.Hu_Type_Haidilao;

		int wanPengGangCount = 0;
		int tiaoPengGangCount = 0;
		int tongPengGangCount = 0;
		int fengPengGangCount = 0;

		foreach (var i in pengGangList) {
			if (i.Key >= 0 && i.Key <= 8)
				wanPengGangCount++;
			if (i.Key >= 9 && i.Key <= 17)
				tiaoPengGangCount++;
			if (i.Key >= 18 && i.Key <= 26)
				tongPengGangCount++;
			if (i.Key >= 27 && i.Key <= 30)
				fengPengGangCount++;
		}

		bool yise = false;
		if (wanCount > 0 && (tiaoCount + tiaoPengGangCount + tongCount + tongPengGangCount) == 0)
			yise = true;
		if (tiaoCount > 0 && (wanCount + wanPengGangCount + tongCount + tongPengGangCount) == 0)
			yise = true;
		if (tongCount > 0 && (tiaoCount + tiaoPengGangCount + wanCount + wanPengGangCount) == 0)
			yise = true;
		if (yise) {
			if ((fengCount + fengPengGangCount) > 0)
				result += Rule.Hu_Type_Hunyise;
			else
				result += Rule.Hu_Type_Qingyise;
		}

		if (oneCardCount == 1 && fourCardCount == 0 && twoCardCount == 0 && threeCardCount == 0)
			result += Rule.Hu_Type_Quanqiududiao;
		else if (oneCardCount == 0 && fourCardCount == 0 && twoCardCount == 2)
			result += Rule.Hu_Type_DuiDuihu;
		else if (oneCardCount == 1 && fourCardCount == 0 && twoCardCount == 0)
			result += Rule.Hu_Type_DuiDuihu;

		if ((flag & Rule.Hu_Flag_XiaoGang) > 0)
			result += Rule.Hu_Type_Xiaogangkaihua;
		if ((flag & Rule.Hu_Flag_DaGang) > 0)
			result += Rule.Hu_Type_Dagangkaihua;

		// yajue
		if (pengGangList.Count > 0) {
			bool yadang = false;
			bool bianzhi = false;
			List<int> huList = getHuCount(cardList);
			if (huList.Count == 1) {
				int cardIndex = huList [0];
				if (cardIndex < 27 && pengGangList.ContainsKey(cardIndex)) {
					int card = cardIndex % 9;
					// yadang
					if (card > 0 && card < 8) {
						if (cardList [cardIndex] <= 2 && cardList [cardIndex - 1] == cardList [cardIndex] && cardList [cardIndex + 1] == cardList [cardIndex])
							yadang = true;
					}

					// bianzhi
					if (card == 2) {
						if (cardList [cardIndex] <= 2 && cardList [cardIndex - 2] == cardList [cardIndex] && cardList [cardIndex - 1] == cardList [cardIndex])
							bianzhi = true;
					}
					if (card == 6) {
						if (cardList [cardIndex] <= 2 && cardList [cardIndex + 1] == cardList [cardIndex] && cardList [cardIndex + 2] == cardList [cardIndex])
							bianzhi = true;
					}
				}
			}

			if (yadang || bianzhi)
				result += Rule.Hu_Type_Yajue;
		}

		if (result > 0 || (flag >> 8) >= 4)
			result += Rule.Hu_Type_Xiaohu;
		return result;
	}

	public int CheckTing(List<GameObject> handList, List<List<GameObject>> pengGangList, int flag)
	{
		int[] pai = new int[NanjingConfig.HUA_INDEX];
		ResetCardList(pai);
		for (int i = 0; i < handList.Count; i++) {
			int card = handList [i].GetComponent<bottomScript>().getPoint();
			if (card < NanjingConfig.HUA_INDEX)
				pai [card]++;
		}
		Dictionary<int, int> pengGang = new Dictionary<int, int>();
		for (int i = 0; i < pengGangList.Count; i++) {
			if (pengGangList [i].Count > 0) {
				var card = pengGangList [i] [0].GetComponent<TopAndBottomCardScript>();
				if (card != null) {
					pengGang.Add(card.getPoint(), pengGangList [i].Count);
				}
			}
		}

		return CheckTing(pai, pengGang, flag);
	}

	public List<int> CheckTingPickCard(int[] cardList, Dictionary<int, int> pengGangList, int flag)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < cardList.Length; i++) {
			if (cardList [i] > 0) {
				int[] cloneList = (int[])cardList.Clone();
				cloneList [i] -= 1;
				int result = CheckTing(cloneList, pengGangList, flag);
				if (result > 0)
					list.Add(i);
			}
		}
		return list;
	}

	public List<int> CheckTingPickCard(List<GameObject> handList, List<List<GameObject>> pengGangList, int flag)
	{
		int[] pai = new int[NanjingConfig.HUA_INDEX];
		ResetCardList(pai);
		for (int i = 0; i < handList.Count; i++) {
			int card = handList [i].GetComponent<bottomScript>().getPoint();
			if (card < NanjingConfig.HUA_INDEX)
				pai [card]++;
		}
		Dictionary<int, int> pengGang = new Dictionary<int, int>();
		for (int i = 0; i < pengGangList.Count; i++) {
			if (pengGangList [i].Count > 0) {
				var card = pengGangList [i] [0].GetComponent<TopAndBottomCardScript>();
				if (card != null) {
					pengGang.Add(card.getPoint(), pengGangList [i].Count);
				}
			}
		}

		return CheckTingPickCard(pai, pengGang, flag);
	}

	public List<int> CheckTingCard(int[] cardList, int[] checkCard, Dictionary<int, int> pengGangList, int flag)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < checkCard.Length; i++) {
			if (checkCard [i] > 0 && cardList [i] < 4) {
				int[] cloneList = (int[])cardList.Clone();
				cloneList [i] += 1;
				int result = CheckHu(cloneList, i, pengGangList, flag);
				if (result > 0)
					list.Add(i);
			}
		}
		return list;
	}

	public List<int> CheckTingCard(List<GameObject> handList, List<List<GameObject>> pengGangList, int flag)
	{
		int[] pai = new int[NanjingConfig.HUA_INDEX];
		int[] paiCheck = new int[NanjingConfig.HUA_INDEX];
		ResetCardList(pai);
		ResetCardList(paiCheck);
		for (int i = 0; i < handList.Count; i++) {
			int card = handList [i].GetComponent<bottomScript>().getPoint();
			if (card < NanjingConfig.HUA_INDEX) {
				pai [card]++;
				if (card >= 27) {
					paiCheck [card] = 1;
				} else {
					paiCheck [card] = 1;
					int temp = card % 9;
					if (temp == 0) {
						paiCheck [card + 1] = 1;
					} else if (temp == 8) {
						paiCheck [card - 1] = 1;
					} else {
						paiCheck [card + 1] = 1;
						paiCheck [card - 1] = 1;
					}
				}
			}
		}
		Dictionary<int, int> pengGang = new Dictionary<int, int>();
		for (int i = 0; i < pengGangList.Count; i++) {
			if (pengGangList [i].Count > 0) {
				var card = pengGangList [i] [0].GetComponent<TopAndBottomCardScript>();
				if (card != null) {
					pengGang.Add(card.getPoint(), pengGangList [i].Count);
				}
			}
		}

		return CheckTingCard(pai, paiCheck, pengGang, flag);
	}

	public List<int> CheckTingCard(List<GameObject> handList, int pickCard, List<List<GameObject>> pengGangList, int flag)
	{
		int[] pai = new int[NanjingConfig.HUA_INDEX];
		int[] paiCheck = new int[NanjingConfig.HUA_INDEX];
		ResetCardList(pai);
		ResetCardList(paiCheck);
		bool removeFlag = false;
		for (int i = 0; i < handList.Count; i++) {
			int card = handList [i].GetComponent<bottomScript>().getPoint();
			if (card < NanjingConfig.HUA_INDEX) {
				if (removeFlag == false && card == pickCard) {
					removeFlag = true;
					continue;
				}
				pai [card]++;
				if (card >= 27) {
					paiCheck [card] = 1;
				} else {
					paiCheck [card] = 1;
					int temp = card % 9;
					if (temp == 0) {
						paiCheck [card + 1] = 1;
					} else if (temp == 8) {
						paiCheck [card - 1] = 1;
					} else {
						paiCheck [card + 1] = 1;
						paiCheck [card - 1] = 1;
					}
				}
			}
		}

		Dictionary<int, int> pengGang = new Dictionary<int, int>();
		for (int i = 0; i < pengGangList.Count; i++) {
			if (pengGangList [i].Count > 0) {
				var card = pengGangList [i] [0].GetComponent<TopAndBottomCardScript>();
				if (card != null) {
					pengGang.Add(card.getPoint(), pengGangList [i].Count);
				}
			}
		}

		return CheckTingCard(pai, paiCheck, pengGang, flag);
	}

	public class Rule
	{
		public static int Hu_Flag_Tianhu = 1 << 1;
		public static int Hu_Flag_Dihu = 1 << 2;
		public static int Hu_Flag_Haidilao = 1 << 3;
		public static int Hu_Flag_DaGang = 1 << 4;
		public static int Hu_Flag_XiaoGang = 1 << 5;
		public static int Hu_Flag_Hua = 1 << 8;

		public static int Hu_Type_Dahu = 0x3FFFC;
		public static int Hu_Type_Xiaohu = 1 << 0;
		public static int Hu_Type_Menqing = 1 << 1;
		public static int Hu_Type_Hunyise = 1 << 2;
		public static int Hu_Type_Qingyise = 1 << 3;
		public static int Hu_Type_DuiDuihu = 1 << 4;
		public static int Hu_Type_Quanqiududiao = 1 << 5;
		public static int Hu_Type_Qidui = 1 << 6;
		public static int Hu_Type_Haohuaqidui = 1 << 7;
		public static int Hu_Type_Chaohaohuaqidui = 1 << 8;
		public static int Hu_Type_Chaochaohaohuaqidui = 1 << 9;
		public static int Hu_Type_Xiaogangkaihua = 1 << 10;
		public static int Hu_Type_Dagangkaihua = 1 << 11;
		public static int Hu_Type_Tianhu = 1 << 12;
		public static int Hu_Type_Dihu = 1 << 13;
		public static int Hu_Type_Yajue = 1 << 14;
		public static int Hu_Type_Wuhuaguo = 1 << 15;
		public static int Hu_Type_Qianggang = 1 << 16;
		public static int Hu_Type_Haidilao = 1 << 17;
		public static int Hu_Type_BaoGang = 1 << 18;
		public static int Hu_Type_BaoPeng = 1 << 19;
	}
}

public class DirectionEnum
{
	public const string Bottom = "B";
	public const string Right = "R";
	public const string Top = "T";
	public const string Left = "L";
}

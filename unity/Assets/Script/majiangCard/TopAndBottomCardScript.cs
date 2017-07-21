using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TopAndBottomCardScript : MonoBehaviour
{
	private int cardPoint;
	private bool isTurn_ = false;
  
	//=========================================
	public Image cardImg;


	public void setPoint(int _cardPoint)
	{
		cardPoint = _cardPoint;//设置所有牌指针
		cardImg.sprite = Resources.Load("Cards/Big/b" + cardPoint, typeof(Sprite)) as Sprite;

	}

	public void setLefAndRightPoint(int _cardPoint)
	{
		cardPoint = _cardPoint;//设置所有牌指针
		cardImg.sprite = Resources.Load("Cards/Left&Right/lr" + cardPoint, typeof(Sprite)) as Sprite;
	}

	public int getPoint()
	{
		return cardPoint;
	}

	public bool isTurn {
		get { return isTurn_; }
		set { isTurn_ = value; }
	}
}

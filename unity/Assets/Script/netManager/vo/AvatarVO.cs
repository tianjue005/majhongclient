using System;

/**
 * 登录接口返回数据封装
 * 
 */
public class AvatarVO
{
	public Account account;

	//public int cardIndex;
	public bool isOnLine;
	public bool isReady;
	public bool main;
	public int mainCount;

	public int roomId;

	//出牌
	public int[] chupais;

	//剩余牌数
	public int commonCards;

	public int[][] paiArray;

	//胡牌才有数据，登录过程不管
	public HupaiResponseItem huReturnObjectVO;

	//分数
	public  int scores;

	public string IP;
	public double longitude = 0;
	public double latitude = 0;
	public string address;

	public AvatarVO()
	{
	}

	public void resetData()
	{
		//	cardIndex = 0;
		isOnLine = false;
		isReady = false;
		main = false;
		roomId = 0;
	}
}


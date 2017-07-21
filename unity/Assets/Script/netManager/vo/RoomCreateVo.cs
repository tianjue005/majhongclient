using System;

[Serializable]
public class RoomCreateVo
{
	public bool hong = false;
	public int ma = 0;
	public int roomId;
	public int roomType;

	/**局数**/
	public int roundNumber;

	public bool sevenDouble = false;

	public int ziMo;
	//1：自摸胡；2、抢杠胡

	public int xiaYu = 0;
	public string name;
	public bool addWordCard = false;
	public int magnification = 0;

	// nanjing
	public bool chengbei = false;
	public bool aa = false;
	public int zashu = 1;
	public int paofen = 0;
	public int roundtype = 0;
	// 0 yuanzi 1 changkaitou
	public int yuanzishu = 0;
	public int yuanzijiesu = 1;
	public bool zhanzhuangbi = false;
	public bool guozhuangbi = false;
	public bool fengfa = false;

	public RoomCreateVo()
	{
	}

	public int realRoundNumber()
	{
		if (roundtype == 0) {
			return NanjingConfig.ROUND_DEFAULT;
		} else {
			return NanjingConfig.ROUND_SET [roundNumber];
		}
	}

	public int realYuanzishu()
	{
		return NanjingConfig.YUANZI_COUNT_SET [yuanzishu];
	}

	public int realYuanziRule()
	{
		return NanjingConfig.YUANZI_RULE_SET [yuanzijiesu];
	}

	public int realZaShu()
	{
		return NanjingConfig.ZASHU_SET [zashu];
	}

	public int realPaofen()
	{
		return NanjingConfig.PAOFEN_SET [paofen];
	}

	public int realScore()
	{
		if (roundtype == 0) {
			return NanjingConfig.YUANZI_COUNT_SET [yuanzishu];
		} else {
			return NanjingConfig.SCORE_DEFAULT;
		}
	}
}


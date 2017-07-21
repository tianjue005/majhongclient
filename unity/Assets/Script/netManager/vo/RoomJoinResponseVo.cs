using System;
using System.Collections.Generic;

[Serializable]
public class RoomJoinResponseVo
{
	public bool addWordCard;
	public bool hong;
	public int ma;
	public string name;
	public int roomId;
	public int roomType;
	public int roundNumber;
	public bool sevenDouble;
	public int xiaYu;
	public int ziMo;
	public int magnification;

	// nanjing
	public bool chengbei = false;
	public bool aa = false;
	public int zashu = 0;
	public int paofen = 0;
	public int roundtype = 0;
	// 0 yuanzi 1 changkaitou
	public int yuanzishu = 0;
	public int yuanzijiesu = 1;
	public bool zhanzhuangbi = false;
	public bool guozhuangbi = false;
	public bool fengfa = false;

	public List<AvatarVO> playerList;
	//public LastOperationVo lastOperationVo;
	public RoomJoinResponseVo()
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
}


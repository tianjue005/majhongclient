using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePreferences
{
	private static GamePreferences _instance;

	public static GamePreferences Instance {
		get {
			if (_instance == null)
				_instance = new GamePreferences();
			return _instance;
		}
	}

	private GamePreferences()
	{
	}

	public static int GetUniqueId()
	{
		return PlayerPrefs.GetInt("UniqueId", -1);
	}

	public static void SetUniqueId(int id)
	{
		PlayerPrefs.SetInt("UniqueId", id);
	}


	//======================================

	private string _sessionId = null;

	public  string SessionId {
		get {
			if (Utils.IsNull(_sessionId)) {
				_sessionId = PlayerPrefs.GetString("sessionid", "");
			}
			return _sessionId;
		}

		set {
			_sessionId = value;
			PlayerPrefs.SetString("sessionid", value);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
	public static bool IsNull(string args)
	{
		if (args == null || args == "")
			return true;
		return false;
	}
}

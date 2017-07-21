using System;

public class ZhanjiSearchRequest: ClientRequest
{
	public ZhanjiSearchRequest(string roomid)
	{
		headCode = APIS.ZHANJI_SEARCH_REQUEST;
		messageContent = roomid;
	}
}


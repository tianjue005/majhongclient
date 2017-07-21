using System;

public class HeadRequest : ClientRequest
{
	public HeadRequest()
	{
		headCode = APIS.Head;
		messageContent = "";
	}
}


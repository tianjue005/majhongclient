using System;

public class GetContactInfoRequest :ClientRequest
{
	public GetContactInfoRequest()
	{
		headCode = APIS.CONTACT_INFO_REQUEST;
		messageContent = "";
	}
}


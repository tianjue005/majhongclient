using System;
using LitJson;

namespace AssemblyCSharp {
  public class BuhuaRequest : ClientRequest {
    public BuhuaRequest(int cardPoint) {
      headCode = APIS.BUHUA_REQUEST;
      BuhuaRequestVO vo = new BuhuaRequestVO();
      vo.cardPoint = cardPoint;
      messageContent = JsonMapper.ToJson(vo);
    }
  }
}
using System;
using LitJson;

namespace AssemblyCSharp {
  public class MessageBoxRequest : ClientRequest {
    public MessageBoxRequest(int type, int uuid, string message) {
      headCode = APIS.MessageBox_Request;

      MessageRequestVo vo = new MessageRequestVo();
      vo.type = type;
      vo.uuid = uuid;
      vo.message = message;
      messageContent = JsonMapper.ToJson(vo);
    }
  }
}
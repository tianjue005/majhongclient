using System;
using LitJson;

namespace AssemblyCSharp {
  public class LocationRequest : ClientRequest {
    public LocationRequest(double longitude, double latitude, string address) {
      headCode = APIS.MSG_LOCATION;

      LocationRequestVo vo = new LocationRequestVo();
      vo.longitude = longitude;
      vo.latitude = latitude;
      vo.address = address;
      messageContent = JsonMapper.ToJson(vo);
    }
  }
}


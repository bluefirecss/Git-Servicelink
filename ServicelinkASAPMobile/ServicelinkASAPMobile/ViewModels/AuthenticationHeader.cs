using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicelinkASAPMobile.ViewModels
{
    [Serializable()]
    public class AuthenticationHeader : System.Web.Services.Protocols.SoapHeader
    {
        public string Username;
        public string Password;
        public string Version;
        public string DeviceType;

        public string GetJson(AuthenticationHeader authHeader)
        {
#if __IOS__
			DeviceType = "iOS";
#elif __ANDROID__
			DeviceType = "Android";
#endif
            Username = authHeader.Username;
            Password = authHeader.Password;
            Version = authHeader.Version;
            DeviceType = authHeader.DeviceType;
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}

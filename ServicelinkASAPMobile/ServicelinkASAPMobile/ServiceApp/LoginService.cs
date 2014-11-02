using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.ViewModels;

namespace ServicelinkASAPMobile.Service
{
    public class LoginService 
    {
       
        public Task<bool> LoginAsync(string username, string password, string deviceType, string version, CancellationToken cancellationToken = default(CancellationToken))
        {
            string url = ConfigurationManager.AppSettings["wsUrl"] + "/AuthenticateDevice";
            WebServiceClient client = new WebServiceClient(url);
            return Task.Factory.StartNew(() =>
            {               
                AuthenticationHeader authHeader = new AuthenticationHeader();
                authHeader.Username = username;
                authHeader.Password = password;
                authHeader.DeviceType = deviceType;
                authHeader.Version = version;
                AccountResponse response = client.UserAuthenticate(authHeader);
                if (response.Success == true)
                {
                    return true;
                }
                else { return false; }
            }, cancellationToken);
        }

    }
}

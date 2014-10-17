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
    public static class LoginWebService
    {

        public static bool LoginAsync(string username, string password, string deviceType, string version, CancellationToken cancellationToken = default(CancellationToken))
        {	   
			bool succeed = false;
			WebServiceClient client = new WebServiceClient (ProjConfiguration.GetWebMehtod(ProjConfiguration.WebMethod.AuthenticateDevice));
            Task.Factory.StartNew(() =>
            {               
                AuthenticationHeader authHeader = new AuthenticationHeader();
                authHeader.Username = username;
                authHeader.Password = password;
                authHeader.DeviceType = deviceType;
                authHeader.Version = version;
                AccountResponse response = client.UserAuthenticate(authHeader);
                if (response.Success == true)
                {
						succeed = true;
                }
                
            }, cancellationToken);

			return succeed;
        }

    }
}

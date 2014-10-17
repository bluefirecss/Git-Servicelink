using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.ViewModels;

namespace ServicelinkASAPMobile.Service
{
    public class AccountDataAccess
    {
        public Task<List<User>> GetUserAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
			return Database.GetConnection(cancellationToken)
                .Table<User>()
                .OrderBy(i => i.UserName)
                .ToListAsync();
		
        }

        public Task<int> SaveUserAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user.UserName == "")
                return Database.GetConnection(cancellationToken).InsertAsync(user);
            else
                return Database.GetConnection(cancellationToken).UpdateAsync(user);
        }

		public AuthenticationHeader GetAuthenticationHeader(){
			User user = new User ();
			GetUserAsync ().ContinueWith (t => {
				user= t.Result.FirstOrDefault();
			});

			AuthenticationHeader authHeader = new AuthenticationHeader();
			authHeader.Username = user.UserName;
			authHeader.Password = user.Password;
			authHeader.DeviceType = ProjConfiguration.deviceType;
			authHeader.Version = ProjConfiguration.appVersion;

			return authHeader;
		}

		public Boolean AllowUserLogin(){
			Boolean authenticated = true;
			User user = null;
			Boolean changed = false;
			DateTime lastLogin;
			Boolean loggedOut = false;

			GetUserAsync().ContinueWith (t => {
				user = t.Result.FirstOrDefault();

			});

			if (user == null){
				authenticated = false;
			} else {
				changed = user.UserInfoChanged;
				lastLogin= user.LastSuccessfulLoginDate;
				loggedOut= user.UserLoggedout;

				if (changed) {
					authenticated = false;
				} else if (loggedOut) {
					authenticated = false;
				} else {
					if (lastLogin.AddDays (90).CompareTo (DateTime.Now) >= 0) {
						authenticated = false;
					}
				}
			}
			return authenticated;
		}
    }
}

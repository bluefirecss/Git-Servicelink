using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServicelinkASAPMobile.Data;

namespace ServicelinkASAPMobile
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

		public Boolean AllowUserLogin(){
			Boolean authenticated = true;
            User user = null;
            GetUserAsync().ContinueWith(t =>
             {
                 user = t.Result.FirstOrDefault<User>();
             });
			if (user == null) {
				authenticated = false;
			} else {
				Boolean changed = user.UserInfoChanged;
				DateTime lastLogin = user.LastSuccessfulLoginDate;
				Boolean loggedOut = user.UserLoggedout;
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

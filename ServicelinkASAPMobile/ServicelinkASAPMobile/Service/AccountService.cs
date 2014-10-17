using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServicelinkASAPMobile.Data;

namespace ServicelinkASAPMobile.Service
{
    public class AccountService
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
    }
}

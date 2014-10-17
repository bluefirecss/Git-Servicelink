using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServicelinkASAPMobile.Data;

namespace ServicelinkASAPMobile
{
    public interface IAccountService
    {

        Task<List<User>> GetItemsAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<int> SaveUserAsync(User user, CancellationToken cancellationToken = default(CancellationToken));

    }
}

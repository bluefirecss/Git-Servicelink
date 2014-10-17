using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServicelinkASAPMobile
{
	public interface ILoginService
	{
        Task<bool> LoginAsync(string username, string password, CancellationToken cancellationToken = default(CancellationToken));
	}
}


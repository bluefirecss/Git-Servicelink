using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServicelinkASAPMobile.Data;

namespace ServicelinkASAPMobile
{
    public interface IPostingService
    {

        Task<List<Posting>> GetAssignmentsAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<List<Document>> GetDocumentsAsync(CancellationToken cancellationToken = default(CancellationToken));


    }
}


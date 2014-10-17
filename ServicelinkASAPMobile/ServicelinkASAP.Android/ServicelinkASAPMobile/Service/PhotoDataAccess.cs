using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServicelinkASAPMobile.Data;

namespace ServicelinkASAP.Android
{
	public class PhotoDataAccess
	{
		public PhotoDataAccess ()
		{
		}


		public Task<List<Photo>> GetPhotos_Async(string orderID, CancellationToken cancelToke = default(CancellationToken) )
		{
			return Database.GetConnection(cancelToke)
				.QueryAsync<Photo>(@"
                    select Photos.*,                                
                           COUNT(Photos.Id) AS TotalPhotos                               
                    from Photos
                    where Photos.OrderID == ?              
                ", orderID);
		}

		public Task<int> DeletePhoto_Async(Photo photo, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Database.GetConnection(cancellationToken).DeleteAsync(photo);
		}

		public Task<int> DeletePhotoList_Async(List<Photo> photos, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Database.GetConnection(cancellationToken).DeleteAsync(photos);
		}
	}
}


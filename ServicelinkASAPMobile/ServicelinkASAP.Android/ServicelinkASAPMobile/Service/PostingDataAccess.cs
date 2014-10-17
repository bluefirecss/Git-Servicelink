using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServicelinkASAPMobile.Data;

namespace ServicelinkASAPMobile.Service
{
    public class PostingDataAccess
    {
		public Task<List<Posting>> GetPendingPostings_Async( CancellationToken cancelToke = default(CancellationToken) )
        {
            return Database.GetConnection(cancelToke)
               .QueryAsync<Posting>(@"
                    select Posting.*,                                
                           COUNT(Posting.Id) AS TotalPostings                               
                    from Posting
                    where Posting.Status != ? and Posting.Status != ? and Posting.Status != ?              
                ", PostingStatus.Rejected, PostingStatus.Complete, PostingStatus.Cancelled);
        }

		public Task<List<Posting>> GetCompletedPostings_Async( CancellationToken cancelToke = default(CancellationToken) )
		{
			return Database.GetConnection(cancelToke)
				.QueryAsync<Posting>(@"
                    select Posting.*,                                
                           COUNT(Posting.Id) AS TotalPostings                               
                    from Posting
                    where Posting.Status == ? or Posting.Status != ?              
                ", PostingStatus.Complete, PostingStatus.Rejected);
		}

		public Task<List<Posting>> GetCancelledPostings_Async( CancellationToken cancelToke = default(CancellationToken) )
		{
			return Database.GetConnection(cancelToke)
				.QueryAsync<Posting>(@"
                    select Posting.*,                                
                           COUNT(Posting.Id) AS TotalPostings                               
                    from Posting
                    where Posting.Status == ?              
                ", PostingStatus.Cancelled);
		}


        //public Task<List<Item>> GetItemsAsync(CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    return Database.GetConnection(cancellationToken)
        //        .Table<Item>()
        //        .OrderBy(i => i.Name)
        //        .ToListAsync();
        //}

        public Task<List<Posting>> GetPostByID_Async(string orderID, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Database.GetConnection(cancellationToken)
                .QueryAsync<Posting>(@"
                    select Posting.*
                    from Posting
                    where  Posting.OrderID= ?",
                    orderID);
        }

        public Task<int> SavePosting_Async(Posting posting, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Database.GetConnection(cancellationToken)
                .UpdateAsync(posting);
        }

        //public Task<int> SaveLaborAsync(Labor labor, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    if (labor.Id == 0)
        //        return Database.GetConnection(cancellationToken).InsertAsync(labor);
        //    else
        //        return Database.GetConnection(cancellationToken).UpdateAsync(labor);
        //}


        public Task<int> DeletePosting_Async(Posting posting, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Database.GetConnection(cancellationToken).DeleteAsync(posting);
        }

		public Task<int> DeletePostingList_Async(List<Posting> postings, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Database.GetConnection(cancellationToken).DeleteAsync(postings);
		}

        private List<Posting> _postings;
		public Task<int> DeletePostingsByStatus_Async(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_postings == null) {
				_postings = new List<Posting> {
					new Posting { Status = PostingStatus.Complete },
					new Posting { Status = PostingStatus.Rejected },                      
				};
			}
			/*
                foreach (Posting p in _postings)
                {
                    Task<int> succeed = DeletePosting_Async(p, cancellationToken);
                    if (succeed.Result == 0)
                    {
                        failed.Add(p);
                    }
                }
           */
			return  DeletePostingList_Async (_postings, cancellationToken);

		}
    }
}

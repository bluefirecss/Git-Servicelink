using System;
using System.Collections.Generic;

using Android.App;
using Android.Util;
using Android.OS;
using Android.Content;

using ServicelinkASAPMobile;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.Utilities;

namespace ServicelinkASAPMobile
{

	[Service]
	public class SyncService:Android.App.Service
	{

		PostingDataAccess pda = new PostingDataAccess();
		Http http = new Http();
		public SyncService ()
		{
		}

		readonly string logTag = "Sync Service";
		IBinder binder;

		public override IBinder OnBind (Intent intent)
		{
			binder = new ServiceBinder(this);
			return binder;
		}

		public void StartSyncing(){
			List<Posting> postings = new List<Posting>();
			//string error = "";
			http.UploadPostings (postings.ToArray()).ContinueWith(t=>{

				if (t.Result.Count == 0) {
					Log.Debug (logTag, "Syncing has errors");
				} else {
					foreach (string orderID in t.Result) {
						pda.DeletePostingByID_Async (orderID);
					}
				}
			});

		}

	}
}


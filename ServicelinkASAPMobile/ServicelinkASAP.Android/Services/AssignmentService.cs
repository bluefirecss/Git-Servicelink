
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using ServicelinkASAPMobile;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.Service;

namespace ServicelinkASAP.Android
{
	[Service]
	[IntentFilter(new String[]{"ServicelinkASAP.AssignmentService"})]
	public class AssignmentService : IntentService
	{
		IBinder binder;
		List<Posting> postings;
		PostingDataAccess pda= new PostingDataAccess();
		Http http = new Http();
		public const string AssignmentUpdatedAction = "AssignmentUpdated";

		protected override void OnHandleIntent (Intent intent)
		{
			if (intent.GetStringExtra ("Posting") == "true") {
				http.GetPostings ().ContinueWith (t=>{
					postings= t.Result;
				});

				foreach(Posting p in postings){
					pda.SavePosting_Async (p).ContinueWith (t => {
						Console.Write(t.Result.ToString());
					});
				}
			}
			var assignmentIntent = new Intent (AssignmentUpdatedAction); 

			SendOrderedBroadcast (assignmentIntent, null);
		}

		public List<Posting> GetPostings(){
			return postings;
		}
			
		public override IBinder OnBind(Intent intent){
			binder = new AssignmentServiceBinder (this);
			return binder;
		}

	


		/*
		public override void OnReceive (Context context, Intent intent)
		{
			Toast.MakeText (context, "Received intent!", ToastLength.Short).Show ();
		}*/
	}

	public class AssignmentServiceBinder:Binder{
		AssignmentService service;

		public AssignmentServiceBinder(AssignmentService service){
			this.service = service;
		}
		public AssignmentService GetAssignmentService(){
			return service;
		}
	}
}


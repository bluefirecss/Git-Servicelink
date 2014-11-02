using System;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Util;

using ServicelinkASAPMobile;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.Service;

namespace ServicelinkASAP.Android
{
	[Activity (Label= "AssignmentService")]
	public class ExpandableListViewActivity:ExpandableListActivity
	{
		bool isBound = false;
		Http serviceLinkWS = new Http();
		List<Menu> tableItems = new List<Menu>();
		PostingDataAccess pda = new PostingDataAccess ();
		ImageButton btnRefresh;
		AssignmentServiceBinder binder;
		ProgressBar progressbar;
		AssignmentReceiver assignmentReceiver;
		Intent assignmentServiceIntent;
		AssignmentServiceConnection assignmentServiceConnection;
        public Action<MenuItem, string> MenuItemSelected = delegate { };
        //ApplicationShared app;
        //AssignmentListViewFragment assignmentViewFragment;

		int pending_P = 0;
		int complete_P = 0;
		int cancelled_P =0;

		int pending_S = 0;
		int complete_S = 0;
		int cancelled_S =0;
		int ppt_S =0;
		int hold_S =0;

		public ExpandableListViewActivity ()
		{
		}

		protected override void OnStart(){
			base.OnStart ();
            //app = (ApplicationShared)Application.ApplicationContext;  

			var intentFilter = new IntentFilter (AssignmentService.AssignmentUpdatedAction){Priority = (int)IntentFilterPriority.HighPriority};
			RegisterReceiver (assignmentReceiver, intentFilter);

			assignmentServiceConnection = new AssignmentServiceConnection (this);
			BindService (assignmentServiceIntent, assignmentServiceConnection, Bind.AutoCreate);


		}

		protected override void OnStop ()
		{
			base.OnStop ();

			if (isBound) {
				UnbindService (assignmentServiceConnection);

				isBound = false;
			}

			UnregisterReceiver (assignmentReceiver);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			if (ApplicationShared.Current.GetNetworkActive()) {
				GetMenuData ();
			}
		}

		protected override void OnCreate(Bundle bundle){
			base.OnCreate(bundle);
			assignmentServiceIntent = new Intent ("ServicelinkASAP.AssignmentService");
			assignmentReceiver = new AssignmentReceiver ();

			SetContentView (Resource.Layout.MenuLayout);
		    btnRefresh = FindViewById<ImageButton> (Resource.Id.btnRefresh);
			btnRefresh.Visibility = ViewStates.Invisible;
			progressbar = FindViewById<ProgressBar> (Resource.Id.progressRing);
			progressbar.Visibility = ViewStates.Visible;
			btnRefresh.Click += (object sender, EventArgs e) => {
                btnRefresh.Visibility = ViewStates.Invisible;
                progressbar.Visibility = ViewStates.Visible;
				if (!ApplicationShared.Current.GetNetworkActive())
                {
                    progressbar.Visibility = ViewStates.Invisible;
                    btnRefresh.Visibility = ViewStates.Visible;
					ApplicationShared.Current.InvokeBaseAlertDialog("Sync Error", "There is no network connection, failed to sync data.");
                    return;
                }
				List<Posting> postings = new List<Posting>();
				serviceLinkWS.GetPostings().ContinueWith(t =>
					{
						postings = t.Result;
					});

				if (postings.Count > 0)
				{
					foreach (Posting p in postings)
					{
						pda.SavePosting_Async(p);
					}
					GetMenuData();
				}
			};
			GetMenuData ();
			ExpandableListView listView = FindViewById<ExpandableListView> (Resource.Id.MenuListView);
			var adapter = new ExpendableListViewAdapter (this, tableItems);
			adapter.NotifyDataSetChanged ();
			listView.SetAdapter (adapter);
			listView.SetOnChildClickListener (this);

		}

		private void GetMenuData(){
			if (tableItems.Count > 0) {
				tableItems = new List<Menu> ();
				pda.GetPendingPostings_Async ().ContinueWith (t =>{
					pending_P= t.Result.Count;
				});

				pda.GetCompletedPostings_Async ().ContinueWith (t =>{
					complete_P = t.Result.Count;
				});

				pda.GetCancelledPostings_Async ().ContinueWith (t =>{
					cancelled_P = t.Result.Count;
				});

				tableItems.Add(new Menu() { Type = "Postings", 
					MenuItems = new MenuItem[] 
					{ 
						new MenuItem { Name="Pending", Count = pending_P },
						new MenuItem { Name="Complete", Count = complete_P },
						new MenuItem { Name="Cancelled", Count = cancelled_P }
					}});
				tableItems.Add(new Menu() { Type = "Sales", 
					MenuItems = new MenuItem[] 
					{ 
						new MenuItem { Name="Pending", Count = pending_S },
						new MenuItem { Name="Complete", Count = complete_S },
						new MenuItem { Name="Hold", Count = hold_S },
						new MenuItem { Name="Postponed", Count = ppt_S },
						new MenuItem { Name="Cancelled", Count = cancelled_S }
					}});
				tableItems.Add(new Menu() { Type = "Account", 
					MenuItems = new MenuItem[] 
					{ 
						new MenuItem { Name="User Info" },
						new MenuItem { Name="Signature"},
						new MenuItem { Name="Log Out"}
					}});

			}
			progressbar.Visibility = ViewStates.Invisible;
			btnRefresh.Visibility = ViewStates.Visible;
			if (ApplicationShared.Current.GetSyncStatus()) {
				ApplicationShared.Current.SetSyncStatus(false);
			}
		}
			
		public override bool OnChildClick (ExpandableListView parent, View v, int groupPosition, int childPosition, long id)
		{
           
			MenuItem mItem = tableItems [groupPosition].MenuItems [childPosition];
            if (mItem != null)
            {
                MenuItemSelected(mItem, tableItems[groupPosition].Type);
                /*
                var intent = new Intent(this, typeof(AssignmentListViewActivity));
                string intentValue = Newtonsoft.Json.JsonConvert.SerializeObject(mItem);
                intent.PutExtra(tableItems[groupPosition].Type, intentValue);
                StartActivity(intent);
                 * */
            }
			return true;
		}
			
		void RetreiveAssignments(){
			if (isBound) {
				RunOnUiThread (() => {
					var assignment = binder.GetAssignmentService ().GetPostings ();
					if(assignment != null){
						pda.GetPendingPostings_Async ().ContinueWith (t =>{
							pending_P= t.Result.Count;
						});

						pda.GetCompletedPostings_Async ().ContinueWith (t =>{
							complete_P = t.Result.Count;
						});

						pda.GetCancelledPostings_Async ().ContinueWith (t =>{
							cancelled_P = t.Result.Count;
						});

					}else{
						Log.Debug ("AssignmentService", "assignment is null");
					}
				});
			}
		}

	    class AssignmentReceiver : BroadcastReceiver
	    {
		    public override void OnReceive (Context context, Intent intent)
			{
				((ExpandableListViewActivity)context).RetreiveAssignments();

				InvokeAbortBroadcast ();
			}
		}

	  class AssignmentServiceConnection : Java.Lang.Object, IServiceConnection
	  {
			ExpandableListViewActivity activity;

			public AssignmentServiceConnection (ExpandableListViewActivity activity)
			{
				this.activity = activity;
			}

			public void OnServiceConnected (ComponentName name, IBinder service)
			{
				var _ServiceBinder = service as AssignmentServiceBinder;
				if (_ServiceBinder != null) {
					var binder = (AssignmentServiceBinder)service;
					activity.binder = binder;
					activity.isBound = true;
				}
			}

			public void OnServiceDisconnected (ComponentName name)
			{
				activity.isBound = false;
			}

          
		}
	}
}


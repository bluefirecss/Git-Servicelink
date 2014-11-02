
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views.Animations;

using ServicelinkASAPMobile;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.Service;

namespace ServicelinkASAP.Android
{
	public class AssignmentListViewFragment:ListFragment
	{
        Task currentTask;
        Http serviceLinkWS = new Http();
        List<Assignment> assignments = new List<Assignment>();
        List<Posting> postings = new List<Posting>();
        List<Sale> sales = new List<Sale>();
        PostingDataAccess pda = new PostingDataAccess();
		ImageButton btnRefresh;
		ProgressBar progressbar;
		//LocalActivityManager localManager;
        public event Action<Assignment> AssignmentSelected = delegate { };
        public event Action RefreshList = delegate { };
        string queue = "";
        MenuItem mItem = new MenuItem();
        //ApplicationShared app;
        public AssignmentListViewFragment()
        {
        }
        public AssignmentListViewFragment(MenuItem item, string queueName)
        {
            queue = queueName;
            mItem = item;
        }

        public override void OnStart()
        {
            base.OnStart();
            _cancellationTokenSource = new CancellationTokenSource();
        }
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
            RetainInstance = true;
            SetHasOptionsMenu(true);
           
            // declare localManger to attach activity to fragment
			//localManager = new LocalActivityManager (Activity, true);
            //localManager.DispatchCreate (savedInstanceState);
           
		}

        /// <summary>
        /// Way to attach activity to fragment in OnCreateView
        /// </summary>
        /// <param name="method">  
        /// var intent = new Intent(Activity, typeof(AssignmentListViewActivity));
          ///    pass the index of the assignment through to the actual assignment activity
          ///   var window = localManager.StartActivity("AssignmentListViewActivity", intent);
          ///    View currentView = window.DecorView;
          ///   currentView.Visibility = ViewStates.Visible;
          ///  currentView.FocusableInTouchMode = true;
          ///   ((ViewGroup)currentView).DescendantFocusability = DescendantFocusability.AfterDescendants;
         ///   return currentView;
        /// </param>
        /// <returns></returns>

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
            return inflater.Inflate(Resource.Layout.AssignmentListLayout, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
			base.OnViewCreated(view, savedInstanceState);
			btnRefresh = view.FindViewById<ImageButton> (Resource.Id.btnRefresh_a);
			btnRefresh.Visibility = ViewStates.Visible;
			progressbar = view.FindViewById<ProgressBar> (Resource.Id.progress_a);
			progressbar.Visibility = ViewStates.Invisible;

            btnRefresh.Click += (object sender, EventArgs e) =>
            {
				ApplicationShared.Current.SetSyncStatus(true);
                SyncData(view);
            };

            ListView.SetDrawSelectorOnTop(true);
            ListView.Selector = new ColorDrawable(Color.White);
            if (ListAdapter == null)
            { 
                ListAdapter = new AssignmentListAdapter(view.Context);
                DataBind(view);
                BindListViewAdapter();
            }
        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            base.OnListItemClick(l, v, position, id);
            var adapter = (AssignmentListAdapter)ListView.Adapter;
			App.OrderID = adapter.Assignment [position].posting.OrderID;
            AssignmentSelected(adapter.Assignment[position]);
        }

        CancellationTokenSource _cancellationTokenSource;
        public void SyncData(View view)
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                Task task = Task.Factory.StartNew(() =>
                {
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
                    }

						DataBind(view);
						((AssignmentListAdapter)ListAdapter).NotifyDataSetChanged ();
                });
                currentTask = task;
            }
        }

         void BindListViewAdapter()
        {
            var adapter = (AssignmentListAdapter)ListAdapter;
            adapter.Assignment = assignments;
            adapter.NotifyDataSetChanged();
        }

        async void DataBind(View view)
        {
            string headerText = queue + " - ";
            Assignment assign = new Assignment();
            assignments = new List<Assignment>();
			if (queue == "Postings" || queue == string.Empty)
            {
               
                switch (mItem.Name.ToLower())
                {
                    case "pending":
                       
                        await Task.Factory.StartNew(() =>
                        {
                            pda.GetPendingPostings_Async().ContinueWith(t =>
                            {
                                postings = t.Result;
                            });
                        });

                        break;
                    case "cancelled":
                      
                        await Task.Factory.StartNew(() =>
                      {
                          pda.GetCancelledPostings_Async().ContinueWith(t =>
                          {
                              postings = t.Result;
                          });
                      });
                        break;
                    case "complete":
                       
                        await Task.Factory.StartNew(() =>
                    {
                        pda.GetCompletedPostings_Async().ContinueWith(t =>
                        {
                            postings = t.Result;
                        });
                    });
                        break;
                    default:
                       
                        await Task.Factory.StartNew(() =>
                   {
                       pda.GetPendingPostings_Async().ContinueWith(t =>
                       {
                           postings = t.Result;
                       });
                   });
                      
                        break;
                }
                foreach (Posting p in postings)
                {
                    assign.Type = AssignmentType.Postings;
                    assign.posting = p;
                    assignments.Add(assign);
                }
                headerText += mItem.Name + " (" + postings.Count.ToString() + ")";
            }
            if (queue == "Sales")
            {
                switch (mItem.Name.ToLower())
                {
                    case "pending":
                        break;
                    case "cancelled":
                        break;
                    case "complete":
                        break;
                    case "postponed":
                        break;
                    case "hold":
                        break;
                    default: //pending
                        break;
                }
                foreach (Sale s in sales)
                {
                   
                    assign.Type = AssignmentType.Sales;
                    assign.sale = s;
                    assignments.Add(assign);
                }
                headerText += mItem.Name + " (" + sales.Count.ToString() + ")";
            }
           
            view.FindViewById<TextView>(Resource.Id.txtHeader).Text = headerText;

           
        }

		public void StartUploadData(){
			ApplicationShared.Current.SyncServiceConnected += (object IntentSender, ServiceConnectedEventArgs args) => {
				((AssignmentListAdapter)ListAdapter).NotifyDataSetChanged();
			};
		}

		public override void OnResume ()
		{
			base.OnResume ();
            currentTask = null;
             _cancellationTokenSource = new CancellationTokenSource();
			if (ApplicationShared.Current.ShouldSyncApplicationDate())
            {
                if (this.View != null)
                {
                    SyncData(this.View);
                }
            }
			//localManager.DispatchResume ();
		}

		/// <summary>
		/// Pause the LocalActivityManager
		/// </summary>
		public override void OnPause ()
		{
			base.OnPause ();
			//localManager.DispatchPause (Activity.IsFinishing);
		}

		/// <summary>
		/// Stop the LocalActivityManager
		/// </summary>
		public override void OnStop ()
		{
			base.OnStop ();
            if (currentTask != null)
            {
                if (currentTask.Status == TaskStatus.Running)
                {
                    _cancellationTokenSource.Cancel();
                    currentTask.Wait(_cancellationTokenSource.Token);
                }
            }
			//localManager.DispatchStop ();
		}


	}
}


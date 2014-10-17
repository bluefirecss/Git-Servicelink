
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

using ServicelinkASAPMobile;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.Service;

namespace ServicelinkASAP.Android
{
	[Activity (Label = "AssignmentListViewActivity")]			
	public class AssignmentListViewActivity : ListActivity
	{
		List<Assignment> assignments = new List<Assignment> ();
		List<Posting> postings = new List<Posting>();
		List<Sale> sales = new List<Sale>();
		PostingDataAccess pda = new PostingDataAccess();
        MenuItem mItem = new MenuItem();
        string queue = "";

        public AssignmentListViewActivity()
        {
        }
        public AssignmentListViewActivity(MenuItem item, string queueName)
        {
            queue = queueName;
            mItem = item;
        }
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

            if (queue == "Postings")
            {
                //intentValue = Intent.GetStringExtra ("Postings");
                //mItem = Newtonsoft.Json.JsonConvert.DeserializeObject<MenuItem> (intentValue);
                switch (mItem.Name.ToLower())
                {
                    case "pending":
                        pda.GetPendingPostings_Async().ContinueWith(t =>
                        {
                            postings = t.Result;
                        });
                        break;
                    case "cancelled":
                        pda.GetCancelledPostings_Async().ContinueWith(t =>
                        {
                            postings = t.Result;
                        });
                        break;
                    case "complete":
                        pda.GetCompletedPostings_Async().ContinueWith(t =>
                        {
                            postings = t.Result;
                        });
                        break;
                    default:
                        pda.GetPendingPostings_Async().ContinueWith(t =>
                        {
                            postings = t.Result;
                        });
                        break;
                }
                foreach (Posting p in postings)
                {
                    Assignment assign = new Assignment();
                    assign.Type = AssignmentType.Postings;
                    assign.posting = p;
                    assignments.Add(assign);
                }
            }
            if (queue == "Sales")
            {
                //intentValue = Intent.GetStringExtra ("Sales");
                //mItem = Newtonsoft.Json.JsonConvert.DeserializeObject<MenuItem> (intentValue);
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
                    default:
                        break;
                }
                foreach (Sale s in sales)
                {
                    Assignment assign = new Assignment();
                    assign.Type = AssignmentType.Sales;
                    assign.sale = s;
                    assignments.Add(assign);
                }
            }
            if (queue == "Account")
            {
                //intentValue = Intent.GetStringExtra ("Account");
                //mItem = Newtonsoft.Json.JsonConvert.DeserializeObject<MenuItem> (intentValue);
                switch (mItem.Name.ToLower())
                {
                    case "user info":
                        break;
                    case "signature":
                        break;
                    default:
                        break;
                }
            }

            /*
			ListView lv = FindViewById<ListView>(Resource.Layout.CustomListItem);
			var adapter = new AssignmentListAdapter (this, assignments);
			lv.Adapter = adapter;
          */
		}




	}
}


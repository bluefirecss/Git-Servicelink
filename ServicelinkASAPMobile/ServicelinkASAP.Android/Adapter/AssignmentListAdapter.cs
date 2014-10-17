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

using ServicelinkASAPMobile;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.Service;

namespace ServicelinkASAP.Android
{
	public class AssignmentListAdapter:BaseAdapter<Assignment>
	{
        public IReadOnlyList<Assignment> Assignment;
		//List<Assignment> assignment = new List<Assignment> ();
		//List<Posting> postings = new List<Posting> ();
		//List<Sale> sales = new List<Sale> ();
		Context context;
        public AssignmentListAdapter(Context context)
            : base()
		{
            this.context = context;
			//this.assignment = items;
		}


		public override long GetItemId(int position)
		{
			return position;
		}
		public override Assignment this[int position]
		{
            get { return Assignment[position]; }
		}
		public override int Count
		{
            get { return Assignment.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			string title = "";
			string detail = "";
            if (Assignment[position].Type == AssignmentType.Postings)
            {
                title = Assignment[position].posting.OrderID.ToString() + " - " + Assignment[position].posting.TSNum;
                detail = Assignment[position].posting.Status.ToString() + " - " + Assignment[position].posting.SaleDateTime.ToString() + " - " + Assignment[position].posting.APN;
			}
            else if (Assignment[position].Type == AssignmentType.Sales)
            {
                title = Assignment[position].sale.OrderID.ToString() + " - " + Assignment[position].sale.TSNum;
                detail = Assignment[position].sale.Status.ToString() + " - " + Assignment[position].sale.SaleDateTime.ToString() + " - " + Assignment[position].sale.APN;
			}
			View view = convertView;
            if (view == null) // no view to re-use, create new
                view = LayoutInflater.FromContext(context).Inflate(Resource.Layout.CustomListItem, null);
			view.FindViewById<TextView>(Resource.Id.txtTitle).Text = title;
			view.FindViewById<TextView>(Resource.Id.txtDetail).Text = detail;
			//view.FindViewById<ImageView>(Resource.Id.Image).SetImageResource(item.ImageResourceId);
			return view;
		}
	}
}


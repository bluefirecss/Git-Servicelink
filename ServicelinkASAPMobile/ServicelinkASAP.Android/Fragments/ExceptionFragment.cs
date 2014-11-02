
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Graphics;
using Android.Net;
using Android.Provider;
using Android.Widget;
using Java.IO;
using Android.Runtime;
using Android.Util;
using Android.Views;

using ServicelinkASAPMobile;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.Utilities;
using ServicelinkASAPMobile.Service;

namespace ServicelinkASAP.Android
{
	public class ExceptionFragment : Fragment
	{
		public event Action submitException = delegate {};
		Posting posting = new Posting();
		EditText txtComment ;
		PostingDataAccess pda = new PostingDataAccess();

		public ExceptionFragment(Posting posting){
			this.posting = posting;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate(Resource.Layout.ExceptionLayout, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			Spinner reasonSpinner = view.FindViewById<Spinner> (Resource.Id.spinnerReason);
			reasonSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_ItemSelected);
			var adapter = ArrayAdapter.CreateFromResource (
				this.View.Context, Resource.Array.Reason_array, global::Android.Resource.Layout.SimpleSpinnerItem);

			adapter.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			reasonSpinner.Adapter = adapter;
			txtComment = this.View.FindViewById <EditText> (Resource.Id.etxtExceptionComment);


		}

		private void spinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;
			string selectedValue = spinner.GetItemAtPosition (e.Position).ToString();
			if (selectedValue != spinner.Prompt) {
				posting.IsException = "Yes";
			}
			txtComment.Text = selectedValue;

		}

		private void SaveException(){
			if (posting.IsException != "Yes") {
				Toast.MakeText (this.View.Context, "Please select from Exception Catalog before save.", ToastLength.Long).Show ();
			} else {
				posting.Exception = txtComment.Text.Trim ();
				posting.SaleDateTime = DateTime.Now.ToString ();
				pda.SavePosting_Async (posting).ContinueWith (t => {
					if (t.IsCompleted) {
						submitException (); // call loadNextInQueue
					} else {
						Toast.MakeText (this.View.Context, "Oops, this action failed.", ToastLength.Long).Show ();
					}
				});
				//submitException (posting);
			}
		}
	}
}


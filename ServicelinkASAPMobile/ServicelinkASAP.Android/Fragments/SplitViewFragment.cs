
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Content.Res;

namespace ServicelinkASAP.Android
{
	public class SplitViewFragment : Fragment
	{
		public Action LoadListViewFragment = delegate { };
		//ApplicationShared app;
		//AssignmentListViewFragment currentList;
		public SplitViewFragment(){

		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			LoadListViewFragment();
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate(Resource.Layout.SplitViewLayout, container, false);
		}

		public override void OnConfigurationChanged (global::Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
		}
	}
}


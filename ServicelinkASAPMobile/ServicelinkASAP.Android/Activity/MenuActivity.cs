using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views.Animations;

using ServicelinkASAPMobile;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.Service;

namespace ServicelinkASAP.Android
{
	[Activity (Label = "MenuActivity")]	
	public class MenuActivity:Activity
	{
		public MenuActivity ()
		{
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
		}


		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return CreateMenuView (inflater, container, savedInstanceState);
		}

		View CreateMenuView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.MenuLayout, null);

			return view;
		}
	}
}


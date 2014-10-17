using System;
using System.Linq;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views.Animations;
using Android.Text;
using ServicelinkASAPMobile.Data;

namespace ServicelinkASAP.Android
{
	public class OrderListFragment:ListFragment
	{

		public event Action<Posting,int> ProductSelected = delegate {};
		//BadgeDrawable basketBadge;
		int badgeCount;
		public OrderListFragment ()
		{
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			RetainInstance = true;
			SetHasOptionsMenu (true);
		} 
	}
}


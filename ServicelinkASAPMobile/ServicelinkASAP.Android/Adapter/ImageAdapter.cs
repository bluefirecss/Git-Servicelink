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

using ServicelinkASAPMobile.Data;

namespace ServicelinkASAP.Android
{
	public class ImageAdapter:BaseAdapter
	{
		Context context;
		List<Photo> photos;

		public ImageAdapter (Context c, List<Photo> photos)
		{
			context = c;
			this.photos = photos;
		}

		public override int Count { get { return photos.Count; } }

		public override Java.Lang.Object GetItem (int position)
		{
			return null;
		}

		public override long GetItemId (int position)
		{
			return 0;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			ImageView i = new ImageView (context);

			i.SetImageResource (photos[position].Id);
			i.LayoutParameters = new Gallery.LayoutParams (150, 100);
			i.SetScaleType (ImageView.ScaleType.FitXy);

			return i;
		}
	}
}


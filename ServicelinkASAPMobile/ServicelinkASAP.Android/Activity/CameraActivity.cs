
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Net;
using Android.Provider;
using Android.Widget;
using Java.IO;

using ServicelinkASAPMobile;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.Service;

/// <summary>
/// not using this activity
/// </summary>
namespace ServicelinkASAP.Android
{

	[Activity (Label = "CameraActivity")]			
	public class CameraActivity : Activity
	{
		//private ImageView _imageView;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Button button = FindViewById<Button>(Resource.Id.camera);
                /*
                _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
                if (App.bitmap != null) {
                    _imageView.SetImageBitmap (App.bitmap);
                    App.bitmap = null;
                }*/
                button.Click += TakeAPicture;

            }

		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			// make it available in the gallery
			Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
			global::Android.Net.Uri contentUri = global::Android.Net.Uri.FromFile (App._file);
			mediaScanIntent.SetData(contentUri);
			SendBroadcast(mediaScanIntent);

			// display in ImageView. We will resize the bitmap to fit the display
			// Loading the full sized image will consume to much memory 
			// and cause the application to crash.
			//int height = Resources.DisplayMetrics.HeightPixels;
			//int width = _imageView.Height ;
			//App.bitmap = App._file.Path.LoadAndResizeBitmap (width, height);
		}

		private void CreateDirectoryForPictures()
		{
			App._dir = new File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "ServicelinkASAP.Android");
			if (!App._dir.Exists())
			{
				App._dir.Mkdirs();
			}
		}

		private bool IsThereAnAppToTakePictures()
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			IList<ResolveInfo> availableActivities = PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
			return availableActivities != null && availableActivities.Count > 0;
		}

		private void TakeAPicture(object sender, EventArgs eventArgs)
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);

			App._file = new File(App._dir, String.Format("{0}_{1}.jpg", App.OrderID , Guid.NewGuid()));

			intent.PutExtra(MediaStore.ExtraOutput, global::Android.Net.Uri.FromFile(App._file));

			StartActivityForResult(intent, 0);
		}

	}
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ServicelinkASAP.Android
{
	[BroadcastReceiver]
	[IntentFilter(new string[]{AssignmentService.AssignmentUpdatedAction}, Priority = (int)IntentFilterPriority.LowPriority)]
	public class AssignmentBroadcastReceiver : BroadcastReceiver
	{
		public AssignmentBroadcastReceiver(){
		}

		public override void OnReceive (Context context, Intent intent)
		{
			var nMgr = (NotificationManager)context.GetSystemService (Context.NotificationService);
			var notification = new Notification (Resource.Drawable.Icon, "New assignment data is available");
			string title="Assignment Updated";
			string details="New assignment data is available";
			var pendingIntent = PendingIntent.GetActivity (context, 0, new Intent (context, typeof(MainActivity)), 0);
			notification.SetLatestEventInfo (context, title, details, pendingIntent);
			nMgr.Notify (0, notification);
		}
	}
}


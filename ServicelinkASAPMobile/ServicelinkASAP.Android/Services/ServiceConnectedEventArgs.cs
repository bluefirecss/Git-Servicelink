using System;
using Android.OS;

namespace ServicelinkASAPMobile
{
	public class ServiceConnectedEventArgs : EventArgs
	{
		public IBinder Binder { get; set; }

	}
}


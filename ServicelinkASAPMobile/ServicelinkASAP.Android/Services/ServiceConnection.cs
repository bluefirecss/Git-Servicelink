using System;

using Android.App;
using Android.Util;
using Android.OS;
using Android.Content;


namespace ServicelinkASAPMobile
{
	public class ServiceConnection: Java.Lang.Object, IServiceConnection
	{
		public event EventHandler <ServiceConnectedEventArgs> ServiceConnected = delegate{};

		protected ServiceBinder binder;
		public ServiceBinder Binder{
			get { return this.binder;}
			set { this.binder = value;}
		}

		public ServiceConnection (ServiceBinder binder)
		{
			if (binder != null) {
				this.binder = binder;
			}
		}

		public void OnServiceConnected (ComponentName name, IBinder service)
		{
			ServiceBinder serviceBinder = service as ServiceBinder;

			if (serviceBinder != null) {
				this.binder = serviceBinder;
				this.binder.IsBound = true;

				this.ServiceConnected (this, new ServiceConnectedEventArgs() {Binder = service});

				serviceBinder.Service.StartSyncing ();
			}
		}

		public void OnServiceDisconnected (ComponentName name) { this.binder.IsBound = false; }
	}
}


using System;
using Android.OS;

namespace ServicelinkASAPMobile
{
	public class ServiceBinder:Binder
	{
		public SyncService Service {
			get { return this.Service; }
		}

		protected SyncService service;

		public bool IsBound { get; set;}

		public ServiceBinder (SyncService service){
			this.service = service;
		}
	}
}


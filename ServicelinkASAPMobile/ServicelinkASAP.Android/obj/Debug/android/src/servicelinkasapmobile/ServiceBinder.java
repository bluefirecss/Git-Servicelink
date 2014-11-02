package servicelinkasapmobile;


public class ServiceBinder
	extends android.os.Binder
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("ServicelinkASAPMobile.ServiceBinder, ServicelinkASAP.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ServiceBinder.class, __md_methods);
	}


	public ServiceBinder () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ServiceBinder.class)
			mono.android.TypeManager.Activate ("ServicelinkASAPMobile.ServiceBinder, ServicelinkASAP.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public ServiceBinder (servicelinkasapmobile.SyncService p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == ServiceBinder.class)
			mono.android.TypeManager.Activate ("ServicelinkASAPMobile.ServiceBinder, ServicelinkASAP.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "ServicelinkASAPMobile.SyncService, ServicelinkASAP.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}

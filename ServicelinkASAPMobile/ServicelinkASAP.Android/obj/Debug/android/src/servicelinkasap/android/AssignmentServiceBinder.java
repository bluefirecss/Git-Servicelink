package servicelinkasap.android;


public class AssignmentServiceBinder
	extends android.os.Binder
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("ServicelinkASAP.Android.AssignmentServiceBinder, ServicelinkASAP.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", AssignmentServiceBinder.class, __md_methods);
	}


	public AssignmentServiceBinder () throws java.lang.Throwable
	{
		super ();
		if (getClass () == AssignmentServiceBinder.class)
			mono.android.TypeManager.Activate ("ServicelinkASAP.Android.AssignmentServiceBinder, ServicelinkASAP.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public AssignmentServiceBinder (servicelinkasap.android.AssignmentService p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == AssignmentServiceBinder.class)
			mono.android.TypeManager.Activate ("ServicelinkASAP.Android.AssignmentServiceBinder, ServicelinkASAP.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "ServicelinkASAP.Android.AssignmentService, ServicelinkASAP.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
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

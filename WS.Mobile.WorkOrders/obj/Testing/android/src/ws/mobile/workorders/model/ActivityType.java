package ws.mobile.workorders.model;


public class ActivityType
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_toString:()Ljava/lang/String;:GetToStringHandler\n" +
			"";
		mono.android.Runtime.register ("WS.Mobile.WorkOrders.Model.ActivityType, WS.Mobile.WorkOrders, Version=1.2.2.1374, Culture=neutral, PublicKeyToken=null", ActivityType.class, __md_methods);
	}


	public ActivityType () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ActivityType.class)
			mono.android.TypeManager.Activate ("WS.Mobile.WorkOrders.Model.ActivityType, WS.Mobile.WorkOrders, Version=1.2.2.1374, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public java.lang.String toString ()
	{
		return n_toString ();
	}

	private native java.lang.String n_toString ();

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

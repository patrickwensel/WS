package ws.mobile.workorders.model;


public class PartType
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_toString:()Ljava/lang/String;:GetToStringHandler\n" +
			"";
		mono.android.Runtime.register ("WS.Mobile.WorkOrders.Model.PartType, WS.Mobile.WorkOrders, Version=1.2.2.1380, Culture=neutral, PublicKeyToken=null", PartType.class, __md_methods);
	}


	public PartType () throws java.lang.Throwable
	{
		super ();
		if (getClass () == PartType.class)
			mono.android.TypeManager.Activate ("WS.Mobile.WorkOrders.Model.PartType, WS.Mobile.WorkOrders, Version=1.2.2.1380, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
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

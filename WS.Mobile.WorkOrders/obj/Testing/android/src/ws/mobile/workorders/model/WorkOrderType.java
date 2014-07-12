package ws.mobile.workorders.model;


public class WorkOrderType
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_toString:()Ljava/lang/String;:GetToStringHandler\n" +
			"";
		mono.android.Runtime.register ("WS.Mobile.WorkOrders.Model.WorkOrderType, WS.Mobile.WorkOrders, Version=1.2.2.1374, Culture=neutral, PublicKeyToken=null", WorkOrderType.class, __md_methods);
	}


	public WorkOrderType () throws java.lang.Throwable
	{
		super ();
		if (getClass () == WorkOrderType.class)
			mono.android.TypeManager.Activate ("WS.Mobile.WorkOrders.Model.WorkOrderType, WS.Mobile.WorkOrders, Version=1.2.2.1374, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public WorkOrderType (java.lang.String p0, java.lang.String p1) throws java.lang.Throwable
	{
		super ();
		if (getClass () == WorkOrderType.class)
			mono.android.TypeManager.Activate ("WS.Mobile.WorkOrders.Model.WorkOrderType, WS.Mobile.WorkOrders, Version=1.2.2.1374, Culture=neutral, PublicKeyToken=null", "System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e:System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1 });
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

package ws.mobile.workorders.model;


public class Branch
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_toString:()Ljava/lang/String;:GetToStringHandler\n" +
			"";
		mono.android.Runtime.register ("WS.Mobile.WorkOrders.Model.Branch, WS.Mobile.WorkOrders, Version=1.2.2.1380, Culture=neutral, PublicKeyToken=null", Branch.class, __md_methods);
	}


	public Branch () throws java.lang.Throwable
	{
		super ();
		if (getClass () == Branch.class)
			mono.android.TypeManager.Activate ("WS.Mobile.WorkOrders.Model.Branch, WS.Mobile.WorkOrders, Version=1.2.2.1380, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public Branch (java.lang.String p0, java.lang.String p1, java.lang.String p2) throws java.lang.Throwable
	{
		super ();
		if (getClass () == Branch.class)
			mono.android.TypeManager.Activate ("WS.Mobile.WorkOrders.Model.Branch, WS.Mobile.WorkOrders, Version=1.2.2.1380, Culture=neutral, PublicKeyToken=null", "System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e:System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e:System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2 });
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

package ws.mobile.workorders.activities;


public class WorkOrderDetailTabActivity
	extends ws.mobile.workorders.activities.AbstractTabItemActivity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("WS.Mobile.WorkOrders.Activities.WorkOrderDetailTabActivity, WS.Mobile.WorkOrders, Version=1.2.2.1380, Culture=neutral, PublicKeyToken=null", WorkOrderDetailTabActivity.class, __md_methods);
	}


	public WorkOrderDetailTabActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == WorkOrderDetailTabActivity.class)
			mono.android.TypeManager.Activate ("WS.Mobile.WorkOrders.Activities.WorkOrderDetailTabActivity, WS.Mobile.WorkOrders, Version=1.2.2.1380, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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

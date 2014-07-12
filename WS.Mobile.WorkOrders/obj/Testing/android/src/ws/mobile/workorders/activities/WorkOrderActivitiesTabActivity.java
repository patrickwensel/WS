package ws.mobile.workorders.activities;


public class WorkOrderActivitiesTabActivity
	extends ws.mobile.workorders.activities.AbstractTabItemActivity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onSaveInstanceState:(Landroid/os/Bundle;)V:GetOnSaveInstanceState_Landroid_os_Bundle_Handler\n" +
			"n_onRestoreInstanceState:(Landroid/os/Bundle;)V:GetOnRestoreInstanceState_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("WS.Mobile.WorkOrders.Activities.WorkOrderActivitiesTabActivity, WS.Mobile.WorkOrders, Version=1.2.2.1374, Culture=neutral, PublicKeyToken=null", WorkOrderActivitiesTabActivity.class, __md_methods);
	}


	public WorkOrderActivitiesTabActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == WorkOrderActivitiesTabActivity.class)
			mono.android.TypeManager.Activate ("WS.Mobile.WorkOrders.Activities.WorkOrderActivitiesTabActivity, WS.Mobile.WorkOrders, Version=1.2.2.1374, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onSaveInstanceState (android.os.Bundle p0)
	{
		n_onSaveInstanceState (p0);
	}

	private native void n_onSaveInstanceState (android.os.Bundle p0);


	public void onRestoreInstanceState (android.os.Bundle p0)
	{
		n_onRestoreInstanceState (p0);
	}

	private native void n_onRestoreInstanceState (android.os.Bundle p0);

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

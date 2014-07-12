package ws.mobile.workorders.activities;


public abstract class AbstractTabItemActivity
	extends ws.mobile.workorders.activities.AbstractActivity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onResume:()V:GetOnResumeHandler\n" +
			"";
		mono.android.Runtime.register ("WS.Mobile.WorkOrders.Activities.AbstractTabItemActivity, WS.Mobile.WorkOrders, Version=1.2.2.1380, Culture=neutral, PublicKeyToken=null", AbstractTabItemActivity.class, __md_methods);
	}


	public AbstractTabItemActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == AbstractTabItemActivity.class)
			mono.android.TypeManager.Activate ("WS.Mobile.WorkOrders.Activities.AbstractTabItemActivity, WS.Mobile.WorkOrders, Version=1.2.2.1380, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onResume ()
	{
		n_onResume ();
	}

	private native void n_onResume ();

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

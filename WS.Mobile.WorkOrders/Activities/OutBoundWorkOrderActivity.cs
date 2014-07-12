using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Content.PM;

namespace  WS.Mobile.WorkOrders.Activities
{
	[Activity]
	public class OutBoundWorkOrderActivity : AbstractTabActivity
	{
		private string _unitNumber;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			_unitNumber = Intent.GetStringExtra ("unitNumber");
			LoadActiveWorkOrder (_unitNumber);

			SetContentView (Resource.Layout.workOrder);

			TabHost.Setup ();

			var attributesTab = TabHost.NewTabSpec ("attributes")
				.SetIndicator (GetString (Resource.String.workOrder_tab_attributes))
				.SetContent (CreateIntent (typeof(OutBoundWorkOrderUnitAttributesTabActivity)));
			TabHost.AddTab (attributesTab);

			var imagesTab = TabHost.NewTabSpec ("images")
				.SetIndicator (GetString (Resource.String.workOrder_tab_images))
				.SetContent (CreateIntent (typeof(OutBoundWorkOrderImagesTabActivity)));
			TabHost.AddTab (imagesTab);
		}

		private Intent CreateIntent (Type tabActivity)
		{
			var intent = new Intent ();
			intent.SetClass (this, tabActivity);

			return intent;
		}
	}
}

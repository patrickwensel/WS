using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Content.PM;

namespace WS.Mobile.WorkOrders.Activities
{
	[Activity]
	public class WorkOrderActivity : AbstractTabActivity
	{
		private string _unitNumber;
		private string _type;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			_unitNumber = Intent.GetStringExtra ("unitNumber");
			_type =Intent.GetStringExtra ("type");
			LoadActiveWorkOrder (_unitNumber);

			SetContentView (Resource.Layout.workOrder);

			TabHost.Setup ();

				var detailsTab = TabHost.NewTabSpec ("details")
		    	.SetIndicator (GetString (Resource.String.workOrder_tab_details))
		    	.SetContent (CreateIntent (typeof(WorkOrderDetailTabActivity)));			    

				var attributesTab = TabHost.NewTabSpec ("attributes")
		    	.SetIndicator (GetString (Resource.String.workOrder_tab_attributes))
		    	.SetContent (CreateIntent (typeof(WorkOrderUnitAttributesTabActivity)));			    

				var activitiesTab = TabHost.NewTabSpec ("activities")
		    	.SetIndicator (GetString (Resource.String.workOrder_tab_activities))
		    	.SetContent (CreateIntent (typeof(WorkOrderActivitiesTabActivity)));			    

				var partsTab = TabHost.NewTabSpec ("parts")
		    	 .SetIndicator (GetString (Resource.String.workOrder_tab_parts))
		    	 .SetContent (CreateIntent (typeof(WorkOrderPartsTabActivity)));			    

				var imagesTab = TabHost.NewTabSpec ("images")
		    	.SetIndicator (GetString (Resource.String.workOrder_tab_images))
		    	.SetContent (CreateIntent (typeof(WorkOrderImagesTabActivity)));
			    
				TabHost.AddTab (detailsTab);
				TabHost.AddTab (attributesTab);
				TabHost.AddTab (activitiesTab);
				TabHost.AddTab (partsTab);
				TabHost.AddTab (imagesTab);
				if (_type == Global.FieldOnRentType) {

					TabHost.TabWidget.GetChildAt (0).Visibility = Android.Views.ViewStates.Gone;
					TabHost.TabWidget.GetChildAt (2).Visibility = Android.Views.ViewStates.Gone;
					TabHost.TabWidget.GetChildAt (3).Visibility = Android.Views.ViewStates.Gone;
				} 	
		}
		
		private Intent CreateIntent (Type tabActivity)
		{
			var intent = new Intent ();
			intent.SetClass (this, tabActivity);
			intent.PutExtra ("type", _type);
			return intent;
		}

		public void SwitchTab(string tab){
			TabHost.SetCurrentTabByTag(tab);
		}
	}
}
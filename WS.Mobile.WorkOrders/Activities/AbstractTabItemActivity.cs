using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Widget;
using WS.Mobile.WorkOrders.Backend;
using Android.Views;

namespace WS.Mobile.WorkOrders.Activities
{
	public abstract class AbstractTabItemActivity : AbstractActivity
	{
		public bool LandscapeOrientation;
		public bool SupressLoadChanges;

		protected override void OnCreate (Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			var newSurfaceOrientation = WindowManager.DefaultDisplay.Rotation;
			LandscapeOrientation = newSurfaceOrientation == Android.Views.SurfaceOrientation.Rotation0 || newSurfaceOrientation == Android.Views.SurfaceOrientation.Rotation180;
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			if (!SupressLoadChanges) {
				SupressLoadChanges = false;
				LoadChanges ();
			}
		}

		protected abstract void LoadChanges ();

		protected void UpdateActiveWorkOrder (JsonObject obj)
		{
			CacheRepository.Save (Global.ActiveWorkOrderObject, obj);
		}

		protected void RemoveObjectFromCollection (string collection, string id)
		{
			var unit = GetActiveWorkOrder ();

			var list = (List<object>)unit [collection];
			list = list.Where (x => (string)((JsonObject)x) ["ID"] != id).ToList ();
			unit [collection] = list;
			
			UpdateActiveWorkOrder (unit);
		}

		protected void AddObjectToCollection (string collection, JsonObject obj)
		{
			var unit = GetActiveWorkOrder ();
			var list = (List<object>)unit [collection];

			if (list == null)
				list = new List<object> ();

			list.Add (obj);

			unit [collection] = list;
			
			UpdateActiveWorkOrder (unit);
		}

		protected object GetProperty (string name)
		{
			var unit = GetActiveWorkOrder ();
			return unit [name];
		}

		protected void UpdateProperty (string name, object value)
		{
			var unit = GetActiveWorkOrder ();
			unit [name] = value;

			UpdateActiveWorkOrder (unit);
		}

		protected object GetCollectionProperty (string collection, string id, string name)
		{
			var unit = GetActiveWorkOrder ();
			var list = (List<object>)unit [collection];
			
			for (int i = 0; i < list.Count; i++) {
				var a = (JsonObject)list [i];
				
				if ((string)a ["ID"] == id) {
					return a [name];
				}
			}
			
			return null;
		}

		protected void UpdateCollectionProperty (string collection, string id, string name, object value)
		{
			var unit = GetActiveWorkOrder ();
			var list = (List<object>)unit [collection];
			
			for (int i = 0; i < list.Count; i++) {
				var a = (JsonObject)list [i];
				
				if ((string)a ["ID"] == id) {
					a [name] = value;
					list [i] = a;
				}
			}
			
			unit [collection] = list;
			UpdateActiveWorkOrder (unit);
		}

		protected void ForceFocus (View view, string tab)
		{
			Task.Factory.StartNew (o => {
				RunOnUiThread (delegate {
					var control = (View)o;

					var activity = ((AbstractTabItemActivity)control.Context);
					var tabHost = ((WorkOrderActivity)activity.Parent).TabHost;
					var currentTab = tabHost.CurrentTabTag;

					activity.SupressLoadChanges = true;
					if (currentTab != tab) {
						tabHost.SetCurrentTabByTag (tab);
					}

					control.RequestFocusFromTouch();
				});
			}, view);
		}

		protected long ParseQty (string text)
		{
			long percentage;
			
			if (!Int64.TryParse (text, out percentage))
				return -1;
			
			return percentage;
		}

		protected bool ValidateQty (long qty, bool supressAlert)
		{
			if (qty <= 0) {
				if (!supressAlert) {
					var builder = new AlertDialog.Builder (this);
					builder.SetTitle ("Quantity Required");
					builder.SetMessage ("Please enter a quantity greater than zero.");
					builder.SetPositiveButton ("OK", NullClick);
					builder.SetCancelable (false);
					builder.Show ();
				}
				
				return false;
			}
			
			return true;
		}
		
		protected long ParseDamagePercentage (string text)
		{
			long percentage;
			
			if (!Int64.TryParse (text, out percentage))
				return 0;
			
			return percentage;
		}
		
		protected bool ValidateDamanagePercentage (long percentage, bool supressAlert)
		{
			if (percentage < 0 || percentage > 100) {
				if (!supressAlert) {
					var builder = new AlertDialog.Builder (this);
					builder.SetTitle ("Damage Billing % Not Valid");
					builder.SetMessage ("The damage billing % of " + percentage + " needs to be between or equal to 0 and 100.");
					builder.SetPositiveButton ("OK", NullClick);
					builder.SetCancelable (false);
					builder.Show ();
				}
				
				return false;
			}
			
			return true;
		}
		
		protected bool ValidateActivityType (string workType)
		{
			if (String.IsNullOrWhiteSpace (workType)) {
				var builder = new AlertDialog.Builder (this);
				builder.SetTitle ("Activity Type Required");
				builder.SetMessage ("Please select an activity type.");
				builder.SetPositiveButton ("OK", NullClick);
				builder.SetCancelable (false);
				builder.Show ();
				
				return false;
			}
			
			return true;
		}


	}
}


using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using WS.Mobile.WorkOrders.Backend;
using WS.Mobile.WorkOrders.Data;
using Android.Preferences;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WS.Mobile.WorkOrders.Activities
{
	public abstract class AbstractTabActivity : TabActivity
	{
		private const int DialogCancel = 0x0100;

		protected CacheRepository CacheRepository {
			get { return ((WorkOrderApplication)Application).CacheRepository; }
		}

		protected WorkOrderRepository WorkOrderRepository {
			get { return ((WorkOrderApplication)Application).WorkOrderRepository; }
		}

		protected void ClearActiveWorkOrder ()
		{
			CacheRepository.Delete (Global.ActiveWorkOrderObject);
		}

		protected void LoadActiveWorkOrder (string unitNumber)
		{
			var obj = (JsonObject)null;
			var workOrderJson = CacheRepository.Get (Global.ActiveWorkOrderObject);

			if (workOrderJson != null) {
				obj = new JsonObject (workOrderJson);

				var activeUnitNumber = (string)obj [Global.FieldUnitNumber];

				// if the active work order is already set there is no need to do anything else
				if (String.Equals (unitNumber, activeUnitNumber, StringComparison.OrdinalIgnoreCase))
					return;
			}

			obj = WorkOrderRepository.Get (unitNumber);

			if (obj == null)
				throw new ApplicationException ("unable to get, because there is no work order for unit number " + unitNumber);

			CacheRepository.Save (Global.ActiveWorkOrderObject, obj);
		}

		public override void OnBackPressed ()
		{
			ShowDialog (DialogCancel);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			var inflater = MenuInflater;
			inflater.Inflate (Resource.Menu.workorder_activity, menu);

			return true;
		}

		protected void NullClick (object sender, DialogClickEventArgs e)
		{
		}

		protected override Dialog OnCreateDialog (int id)
		{
			switch (id) {
			case DialogCancel:
				{
					var builder = new AlertDialog.Builder (this);
					builder.SetTitle ("Are you sure?");
					builder.SetMessage ("This will return you to the main screen without saving your changes. All your changes will be lost. Continue?");
					builder.SetPositiveButton ("Yes", delegate {
						ClearActiveWorkOrder ();
						Finish ();
					});
					builder.SetNegativeButton ("No", NullClick);
					builder.SetCancelable (false);
					return builder.Create ();
				}
			}

			return null;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.menu_version:
				{
					var pref = PreferenceManager.GetDefaultSharedPreferences (WorkOrderApplication.Context);
					var server = pref.GetString (Global.SettingSelectedServer, Global.SettingSelectedServerDefault);
				
					var packageInfo = PackageManager.GetPackageInfo (PackageName, 0);
					var version = packageInfo.VersionName;
				
					var builder = new AlertDialog.Builder (this);
					builder.SetTitle ("Version");
					builder.SetMessage ("You are running version " + version + " of " + GetString (Resource.String.app_name) + " on server " + server + ".");
					builder.SetPositiveButton ("OK", NullClick);
					builder.SetCancelable (false);
					builder.Show();
					return true;
				}

			case Resource.Id.menu_save:
				{
					if (IsFinishing)
						return false;
					var obj = CacheRepository.Get (Global.ActiveWorkOrderObject);	
										
					if (obj == null)
						//return false;
						return true;

					if (ValidateUnitAttributeItem (new JsonObject (obj)))
						return false;
				
					var workOrder = new JsonObject (obj);
					var unitNumber = (string)workOrder [Global.FieldUnitNumber];
				
					WorkOrderRepository.Save (unitNumber, workOrder);

					ClearActiveWorkOrder ();
					Finish ();

					return true;
				}

			case Resource.Id.menu_cancel:
				{
					ShowDialog (DialogCancel);
					return true;
				}
			}

			return false;
		}

		protected override void OnStart ()
		{
			base.OnStart ();

			var actionBar = ActionBar;

			if (actionBar != null) {
				actionBar.SetDisplayShowCustomEnabled (true);

				var view = LayoutInflater.Inflate (Resource.Layout.menu_loggedInUser, null);
				var layout = new Android.App.ActionBar.LayoutParams (Android.App.ActionBar.LayoutParams.FillParent, Android.App.ActionBar.LayoutParams.FillParent);
				actionBar.SetCustomView (view, layout);

				var unitNumber = view.FindViewById<TextView> (Resource.Id.unitNumber);
				var employeeNumber = view.FindViewById<TextView> (Resource.Id.employeeNumber);
				var employeeBranch = view.FindViewById<TextView> (Resource.Id.employeeBranch);

				unitNumber.Text = Intent.GetStringExtra ("unitNumber");
				employeeNumber.Text = CacheRepository.LoggedInEmployeeNumber;
				employeeBranch.Text = CacheRepository.LoggedInBranchName;

				actionBar.SetDisplayUseLogoEnabled (true);
				actionBar.SetLogo (Resource.Drawable.logo);
			}
		}

		protected override void OnStop ()
		{
			CacheRepository.Close ();
			WorkOrderRepository.Close ();

			base.OnStop ();
		}

		private bool ValidateUnitAttributeItem(JsonObject obj)
		{
			var list = ((IEnumerable<object>)obj [Global.FieldUnitAttributes]).Cast<JsonObject> ().OrderBy (a => a ["ID"]);
			var count = list.Select (a => (string)a ["UserDefinedCodeTypeID"] + (string)a ["UserDefinedCodeID"]).Distinct().Count() ;
			if (list.Count () != count) {
				// duplicate exist .. do something ...
				var builder = new AlertDialog.Builder (this);
				builder.SetTitle ("Duplicate Items.");
				builder.SetMessage ("There are duplicate Unit Attribute Items.");
				builder.SetPositiveButton ("OK", NullClick);
				builder.SetCancelable (false);
				builder.Show ();
				return true;
			}
			for (int i = 0; i < list.Count(); i++) {
				var a = (JsonObject)list.ElementAt(i);

				if (String.IsNullOrWhiteSpace((string)a ["UserDefinedCodeTypeDescription"])) {
					var builder = new AlertDialog.Builder (this);
					builder.SetTitle ("Select Category.");
					builder.SetMessage ("Category must be selected");
					builder.SetPositiveButton ("OK", NullClick);
					builder.SetCancelable (false);
					builder.Show ();
					return true;
				}
				if (String.IsNullOrWhiteSpace((string)a ["UserDefinedCodeDescription"])) {
					var builder = new AlertDialog.Builder (this);
					builder.SetTitle ("Select Item Desc.");
					builder.SetMessage ("Item Desc must be selected");
					builder.SetPositiveButton ("OK", NullClick);
					builder.SetCancelable (false);
					builder.Show ();
					return true;
				}
				if (String.IsNullOrWhiteSpace((string)a ["UserDefinedCodeID"])) {
					var builder = new AlertDialog.Builder (this);
					builder.SetTitle ("Set Attribute value.");
					builder.SetMessage ("Item Desc must be selected");
					builder.SetPositiveButton ("OK", NullClick);
					builder.SetCancelable (false);
					builder.Show ();
					return true;
				}

			}

			return false;
		}

	}
}


using System;
using System.Linq;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using WS.Mobile.WorkOrders.Backend;
using WS.Mobile.WorkOrders.Data;
using Android.Net;
using System.Threading.Tasks;
using System.Net;
using Android.Views.InputMethods;

namespace WS.Mobile.WorkOrders.Activities
{
	public abstract class AbstractActivity : Activity
	{
		private const int DialogLogout = 0x0010;
		private const int DialogChangeServer = 0x0100;
		private const int DialogNotOnline = 0x1000;
		private const int DialogVpnNotOnline = 0x1001;

		protected new WorkOrderApplication Application {
			get { return ((WorkOrderApplication)base.Application); }
		}

		protected CacheRepository CacheRepository {
			get { return Application.CacheRepository; }
		}

		protected WorkOrderRepository WorkOrderRepository {
			get { return Application.WorkOrderRepository; }
		}

		protected bool ShowKeyboard (View control)
		{
			var inputMethod = (InputMethodManager)GetSystemService (Context.InputMethodService);
			return inputMethod.ShowSoftInput (control, ShowFlags.Implicit);
		}

		private int LookupHost (string host)
		{
			try {
				var addresses = Task<IPAddress[]>.Factory.FromAsync (Dns.BeginGetHostAddresses, Dns.EndGetHostAddresses, host, null).Result;

				if (addresses == null)
					return 0;

				return addresses.Select (x => BitConverter.ToInt32 (x.GetAddressBytes (), 0)).FirstOrDefault ();
			} catch {
				return 0;
			}
		}

		protected bool IsOnlineWithAlert {
			get {
				var connectivity = (ConnectivityManager)GetSystemService (Context.ConnectivityService);
				var net = connectivity.ActiveNetworkInfo;
				
				var online = net != null && net.IsConnected;
				
				if (!online) {
					RunOnUiThread(delegate{ShowDialog (DialogNotOnline);});
					return online;
				}
				
				var pref = PreferenceManager.GetDefaultSharedPreferences (this);
				var server = pref.GetString (Global.SettingSelectedServer, Global.SettingSelectedServerDefault);
				var hostAddress = LookupHost (server);

				online = hostAddress != 0;

				if (!online) {
					RunOnUiThread(delegate{ShowDialog (DialogVpnNotOnline);});
					return online;
				}

				return online;
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			var inflater = MenuInflater;

			if (this is LoginActivity)
				inflater.Inflate (Resource.Menu.login_activity, menu);
			else
				inflater.Inflate (Resource.Menu.main_activity, menu);

			return true;
		}

		protected void NullClick (object sender, DialogClickEventArgs e)
		{
		}

		protected override Dialog OnCreateDialog (int id)
		{
			switch (id) {
			case DialogNotOnline:
				{
					var builder = new AlertDialog.Builder (this);
					builder.SetTitle ("No Network Connection");
					builder.SetMessage ("You don't currently have a network connection, please return to a place with a known network connection to continue this operation.");
					builder.SetPositiveButton ("OK", NullClick);
					builder.SetCancelable (false);
					return builder.Create ();
				}

			case DialogVpnNotOnline:
				{
					var builder = new AlertDialog.Builder (this);
					builder.SetTitle ("No VPN Connection");
					builder.SetMessage ("You don't currently have a VPN connection to the William Scotsman network, please connect the VPN to the network and try again.");
					builder.SetPositiveButton ("OK", NullClick);
					builder.SetCancelable (false);
					return builder.Create ();
				}

			case DialogChangeServer:
				{
					var pref = PreferenceManager.GetDefaultSharedPreferences(WorkOrderApplication.Context);
					var server = pref.GetString(Global.SettingSelectedServer, Global.SettingSelectedServerDefault);

					var servers = new List<string> (Global.Servers);
					var serverAdapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItemSingleChoice, servers);

					var builder = new AlertDialog.Builder (this);
					builder.SetTitle ("Choose Server");
					builder.SetSingleChoiceItems (
							serverAdapter,
							servers.IndexOf (server),
							(obj, arg) => {
						server = servers [arg.Which];

						using (var prefEdit = pref.Edit()) {
							prefEdit.PutString (Global.SettingSelectedServer, server);
							prefEdit.Commit ();
						}
					});
					builder.SetPositiveButton ("Choose", NullClick);
					return builder.Create ();
				}

			case DialogLogout:
				{
					var builder = new AlertDialog.Builder (this);
					builder.SetTitle ("Are you sure?");
					builder.SetMessage ("Are you sure you want to continue logging out? You will not be able to log back in, if you don't have a network connection.");
					builder.SetPositiveButton ("Yes", delegate {
						Application.ClearCache ();

						var intent = new Intent (this, typeof(LoginActivity));
						StartActivity (intent);
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
			case Resource.Id.menu_changeServer:
				ShowDialog (DialogChangeServer);
				return true;

			case Resource.Id.menu_version:
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

			case Resource.Id.menu_logout:
				ShowDialog (DialogLogout);
				return true;

			default:
				return base.OnOptionsItemSelected (item);
			}
		}

		protected override void OnStart ()
		{
			base.OnStart ();

			var actionBar = ActionBar;

			if (actionBar != null) {
				if (this is LoginActivity) {

				} else {
					actionBar.SetDisplayShowCustomEnabled (true);

					var view = LayoutInflater.Inflate (Resource.Layout.menu_loggedInUser, null);
					var layout = new Android.App.ActionBar.LayoutParams (Android.App.ActionBar.LayoutParams.FillParent, Android.App.ActionBar.LayoutParams.FillParent);
					actionBar.SetCustomView (view, layout);

					var employeeNumber = view.FindViewById<TextView> (Resource.Id.employeeNumber);
					var employeeBranch = view.FindViewById<TextView> (Resource.Id.employeeBranch);

					employeeNumber.Text = CacheRepository.LoggedInEmployeeNumber;
					employeeBranch.Text = CacheRepository.LoggedInBranchName;
				}

				actionBar.SetDisplayUseLogoEnabled (true);
				actionBar.SetLogo (Resource.Drawable.logo);
			}
		}

		public override void OnBackPressed ()
		{
			// do nothing
		}

		protected JsonObject GetActiveWorkOrder ()
		{
			var obj = CacheRepository.Get (Global.ActiveWorkOrderObject);

			if (obj == null)
				throw new ApplicationException ("unable to get, because there is no active work order");

			return new JsonObject (obj);
		}

		protected object GetProperty (string unitNumber, string name)
		{
			var unit = WorkOrderRepository.Get (unitNumber);
			return unit [name];
		}

		protected void UpdateProperty (string unitNumber, string name, object value)
		{
			var unit = WorkOrderRepository.Get (unitNumber);
			unit [name] = value;

			WorkOrderRepository.Save (unitNumber, unit);
		}

		protected object GetCollectionProperty (string unitNumber, string collection, string id, string name)
		{
			var unit = WorkOrderRepository.Get (unitNumber);
			var list = (List<object>)unit [collection];

			for (int i = 0; i < list.Count; i++) {
				var a = (JsonObject)list [i];

				if ((string)a ["ID"] == id) {
					return a [name];
				}
			}

			return null;
		}

		protected void UpdateCollectionProperty (string unitNumber, string collection, string id, string name, object value)
		{
			var unit = WorkOrderRepository.Get (unitNumber);
			var list = (List<object>)unit [collection];

			for (int i = 0; i < list.Count; i++) {
				var a = (JsonObject)list [i];

				if ((string)a ["ID"] == id) {
					a [name] = value;
					list [i] = a;
				}
			}

			unit [collection] = list;
			WorkOrderRepository.Save (unitNumber, unit);
		}

		public Image GetImageByUnitNumberImageID (string unitNumber, string imageID)
		{
			Image image = new Image ();
			var unit = WorkOrderRepository.Get (unitNumber);

			var list = (List<object>)unit [Global.FieldImages];

			for (int i = 0; i < list.Count; i++) {
				var a = (JsonObject)list [i];

				if ((string)a ["ID"] == imageID) {

					if(a.ContainsKey("ID"))
					{
						image.ID = a["ID"].ToString();
					}

					if(a.ContainsKey("ImagePath"))
					{
						image.ImagePath = a["ImagePath"].ToString();
					}

					if(a.ContainsKey("Location"))
					{
						image.Location = a["Location"].ToString();
					}

					if(a.ContainsKey("Damaged"))
					{
						image.Damaged = a["Damaged"].ToString();
					}

					if (a.ContainsKey ("___imageUploaded")) {
						image.ImageUploaded = Convert.ToBoolean(a ["___imageUploaded"].ToString ());
					} else {
						image.ImageUploaded = false;
					}

					return image;
				}
			}
			return image;
		}

	}
}
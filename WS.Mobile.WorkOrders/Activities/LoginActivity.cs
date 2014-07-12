using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using WS.Mobile.WorkOrders.Backend;
using WS.Mobile.WorkOrders.Model;

namespace WS.Mobile.WorkOrders.Activities
{
	[Activity]
	public class LoginActivity : AbstractActivity
	{
		private const string Tag = "login";
		private EditText _employeeNumber;
		private EditText _pin;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.login);
					   
			_employeeNumber = FindViewById<EditText>(Resource.Id.loginEmployeeNumberText);
			_pin = FindViewById<EditText>(Resource.Id.loginPinText);

			_pin.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				if (e.ActionId == Android.Views.InputMethods.ImeAction.Done) {
					FindViewById<Button>(Resource.Id.loginButton).PerformClick();
				}
			};

			Button loginButton = FindViewById<Button>(Resource.Id.loginButton);
			loginButton.Click += OnLogin;
		}

		private void OnLogin (object sender, EventArgs e)
		{
			if (!IsOnlineWithAlert)
				return;

			var progressDialog = ProgressDialog.Show(this, "Logging In", "Please wait...", true);

			var employeeNumber = _employeeNumber.Text;
			Log.Info(Tag, "employee: " + employeeNumber);
			
			var pin = _pin.Text;
			Log.Info(Tag, "pin: " + pin);

			Task.Factory.StartNew(delegate {
				Application.ClearCache();

				var client = new WorkOrdersClient();
				var response = client.Login(employeeNumber, pin);

				if ((bool)response[Global.RestSuccessName] && (long)response["AuthenticationCode"] == 1L) {
					Log.Info(Tag, "auth success");

					var branchId = (string)response["ReportLocationNumber"];

					CacheRepository.LoggedIn = true;
					CacheRepository.LoggedInEmployeeNumber = employeeNumber;
					CacheRepository.LoggedInBranchId = branchId;

					Application.DownloadBusinessUnits();

					var branches = CacheRepository.Branches;
					var branch = branches.Where(x => x.Id == branchId).FirstOrDefault();

					CacheRepository.LoggedInBranchId = (branch == null ? branchId : branch.Id);
					CacheRepository.LoggedInBranchName = (branch == null ? "Unknown" : branch.Name);

					RunOnUiThread(delegate {
						var branchAdapter = new ArrayAdapter<Branch>(this, Android.Resource.Layout.SimpleListItemSingleChoice, branches);
						var branchChooserDialog = new AlertDialog.Builder(this);
						branchChooserDialog.SetTitle("Choose Branch");
						branchChooserDialog.SetSingleChoiceItems(
							branchAdapter, 
							branches.FindIndex(x => x.Id == branchId),
							(obj, arg) => {
								branch = branches[arg.Which];
								CacheRepository.LoggedInBranchId = branchId = (branch == null ? branchId : branch.Id);
								CacheRepository.LoggedInBranchName = (branch == null ? "Unknown" : branch.Name);
							});
						branchChooserDialog.SetPositiveButton("Choose", delegate {
							Task.Factory.StartNew(delegate {
								if (branch != null) {
									if (!Application.PopulateBranchData())
										throw new Exception("Populating the branch data has failed.");
								}
							}).ContinueWith(t => {
									RunOnUiThread(delegate {
										progressDialog.Dismiss();

										if (t.IsFaulted) {
											Log.Info(Tag, "error occurred = " + t.Exception.Message);

											var builder = new AlertDialog.Builder(this);
											builder.SetTitle("Login Failed");
											builder.SetMessage(t.Exception.Message);
											builder.SetPositiveButton("OK", NullClick);
											builder.Show();

											Application.ClearCache();
										} else if (branch == null) {
											Log.Info(Tag, "must choose branch");

											var builder = new AlertDialog.Builder(this);
											builder.SetTitle("Must Choose Branch");
											builder.SetMessage("You must choose a branch to continue, please login again.");
											builder.SetPositiveButton("OK", NullClick);
											builder.Show();
											
											Application.ClearCache();
										} else {
											Finish();
										}
									});
							});
						});
						branchChooserDialog.SetCancelable(false);
						branchChooserDialog.Show();
					});
				} else {
					Log.Info(Tag, "auth failed");

					RunOnUiThread(delegate {
						progressDialog.Hide();
						var error = (string)response["AuthenticationCodeMessage"];
						
						var builder = new AlertDialog.Builder(this);
						builder.SetTitle("Login Failed");
						builder.SetMessage(error);
						builder.SetPositiveButton("OK", NullClick);
						builder.Show();
					});
				}
			});
		}
	}
}
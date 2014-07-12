using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using WS.Mobile.WorkOrders.Backend;
using Android.Content.PM;
using System.Timers;
using System.Reflection;
using Android.Views.Animations;
using System.Threading;


namespace WS.Mobile.WorkOrders.Activities
{
	[Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]
	public class MainActivity : AbstractActivity
	{
		private const string Tag = "main";
		private const string FieldWorkOrderUploadStarted = "__workOrderUploadStarted";
		private const string FieldWorkOrderUploaded = "__workOrderUploaded";
		private const string FieldWorkOrderLocked = "__workOrderLocked";
		private const string FieldWorkOrderImagesUploaded = "__workOrderImagesUploaded";
		private const string FieldErrors = "__workOrderMessages";
		private const string FieldMessage = "__workOrderMessage";
		private const string FieldImageUploaded = "__imageUploaded";
		private const string FieldOmbOrderInbound = "OMBOrderInbound";
		private const string FieldTempPlaceholder = "__tempPlaceHolder";
		private const string FieldType = "Type";
		private const string FieldIsUnitAttributeChanged = "IsUnitAttributeChanged";
		private const string FieldUnitAttributeUploadFailed = "UnitAttributeUploadFailed";
		private const string FieldAppVersionCheckFailed = "AppVersionCheckFailed";
		private const int ColumnOpenButton = 0;
		private const int ColumnOffRentButton = 1;
		private const int ColumnOnRentButton = 2;
		private const int ColumnOmbInput = 3;
		private const int ColumnRefreshOMBButton = 4;
		private const int ColumnMessage = 5;
		private const int ColumnErrorButton = 6;
		private const int ColumnUploadButton = 7;
		private const int ColumnRemoveButton = 8;
		private System.Timers.Timer _refreshWorkOrdersTimer;
		private EditText _unitNumberPart1;
		private EditText _unitNumberPart2;
		private TableLayout _unitsTable;
		private Assembly assembly;
		private AssemblyName assemblyName;
		private object _workOrderLock = new object ();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			assembly = Assembly.GetExecutingAssembly();
			assemblyName = new AssemblyName(assembly.FullName);
			SetContentView (Resource.Layout.main);

			_unitNumberPart1 = FindViewById<EditText> (Resource.Id.unitNumberPart1Text);
			_unitNumberPart2 = FindViewById<EditText> (Resource.Id.unitNumberPart2Text);
			_unitsTable = FindViewById<TableLayout> (Resource.Id.unitsTable);
			
			_unitNumberPart1.AfterTextChanged += (sender, e) => {
				if (_unitNumberPart1.Text.Length == 3) {
					_unitNumberPart2.RequestFocusFromTouch ();
				}
			};
			
			_unitNumberPart2.KeyPress += (sender, e) => {
				var editText = (EditText)sender;
				
				if (e.KeyCode == Keycode.Del && editText.Text == "") {
					var takeLeft = Math.Max (0, _unitNumberPart1.Text.Length - 1);
					_unitNumberPart1.Text = _unitNumberPart1.Text.Substring (0, takeLeft);
					_unitNumberPart1.SetSelection (takeLeft);
					_unitNumberPart1.RequestFocusFromTouch ();
					
					e.Handled = true;
				}
				
				e.Handled = false;
			};
			
			_unitNumberPart2.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				e.Handled = true;
				if (e.ActionId == Android.Views.InputMethods.ImeAction.Done) {
					FindViewById<Button> (Resource.Id.unitAddButton).PerformClick ();
				}
			};
			
			Button addButton = FindViewById<Button> (Resource.Id.unitAddButton);
			addButton.Click += HandleAddUnitClick;

			_refreshWorkOrdersTimer = new System.Timers.Timer {
				AutoReset = true,
				Interval = TimeSpan.FromSeconds(30).TotalMilliseconds
			};

			_refreshWorkOrdersTimer.Elapsed += HandleRefreshWorkOrdersElapsed;
			_refreshWorkOrdersTimer.Start ();
		}

		#region Open Work Order

		private void OpenOnRentOffRent (string unitNumber, bool isOffRent)
		{
			var intent = new Intent ();
			intent.SetClass (this, typeof(WorkOrderActivity));
			if (isOffRent) {
				intent.PutExtra ("type", Global.FieldOffRentType);
			} else {
				intent.PutExtra ("type", Global.FieldOnRentType);
			}
			intent.PutExtra ("unitNumber", unitNumber);
			StartActivity(intent);
		}
		private void HandleOpenOnRentWorkOrderClick (object sender, EventArgs e)
		{
			var control = (Button)sender;
			var unitNumber = (string)control.Tag;
			control.Enabled = false;
			UpdateProperty(unitNumber,FieldType, Global.FieldOnRentType);
			OpenOnRentOffRent (unitNumber,false);
		}
		private void HandleOpenOffRentWorkOrderClick (object sender, EventArgs e)
		{
			var control = (Button)sender;
			var unitNumber = (string)control.Tag;
			control.Enabled = false;
			UpdateProperty(unitNumber,FieldType, Global.FieldOffRentType);

			OpenOnRentOffRent (unitNumber,true);
		}
		#endregion

		#region Upload Work Order

		private void UploadWorkOrder (object o)
		{
			var obj = (JsonObject)o;
			var unitNumber = (string)obj [Global.FieldUnitNumber];
			var guid = (string)obj ["ID"];

			lock (_workOrderLock) {
				obj [FieldWorkOrderUploadStarted] = DateTime.Now.Ticks;

				// reset errors and messages
				obj [FieldErrors] = null;
				obj [FieldMessage] = null;

				WorkOrderRepository.Save (unitNumber, obj);
			}

			var client = new WorkOrdersClient ();
			var hasWorkOrderBeenUploaded = (bool)(obj [FieldWorkOrderUploaded] ?? false);
			var hasWorkOrderBeenLocked = (bool)(obj [FieldWorkOrderLocked] ?? false);
			var hasWorkOrderImagesBeenUploaded = (bool)(obj [FieldWorkOrderImagesUploaded] ?? false);
			var tryToUploadImages = hasWorkOrderBeenLocked;
			var hasUnitAttributeBeenFailed = (bool)(obj [FieldUnitAttributeUploadFailed] ?? false);
			var hasAppVersionCheckFailed = (bool)(obj [FieldAppVersionCheckFailed] ?? false);

			obj ["TabletID"] = Settings.Secure.GetString (ApplicationContext.ContentResolver, Settings.Secure.AndroidId);
			obj ["EmployeeNumber"] = CacheRepository.LoggedInEmployeeNumber;
			obj ["ReportLocationNumber"] = CacheRepository.LoggedInBranchId;
			
			WorkOrderRepository.Save (unitNumber, obj);
			
			// step 1: send work order
			if (!hasWorkOrderBeenUploaded) {
				obj [FieldMessage] = "Uploading work order.";
				WorkOrderRepository.Save (unitNumber, obj);

				SetMessageOnRow (obj);

				var sendingWorkOrder = (JsonObject)obj.Clone ();
				if ((string)sendingWorkOrder [FieldIsUnitAttributeChanged] != null && (string)sendingWorkOrder [FieldIsUnitAttributeChanged] != "1") {
					sendingWorkOrder.Remove ("UnitAttributes");
				}
				var response = client.AddWorkOrder (sendingWorkOrder);
				var returnCode = (long)(response ["ReturnCode"] ?? -1L);
				
				// success 
				hasWorkOrderBeenUploaded = (bool)response [Global.RestSuccessName] && returnCode == 1L;
				hasAppVersionCheckFailed = (bool)response [Global.RestSuccessName] && (returnCode == 2L);
				hasUnitAttributeBeenFailed = (bool)response [Global.RestSuccessName] && (returnCode == 5L);
				// success or JDE Business function call failed
				hasWorkOrderBeenLocked = (bool)response [Global.RestSuccessName] && (returnCode == 1L || returnCode == 4L);
				
				// set statuses
				obj [FieldWorkOrderUploaded] = hasWorkOrderBeenUploaded;
				obj [FieldWorkOrderLocked] = hasWorkOrderBeenLocked;
				obj [FieldUnitAttributeUploadFailed] = hasUnitAttributeBeenFailed;
				obj [FieldAppVersionCheckFailed] = hasAppVersionCheckFailed;
				// set work order id
				obj [Global.FieldWorkOrderId] = response [Global.FieldWorkOrderId];
				
				// save to device
				WorkOrderRepository.Save (unitNumber, obj);

				if (hasAppVersionCheckFailed) {
					obj [FieldMessage] = "App Version Failed.";
					obj [FieldErrors] = response ["WorkOrderMessages"];
					WorkOrderRepository.Save (unitNumber, obj);

					SetMessageOnRow (obj);
					SetErrorMessageOnRow (obj);
					EnableRow (obj);

				}
				if (hasUnitAttributeBeenFailed) {
					obj [FieldMessage] = "UA Upload Failed.";
					obj [FieldErrors] = response ["WorkOrderMessages"];
					WorkOrderRepository.Save (unitNumber, obj);

					SetMessageOnRow (obj);
					SetErrorMessageOnRow (obj);
					EnableRow (obj);

				}
				if (!hasWorkOrderBeenUploaded) {
					obj [FieldMessage] = " WO Upload Failed.";
					obj [FieldErrors] = response ["WorkOrderMessages"];
					WorkOrderRepository.Save (unitNumber, obj);
					
					SetMessageOnRow (obj);
					SetErrorMessageOnRow (obj);
					EnableRow (obj);

				}
				
				// if work order has been locked then upload images
				tryToUploadImages = hasWorkOrderBeenLocked;
			}
			
			// step 2: check if work order was success 
			if (!hasWorkOrderImagesBeenUploaded && tryToUploadImages) {

				var imageObjects = (List<object>)obj [Global.FieldImages];

				int totalImageCount = imageObjects.Count;
				var imagesToUploadCount = 0;
				var imagesUploadCompleteCount = 0;
				var imagesUploadFailedCount = 0;

				foreach (var imageObject in imageObjects) {
					imagesToUploadCount++;
					var message = "Uploading images " + (imagesToUploadCount) + " of " + totalImageCount;

					obj [FieldMessage] = message;
					SetMessageOnRow (obj);
					WorkOrderRepository.Save (unitNumber, obj);

					string imageID = Newtonsoft.Json.JsonConvert.DeserializeObject<Image> (imageObject.ToString()).ID;

					var image = GetImageByUnitNumberImageID (unitNumber, imageID);

					if (!image.ImageUploaded) {

						var imageResponse = client.AddWorkOrderImages (guid, image);

						//var imageUploadSuccess = (bool)imageResponse [Global.RestSuccessName] && (bool)imageResponse ["Success"] == true;
						if (imageResponse) {
							imagesUploadCompleteCount++;
						} else {
							imagesUploadFailedCount++;
						}

						UpdateCollectionProperty (unitNumber, Global.FieldImages, image.ID, FieldImageUploaded, imageResponse);

						obj = WorkOrderRepository.Get (unitNumber);

					} else {
						imagesUploadCompleteCount++;
					}
				}

				if (imagesToUploadCount == imagesUploadCompleteCount) {
					hasWorkOrderImagesBeenUploaded = true;
				} else {
					hasWorkOrderImagesBeenUploaded = false;
				}
			}

			obj [FieldWorkOrderImagesUploaded] = hasWorkOrderImagesBeenUploaded;
			WorkOrderRepository.Save (unitNumber, obj);

				// step 2-2: check if images was success
			if (!hasWorkOrderImagesBeenUploaded) {
				obj [FieldMessage] = "Image upload Failed. Retry";
				WorkOrderRepository.Save (unitNumber, obj);
					
				SetMessageOnRow (obj);
				EnableRow (obj);


			}
			
			obj [FieldWorkOrderUploadStarted] = null;
			WorkOrderRepository.Save (unitNumber, obj);
			
			// step 3: remove row if work order was a success
			if (hasWorkOrderBeenUploaded && hasWorkOrderImagesBeenUploaded) {
				RemoveRow (unitNumber);
			} else if (hasWorkOrderBeenUploaded == false) {
				obj [FieldMessage] = "WO Upload Failed.";
				WorkOrderRepository.Save (unitNumber, obj);
				
				SetMessageOnRow (obj);
			} else if (hasWorkOrderImagesBeenUploaded == false) {
				obj [FieldMessage] = "Image upload Failed. Retry";
				WorkOrderRepository.Save (unitNumber, obj);
				
				SetMessageOnRow (obj);
			}
		}


		private void HandleUploadWorkOrderClick (object sender, EventArgs e)
		{

			if (!IsOnlineWithAlert)
				return;

			var control = (Button)sender;
			var unitNumber = (string)control.Tag;

			var obj = WorkOrderRepository.Get (unitNumber);
			var hasWorkOrderBeenUploaded = (bool)(obj [FieldWorkOrderUploaded] ?? false);
			obj[Global.FieldVersion]= assemblyName.Version.ToString();
			DisableRow (obj);
			ClearErrorMessageOnRow (unitNumber);

			// skip validation if the work order has already been uploaded
			if (!hasWorkOrderBeenUploaded) {
				var parts = (IList<object>)obj [Global.FieldParts];
				var images = (IList<object>)obj [Global.FieldImages];
				var ombNumber = (string)obj [FieldOmbOrderInbound];
				var type = (string)obj [FieldType];
				var sb = new StringBuilder ();
				var count = 0;


				if (String.IsNullOrWhiteSpace (ombNumber))
					sb.AppendFormat ("{0}. {1}" + System.Environment.NewLine + System.Environment.NewLine, ++count, "No OMB Order # was set.");
				if (type == Global.FieldOffRentType) {
					if (parts.Count == 0)
						sb.AppendFormat ("{0}. {1}" + System.Environment.NewLine + System.Environment.NewLine, ++count, "No parts were added.");
				}
				if (images.Count == 0)
					sb.AppendFormat ("{0}. {1}" + System.Environment.NewLine + System.Environment.NewLine, ++count, "No images were added.");

				if (sb.Length > 0) {
					sb.Insert (0, "Are you sure you want to continue without the following in this work order?" + System.Environment.NewLine + System.Environment.NewLine);

					var builder = new AlertDialog.Builder (this);
					builder.SetTitle ("Work Order");
					builder.SetMessage (sb.ToString ());
					builder.SetPositiveButton ("Yes", delegate {
						Task.Factory.StartNew (UploadWorkOrder, obj);
					});
					builder.SetNegativeButton ("No", delegate {
						EnableRow (obj);

					});
					builder.SetCancelable (false);
					builder.Show ();

					return;
				}
			}

			Task.Factory.StartNew (UploadWorkOrder, obj);
		}

		private void HandleRefreshOMBOrderClick (object sender, EventArgs e)
		{
			try {
				var control = (ImageButton)sender;
				var unitNumber = (string)control.Tag;

				//RefeshOMBOrders (unitNumber);

				var progressDialog = ProgressDialog.Show (this, "Refreshing", "Please wait...", true);

				var x = Task.Factory.StartNew (delegate {

					var obj = getOMBOrders (unitNumber);

					var orders = ((IEnumerable<object>)obj ["Result"]).Cast<JsonObject> ();

					var row = GetRow (unitNumber);

					if (row == null)
						return;

					RunOnUiThread (delegate {
					List<string> OMBOrders = new List<string> ();
					string orderNumbers;
					OMBOrders.Add ("");

					if (orders.Count () != 0) {
						foreach (var obm in orders) {
							orderNumbers = (string)(obm ["SKey"] + " -" + obm ["QStatus"] + " - " + obm ["Type"] + " - " + obm ["Name"]);
							OMBOrders.Add (orderNumbers.Substring (0, orderNumbers.Length > 35 ? 35 : orderNumbers.Length));
						}
					}

					var workOrderDropDown = (Spinner)row.GetChildAt (ColumnOmbInput);
					workOrderDropDown.Adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleSpinnerDropDownItem, OMBOrders);

					var obj1 = WorkOrderRepository.Get (unitNumber);
					var ombNumber = (string)obj1 [FieldOmbOrderInbound];
					if (!String.IsNullOrWhiteSpace (ombNumber)) {
						workOrderDropDown.SetSelection (OMBOrders.FindIndex (0, o => o.Contains (ombNumber)));
					}

					});


				}).ContinueWith (t => progressDialog.Dismiss ());

			} catch (Exception ex) {
				string x = ex.Message.ToString ();
				//var tryAgain = true;
			}
		}

		private JsonObject getOMBOrders (object unitNumberObject)
		{   
			var unitNumber = (string)unitNumberObject;
			var online = IsOnlineWithAlert;
			var tryAgain = !online;
			if (online) {
				try {
				
					var client = new WorkOrdersClient ();
					var obj = client.GetOMBOrders (unitNumber);
					return obj;
				
				} catch (Exception e) {
					string x = e.Message.ToString ();
					tryAgain = true;
				}
			}

			return null;
		}


		#endregion

		#region Add Unit

		private void DownloadUnit (object unitNumberObject)
		{   
			var unitNumber = (string)unitNumberObject;
			var online = IsOnlineWithAlert;
			var tryAgain = !online;

			if (online) {
				try {
					var client = new WorkOrdersClient ();
					var obj = client.GetUnit (unitNumber);
				
					if ((bool)obj [Global.RestSuccessName] && (long)obj ["UnitReturnCode"] == 1L) {
						obj [Global.FieldUnitNumber] = unitNumber;
						obj ["ID"] = Guid.NewGuid ().ToString ();
						WorkOrderRepository.Save (unitNumber, obj);

						// reset the button to remove any events from the download again
						RunOnUiThread (delegate {
							var row = GetRow (unitNumber);

							if (row == null)
								return;

							row.RemoveViewAt(ColumnOpenButton);
							var openUnitButton = new Button (this);
							openUnitButton.Tag = unitNumber;
							openUnitButton.Enabled = false;
							openUnitButton.Text = unitNumber;
							row.AddView(openUnitButton, ColumnOpenButton);

							//populate the OMBOrder dropdown
							List<string> orders;
							orders =PopulateOMBOrderDropDown ( obj);
							var workOrderDropDown = (Spinner)row.GetChildAt (ColumnOmbInput);
							workOrderDropDown.Adapter = new ArrayAdapter<string> (this,Android.Resource.Layout.SimpleSpinnerDropDownItem , orders);

						});

						EnableRow (obj);
						ResetOpenWorkOrderButtonTextOnRow (obj);

					} else {
						var error = (string)obj ["UnitReturnCodeMessage"];
					
						RunOnUiThread (delegate {
							var builder = new AlertDialog.Builder (this);
							builder.SetTitle (unitNumber + " Failed");
							builder.SetMessage (error);
							builder.SetPositiveButton ("OK", NullClick);
							builder.Show ();
						
							RemoveRow (unitNumber);
						});
					}
				} catch (Exception exc) {
					tryAgain = true;
					Log.Error(Tag, exc.ToString());
				}
			}

			if (tryAgain) {
				RunOnUiThread (delegate {
					var row = GetRow (unitNumber);

					if (row == null)
						return;

					var offRentButton = (Button)row.GetChildAt (ColumnOffRentButton);
					var onRentButton = (Button)row.GetChildAt (ColumnOnRentButton);
					var refreshOMBOrderButton = (ImageButton)row.GetChildAt (ColumnRefreshOMBButton);


					row.RemoveViewAt(ColumnOpenButton);
					var openUnitButton = new Button (this);
					openUnitButton.Tag = unitNumber;
					openUnitButton.Enabled = false;//true;
					openUnitButton.Text = "Download " + unitNumber;
					openUnitButton.Click += (x, y) => {
						var button = (Button)x;
						button.Enabled = false;Log.Debug(Tag, "response: 1");
						button.Text = "Downloading " + unitNumber;
						Log.Debug(Tag, "response: 1");
						Task.Factory.StartNew (DownloadUnit, unitNumber);
					};

					row.AddView(openUnitButton, ColumnOpenButton);

					var removeButton = (Button)row.GetChildAt(ColumnRemoveButton);
					removeButton.Enabled = true;
					offRentButton.Enabled=false;
					onRentButton.Enabled=false;
					refreshOMBOrderButton.SetImageResource (Resource.Drawable.refresh_btn_25_Grey);
					refreshOMBOrderButton.Enabled=false;
				});
			}
		}

		private List<string> PopulateOMBOrderDropDown ( JsonObject obj) 
		{
			List<string> OMBOrders = new List<string> ();
			string orderNumbers;
			OMBOrders.Add ("");
			var orders = ((IEnumerable<object>)obj["OMBOrders"]).Cast<JsonObject>();
			if (orders.Count() != 0) {
				foreach (var obm in orders) {
					orderNumbers = (string)(obm ["SKey"] + " -" + obm ["QStatus"] + " - " + obm ["Type"] + " - " + obm ["Name"]);
					OMBOrders.Add (orderNumbers.Substring(0, orderNumbers.Length>35?35:orderNumbers.Length));
				}
			}
			return OMBOrders; 
		}

		private void DeleteImages (string unitNumber)
		{
			Task.Factory.StartNew (delegate {
				var dir = new Java.IO.File (
					Android.OS.Environment.GetExternalStoragePublicDirectory (Android.OS.Environment.DirectoryPictures),
					unitNumber);
				
				if (dir.Exists ()) {
					var files = dir.ListFiles ();
					foreach (var file in files) {
						file.Delete ();
					}

					var dir2 = new Java.IO.File (
						Android.OS.Environment.GetExternalStoragePublicDirectory (Android.OS.Environment.DirectoryPictures),
						unitNumber + "_1");

					dir.RenameTo(dir2);

					dir2.Delete();
				}
				
				// http://stackoverflow.com/questions/5250515/how-to-update-the-android-media-database
				var mediaScan = new Intent (Intent.ActionMediaMounted);
				var imageDir = dir.ParentFile;
				var imageDirUri = Android.Net.Uri.FromFile (imageDir);
				mediaScan.SetData (imageDirUri);
				SendBroadcast (mediaScan);
			});
		}

		private void HandleAddUnitClick (object sender, EventArgs e)
		{
			var part1 = _unitNumberPart1.Text;
			part1 = part1.ToUpper ();
			part1 = part1.Trim ();
			
			var part2 = _unitNumberPart2.Text;
			
			if (String.IsNullOrWhiteSpace (part1) || String.IsNullOrWhiteSpace (part2)) {
				var builder = new AlertDialog.Builder (this);
				builder.SetTitle ("Required");
				builder.SetMessage ("Both fields of the unit number are required.");
				builder.SetPositiveButton ("OK", NullClick);
				builder.Show ();
				
				return;
			}
			
			var unitNumber = part1 + "-" + part2;
			
			_unitNumberPart1.Text = "";
			_unitNumberPart1.RequestFocusFromTouch ();
			
			_unitNumberPart2.Text = "";
			
			if (GetRow (unitNumber) != null) {
				var builder = new AlertDialog.Builder (this);
				builder.SetTitle (unitNumber + " Failed");
				builder.SetMessage ("You have already added " + unitNumber);
				builder.SetPositiveButton ("OK", NullClick);
				builder.Show ();
				
				return;
			}
			
			var fakeObj = new JsonObject ();
			fakeObj [Global.FieldUnitNumber] = unitNumber;
			fakeObj [FieldTempPlaceholder] = true;

			WorkOrderRepository.Save (unitNumber, fakeObj);

			AddRow (fakeObj);

			var row = GetRow (unitNumber);
			var openUnitButton = (Button)row.GetChildAt (ColumnOpenButton);
			openUnitButton.Text = "Downloading " + unitNumber;
			Log.Debug(Tag, "response: 2");
			openUnitButton.Enabled = false;

			DisableRow (fakeObj);

			Task.Factory.StartNew (DownloadUnit, unitNumber);
		}

		#endregion

		#region Row Functions
		
		private TableRow GetRow (string unitNumber)
		{
			var view = _unitsTable.FindViewWithTag (unitNumber);
			var rowFound = view != null && view is TableRow;

			if (!rowFound) {
				Log.Error (Tag, "row not found for " + unitNumber);
				return null;
			}
			
			return (TableRow)view;
		}

		private bool ValidateWorkOrderCanBeUploaded (JsonObject obj)
		{
			var hasWorkOrderBeenUploaded = (bool)(obj [FieldWorkOrderUploaded] ?? false);
			var hasWorkOrderImagesBeenUploaded = (bool)(obj [FieldWorkOrderImagesUploaded] ?? false);
			var hasUnitAttributeBeenFailed = (bool)(obj [FieldUnitAttributeUploadFailed] ?? false);
			var rentType = (string)obj [FieldType];
			var activities = (IList<object>)obj [Global.FieldActivities];
			if (rentType == Global.FieldOffRentType) { 
				return activities != null && activities.Count > 0 && (!hasWorkOrderBeenUploaded || !hasWorkOrderImagesBeenUploaded || hasUnitAttributeBeenFailed);
			}else {
				var images = (List<object>)obj[Global.FieldImages];
				return images != null && images.Count>0 && (!hasWorkOrderBeenUploaded || !hasWorkOrderImagesBeenUploaded || hasUnitAttributeBeenFailed);
			}
		}

		private void DisableRowIfActiveUploadIsHappeningForWorkOrder (JsonObject obj)
		{
			if (obj [FieldWorkOrderUploadStarted] != null) {
				var startedOn = new DateTime ((long)obj [FieldWorkOrderUploadStarted]);
				if ((DateTime.Now - startedOn) < TimeSpan.FromMinutes (5))
					DisableRow (obj);
			}
		}

		private void DisableRow (JsonObject obj)
		{
			RunOnUiThread (delegate {
				var unitNumber = (string)obj [Global.FieldUnitNumber];
				var row = GetRow (unitNumber);

				if (row == null)
					return;

				var openUnitButton = (Button)row.GetChildAt (ColumnOpenButton);
				openUnitButton.Enabled = false;

				var openOffRentUnitButton = (Button)row.GetChildAt (ColumnOffRentButton);
				openOffRentUnitButton.Enabled = false;

				var openOnRentUnitButton = (Button)row.GetChildAt (ColumnOnRentButton);
				openOnRentUnitButton.Enabled = false;
				
				var ombNumber = (Spinner)row.GetChildAt (ColumnOmbInput);
				ombNumber.Enabled = false;

				var refreshOMBOrdersButton = (ImageButton)row.GetChildAt (ColumnRefreshOMBButton);
				refreshOMBOrdersButton.Enabled = false;

				var uploadUnitButton = (Button)row.GetChildAt (ColumnUploadButton);
				uploadUnitButton.Enabled = false;
				
				var removeUnitButton = (Button)row.GetChildAt (ColumnRemoveButton);
				removeUnitButton.Enabled = false;
			});
		}
		private bool IsUnitOffRent(string unitNumber)
		{
			var unit = WorkOrderRepository.Get(unitNumber);
			var images = (List<object>)unit[Global.FieldImages];

			if (images == null || images.Count==0) {
				return true;
			}
			string offRent = (string)GetProperty (unitNumber, FieldType);
			if (offRent == Global.FieldOffRentType || string.IsNullOrEmpty(offRent) ) {
				return true;
			} else { return false;}
		}
		private bool IsUnitOnRent(string unitNumber)
		{
			var unit = WorkOrderRepository.Get(unitNumber);
			var images = (List<object>)unit[Global.FieldImages];
			var activities = (List<object>)unit[Global.FieldActivities];
			var parts = (List<object>)unit[Global.FieldParts];
			if ((images == null || images.Count==0) &&(activities== null || activities.Count ==0) && (parts == null || parts.Count==0  ) ) {
				return true;
			}
			string onRent = (string)GetProperty (unitNumber, FieldType);
			if (onRent == Global.FieldOnRentType || string.IsNullOrEmpty(onRent) ) {
				return true;
			} else { return false;}
		}

		private void EnableRow (JsonObject obj)
		{
			RunOnUiThread (delegate {
				var unitNumber = (string)obj [Global.FieldUnitNumber];
				var hasWorkOrderBeenLocked = (bool)(obj [FieldWorkOrderLocked] ?? false);
				
				var row = GetRow (unitNumber);

				if (row == null)
					return;

				var openUnitButton = (Button)row.GetChildAt (ColumnOpenButton);
				openUnitButton.Enabled = !hasWorkOrderBeenLocked;


				//refreshOMBOrderButton.Enabled = !hasWorkOrderBeenLocked;

				var openOffRentUnitButton = (Button)row.GetChildAt (ColumnOffRentButton);
				var openOnRentUnitButton = (Button)row.GetChildAt (ColumnOnRentButton);
				var refreshOMBOrderButton = (ImageButton)row.GetChildAt (ColumnRefreshOMBButton);
				if (obj [FieldTempPlaceholder] == null) {
					openOffRentUnitButton.Enabled = !hasWorkOrderBeenLocked && IsUnitOffRent (unitNumber);
					openOnRentUnitButton.Enabled = !hasWorkOrderBeenLocked && IsUnitOnRent (unitNumber);
					if(!hasWorkOrderBeenLocked)
					{
						refreshOMBOrderButton.Enabled = true;
						refreshOMBOrderButton.SetImageResource (Resource.Drawable.refresh_btn_25);
					}
				} else {
					openOffRentUnitButton.Enabled = false;
					openOnRentUnitButton.Enabled = false;
					refreshOMBOrderButton.SetImageResource (Resource.Drawable.refresh_btn_25_Grey);
					refreshOMBOrderButton.Enabled = false;
				}
				var ombNumber = (Spinner)row.GetChildAt (ColumnOmbInput);
				ombNumber.Enabled = !hasWorkOrderBeenLocked;

				var uploadUnitButton = (Button)row.GetChildAt (ColumnUploadButton);
				uploadUnitButton.Enabled = ValidateWorkOrderCanBeUploaded (obj);
				
				var removeUnitButton = (Button)row.GetChildAt (ColumnRemoveButton);
				removeUnitButton.Enabled = true;
			});
		}

		private void ResetOpenWorkOrderButtonTextOnRow (JsonObject obj)
		{
			RunOnUiThread (delegate {
				var unitNumber = (string)obj [Global.FieldUnitNumber];
				var workOrderId = obj [Global.FieldWorkOrderId];
				var hasWorkOrderBeenLocked = (bool)(obj [FieldWorkOrderLocked] ?? false);
				var row = GetRow (unitNumber);

				if (row == null)
					return;

				var text = unitNumber + ((workOrderId == null || !(workOrderId is long) || (long)workOrderId == 0L) ? "" : " (" + workOrderId + ")");

				var openUnitButton = (Button)row.GetChildAt (ColumnOpenButton);
				var offRentButton = (Button)row.GetChildAt (ColumnOffRentButton);
				var onRentButton = (Button)row.GetChildAt (ColumnOnRentButton);
				var refreshOMBOrderButton = (ImageButton)row.GetChildAt (ColumnRefreshOMBButton);
				// only update if this is a real work order
				if (obj [FieldTempPlaceholder] == null){
					openUnitButton.Text = text;
					offRentButton.Enabled=!hasWorkOrderBeenLocked && IsUnitOffRent (unitNumber);
					onRentButton.Enabled=!hasWorkOrderBeenLocked && IsUnitOnRent(unitNumber);				
					refreshOMBOrderButton.Enabled = !hasWorkOrderBeenLocked;
				}

			});
		}

		private void SetMessageOnRow (JsonObject obj)
		{
			RunOnUiThread (delegate {
				var unitNumber = (string)obj [Global.FieldUnitNumber];

				var row = GetRow (unitNumber);

				if (row == null)
					return;

				var message = (string)obj [FieldMessage];
				
				var messageText = (TextView)row.GetChildAt (ColumnMessage);
				messageText.Text = message;
			});
		}
		
		private void SetErrorMessageOnRow (JsonObject obj)
		{
			if (obj [FieldErrors] != null) {
				RunOnUiThread (delegate {
					var unitNumber = (string)obj [Global.FieldUnitNumber];

					var row = GetRow (unitNumber);

					if (row == null)
						return;

					var messages = (List<object>)obj [FieldErrors];
					var sb = new StringBuilder ();
					
					row.RemoveViewAt (ColumnErrorButton);
					
					var count = 0;
					foreach (var message in messages.Cast<JsonObject>()) {
						sb.AppendFormat ("{0}. {1}" + System.Environment.NewLine + System.Environment.NewLine, ++count, message ["Message"]);
					}
					
					var showMessage = sb.ToString ();
					var messageButton = new Button (this);
					messageButton.Text = "View Errors";
					messageButton.Click += delegate {
						RunOnUiThread (delegate {
							var builder = new AlertDialog.Builder (this);
							builder.SetTitle (unitNumber + " Validation Errors");
							builder.SetMessage (showMessage);
							builder.SetPositiveButton ("OK", NullClick);
							builder.Show ();
						});
					};
					row.AddView (messageButton, ColumnErrorButton);
				});
			}
		}
		
		private void ClearErrorMessageOnRow (string unitNumber)
		{
			RunOnUiThread (delegate {
				var row = GetRow (unitNumber);

				if (row == null)
					return;

				row.RemoveViewAt (ColumnErrorButton);
				
				var messageButton = new TextView (this);
				row.AddView (messageButton, ColumnErrorButton);
				
				UpdateProperty (unitNumber, FieldMessage, null);
				UpdateProperty (unitNumber, FieldErrors, null);
			});
		}
		
		private void RemoveRow (string unitNumber)
		{
			RunOnUiThread (delegate {
				var row = GetRow (unitNumber);

				if (row == null)
					return;

				_unitsTable.RemoveView (row);

				WorkOrderRepository.Delete (unitNumber);
			});
			
			DeleteImages (unitNumber);
		}
		
		private void RemoveRowWithAlert (string unitNumber)
		{
			var builder = new AlertDialog.Builder (this);
			builder.SetTitle ("Remove " + unitNumber);
			builder.SetMessage ("Are you sure you want to remove " + unitNumber + " and associated work order information from the device?");
			builder.SetPositiveButton ("Yes", delegate {
				RemoveRow (unitNumber);
			});
			builder.SetNegativeButton ("No", NullClick);
			builder.Show ();
		}
		
		#endregion

		#region Refresh Work Orders

		private void HandleRefreshWorkOrdersElapsed (object sender, ElapsedEventArgs e)
		{
			var workOrders = (IEnumerable<JsonObject>)null;

			lock (_workOrderLock) {
				workOrders = WorkOrderRepository.GetAll ();
			}

			foreach (var obj in workOrders) {
				SetMessageOnRow (obj);
				SetErrorMessageOnRow (obj);
				EnableRow (obj);
				ResetOpenWorkOrderButtonTextOnRow (obj);
				DisableRowIfActiveUploadIsHappeningForWorkOrder (obj);
			}
		}

		#endregion

		protected override void OnResume ()
		{
			base.OnResume ();
			
			if (!CacheRepository.LoggedIn ||  CacheRepository.AttributeCategories == null) {
				var intent = new Intent (this, typeof(LoginActivity));
				StartActivity (intent);
			}
			
			try {
				var activeWorkOrder = GetActiveWorkOrder ();

				if (activeWorkOrder != null) {
					var unitNumber = (string)activeWorkOrder [Global.FieldUnitNumber];
					var type = (string)activeWorkOrder [FieldType];
					OpenOnRentOffRent (unitNumber,type== Global.FieldOffRentType? true: false);

				}
			} catch (Exception exc) {
				Log.Error(Tag, exc.ToString());
			}

			_unitNumberPart1.RequestFocusFromTouch ();
			LoadWorkOrders ();
		}

		private void LoadWorkOrders ()
		{
			var rows = _unitsTable.ChildCount;
			for (int i = rows -2; i > 0; i--)
				_unitsTable.RemoveViewAt (i);
			
			var workOrders = WorkOrderRepository.GetAll ();
			
			foreach (var obj in workOrders)
				AddRow (obj);
		}

		private void AddRow (JsonObject obj)
		{
			JsonObject objUnit = null;
			var unitNumber = (string)obj [Global.FieldUnitNumber];
			var row = new TableRow (this);
			row.Tag = unitNumber;

			var hasWorkOrderBeenLocked = (bool)(obj [FieldWorkOrderLocked] ?? false);
			if (obj [FieldTempPlaceholder] == null) {
			 objUnit = WorkOrderRepository.Get(unitNumber);
			}
				

			var openUnitButton = new Button (this);
			openUnitButton.Tag = unitNumber;
			openUnitButton.Enabled = false;//!hasWorkOrderBeenLocked;

			if (obj [FieldTempPlaceholder] != null) {
				openUnitButton.Text = "Download " + unitNumber;
				openUnitButton.Click += (x, y) => {
					var button = (Button)x;
					button.Enabled = false;
					button.Text = "Downloading " + unitNumber;
					Log.Debug(Tag, "response: 3");
					DownloadUnit (unitNumber);
				};
			} else {
				openUnitButton.Text = unitNumber;
			} 

			row.AddView (openUnitButton);

			/* off Rent Button */
			var offRentButton = new Button (this);
			offRentButton.Tag = unitNumber;
			offRentButton.Enabled = false;//!hasWorkOrderBeenLocked;
			offRentButton.Text = GetString(Resource.String.unit_Off_Rent);
			offRentButton.Click += HandleOpenOffRentWorkOrderClick;
			row.AddView (offRentButton);
			/* on Rent Button */
			var onRentButton = new Button (this);
			onRentButton.Tag = unitNumber;
			onRentButton.Enabled = false;//!hasWorkOrderBeenLocked;
		    onRentButton.Text = GetString(Resource.String.unit_On_Rent);
			onRentButton.Click += HandleOpenOnRentWorkOrderClick;
			row.AddView (onRentButton);
			// OMBOrder dropdown
			var workOrderDropDown = new Spinner (this);
			workOrderDropDown.Enabled = !hasWorkOrderBeenLocked;
			if (obj [FieldTempPlaceholder] != null) {
			    workOrderDropDown.Adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleSpinnerDropDownItem, new List<string> { "" });

			} else {
				//populate the OMB dropdown
				RunOnUiThread (delegate {
									 
					List<string> orders;
					orders = PopulateOMBOrderDropDown (objUnit);
					workOrderDropDown.Adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleSpinnerDropDownItem, orders);

					var ombNumber = (string)obj [FieldOmbOrderInbound];
					if (!String.IsNullOrWhiteSpace (ombNumber)) {
						workOrderDropDown.SetSelection(orders.FindIndex (0,o => o.Contains(ombNumber)));
					}
				});
			}
			workOrderDropDown.ItemSelected += (sender, e) => {
				UpdateProperty(unitNumber, FieldOmbOrderInbound, string.IsNullOrEmpty(workOrderDropDown.GetItemAtPosition(e.Position).ToString())? "": workOrderDropDown.GetItemAtPosition(e.Position).ToString().Substring(0,6)); 
			};

			var ombSpinnerLayout = new TableRow.LayoutParams(TableRow.LayoutParams.WrapContent, TableRow.LayoutParams.WrapContent); 
			ombSpinnerLayout.Gravity = GravityFlags.CenterVertical;
			ombSpinnerLayout.Height = 50;
			ombSpinnerLayout.Width = 300;

			row.AddView (workOrderDropDown, ombSpinnerLayout);

			//Referesh OMB Orders

			var refreshOMBOrderButton = new ImageButton (this);
			refreshOMBOrderButton.Tag = unitNumber;
			refreshOMBOrderButton.Click += HandleRefreshOMBOrderClick;
			refreshOMBOrderButton.SetImageResource (Resource.Drawable.refresh_btn_25_Grey);

			var layout = new TableRow.LayoutParams(TableRow.LayoutParams.WrapContent, TableRow.LayoutParams.WrapContent); 
			layout.Gravity = GravityFlags.CenterVertical;

			refreshOMBOrderButton.Enabled = false;

			row.AddView (refreshOMBOrderButton, layout);

			var workOrderMessage = new TextView (this);
			
			if (obj [FieldMessage] != null)
				workOrderMessage.Text = (string)obj [FieldMessage];
			
			row.AddView (workOrderMessage);

			var messageButton = new TextView (this);
			if(!string.IsNullOrEmpty((string)obj [FieldIsUnitAttributeChanged])){
				messageButton.Text = GetString(Resource.String.attributeChangeText);
			}
			row.AddView (messageButton);
			
			var uploadWorkOrderButton = new Button (this);
			uploadWorkOrderButton.Tag = unitNumber;
			// if work order type is inBound, Outbound
			if (IsUnitOnRent(unitNumber) &&  IsUnitOffRent(unitNumber)) {

				uploadWorkOrderButton.Text = GetString (Resource.String.units_upload);

			}else {
				if (IsUnitOffRent(unitNumber) ) {
					uploadWorkOrderButton.Text = GetString (Resource.String.units_uploadOffRent);
				}  if (IsUnitOnRent(unitNumber)) {
					uploadWorkOrderButton.Text = GetString (Resource.String.units_uploadOnRent);
				}
			}
			//
			uploadWorkOrderButton.Enabled = ValidateWorkOrderCanBeUploaded (obj);
			uploadWorkOrderButton.Click += HandleUploadWorkOrderClick;
			row.AddView (uploadWorkOrderButton);



			var removeButton = new Button (this);
			removeButton.Text = GetString (Resource.String.workOrder_remove);
			removeButton.Click += delegate {
				RemoveRowWithAlert (unitNumber);
			};
			row.AddView (removeButton);

			// add row to table
			_unitsTable.AddView (row, _unitsTable.ChildCount - 1);

			// reset open work order text
			ResetOpenWorkOrderButtonTextOnRow (obj);

			// set work order
			SetErrorMessageOnRow (obj);

			// enable row
			EnableRow (obj);
			// disable row based on if there is an active upload
			DisableRowIfActiveUploadIsHappeningForWorkOrder (obj);
		}
	}
}
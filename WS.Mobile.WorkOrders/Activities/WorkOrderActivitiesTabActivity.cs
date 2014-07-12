using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using WS.Mobile.WorkOrders.Backend;
using WS.Mobile.WorkOrders.Model;
using Android.Util;
using Android.Content.PM;
using System.Threading.Tasks;
using System.Threading;
using Android.Views.InputMethods;

namespace WS.Mobile.WorkOrders.Activities
{
	[Activity]
	public class WorkOrderActivitiesTabActivity : AbstractTabItemActivity
	{
		private List<ActivityType> _activities;
		private List<ActivityType> _internalActivities;
		private List<ActivityType> _externalActivities;
		private TableLayout _activitiesTable;
		private RadioGroup _locationGroup;
		private RadioButton _interiorButton;
		private RadioButton _exteriorButton;
		private Spinner _typeSpinner;
		private EditText _qtyNumber;
		private EditText _damageBillingPercentageNumber;
		private RadioGroup _typeGroup;
		private RadioButton _addCheck;
		private RadioButton _removeCheck;
		private RadioButton _replaceCheck;
		private RadioButton _repairCheck;
		private RadioButton _serviceCheck;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.workOrderActivitiesTab);

			_activitiesTable = FindViewById<TableLayout>(Resource.Id.workOrderActivitiesTable);

			_locationGroup = FindViewById<RadioGroup>(Resource.Id.workOrderActivityLocationGroup);
			_interiorButton = FindViewById<RadioButton>(Resource.Id.workOrderActivityInteriorRadio);
			_exteriorButton = FindViewById<RadioButton>(Resource.Id.workOrderActivityExteriorRadio);
			_typeSpinner = FindViewById<Spinner>(Resource.Id.workOrderActivityTypeSpinner);
			_qtyNumber = FindViewById<EditText>(Resource.Id.workOrderQtyNumber);
			_damageBillingPercentageNumber = FindViewById<EditText>(Resource.Id.workOrderDamageBillingPercentageNumber);
			_typeGroup = FindViewById<RadioGroup>(Resource.Id.workOrderActivityTypeGroup);
			_addCheck = FindViewById<RadioButton>(Resource.Id.workOrderActivityAddCheck);
			_removeCheck = FindViewById<RadioButton>(Resource.Id.workOrderActivityRemoveCheck);
			_replaceCheck = FindViewById<RadioButton>(Resource.Id.workOrderActivityReplaceCheck);
			_repairCheck = FindViewById<RadioButton>(Resource.Id.workOrderActivityRepairCheck);
			_serviceCheck = FindViewById<RadioButton>(Resource.Id.workOrderActivityServiceCheck);

			_activities = CacheRepository.Activities;

			_internalActivities = _activities
				.Where(x => x.IsInternal)
				.OrderBy(x => x.Category)
				.ThenBy(x => x.Name)
				.ToList();

			_externalActivities = _activities
				.Where(x => x.IsExternal)
				.OrderBy(x => x.Category)
				.ThenBy(x => x.Name)
				.ToList();

			var firstActivity = _internalActivities.FirstOrDefault ();
			var firstActivityWorkType = firstActivity.WorkTypes;
			_addCheck.Enabled = firstActivityWorkType.Contains("ADD");
			_removeCheck.Enabled = firstActivityWorkType.Contains("REMOVE");
			_replaceCheck.Enabled = firstActivityWorkType.Contains("REMRPL");
			_repairCheck.Enabled = firstActivityWorkType.Contains("REPAIR");
			_serviceCheck.Enabled = firstActivityWorkType.Contains("SERV");

			_typeSpinner.Adapter = new ArrayAdapter<ActivityType>(this, Android.Resource.Layout.SimpleListItemSingleChoice, _internalActivities);

			if (LandscapeOrientation) {
				_typeSpinner.LayoutParameters.Width = 400;
				_qtyNumber.LayoutParameters.Width = 150;
			} else {
				_typeSpinner.LayoutParameters.Width = 275;
				_qtyNumber.LayoutParameters.Width = 75;
			}

			Button addButton = FindViewById<Button>(Resource.Id.workOrderActivityAddButton);
			addButton.Click += OnActivityAddClick;

			_qtyNumber.EditorAction += (sender, e) => {
				e.Handled = true;

				_damageBillingPercentageNumber.RequestFocusFromTouch();
			};

			_damageBillingPercentageNumber.EditorAction += (sender, e) => {
				e.Handled = true;

				if (_addCheck.Enabled) _addCheck.RequestFocusFromTouch();
				else if (_removeCheck.Enabled) _removeCheck.RequestFocusFromTouch();
				else if (_replaceCheck.Enabled) _replaceCheck.RequestFocusFromTouch();
				else if (_repairCheck.Enabled) _repairCheck.RequestFocusFromTouch();
				else if (_serviceCheck.Enabled) _serviceCheck.RequestFocusFromTouch();
			};

			_locationGroup.CheckedChange += HandleLocationChanged;
			_typeSpinner.ItemSelected += HandleTypeSelected;
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);

			outState.PutBooleanArray("ActivityTypeState", new[] {
				_addCheck.Enabled,
				_removeCheck.Enabled,
				_replaceCheck.Enabled,
				_repairCheck.Enabled,
				_serviceCheck.Enabled });
		}

		protected override void OnRestoreInstanceState (Bundle savedInstanceState)
		{
			base.OnRestoreInstanceState (savedInstanceState);

			var activityTypeState = savedInstanceState.GetBooleanArray("ActivityTypeState");

			_addCheck.Enabled = activityTypeState[0];
			_removeCheck.Enabled = activityTypeState[1];
			_replaceCheck.Enabled = activityTypeState[2];
			_repairCheck.Enabled = activityTypeState[3];
			_serviceCheck.Enabled = activityTypeState[4];
		}

		private void HandleLocationChanged (object sender, RadioGroup.CheckedChangeEventArgs e)
		{
			switch(e.CheckedId) {
				case Resource.Id.workOrderActivityExteriorRadio:
					_typeSpinner.Adapter = new ArrayAdapter<ActivityType>(this, Android.Resource.Layout.SimpleListItemSingleChoice, _externalActivities);
					break;
					
				case Resource.Id.workOrderActivityInteriorRadio:
					_typeSpinner.Adapter = new ArrayAdapter<ActivityType>(this, Android.Resource.Layout.SimpleListItemSingleChoice, _internalActivities);
					break;
			}
		}

		private void HandleTypeSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			var spinner = (Spinner)sender;
			var selectedItem = (ActivityType)spinner.GetItemAtPosition(e.Position);
			var workTypes = selectedItem.WorkTypes;
			
			_addCheck.Enabled = workTypes.Contains("ADD");
			_removeCheck.Enabled = workTypes.Contains("REMOVE");
			_replaceCheck.Enabled = workTypes.Contains("REMRPL");
			_repairCheck.Enabled = workTypes.Contains("REPAIR");
			_serviceCheck.Enabled = workTypes.Contains("SERV");

			_addCheck.Checked = false;
			_removeCheck.Checked = false;
			_replaceCheck.Checked = false;
			_repairCheck.Checked = false;
			_serviceCheck.Checked = false;
			
			_typeGroup.ClearCheck();

			_qtyNumber.RequestFocusFromTouch ();
			ShowKeyboard (_qtyNumber);
		}

		private void ResetForm ()
		{
			_qtyNumber.Text = "";
			_damageBillingPercentageNumber.Text = "";

			_addCheck.Checked = false;
			_removeCheck.Checked = false;
			_replaceCheck.Checked = false;
			_repairCheck.Checked = false;
			_serviceCheck.Checked = false;

			_typeGroup.ClearCheck();

			_typeSpinner.RequestFocusFromTouch();
		}
		
		private void OnActivityAddClick (object sender, EventArgs e)
		{
			var qty = ParseQty (_qtyNumber.Text);
			var damagePercentage = ParseDamagePercentage(_damageBillingPercentageNumber.Text);
			var workType = _addCheck.Checked ? "ADD" : 
				_removeCheck.Checked ? "REMOVE" : 
				_replaceCheck.Checked ? "REMRPL" : 
				_repairCheck.Checked ? "REPAIR" : 
				_serviceCheck.Checked ? "SERV" : 
				"";

			if (!ValidateQty(qty, supressAlert: false) || !ValidateDamanagePercentage(damagePercentage, supressAlert: false) || !ValidateActivityType(workType))
				return;

			var selectedActivity = (ActivityType)_typeSpinner.SelectedItem;
			var activity = (JsonObject)selectedActivity.Object.Clone();

			activity["ID"] = Guid.NewGuid().ToString();
			activity["Location"] = _interiorButton.Checked ? "I" : 
						_exteriorButton.Checked ? "E" : 
							"X";

			activity["ActivityID"] = selectedActivity.Id;
			activity["Activity"] = selectedActivity.Name;
			
			activity["Qty"] = qty;
			activity["DamageBillingPercentage"] = damagePercentage;
			
			activity["WorkType"] = workType;

			AddObjectToCollection(Global.FieldActivities, activity);
			AddRow(activity);
			
			ResetForm();
		}
		
		private void AddRow (JsonObject activity)
		{
			var row = new TableRow(this);
			var id = (string)activity["ID"];
			
			var locationText = new TextView(this);
			locationText.Text = (string)activity["Location"] == "I" ? GetString(Resource.String.workOrder_interior) : 
						(string)activity["Location"] == "E" ? GetString(Resource.String.workOrder_exterior) : 
							GetString(Resource.String.workOrder_unknown);
			row.AddView(locationText);
			
			var activityText = new TextView(this);
			activityText.Text = (string)activity["Activity"];
			row.AddView(activityText);
			
			var qtyText = new EditText(this);
			qtyText.ImeOptions = ImeAction.None;
			qtyText.InputType = Android.Text.InputTypes.ClassNumber;

			var qtyFilter = new IInputFilter[] { new InputFilterLengthFilter(9) };
			qtyText.SetFilters(qtyFilter);
			qtyText.Text = ((long)activity["Qty"]).ToString();
			qtyText.TextChanged += (sender, e) => {
				var text = new String(e.Text.ToArray());
				var qty = ParseQty(text);

				if (ValidateQty(qty, supressAlert: true))
					UpdateCollectionProperty(Global.FieldActivities, id, "Qty", qty);
			};
			qtyText.FocusChange += (sender, e) => {
				if (!e.HasFocus) {
					var control = (EditText)sender;
					var text = control.Text;
					var qty = ParseQty(text);

					if (!ValidateQty(qty, supressAlert: false))
						ForceFocus(control, "activities");
				}
			};
			qtyText.Gravity = GravityFlags.Right;
			row.AddView(qtyText);

			var dbText = new EditText(this);
			dbText.ImeOptions = ImeAction.None;
			dbText.InputType = Android.Text.InputTypes.ClassNumber;
			
			var dbFilter = new IInputFilter[] { new InputFilterLengthFilter(3) };
			var damageBilling = ((long)activity["DamageBillingPercentage"]).ToString();
			
			dbText.SetFilters(dbFilter);
			
			if (damageBilling == "0")
				dbText.Hint = "0";
			else 
				dbText.Text = damageBilling;

			dbText.TextChanged += (sender, e) => {
				var text = new String(e.Text.ToArray());
				var damagePercentage = ParseDamagePercentage(text);

				if (ValidateDamanagePercentage(damagePercentage, supressAlert: true))
					UpdateCollectionProperty(Global.FieldActivities, id, "DamageBillingPercentage", damagePercentage);
			};
			dbText.FocusChange += (sender, e) => {
				if (!e.HasFocus) {
					var control = (EditText)sender;
					var text = control.Text;
					var damagePercentage = ParseDamagePercentage(text);

					if (!ValidateDamanagePercentage(damagePercentage, supressAlert: false))
						ForceFocus(control, "activities");
				}
			};
			dbText.Gravity = GravityFlags.Right;
			row.AddView(dbText);
			
			var workTypeText = new TextView(this);
			workTypeText.Text = (string)activity["WorkType"] == "ADD" ? GetString(Resource.String.workOrder_add) : 
						(string)activity["WorkType"] == "REMOVE" ? GetString(Resource.String.workOrder_remove) : 
						(string)activity["WorkType"] == "REMRPL" ? GetString(Resource.String.workOrder_replace) : 
						(string)activity["WorkType"] == "REPAIR" ? GetString(Resource.String.workOrder_repair) : 
						(string)activity["WorkType"] == "SERV" ? GetString(Resource.String.workOrder_service) : 
						GetString(Resource.String.workOrder_unknown);
			row.AddView(workTypeText);
			
			var removeButton = new Button(this);
			removeButton.Text = GetString(Resource.String.workOrder_remove);
			removeButton.Click += delegate {
				RemoveRow(id, row);
			};
			row.AddView(removeButton);

			_activitiesTable.AddView(row);
		}

		private void RemoveRow (string id, TableRow row)
		{
			RemoveObjectFromCollection(Global.FieldActivities, id);

			RunOnUiThread(delegate {
				_activitiesTable.RemoveView(row);
			});
		}

		protected override void LoadChanges ()
		{
			var unit = GetActiveWorkOrder();

			// remove all rows except the header
			while(_activitiesTable.ChildCount > 4)
				_activitiesTable.RemoveViewAt(4);

			var activities = (List<object>)unit[Global.FieldActivities];
			if (activities != null) {
				foreach (var activity in activities)
					AddRow((JsonObject)activity);
			}
		}
	}
}
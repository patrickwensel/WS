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
using System.Threading;
using Android.Views.InputMethods;

namespace WS.Mobile.WorkOrders.Activities
{
	[Activity]
	public class WorkOrderPartsTabActivity : AbstractTabItemActivity
	{
		private List<PartType> _parts;
		private TableLayout _partsTable;
		private Spinner _typeSpinner;
		private EditText _qtyNumber;
		private EditText _damageBillingPercentageNumber;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate(bundle);
			
			SetContentView(Resource.Layout.workOrderPartsTab);

			_partsTable = FindViewById<TableLayout>(Resource.Id.workOrderPartsTable);
			
			_typeSpinner = FindViewById<Spinner>(Resource.Id.workOrderPartsTypeSpinner);
			_qtyNumber = FindViewById<EditText>(Resource.Id.workOrderQtyNumber);
			_damageBillingPercentageNumber = FindViewById<EditText>(Resource.Id.workOrderDamageBillingPercentageNumber);
			
			_parts = CacheRepository.Parts;
			
			_typeSpinner.Adapter = new ArrayAdapter<PartType>(this, Android.Resource.Layout.SimpleListItemSingleChoice, _parts);
			
			Button addButton = FindViewById<Button>(Resource.Id.workOrderPartsAddButton);
			addButton.Click += OnPartAddClick;
			
			if (LandscapeOrientation) {
				_qtyNumber.LayoutParameters.Width = 150;
			} else {
				_qtyNumber.LayoutParameters.Width = 75;
			}

			_qtyNumber.EditorAction += (sender, e) => {
				e.Handled = true;
				_damageBillingPercentageNumber.RequestFocusFromTouch();
			};
			
			_damageBillingPercentageNumber.EditorAction += (sender, e) => {
				e.Handled = true;
				if (e.ActionId == Android.Views.InputMethods.ImeAction.Done) {
					FindViewById<Button>(Resource.Id.workOrderPartsAddButton).PerformClick();
				}
			};

			ThreadPool.QueueUserWorkItem (state => {
				Thread.Sleep(TimeSpan.FromMilliseconds(100));
				RunOnUiThread(delegate {
					_typeSpinner.ItemSelected += TypeItemSelected;
					_typeSpinner.RequestFocusFromTouch();
				});
			});
		}
		
		private void TypeItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			_qtyNumber.RequestFocusFromTouch ();
			ShowKeyboard (_qtyNumber);
		}
		
		private void ResetForm ()
		{
			_qtyNumber.Text = "";
			_damageBillingPercentageNumber.Text = "";
			
			_typeSpinner.RequestFocusFromTouch();
		}
		
		private void OnPartAddClick (object sender, EventArgs e)
		{
			var qty = ParseQty(_qtyNumber.Text);
			var damagePercentage = ParseDamagePercentage(_damageBillingPercentageNumber.Text);
			
			if (!ValidateQty(qty, supressAlert: false) || !ValidateDamanagePercentage(damagePercentage, supressAlert: false))
				return;

			var selectedPart = (PartType)_typeSpinner.SelectedItem;
			var part = (JsonObject)selectedPart.Object.Clone();
			
			part["ID"] = Guid.NewGuid().ToString();

			part["PartID"] = selectedPart.Id;
			part["Part"] = selectedPart.Name;
			
			part["Qty"] = qty;
			part["DamageBillingPercentage"] = damagePercentage;

			AddObjectToCollection(Global.FieldParts, part);
			AddRow(part);
			
			ResetForm();
		}
		
		private void AddRow (JsonObject part)
		{
			var row = new TableRow(this);
			var id = (string)part["ID"];
			
			var partText = new TextView(this);
			partText.Text = (string)part["Part"];
			row.AddView(partText);
			
			var qtyText = new EditText(this);
			qtyText.ImeOptions = ImeAction.None;
			qtyText.InputType = Android.Text.InputTypes.ClassNumber;

			var qtyFilter = new IInputFilter[] { new InputFilterLengthFilter(9) };
			qtyText.SetFilters(qtyFilter);
			qtyText.Text = ((long)part["Qty"]).ToString();
			qtyText.TextChanged += (sender, e) => {
				var text = new String(e.Text.ToArray());
				var qty = ParseQty(text);
				
				if (ValidateQty(qty, supressAlert: true))
					UpdateCollectionProperty(Global.FieldParts, id, "Qty", qty);
			};
			qtyText.FocusChange += (sender, e) => {
				if (!e.HasFocus) {
					var control = (EditText)sender;
					var text = control.Text;
					var qty = ParseQty(text);
					
					if (!ValidateQty(qty, supressAlert: false))
						ForceFocus(control, "parts");
				}
			};
			qtyText.Gravity = GravityFlags.Right;
			row.AddView(qtyText);
			
			var dbText = new EditText(this);
			dbText.ImeOptions = ImeAction.None;
			dbText.InputType = Android.Text.InputTypes.ClassNumber;

			var dbFilter = new IInputFilter[] { new InputFilterLengthFilter(3) };
			var damageBilling = ((long)part["DamageBillingPercentage"]).ToString();

			dbText.SetFilters(dbFilter);

			if (damageBilling == "0")
				dbText.Hint = "0";
			else 
				dbText.Text = damageBilling;

			dbText.TextChanged += (sender, e) => {
				var text = new String(e.Text.ToArray());
				var damagePercentage = ParseDamagePercentage(text);
				
				if (ValidateDamanagePercentage(damagePercentage, supressAlert: true))
					UpdateCollectionProperty(Global.FieldParts, id, "DamageBillingPercentage", damagePercentage);
			};
			dbText.FocusChange += (sender, e) => {
				if (!e.HasFocus) {
					var control = (EditText)sender;
					var text = control.Text;
					var damagePercentage = ParseDamagePercentage(text);
					
					if (!ValidateDamanagePercentage(damagePercentage, supressAlert: false))
						ForceFocus(control, "parts");
				}
			};
			dbText.Gravity = GravityFlags.Right;
			row.AddView(dbText);

			row.AddView (new TextView (this));

			var removeButton = new Button(this);
			removeButton.Text = GetString(Resource.String.workOrder_remove);
			removeButton.Click += delegate {
				RemoveRow(id, row);
			};
			row.AddView(removeButton);
			
			_partsTable.AddView(row);
		}
		
		private void RemoveRow (string id, TableRow row)
		{
			RemoveObjectFromCollection(Global.FieldParts, id);
			
			RunOnUiThread(delegate {
				_partsTable.RemoveView(row);
			});
		}

		protected override void LoadChanges ()
		{
			var unit = GetActiveWorkOrder();

			// remove all rows except the header
			while(_partsTable.ChildCount > 4)
				_partsTable.RemoveViewAt(4);

			var parts = (List<object>)unit[Global.FieldParts];
			if (parts != null) {
				foreach (var part in parts)
					AddRow((JsonObject)part);
			}
		}
	}
}
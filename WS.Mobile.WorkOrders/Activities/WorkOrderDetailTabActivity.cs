using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
using WS.Mobile.WorkOrders.Backend;
using WS.Mobile.WorkOrders.Model;
using Android.Content.PM;

namespace WS.Mobile.WorkOrders.Activities
{
	[Activity]
	public class WorkOrderDetailTabActivity : AbstractTabItemActivity
	{
		private const string Tag = "wo-activity";
		private string _type;
		private List<Branch> _branches;
		private List<WorkOrderType> _workOrderTypes;
		private TextView _branchName;
		private EditText _additionalWorkText;
		private Spinner _workOrderTypeSpinner;
		private Spinner _workOrderRevenueBranchSpinner;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.workOrderDetailTab);
			_type =Intent.GetStringExtra ("type");

			_branchName = FindViewById<TextView>(Resource.Id.branchName);

			_additionalWorkText = FindViewById<EditText>(Resource.Id.additionalWorkText);
			_additionalWorkText.TextChanged += (sender, e) => {
				var text = new String(e.Text.ToArray());
				UpdateProperty(FieldAdditionalWork, text);
				// This is a hack to set the current tab to Unit Attributes. Instantiation of Other activities seems to require 
				// WorkOrderDetailTabActivity. for example when rotating the device while on any tab.
				if (_type == Global.FieldOnRentType) {
					WorkOrderActivity ta = (WorkOrderActivity) this.Parent;
					ta.SwitchTab ("attributes");
				}
			};

			_workOrderTypes = CacheRepository.WorkOrderTypes;

			_workOrderTypeSpinner = FindViewById<Spinner>(Resource.Id.workOrderTypeSpinner);
			_workOrderTypeSpinner.Adapter = new ArrayAdapter<WorkOrderType>(this, Android.Resource.Layout.SimpleListItemSingleChoice, _workOrderTypes);
			_workOrderTypeSpinner.ItemSelected += (sender, e) => UpdateProperty(FieldWorkOrderType, ((WorkOrderType)((Spinner)sender).GetItemAtPosition(e.Position)).Id);

			_branches = CacheRepository.Branches;

			_workOrderRevenueBranchSpinner = FindViewById<Spinner>(Resource.Id.workOrderRevenueBranchSpinner);
			_workOrderRevenueBranchSpinner.Adapter = new ArrayAdapter<Branch>(this, Android.Resource.Layout.SimpleListItemSingleChoice, _branches);
			_workOrderRevenueBranchSpinner.ItemSelected += (sender, e) => {
				var b = (Branch)((Spinner)sender).GetItemAtPosition(e.Position);

				UpdateProperty(FieldRevenueBranch, b.Id);
				UpdateProperty(FieldCompany, b.Company);
			};


		}

		private const string FieldRevenueBranch = "RevenueBranch";
		private const string FieldCompany = "Company";
		private const string FieldWorkOrderType = "UserDefinedCodeID";
		private const string FieldAdditionalWork = "AdditionalWork";

		protected override void LoadChanges ()
		{
			_branchName.Text = CacheRepository.LoggedInBranchName;

			var unit = GetActiveWorkOrder();

			if (unit[FieldRevenueBranch] == null)
				unit[FieldRevenueBranch] = CacheRepository.LoggedInBranchId;

			if (unit[FieldWorkOrderType] == null)
				unit[FieldWorkOrderType] = "G";

			if (unit[FieldAdditionalWork] == null)
				unit[FieldAdditionalWork] = "";

			if (unit["MobileWorkOrderImages"] == null)
				unit["MobileWorkOrderImages"] = new List<JsonObject>();

			if (unit["MobileWorkOrderParts"] == null)
				unit["MobileWorkOrderParts"] = new List<JsonObject>();

			if (unit["MobileWorkOrderActivities"] == null)
				unit["MobileWorkOrderActivities"] = new List<JsonObject>();

				UpdateActiveWorkOrder(unit);

			_additionalWorkText.Text = (string)GetProperty(FieldAdditionalWork);
			
			var fieldRevenueBranch = (string)GetProperty(FieldRevenueBranch);
			var workOrderType = (string)GetProperty(FieldWorkOrderType);

			_workOrderRevenueBranchSpinner.SetSelection(_branches.FindIndex(0, b => b.Id == fieldRevenueBranch));
			_workOrderTypeSpinner.SetSelection(_workOrderTypes.FindIndex(0, t => t.Id == workOrderType));
		}
	}
}
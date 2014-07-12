using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
using WS.Mobile.WorkOrders.Backend;
using Android.Content.PM;
using WS.Mobile.WorkOrders.Model ;
using Android.Text;
using Android.Util;

namespace WS.Mobile.WorkOrders.Activities
{
	[Activity]
	public class OutBoundWorkOrderUnitAttributesTabActivity :  AbstractTabItemActivity
	{
		private List<AttributeCategory> unitCategories ;
		List<string> category;
		private int rowCount=0;
		private const int ColumnCategoryInput =0;
		private const int ColumnAttributeInput =1;
		private const int ColumnItemDescInput =2;
		private const int ColumnDescriptionInput =3;
		private JsonObject unit ;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.workOrderUnitAttributesTab);
			DataInitialization ();

		}
		private void DataInitialization()
		{
			unit = GetActiveWorkOrder();
			LoadChanges();
			unitCategories= CacheRepository.AttributeCategories;
			category= unitCategories.Select( c=> c.Category).ToList().Distinct().ToList();
			category.Add ("");
			category.Sort ();
			FindViewById<Button> (Resource.Id.attributeAddButton).Enabled=false;
			var attributeEditButton = FindViewById<Button> (Resource.Id.attributeEditButton);
			attributeEditButton.Click += (object sender, EventArgs e) => {
				EnableControlsForEdit();
				attributeEditButton.Enabled=false;
			};
		}
		private void EnableControlsForEdit()
		{
			var description = FindViewById<EditText> (Resource.Id.workOrderDescriptionLabel);
			var description2 = FindViewById<EditText> (Resource.Id.workOrderDescriptionLabel2);
			description.Enabled = true;
			description2.Enabled = true;
			string description1Value = description.Text;
			string description2Value = description2.Text;
			description.AfterTextChanged += (sender, e) => {
				if(description1Value != description.Text) {
					UpdateProperty ( "Description1", description.Text);
					UpdateProperty ( "IsUnitAttributeChanged", "1");
				}

			};
			description2.AfterTextChanged += (sender, e) => {
				if(description2Value != description2.Text) {
					UpdateProperty ("Description2", description2.Text);
					UpdateProperty ( "IsUnitAttributeChanged", "1");
				}
			};
			var xButtonAdd = FindViewById<Button> (Resource.Id.attributeAddButton);
			xButtonAdd.Enabled = true;
			xButtonAdd.Text = GetString(Resource.String.attributeAddButtonText);
			xButtonAdd.Click += HandleAddUnitAttributeClick;

			RunOnUiThread (delegate {
				var table = FindViewById<TableLayout>(Resource.Id.workOrderDescriptionTable);
				var attributes = ((IEnumerable<object>)unit["UnitAttributes"]).Cast<JsonObject>().OrderBy(a=>a["UserDefinedCodeTypeDescription"]).ThenBy(a=> a["UserDefinedCodeDescription"]);
				table.RemoveViews (1, attributes.Count() );
				int count = 1;
				foreach (var attr in attributes) {
					var row =AddRows(attr,count,category,unitCategories);
					table.AddView(row);
					count++;
					rowCount=count;
				}

			});
		}
		private int ResetColor(int count)
		{
			if (count % 2 == 0) {
				return Resource.Color.Black;
			} else {
				return Resource.Color.Gray;
			}
		}
		private TableRow AddRows (JsonObject attr, int count, List<string> category,List<AttributeCategory> unitCategories) {
			var row = new TableRow(this);

			row.SetBackgroundResource(ResetColor(count));
			string uniquIdentifier;
			var id = (string)attr["ID"];						
			if (attr ["ID"] == null) {
				id= Guid.NewGuid ().ToString();
				attr ["ID"] = id;
				AddObjectToCollection(Global.FieldUnitAttributes, attr);
			}
			uniquIdentifier = id;
			var categoryDropdown = new Spinner(this);
			categoryDropdown.Adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleSpinnerDropDownItem,category );

			var categoryValue = (string)attr["UserDefinedCodeTypeDescription"];
			var catbc = categoryDropdown.Background;
			if (!String.IsNullOrEmpty (categoryValue)) {
				categoryDropdown.SetSelection (category.FindIndex (0, o => o.Equals (categoryValue.Trim ())));
				categoryDropdown.SetBackgroundDrawable(catbc);
			} else {
				categoryDropdown.SetBackgroundResource (Resource.Color.SeaShell);
			}

			categoryDropdown.ItemSelected += (sender, e) => {
				if(string.IsNullOrEmpty(categoryDropdown.GetItemAtPosition(e.Position).ToString())) {categoryDropdown.SetBackgroundResource(Resource.Color.SeaShell);}else {categoryDropdown.SetBackgroundDrawable(catbc);}
				if (categoryValue!=categoryDropdown.GetItemAtPosition(e.Position).ToString()) {
				categoryValue = categoryDropdown.GetItemAtPosition(e.Position).ToString();
				UpdateCollectionProperty(Global.FieldUnitAttributes, id, "UserDefinedCodeTypeDescription", string.IsNullOrEmpty(categoryValue)? "": categoryValue);
				 }
				//Populate the Item Desc dropdown
				RunOnUiThread (delegate {
					var populateItemDescDropdown = (Spinner)row.GetChildAt (ColumnItemDescInput);
					List<string> itemDescsList =unitCategories.Where( c=> c.Category== categoryValue).Select( o => o.ItemDescription).Distinct().ToList();
					itemDescsList.Add("");
					itemDescsList.Sort();
					PopulateAndSelectDropDown(populateItemDescDropdown,itemDescsList,(string)attr["UserDefinedCodeDescription"]);

					});
			};
			row.AddView(categoryDropdown);

			var text2 = new TextView(this);
			text2.Text = (string)attr["UserDefinedCodeID"];
			row.AddView(text2);

			var itemDescDropdown = new Spinner(this);
			categoryValue = string.IsNullOrEmpty (categoryValue) ? string.Empty : categoryValue.Trim ();
			List<string> itemDescs =unitCategories.Where( c=> c.Category== categoryValue).Select( o => o.ItemDescription).Distinct().ToList();
			string attValue = (string)attr ["UserDefinedCodeDescription"];
			PopulateAndSelectDropDown (itemDescDropdown, itemDescs, attValue);
			var itembc = itemDescDropdown.Background;
			itemDescDropdown.ItemSelected += (sender, e) => {
				if(attValue !=itemDescDropdown.GetItemAtPosition(e.Position).ToString()) {
					    attValue= itemDescDropdown.GetItemAtPosition(e.Position).ToString();
						if(string.IsNullOrEmpty(attValue)) {itemDescDropdown.SetBackgroundResource(Resource.Color.SeaShell);
							text2.Text="";
						UpdateCollectionProperty(Global.FieldUnitAttributes, id, "UserDefinedCodeID", "");
						;}else {itemDescDropdown.SetBackgroundDrawable(itembc) ;

						//uniquIdentifier = GetUniqueId(unitCategories.Where( c=> c.Category== categoryValue && c.ItemDescription==attValue).FirstOrDefault());
						//if(ValidateUniqueUnitAttributeItem(id,uniquIdentifier)){
							UpdateCollectionProperty(Global.FieldUnitAttributes, id, "UserDefinedCodeDescription", string.IsNullOrEmpty(attValue)? "": attValue);
							UpdateProperty ( "IsUnitAttributeChanged", "1");
							if(id !=uniquIdentifier){
							//UpdateCollectionProperty(Global.FieldUnitAttributes, id, "ID", uniquIdentifier);
								} 
				                //update the Attribute column
								RunOnUiThread (delegate {

									var updateAttributeText =(TextView)row.GetChildAt (ColumnAttributeInput);
									AttributeCategory att = unitCategories.Where( c=> c.Category== categoryValue).Where(d =>
									d.ItemDescription == attValue).FirstOrDefault();
									if(att== null){ updateAttributeText.Text="";UpdateCollectionProperty(Global.FieldUnitAttributes, id, "UserDefinedCodeID", "");} 
									else {updateAttributeText.Text =att.Attribute;UpdateCollectionProperty(Global.FieldUnitAttributes, id, "UserDefinedCodeID", att.Attribute);
										UpdateCollectionProperty(Global.FieldUnitAttributes, id, "ProductCode",att.ProductCode );
										UpdateCollectionProperty(Global.FieldUnitAttributes, id, "UserDefinedCodeTypeID",att.Description );}

									});
							//} 
					//else {itemDescDropdown.SetBackgroundResource(Resource.Color.SeaShell);}
					 }

				}
				};

			row.AddView(itemDescDropdown);

			var text4 = new EditText(this);
			var txtFilter = new IInputFilter[] { new InputFilterLengthFilter(300) };
			text4.SetFilters(txtFilter);
					 
			text4.SetMaxLines (6);
			text4.SetMaxEms (8);

			string descriptionValue = string.IsNullOrEmpty((string)attr["FleetAttributeDescription"])?"":(string)attr["FleetAttributeDescription"];
			//update the changed value
			text4.AfterTextChanged += (sender, e) => {
			string text4Value = string.IsNullOrEmpty (text4.Text) ? "" : text4.Text;
				if (text4Value != descriptionValue){
					UpdateCollectionProperty(Global.FieldUnitAttributes, id, "FleetAttributeDescription", text4.Text);
				UpdateProperty ( "IsUnitAttributeChanged", "1");
				}
			};
			text4.Text = (string)attr["FleetAttributeDescription"];	
			row.AddView(text4);

			var xButtonDelete = new Button(this);
			xButtonDelete.Id = count;
			xButtonDelete.Tag = id;
			xButtonDelete.Text = GetString(Resource.String.attributeDeleteButtonText);
			xButtonDelete.Click += HandleDeleteUnitAttributeClick;
			row.AddView (xButtonDelete);

			row.Id = count;
			return row;
		}
		private string GetUniqueId(AttributeCategory attributeCategory)
		{
			if (attributeCategory != null) {
				return attributeCategory.ProductCode.Trim() + "|" + attributeCategory.Description.Trim() + "|" + attributeCategory.Attribute.Trim();
			} else {
				return string.Empty; 
			}
		}
		private void PopulateAndSelectDropDown( Spinner nameOfDropDown, List<string> listItems, string itemSelected )
		{
			nameOfDropDown.Adapter = new ArrayAdapter<string> (this,Android.Resource.Layout.SimpleSpinnerDropDownItem , listItems);
			itemSelected= string.IsNullOrEmpty (itemSelected) ? string.Empty : itemSelected.Trim ();
			nameOfDropDown.SetSelection(listItems.FindIndex (0,o => o.Equals(itemSelected)));
		}
		private void HandleAddUnitAttributeClick  (object sender, EventArgs e)
		{
			var control = (Button)sender;
			//var id = control.Id;
			AddUnitAttribute (rowCount,category,unitCategories);
		}
		private void AddUnitAttribute(int id,List<string> category,List<AttributeCategory> unitCategories )
		{
			var table = FindViewById<TableLayout>(Resource.Id.workOrderDescriptionTable);
			var row =AddRows(new JsonObject(),id+1,category,unitCategories);
			table.AddView(row,1);
			rowCount++;
		}

		private void HandleDeleteUnitAttributeClick  (object sender, EventArgs e)
		{
			var control = (Button)sender;
			var id = control.Id;
			var tag = (string)control.Tag;
			DeleteRowWithAlert (id, tag);
		}
		private void DeleteRowWithAlert (int id, string tag)
		{
			var builder = new AlertDialog.Builder (this);
			builder.SetTitle ("Remove " + id);
			builder.SetMessage ("Are you sure you want to remove this attribute value and associated information from the device?");
			builder.SetPositiveButton ("Yes", delegate {
				// first delete from the unit attribute and then from the screen
				RemoveObjectFromCollection(Global.FieldUnitAttributes, tag);
				UpdateProperty ( "IsUnitAttributeChanged", "1");
				RunOnUiThread(delegate {
				DeleteUnitAttribute (id);
				});
			});
			builder.SetNegativeButton ("No", NullClick);
			builder.Show ();
		}
		private void DeleteUnitAttribute(int id)
		{
			var table = FindViewById<TableLayout>(Resource.Id.workOrderDescriptionTable);
			TableRow rowDelete = (TableRow)table.FindViewById(id);
			table.RemoveView (rowDelete);
			if (id +1 < rowCount) {
				// adjust the control Id and row Id after the deleted row
				do {
					TableRow row = (TableRow)table.FindViewById(id+1);
					if(row==null) break;
					Button xButton=	(Button)row.GetChildAt(4);
					xButton.Id= id;
					row.Id=id;
					id++;
				} while(id +1 < rowCount);
			}

			rowCount--;
		}
		protected override void OnResume ()
		{
			//always supress load changes, this gets called onCreate
			SupressLoadChanges = true;
			base.OnResume ();
		}

		protected override void LoadChanges ()
		{

			FindViewById<TextView>(Resource.Id.workOrderSerialNumberLabel).Text = unit["UnitNumber"].ToString();
			FindViewById<TextView>(Resource.Id.workOrderBranchNumberLabel).Text = unit["BusinessUnit"].ToString();
			FindViewById<TextView>(Resource.Id.workOrderDateAcquiredLabel).Text = DateTime.Parse(unit["DateAcquired"].ToString()).ToString("MM/dd/yyyy");
			FindViewById<TextView>(Resource.Id.workOrderManufacturerSerialNumberLabel).Text = unit["SerialNumber"].ToString();
			FindViewById<TextView>(Resource.Id.workOrderEquipmentClassLabel).Text = unit["EquipmentClass"].ToString();
			FindViewById<TextView>(Resource.Id.workOrderManufacturerLabel).Text = unit["Manufacturer"].ToString();
			FindViewById<TextView>(Resource.Id.workOrderModelYearLabel).Text = unit["ModelYear"].ToString();
			FindViewById<TextView>(Resource.Id.workOrderBoxLengthLabel).Text = unit["BoxLength"].ToString();
			FindViewById<TextView>(Resource.Id.workOrderSquareFootageLabel).Text = unit["SquareFootage"].ToString();
			FindViewById<TextView>(Resource.Id.workOrderBoxWidthLabel).Text = unit["BoxWidth"].ToString();
			FindViewById<EditText> (Resource.Id.workOrderDescriptionLabel).Text = GetProperty (unit ["UnitNumber"].ToString (), "Description1").ToString(); //unit["Description1"].ToString();
			FindViewById<EditText> (Resource.Id.workOrderDescriptionLabel).Enabled = false;
			FindViewById<EditText> (Resource.Id.workOrderDescriptionLabel2).Text = GetProperty (unit ["UnitNumber"].ToString (), "Description2").ToString();// unit["Description2"].ToString();
			FindViewById<EditText> (Resource.Id.workOrderDescriptionLabel2).Enabled = false;
			FindViewById<TextView>(Resource.Id.workOrderTotalLengthLabel).Text = unit["TotalLength"].ToString();

			var table = FindViewById<TableLayout>(Resource.Id.workOrderDescriptionTable);
			var attributes = ((IEnumerable<object>)unit[Global.FieldUnitAttributes]).Cast<JsonObject>().OrderBy(a=>a["UserDefinedCodeTypeDescription"]).ThenBy(a=> a["UserDefinedCodeDescription"]);

			int count = 0;
			foreach (var attr in attributes) {
				var row = new TableRow(this);

				if (count % 2 == 0) {
					row.SetBackgroundResource(Resource.Color.Black);
				} else {
					row.SetBackgroundResource(Resource.Color.Gray);
				}	
						
				var text1 = new TextView(this);
				text1.Text = (string)attr["UserDefinedCodeTypeDescription"];
				row.AddView(text1);

				var text2 = new TextView(this);
				text2.Text = (string)attr["UserDefinedCodeID"];
				row.AddView(text2);

				var text3 = new TextView(this);
				text3.Text = (string)attr["UserDefinedCodeDescription"];
				row.AddView(text3);

				var text4 = new TextView(this);
				text4.Text = (string)attr["FleetAttributeDescription"];
				text4.SetMaxLines (6);
				text4.SetMaxEms (8);
				row.AddView(text4);

				var text5 = new TextView(this);
				text5.Text = "";
				row.AddView(text5);	

				table.AddView(row);
				count++;
			}
		}
	}
}
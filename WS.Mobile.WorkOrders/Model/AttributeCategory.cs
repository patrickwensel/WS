using System;

namespace WS.Mobile.WorkOrders.Model
{
	public class AttributeCategory : Java.Lang.Object
	{ 
		public AttributeCategory ( string id, string productCode,string category, string attribute, string itemDescription, string description)
		{
			ID = id;
			ProductCode = string.IsNullOrEmpty(productCode)? "" : productCode.Trim ();
			Category = string.IsNullOrEmpty(category) ? "" : category.Trim();
			Attribute = string.IsNullOrEmpty(attribute)? "" : attribute.Trim();
			ItemDescription = string.IsNullOrEmpty(itemDescription)? "": itemDescription.Trim();
			Description = string.IsNullOrEmpty(description)?"": description.Trim();
		}
		public string ID { get; private set; }
		public string ProductCode { get; private set; }
		public string Category { get; private set; }
		public string Attribute { get; private set; }
		public string ItemDescription { get; private set; }
		public string Description { get; private set; }

		public override string ToString()
		{
			return Category;
		}
	}
}

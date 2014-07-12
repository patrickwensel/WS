using System;
using System.Collections.Generic;
using System.Linq;
using WS.Mobile.WorkOrders.Backend;

namespace WS.Mobile.WorkOrders.Model
{
	public class UnitAttribute : Java.Lang.Object
	{ 
		public UnitAttribute (string id, string productCode , string category, string attribute, string itemDesc, string description , JsonObject obj)
		{
			Id = id;
			ProductCode  = productCode;
			Category = category.Trim();
			AttributeCode = attribute.Trim();
			ItemDesc = itemDesc.Trim();
			Description = description.Trim ();
			Object = obj;
		}

		public string Id { get; private set; }
		public string ProductCode { get; private set; }
		public string Category { get; private set; }
		public string AttributeCode { get; private set; }
		public string ItemDesc { get; private set; }
		public string Description { get; private set; }
		public JsonObject Object { get; private set; }
		public override string ToString()
		{
			return Category;
		}
	}
}

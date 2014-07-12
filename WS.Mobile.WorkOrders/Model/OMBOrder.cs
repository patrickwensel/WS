using System;
using System.Linq;
using WS.Mobile.WorkOrders.Backend;

namespace WS.Mobile.WorkOrders.Model
{
	public class OMBOrder : Java.Lang.Object
	{ 
		public OMBOrder(long id,string orderNumber, string abalph, string quequeStatus,string memberDescription, JsonObject obj)
		{
			Id = id;
			OrderNumber = orderNumber;
			Abalph = abalph.Trim();
			QuequeStatus = quequeStatus.Trim ();
			MemberDescription = memberDescription.Trim ();
			Object = obj;
		}

		public long Id { get; private set; }
		public string OrderNumber { get; private set; }
		public string Abalph { get; private set; }
		public string QuequeStatus { get; private set; }
		public string MemberDescription { get; private set; }

		public JsonObject Object { get; private set; }

		public override string ToString()
		{
			return OrderNumber + " - " + Abalph + " - " + QuequeStatus + " - " + MemberDescription ; 
		}
	}
}
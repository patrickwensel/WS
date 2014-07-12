using System;

namespace WS.Mobile.WorkOrders
{
	public static class Global
	{
		public static readonly string[] Servers = new[] { "net1dev.willscot.com", "net1py.willscot.com", "net1.willscot.com" };

		public const string RestSuccessName = "__success";
		public const string RestErrorName = "__errorStatus";

		public const string FieldWorkOrderId = "WorkOrderID";
		public const string FieldActivities = "MobileWorkOrderActivities";
		public const string FieldParts = "MobileWorkOrderParts";
		public const string FieldImages = "MobileWorkOrderImages";
		public const string FieldUnitNumber = "UnitNumber";
		public const string FieldOffRentType = "Inbound";
		public const string FieldOnRentType = "Outbound";
		public const string FieldUnitAttributes = "UnitAttributes";
		public const string FieldVersion = "VersionNumber";
		public const string SettingSelectedServer = "SelectedServer";
		#if PRODUCTION
		public const string SettingSelectedServerDefault = "net1.willscot.com";
		#elif TESTING
		public const string SettingSelectedServerDefault = "net1py.willscot.com";
		#else
		public const string SettingSelectedServerDefault = "net1dev.willscot.com";
		#endif
		public const string ActiveWorkOrderObject = "ActiveWorkOrderObject";
	}
}
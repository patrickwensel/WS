using System;
using System.Collections.Generic;
using Android.Content;
using Android.Database.Sqlite;
using Android.Util;
using Java.Util;
using WS.Mobile.WorkOrders.Backend;

namespace WS.Mobile.WorkOrders.Data
{
	public class WorkOrderRepository : SQLiteOpenHelper
	{
		private const string Tag = "WorkOrdersTable";

		private const string DatabaseTable = "WorkOrders";
		private const int DatabaseVersion = 1;

		public WorkOrderRepository (Context context)
			: base(context, DatabaseTable, null, DatabaseVersion)
		{
		}

		public override void OnCreate (SQLiteDatabase db)
		{
			db.ExecSQL("CREATE TABLE WorkOrders (UnitNumber TEXT PRIMARY KEY, Object TEXT, AddedDate BIGINT);");
		}
		
		public override void OnUpgrade (SQLiteDatabase db, int oldVersion, int newVersion)
		{

		}

		public IEnumerable<JsonObject> GetAll ()
		{
			var workOrderCursor = ReadableDatabase.Query("WorkOrders", new[] { "Object" }, null, null, null, null, "AddedDate");

			while (workOrderCursor.MoveToNext()) {
				yield return new JsonObject(workOrderCursor.GetString(0));
			}
		}
		
		public JsonObject Get (string unitNumber)
		{
			var workOrderCursor = ReadableDatabase.Query("WorkOrders", new[] { "Object" }, "UnitNumber = ?", new[] { unitNumber }, null, null, null);
			var obj = (JsonObject)null;

			if (workOrderCursor.MoveToFirst())
				obj = new JsonObject(workOrderCursor.GetString(0));

			if (obj == null)
				Log.Error(Tag, "record not found for " + unitNumber);

			return obj;
		}

		public bool Exists(string unitNumber) {
			using (var cursor = ReadableDatabase.RawQuery("select 1 from WorkOrders where UnitNumber = ?", new[] { unitNumber })) {
				var exists = cursor.Count > 0;
				cursor.Close();
				return exists;
			}
		}
		
		public void Delete (string unitNumber)
		{
			WritableDatabase.Delete("WorkOrders", "UnitNumber = ?", new[] { unitNumber });
		}
		
		public void Save (string unitNumber, JsonObject obj)
		{
			var values = new ContentValues();
			values.Put("Object", obj.ToString());

			if (!Exists(unitNumber)) {
				values.Put("UnitNumber", unitNumber);
				values.Put("AddedDate", new Date().Time);
				WritableDatabase.Insert("WorkOrders", null, values);
			} else {
				WritableDatabase.Update("WorkOrders", values, "UnitNumber = ?", new[] { unitNumber });
			}
		}



	}
}


using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Database.Sqlite;
using WS.Mobile.WorkOrders.Backend;
using WS.Mobile.WorkOrders.Model;
using Android.Util;
using Java.Lang;

namespace WS.Mobile.WorkOrders.Data
{
	public class CacheRepository : SQLiteOpenHelper
	{
		private const string Tag = "CacheTable";
		private const string DatabaseTable = "Cache";
		private const int DatabaseVersion = 1;
		
		public CacheRepository (Context context)
			: base(context, DatabaseTable, null, DatabaseVersion)
		{
		}
		
		public override void OnCreate (SQLiteDatabase db)
		{
			db.ExecSQL ("CREATE TABLE Cache (Key TEXT PRIMARY KEY, Value TEXT);");
		}
		
		public override void OnUpgrade (SQLiteDatabase db, int oldVersion, int newVersion)
		{
			db.ExecSQL ("DROP TABLE IF EXISTS Cache");
			OnCreate (db);
		}

		#region Properties

		public bool LoggedIn {
			get { return Get<bool> ("LoggedIn"); }
			set { Save ("LoggedIn", value); }
		}

		public string LoggedInEmployeeNumber {
			get { return Get ("LoggedInEmployeeNumber"); }
			set { Save ("LoggedInEmployeeNumber", value); }
		}

		public string LoggedInBranchName {
			get { return Get ("LoggedInBranchName"); }
			set { Save ("LoggedInBranchName", value); }
		}

		public string LoggedInBranchId {
			get { return Get ("LoggedInBranchId"); }
			set { Save ("LoggedInBranchId", value); }
		}

		public bool DataDownloaded {
			get { return Get<bool> ("DataDownloaded"); }
			set { Save ("DataDownloaded", value); }
		}

		public List<Branch> Branches {
			get {
				try {
					var rawValue = Get ("Branches");
					Log.Info(Tag, rawValue);
					
					var value = new JsonObject (rawValue);

					return ((IEnumerable<object>)value ["Result"]).AsParallel ().Select (obj => {
						var d = (JsonObject)obj;
						return new Branch ((string)d ["ID"], (string)d ["Description"], (string)d ["Company"]);
					}).OrderBy (x => x.Name).ToList ();
				} catch (System.Exception exc) {
					Log.Wtf (Tag, Throwable.FromException(exc));
					return null;
				}
			}
		}

		public void SetBranches (JsonObject value)
		{
			Save ("Branches", value);
		}

		public List<PartType> Parts {
			get {
				try {
					var rawValue = Get ("Parts");
					Log.Info(Tag, rawValue);
					
					var value = new JsonObject (rawValue);

					return ((IEnumerable<object>)value ["Result"]).AsParallel ().Select (obj => {
						var d = (JsonObject)obj;
						return new PartType ((long)d ["ID"], (string)d ["SalesCatalogSection"], (string)d ["Description"], d);
					}).ToList ();
				} catch (System.Exception exc) {
					Log.Wtf (Tag, Throwable.FromException(exc));
					return null;
				}
			}
		}

		public void SetParts (JsonObject value)
		{
			Save ("Parts", value);
		}

		public List<ActivityType> Activities {
			get {
				try {
					var rawValue = Get ("Activities");
					Log.Info(Tag, rawValue);
					
					var value = new JsonObject (rawValue);

					return ((IEnumerable<object>)value ["Result"]).AsParallel ().Select (obj => {
						var d = (JsonObject)obj;
						return new ActivityType ((long)d ["ID"], (string)d ["SalesCatalogSection"], (string)d ["Description"], (string)d ["WorkTypes"], (string)d ["SalesCategoryCode4"], d);
					}).ToList ();
				} catch (System.Exception exc) {
					Log.Wtf (Tag, Throwable.FromException(exc));
					return null;
				}
			}
		}

		public void SetActivities (JsonObject value)
		{
			Save ("Activities", value);
		}

		public List<WorkOrderType> WorkOrderTypes {
			get {
				try {
					var rawValue = Get ("WorkOrderTypes");
					Log.Info(Tag, rawValue);

					var value = new JsonObject (rawValue);

					return ((IEnumerable<object>)value ["Result"]).AsParallel ().Select (obj => {
						var d = (JsonObject)obj;
						return new WorkOrderType ((string)d ["UserDefinedCodeID"], (string)d ["Description1"]);
					}).ToList ();
				} catch (System.Exception exc) {
					Log.Wtf (Tag, Throwable.FromException(exc));
					return null;
				}
			}
		}

		public void SetWorkOrderTypes (JsonObject value)
		{
			Save ("WorkOrderTypes", value);
		}

		public List<AttributeCategory> AttributeCategories {
			get {
				try {
					var rawValue = Get ("AttributeCategories");
					Log.Info(Tag, rawValue);

					var value = new JsonObject (rawValue);

					return ((IEnumerable<object>)value ["Result"]).AsParallel ().Select (obj => {
						var d = (JsonObject)obj;
						//return new AttributeCategory ((string)d ["Description"],(string)d ["Code"], (string)d ["ItemDetail"], (string)d ["SortSequence"]);
						return new AttributeCategory ((string)d ["ID"],(string)d ["ProductCode"],(string)d ["Description"],(string)d ["UserDefinedCodeID"], (string)d ["Description1"], (string)d ["UserDefinedCodeTypeID"]);
					}).ToList ();
				} catch (System.Exception exc) {
					Log.Wtf (Tag, Throwable.FromException(exc));
					return null;
				}
			}
		}

		public void SetAttributeCategories (JsonObject value)
		{
			Save ("AttributeCategories", value);
		}

		#endregion

		public T Get<T> (string key)
		{
			var value = Get (key);
			
			if (value == null)
				return default(T);
			
			return (T)Convert.ChangeType (value, typeof(T));
		}

		public string Get (string value)
		{
			var workOrderCursor = ReadableDatabase.Query ("Cache", new[] { "Value" }, "Key = ?", new[] { value }, null, null, null);
			
			if (workOrderCursor.MoveToFirst ())
				return workOrderCursor.GetString (0);

			return null;
		}

		public void DeleteAll ()
		{
			WritableDatabase.Delete ("Cache", null, null);
		}

		public void Delete (string value)
		{
			WritableDatabase.Delete ("Cache", "Key = ?", new[] { value });
		}
		
		public void Save<T> (string key, T value)
		{
			if (value == null) {
				Delete (key);
				return;
			}

			var values = new ContentValues ();
			values.Put ("Key", key);
			values.Put ("Value", value.ToString ());
			
			WritableDatabase.InsertWithOnConflict ("Cache", null, values, Conflict.Replace);
		}
	}
}
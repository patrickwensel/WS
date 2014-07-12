using System;
using System.Threading.Tasks;
using Android.App;
using Android.Runtime;
using WS.Mobile.WorkOrders.Backend;
using WS.Mobile.WorkOrders.Data;
using Android.Util;

namespace WS.Mobile.WorkOrders
{
	[Application]
	public class WorkOrderApplication : Application
	{
		private const string Tag = "App";

		public WorkOrderRepository WorkOrderRepository { get; private set; }

		public CacheRepository CacheRepository { get; private set; }

		public WorkOrderApplication (IntPtr handle, JniHandleOwnership transfer)
					: base(handle, transfer)
		{
		}

		public override void OnCreate ()
		{
			base.OnCreate ();
		
			WorkOrderRepository = new WorkOrderRepository (this);
			CacheRepository = new CacheRepository (this);
		}

		public void DownloadBusinessUnits ()
		{
			var client = new WorkOrdersClient ();

			CacheRepository.SetBranches (client.GetBusinessUnits ());
		}

		public bool PopulateBranchData ()
		{
			var success = true;
			var client = new WorkOrdersClient ();
			var loggedInBranch = CacheRepository.LoggedInBranchId;
			var locked = new object ();

			Action<string, Action<JsonObject>, JsonObject> set = (name, func, obj) => {
				// only allow single instance to write to the database at a time
				lock (locked) {
					var objSuccess = (bool)obj [Global.RestSuccessName];
					success = success && objSuccess;

					Log.Error(Tag, obj.ToString());

					func (obj);
				}
			};

			Task.WaitAll (
				Task.Factory.StartNew (delegate {
					set ("Activities", CacheRepository.SetActivities, client.GetActivities (loggedInBranch));
				}),
				Task.Factory.StartNew (delegate {
					set ("Parts", CacheRepository.SetParts, client.GetParts (loggedInBranch));
				}),
				Task.Factory.StartNew (delegate {
					set ("WorkOrderTypes", CacheRepository.SetWorkOrderTypes, client.GetWorkOrderTypes ());
				}),
				Task.Factory.StartNew (delegate {
					set ("AttributeCategories", CacheRepository.SetAttributeCategories, client.GetAttributeCategories());
				})
			);
			
			CacheRepository.DataDownloaded = success;
			return success;
		}

		public void ClearCache ()
		{
			CacheRepository.DeleteAll ();
		}
	}
}

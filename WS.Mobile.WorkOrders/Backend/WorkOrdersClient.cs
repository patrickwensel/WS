using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Android.Preferences;
using Android.Util;
using Java.IO;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Validation;
using System.Timers;
using System.Diagnostics;
using System.Threading;

namespace WS.Mobile.WorkOrders.Backend
{
	public class WorkOrdersClient
	{
		private const string Tag = "client";
		private readonly RestClient _client;
		private string _version ;
		public WorkOrdersClient ()
		{
			var assembly = Assembly.GetExecutingAssembly();
			AssemblyName assemblyName = new AssemblyName(assembly.FullName);
			//var version = assemblyName.Version;
			_version=assemblyName.Version.ToString();
			var pref = PreferenceManager.GetDefaultSharedPreferences(WorkOrderApplication.Context);
			var server = pref.GetString(Global.SettingSelectedServer, Global.SettingSelectedServerDefault);

			Log.Info(Tag, "server: " + server);

			_client = new RestClient();
			_client.UserAgent = "ws.workorders/" + _version;
			_client.BaseUrl =  "http://" + server + "/api/workorders";
		}

		private JsonObject ExecuteWebRequest (WebRequest request)
		{
			HttpWebResponse response;

			try {
				response = (HttpWebResponse)request.GetResponse();
			} catch (WebException exc) {
				Log.Error(Tag, "response: failed - " + exc.Status);

				return new JsonObject(new Dictionary<string,object> { 
					{ Global.RestSuccessName, false }, 
					{ Global.RestErrorName, exc.Status } });
			}

			string content;
			using (var responseStream = response.GetResponseStream())
			using (var stringStream = new StreamReader(responseStream)) {
				content = stringStream.ReadToEnd();
			}

			if (String.IsNullOrWhiteSpace(content)) {
				Log.Error(Tag, "response: failed - no content");
				return new JsonObject(new Dictionary<string, object> { { Global.RestSuccessName, false } });
			}

			var data = new JsonObject(content);
			data[Global.RestSuccessName] = true;
			
			Log.Info(Tag, "response: success");
			Log.Debug(Tag, "response: " + content);
			return data;
		}

		private JsonObject ExecuteArray (RestRequest request)
		{
			Log.Info(Tag, "request: " + _client.BuildUri(request).ToString());
			var response = _client.Execute(request);
			
			if (String.IsNullOrWhiteSpace(response.Content) || response.StatusCode != HttpStatusCode.OK) {
				Log.Error(Tag, "response: failed");
				return new JsonObject(new Dictionary<string, object> { { Global.RestSuccessName, false } });
			}
			
			var data = new JsonObject();
			data[Global.RestSuccessName] = true;
			data["Result"] = JArray.Parse(response.Content);
			
			Log.Info(Tag, "response: success");
			Log.Debug(Tag, "response: " + response.Content);
			return data;
		}

		private JsonObject Execute (RestRequest request)
		{
			Log.Info(Tag, "request: " + _client.BuildUri(request).ToString());
			var response = _client.Execute(request);

			if (String.IsNullOrWhiteSpace(response.Content) || response.StatusCode != HttpStatusCode.OK) {
				Log.Error(Tag, "response: failed");
				return new JsonObject(new Dictionary<string, object> { { Global.RestSuccessName, false } });
			}

			var data = new JsonObject(response.Content);
			data[Global.RestSuccessName] = true;

			Log.Info(Tag, "response: success");
			Log.Debug(Tag, "response: " + response.Content);
			return data;
		}

		// http -f post http://ws124nt.willscot.com/api/workorders/login employeeNumber=01129 password=0000
		public JsonObject Login (string employeeNumber, string pin)
		{
			Require.Argument("employeeNumber", employeeNumber);
			Require.Argument("pin", pin);
			Require.Argument ("version", _version);
			var request = new RestRequest();
			request.Method = Method.POST;
			request.Resource = "login";
			request.AddParameter("employeeNumber", employeeNumber);
			request.AddParameter("password", pin);
			request.AddParameter ("versionNumber", _version);
			return Execute(request);
		}

		// http get http://ws124nt.willscot.com/api/workorders/businessUnits
		public JsonObject GetBusinessUnits ()
		{
			var request = new RestRequest();
			request.Method = Method.GET;
			request.Resource = "businessUnits";

			return ExecuteArray(request);
		}

		// http get http://ws124nt.willscot.com/api/workorders/unit?unitNumber=CBC-05831
		public JsonObject GetUnit (string unitNumber)
		{
			Require.Argument("unitNumber", unitNumber);

			var request = new RestRequest();
			request.Method = Method.GET;
			request.Resource = "unit";

			request.AddParameter("unitNumber", unitNumber);
			request.AddParameter("version", _version);
			return Execute(request);
		}

		// http get http://ws124nt.willscot.com/api/workorders/OMBOrders?unitNumber=CBC-05831
		public JsonObject GetOMBOrders (string unitNumber)
		{
			Require.Argument("unitNumber", unitNumber);

			var request = new RestRequest();
			request.Method = Method.GET;
			request.Resource = "OMBOrders";

			request.AddParameter("unitNumber", unitNumber);
			request.AddParameter("version", _version);
			return ExecuteArray(request);
		}

		// http get http://ws124nt.willscot.com/api/workorders/activities?businessUnitID=13020
		public JsonObject GetActivities (string businessUnitID)
		{
			Require.Argument("businessUnitID", businessUnitID);

			var request = new RestRequest();
			request.Method = Method.GET;
			request.Resource = "activities";

			request.AddParameter("businessUnitID", businessUnitID);

			return ExecuteArray(request);
		}

		// http get http://ws124nt.willscot.com/api/workorders/parts?businessUnitID=13020
		public JsonObject GetParts (string businessUnitID)
		{
			Require.Argument("businessUnitID", businessUnitID);
			
			var request = new RestRequest();
			request.Method = Method.GET;
			request.Resource = "parts";
			
			request.AddParameter("businessUnitID", businessUnitID);
			
			return ExecuteArray(request);
		}

		// http get http://ws124nt.willscot.com/api/workorders/workOrderTypes
		public JsonObject GetWorkOrderTypes ()
		{
			var request = new RestRequest();
			request.Method = Method.GET;
			request.Resource = "workOrderTypes";
			
			return ExecuteArray(request);
		}

		// http post http://net1dev.willscot.com/api/workorders/addWorkOrder
		public JsonObject AddWorkOrder (JsonObject obj)
		{
			var request = new RestRequest();
			request.Method = Method.POST;
			request.Resource = "addWorkOrder";
			request.RequestFormat = DataFormat.Json;
			request.AddParameter ("versionNumber", _version);
			var url = _client.BuildUri(request);
			Log.Info(Tag, "request: " + url.ToString());
			var webRequest = (HttpWebRequest)WebRequest.Create(url);
			webRequest.Method = request.Method.ToString();
			webRequest.Accept = "application/json";
			webRequest.KeepAlive = true;
			webRequest.ContentType = "application/json";
			webRequest.AutomaticDecompression = DecompressionMethods.GZip;
			webRequest.SendChunked = true;

			var json = obj.ToString();
			var jsonBytes = Encoding.ASCII.GetBytes(json);

			Log.Info(Tag, "request: " + json);
			const int BufferSize = 4096;
			using (var requestStream = webRequest.GetRequestStream()) {
				using (var jsonByteStream = new MemoryStream(jsonBytes)) {
					var buffer = new byte[BufferSize];
						
					while (true) {
						// make sure that the stream can be read from
						if (!jsonByteStream.CanRead)
							break;
							
						int bytesReturned = jsonByteStream.Read(buffer, 0, BufferSize);
							
						// if not bytes were returned the end of the stream has been reached
						// and the loop should exit
						if (bytesReturned == 0)
							break;
							
						// write bytes to the request
						requestStream.Write(buffer, 0, bytesReturned);
					}
						
					jsonByteStream.Close();
				}
					
				requestStream.Flush();
				requestStream.Close();
			}

			return ExecuteWebRequest(webRequest);
		}

		// http post http://net1dev.willscot.com/api/workorders/addWorkOrderImage?unitNumber=CBC-05831
		public bool AddWorkOrderImages (string guid, Image image)
		{

			var webRequestResult = new JsonObject ();

			for (int attempts = 0; attempts < 5; attempts++) {
				try {

					var id = image.ID;
					var imagePath = image.ImagePath;
					long imageByteCount = 0;

					System.IO.FileInfo file = new System.IO.FileInfo (imagePath);
					imageByteCount = file.Length;

					var request = new RestRequest ();
					request.Method = Method.POST;
					request.Resource = "addWorkOrderImages?guid={guid}";

					request.AddUrlSegment ("guid", guid);

					const string Boundry = "next image";

					var url = _client.BuildUri (request);
					Log.Info (Tag, "request: " + url.ToString ());
					var webRequest = (HttpWebRequest)WebRequest.Create (url);
					webRequest.Method = request.Method.ToString ();
					webRequest.Accept = "application/json";
					webRequest.KeepAlive = true;
					webRequest.ContentType = "multipart/mixed; boundary=\"" + Boundry + "\"";
					webRequest.AutomaticDecompression = DecompressionMethods.GZip;
					webRequest.SendChunked = true;

					var pre = @"
--{0}
Content-Disposition: attachment; name=""{1}""; filename=""{2}""; read-date=""{3:r}""
Content-Type: image/jpeg

";

					var post = @"
--{0}--
";
					const int BufferSize = 4096;
					using (var requestStream = webRequest.GetRequestStream ()) {
						var bytes = new byte[0];

						bytes = Encoding.ASCII.GetBytes (String.Format (pre, Boundry, id, id + ".jpg", DateTime.Now));
						requestStream.Write (bytes, 0, bytes.Length);



						Log.Info (Tag, "upload image: " + imagePath);
						using (var imageBytes = new RandomAccessFile (imagePath, "r")) {
							var buffer = new byte[BufferSize];



							while (true) {
								int bytesReturned = imageBytes.Read (buffer);

								// if not bytes were returned the end of the stream has been reached
								// and the loop should exit
								if (bytesReturned == -1)
									break;

								// write bytes to the request
								requestStream.Write (buffer, 0, bytesReturned);
							}

							imageBytes.Close ();
						}

						requestStream.Flush ();

						bytes = Encoding.ASCII.GetBytes (String.Format (post, Boundry));
						requestStream.Write (bytes, 0, bytes.Length);

						requestStream.Close ();
					}

					for (int byteCheckAttempts = 0; byteCheckAttempts < 5; byteCheckAttempts++) {

						webRequestResult = ExecuteWebRequest (webRequest);

						Require.Argument ("imageByteCount", imageByteCount);
						Require.Argument ("guid", guid);
						Require.Argument ("id", id);

						var request2 = new RestRequest ();
						request2.Method = Method.GET;
						request2.Resource = "ImageByteCount";

						request2.AddParameter ("imageByteCount", imageByteCount);
						request2.AddParameter ("guid", guid);
						request2.AddParameter ("id", id);

						var request2Results = Execute (request2);

						if (request2Results.ContainsKey ("Complete")) {
							var value = request2Results ["Complete"].ToString ();

							if (value == "True") {

								return true;
							} else {
								var stopwatch = Stopwatch.StartNew ();
								stopwatch = Stopwatch.StartNew ();
								Thread.Sleep (10000);
								stopwatch.Stop ();
							}
						}
					}

				} catch (Exception ex) {		
					var x = ex.Message;
					Log.Error (Tag, "response: failed - " + x);

					var stopwatch = Stopwatch.StartNew ();
					stopwatch = Stopwatch.StartNew ();
					Thread.Sleep (5000);
					stopwatch.Stop ();
				}
			}

			return false;

		}

		// http get http://ws124nt.willscot.com/api/workorders/getAttributes
		public JsonObject GetAttributeCategories ()
		{
			var request = new RestRequest();
			request.Method = Method.GET;
			request.Resource = "getAttributes";

			return ExecuteArray(request);
		}

	}

	public class ImageByteResponse
	{
		public bool Complete { get; set; }
	}

}

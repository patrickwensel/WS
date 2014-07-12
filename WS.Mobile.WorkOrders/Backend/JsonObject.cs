using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WS.Mobile.WorkOrders.Backend
{
	public class JsonObject : DynamicObject, IDictionary<string, object>, ICloneable
	{
		private readonly IDictionary<string, object> _model;

		public JsonObject ()
            : this(new Dictionary<string, object>())
		{
		}

		public JsonObject (string json)
            : this(JsonConvert.DeserializeObject<Dictionary<string, object>>(json))
		{
		}

		public JsonObject (IDictionary<string, object> model)
		{
			_model = new Dictionary<string, object> (model, StringComparer.OrdinalIgnoreCase);
		}

		private static string GetSingleIndexOrNull (object[] indexes)
		{
			if (indexes.Length == 1)
				return (string)indexes [0];

			return null;
		}

		private object WrapObjectIfNessisary (object result)
		{
			// handle special types in model object
			if (result is JObject)
				result = new JsonObject (((IDictionary<string, JToken>)result).ToDictionary (x => x.Key, x => (object)x.Value));
			else if (result is JValue)
				result = ((JValue)result).Value;
			else if (result is IDictionary<string, object>)
				result = new JsonObject ((IDictionary<string, object>)result);
			else if (result is ICollection || result is Array || result is JArray) {
				var itemList = new List<object> ();
				foreach (var item2 in (IEnumerable)result)
					itemList.Add (WrapObjectIfNessisary (item2));

				result = itemList;
			}

			return result;
		}

		public override IEnumerable<string> GetDynamicMemberNames ()
		{
			return _model.Keys;
		}

		public override bool TryConvert (ConvertBinder binder, out object result)
		{
			result = null;

			if (!binder.Type.IsAssignableFrom (_model.GetType ()))
				throw new InvalidOperationException (String.Format (@"Unable to convert to ""{0}"".", binder.Type));

			result = _model;
			return true;
		}

		public override bool TryGetIndex (GetIndexBinder binder, object[] indexes, out object result)
		{
			var key = GetSingleIndexOrNull (indexes);

			TryGetValue (key, out result);
			result = WrapObjectIfNessisary (result);

			return true;
		}

		public override bool TryGetMember (GetMemberBinder binder, out object result)
		{
			TryGetValue (binder.Name, out result);
			result = WrapObjectIfNessisary (result);

			return true;
		}

		public override bool TryInvokeMember (InvokeMemberBinder binder, object[] args, out object result)
		{
			result = _model.GetType ().InvokeMember (binder.Name, BindingFlags.InvokeMethod, null, _model, args);
			result = WrapObjectIfNessisary (result);

			return true;
		}

		public override bool TrySetIndex (SetIndexBinder binder, object[] indexes, object value)
		{
			var key = GetSingleIndexOrNull (indexes);

			if (!String.IsNullOrEmpty (key))
				_model [key] = WrapObjectIfNessisary (value);

			return true;
		}

		public override bool TrySetMember (SetMemberBinder binder, object value)
		{
			_model [binder.Name] = WrapObjectIfNessisary (value);
			return true;
		}

		public override string ToString ()
		{
			return JsonConvert.SerializeObject (_model);
		}

    #region ICloneable

		public object Clone ()
		{
			return new JsonObject (ToString ());
		}

    #endregion

    #region IDictionary<string,object> Members

		public void Add (string key, object value)
		{
			_model.Add (key, value);
		}

		public bool ContainsKey (string key)
		{
			return _model.ContainsKey (key);
		}

		[JsonIgnore]
		public ICollection<string> Keys {
			get { return _model.Keys; }
		}

		public bool Remove (string key)
		{
			return _model.Remove (key);
		}

		public bool TryGetValue (string key, out object value)
		{
			value = null;

			if (String.IsNullOrEmpty (key))
				return true;

			return _model.TryGetValue (key, out value);
		}

		[JsonIgnore]
		public ICollection<object> Values {
			get { return _model.Values; }
		}

		[JsonIgnore]
		public object this [string key] {
			get {
				object result;
				TryGetValue (key, out result);
				result = WrapObjectIfNessisary (result);

				return result;
			}
			set {
				if (!String.IsNullOrEmpty (key))
					_model [key] = WrapObjectIfNessisary (value);
			}
		}

    #endregion

    #region ICollection<KeyValuePair<string,object>> Members

		void ICollection<KeyValuePair<string, object>>.Add (KeyValuePair<string, object> item)
		{
			_model.Add (item);
		}

		void ICollection<KeyValuePair<string, object>>.Clear ()
		{
			_model.Clear ();
		}

		bool ICollection<KeyValuePair<string, object>>.Contains (KeyValuePair<string, object> item)
		{
			return _model.Contains (item);
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo (KeyValuePair<string, object>[] array, int arrayIndex)
		{
			_model.CopyTo (array, arrayIndex);
		}

		[JsonIgnore]
		int ICollection<KeyValuePair<string, object>>.Count {
			get { return _model.Count; }
		}

		[JsonIgnore]
		bool ICollection<KeyValuePair<string, object>>.IsReadOnly {
			get { return _model.IsReadOnly; }
		}

		bool ICollection<KeyValuePair<string, object>>.Remove (KeyValuePair<string, object> item)
		{
			return _model.Remove (item);
		}

    #endregion

    #region IEnumerable<KeyValuePair<string,object>> Members

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator ()
		{
			return _model.ToDictionary (
        x => x.Key,
        x => WrapObjectIfNessisary (x.Value))
        .GetEnumerator ();
		}

    #endregion

    #region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return ((IEnumerable<KeyValuePair<string, object>>)this).GetEnumerator ();
		}

    #endregion
	}
}
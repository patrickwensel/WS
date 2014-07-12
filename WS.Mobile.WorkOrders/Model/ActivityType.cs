using System;
using System.Collections.Generic;
using System.Linq;
using WS.Mobile.WorkOrders.Backend;

namespace WS.Mobile.WorkOrders.Model
{
    public class ActivityType : Java.Lang.Object
    {
        public ActivityType(long id, string category, string name, string workTypes, string locationType, JsonObject obj)
        {
            Id = id;
            Category = category;
            Name = name.Trim();
            WorkTypes = workTypes.Trim().Split(new[] { ',' }).ToList();
            LocationType = locationType.FirstOrDefault();
            Object = obj;
        }
        
        public long Id { get; private set; }
        public string Category { get; private set; }
        public string Name { get; private set; }
        public List<string> WorkTypes { get; private set; }

        public char LocationType { get; private set; }
        public bool IsInternal { get { return LocationType == 'I'; } }
        public bool IsExternal { get { return LocationType == 'E'; } }

        public JsonObject Object { get; private set; }

        public override string ToString()
        {
            return Category + " - " + Name;
        }
    }
}
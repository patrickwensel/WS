using System;
using System.Linq;
using WS.Mobile.WorkOrders.Backend;

namespace WS.Mobile.WorkOrders.Model
{
    public class PartType : Java.Lang.Object
    {
        public PartType(long id, string category, string name, JsonObject obj)
        {
            Id = id;
            Category = category;
            Name = name.Trim();
            Object = obj;
        }
        
        public long Id { get; private set; }
        public string Category { get; private set; }
        public string Name { get; private set; }

        public JsonObject Object { get; private set; }

        public override string ToString()
        {
            return Category + " - " + Name;
        }
    }
}
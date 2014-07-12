using System;

namespace WS.Mobile.WorkOrders.Model
{
    public class WorkOrderType : Java.Lang.Object
    {
        public WorkOrderType(string id, string name)
        {
            Id = id.Trim();
            Name = name.Trim();
        }
        
        public string Id { get; private set; }
        public string Name { get; private set; }
        
        public override string ToString()
        {
            return Name;
        }
    }
}


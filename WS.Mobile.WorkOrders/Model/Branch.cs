using System;

namespace WS.Mobile.WorkOrders.Model
{
    public class Branch : Java.Lang.Object
    {
        public Branch(string id, string name, string company)
        {
            Id = id.Trim();
            Name = name.Trim();
            Company = company.Trim();
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Company { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}


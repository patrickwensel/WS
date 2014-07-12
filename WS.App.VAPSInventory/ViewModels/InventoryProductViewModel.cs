using System.ComponentModel;

namespace WS.App.VAPSInventory.ViewModels
{

    public class InventoryProductViewModel
    {

        public int ID { get; set; }

        public string ProductCategory { get; set; }

        public string Product { get; set; }

        [DisplayName("Usable")]
        public int? Usable { get; set; }

        [DisplayName("Repairable")]
        public int? Repairable { get; set; }

    }

}
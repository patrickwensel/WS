using System.Collections.Generic;

namespace WS.App.VAPSInventory.ViewModels
{
    public class InventoryViewModel
    {
        public decimal LocationNumber { get; set; }
        public decimal Month { get; set; }
        public decimal Year { get; set; }

        public List<InventoryProductViewModel> InventoryProductViewModels { get; set; }

    }
}
using System;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof(WS.App.VAPSInventory.App_Start.MySuperPackage), "PreStart")]

namespace WS.App.VAPSInventory.App_Start {
    public static class MySuperPackage {
        public static void PreStart() {
            MVCControlsToolkit.Core.Extensions.Register();
        }
    }
}
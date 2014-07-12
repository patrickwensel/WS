using System;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof(WS.App.Payment.App_Start.MySuperPackage), "PreStart")]

namespace WS.App.Payment.App_Start {
    public static class MySuperPackage {
        public static void PreStart() {
            MVCControlsToolkit.Core.Extensions.Register();
        }
    }
}
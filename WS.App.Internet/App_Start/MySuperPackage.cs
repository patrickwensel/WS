using System;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof(WS.App.Internet.App_Start.MySuperPackage), "PreStart")]

namespace WS.App.Internet.App_Start {
    public static class MySuperPackage {
        public static void PreStart() {
            MVCControlsToolkit.Core.Extensions.Register();
        }
    }
}
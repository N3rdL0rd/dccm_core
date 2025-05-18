﻿using ModCore.Events.Interfaces;
using ModCore.Events.Interfaces.VM;
using System.Runtime.InteropServices;

namespace ModCore.Modules
{
    [CoreModule(CoreModuleAttribute.CoreModuleKind.Preload,
        CoreModuleAttribute.SupportOSKind.Windows)]
    internal class WindowsPlatformUtils : CoreModule<WindowsPlatformUtils>, IOnHashlinkVMReady
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]

        private delegate void FreeConsole_handler();
        public override int Priority => ModulePriorities.PlatformUtils;

        private static void FreeConsole()
        {

        }

        public void OnHashlinkVMReady()
        {
            var kernel32 = NativeLibrary.Load("kernel32.dll");
            var freeconsole = NativeLibrary.GetExport(kernel32, "FreeConsole");

            if (!Core.Config.Value.AllowCloseConsole)
            {
                NativeHooks.Instance.CreateHook(freeconsole, (FreeConsole_handler)FreeConsole);
            }

            NativeLibrary.Free(kernel32);
        }
    }
}

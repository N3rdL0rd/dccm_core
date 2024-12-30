﻿using ModCore.Hashlink.Hooks;
using ModCore.Track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace ModCore
{
    internal unsafe partial class Native
    {
        public static nint ModcorenativeHandle { get; } = NativeLibrary.Load(MODCORE_NATIVE_NAME);
        static Native()
        {
            foreach (var m in typeof(Native).GetMethods(BindingFlags.NonPublic | BindingFlags.Public |
                BindingFlags.Static))
            {
                var attr = m.GetCustomAttribute<UnmanagedCallersOnlyAttribute>();
                if (attr == null)
                {
                    continue;
                }
                var name = string.IsNullOrEmpty(attr.EntryPoint) ? m.Name : attr.EntryPoint;
                if (NativeLibrary.TryGetExport(ModcorenativeHandle, "csapi_" + name, out var addr))
                {
                    *(nint*)addr = m.MethodHandle.GetFunctionPointer();
                }
            }
        }
        #region Exports

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static void PrintStackDebug()
        {
            var stack = new MixStackTrace(0, true);
            for (int i = 0; i < stack.FrameCount; i++)
            {
                Console.WriteLine(stack.GetFrame(i)!.ToString());
            }
            
        }

        #endregion
    }
}

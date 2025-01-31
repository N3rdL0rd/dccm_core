﻿
using ModCore.Events;
using ModCore.Events.Interfaces;
using ModCore.Events.Interfaces.VM;
using ModCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Modules
{
    [CoreModule]
    internal unsafe class NativeModuleResolver : CoreModule<NativeModuleResolver>,
        IOnCoreModuleInitializing,
        IOnResolveNativeFunction,
        IOnResolveNativeLib
    {
        public override int Priority => ModulePriorities.NativeModuleResolver;

        private readonly Dictionary<string, Dictionary<string, nint>> knownNativeFunctions = [];

        private void RegisterType( string libname, Type type )
        {
            if (!knownNativeFunctions.TryGetValue(libname, out var dict))
            {
                dict = [];
                knownNativeFunctions.Add(libname, dict);
            }
            foreach (var v in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                var attr = v.GetCustomAttribute<UnmanagedCallersOnlyAttribute>();
                if (attr == null)
                {
                    continue;
                }
                var name = string.IsNullOrEmpty(attr.EntryPoint) ? v.Name : attr.EntryPoint;
                dict.Add(name, v.MethodHandle.GetFunctionPointer());
            }

        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static int NativeReturnFalse()
        {
            return 0;
        }
        private static readonly delegate* unmanaged[Cdecl]< int > ptr_NativeReturnFalse = &NativeReturnFalse;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static void NativeNotImplemented()
        {
            new NotImplementedException().HashlinkThrow();
        }
        private static readonly delegate* unmanaged[Cdecl]< void > ptr_NativeNotImplemented = &NativeNotImplemented;

        void IOnCoreModuleInitializing.OnCoreModuleInitializing()
        {
            
        }


        EventResult<nint> IOnResolveNativeFunction.OnResolveNativeFunction( IOnResolveNativeFunction.NativeFunctionInfo info )
        {
            if (info.libname == "chroma")
            {
                //Not Support
                return (nint)ptr_NativeReturnFalse;
            }
            if (knownNativeFunctions.TryGetValue(info.libname, out var dict))
            {
                if (dict.TryGetValue(info.name, out var result))
                {
                    return result;
                }
            }
            return default;
        }

        EventResult<nint> IOnResolveNativeLib.OnResolveNativeLib( string name )
        {
            if (name == "std" || name == "builtin")
            {
                return default;
            }

            var path = FolderInfo.CoreNativeRoot.GetFilePath(name + ".hdll");
            if (!File.Exists(path) || !NativeLibrary.TryLoad(path, out var result))
            {
                return default;
            }
            Logger.Information("Loading native module from {path}", path);
            return result;
        }
    }
}

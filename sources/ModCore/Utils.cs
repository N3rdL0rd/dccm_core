﻿
using ModCore.Trace;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ModCore
{
    internal static class Utils
    {
       
        public static string GetDisplayName(this StackFrame frame)
        {
            var sb = new StringBuilder();
            if (frame is HLStackFrame)
            {
                sb.Append("(Hashlink Jit) ");
                sb.Append(frame.ToString().Trim());
            }
            else if (frame is NativeStackFrame)
            {
                sb.Append("(Native Frame) ");
                sb.Append(frame.ToString().Trim());
            }
            else
            {
                sb.Append("(.NET Runtime) ");
                var method = frame.GetMethod();
                if (method != null)
                {
                    sb.Append(method.GetID());
                }
                if (frame.HasSource())
                {
                    sb.Append(' ');
                    sb.Append(frame.GetFileName());
                    sb.Append(':');
                    sb.Append(frame.GetFileLineNumber());
                }
            }
            return sb.ToString();
        }
        public static bool MemCmp(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static nint GetFrameIP(this StackFrame frame)
        {
            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_nativeOffset")]
            static extern ref int GetNativeOffset(StackFrame frame);

            var offset = GetNativeOffset(frame);
            return offset + (frame.GetMethod()?.MethodHandle.GetFunctionPointer() ?? 0);
        }

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_stackTraceString")]
        public static extern ref string? Exception_stackTraceString(Exception ex);
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_stackTrace")]
        public static extern ref object? Exception_stackTrace(Exception ex);

        public static void FillExternStackTrace(Exception ex)
        {
            ref object? stackTrace = ref Exception_stackTrace(ex);
            ref string? stackTraceString = ref Exception_stackTraceString(ex);

            if(stackTrace is MixStackTrace)
            {
                return;
            }
            var st = new MixStackTrace(new(ex, true));
            stackTrace = st;
            stackTraceString = st.ToString();
        }

        public static Type?[] SafeGetAllTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }catch(ReflectionTypeLoadException ex)
            {
                return ex.Types;
            }
        }
        public static byte[] HashFile(string path)
        {
            using var fs = File.OpenRead(path);
            return SHA256.HashData(fs);
        }
    }
}

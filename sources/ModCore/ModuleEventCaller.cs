﻿using Mono.Cecil.Cil;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ModCore
{
    internal delegate void ModuleEventCall(object self, nint refArg);
    internal static class ModuleEventCaller<TEvent>
    {
        private readonly static ModuleEventCall call;

        private static ModuleEventCall GenerateCall(MethodInfo method)
        {
            var @params = method.GetParameters();
            if (@params.Length >= 2)
            {
                throw new NotSupportedException("Methods with multiple parameters are not supported");
            }

            DynamicMethodDefinition caller = new($"ModuleEventCall+{method.Name}", typeof(void), [typeof(object), typeof(nint)]);
            var il = caller.GetILProcessor();
            il.Emit(OpCodes.Ldarg_0);
            if (@params.Length == 1)
            {
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldobj, @params[0].ParameterType);
            }
            il.Emit(OpCodes.Callvirt, method);
            il.Emit(OpCodes.Ret);
            return caller.Generate().CreateDelegate<ModuleEventCall>();
        }

        private static MethodInfo FindEventMethod(Type type)
        {
            return type
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.DeclaredOnly)
                .Where(x => x.GetParameters().Length < 2)
                .First();
        }

        static ModuleEventCaller()
        {
            call = GenerateCall(FindEventMethod(typeof(TEvent)));
        }

        public static void Invoke(TEvent self, nint refOfarg)
        {
            call(self!, refOfarg);
        }
        public static unsafe void Invoke<TArg>(TEvent self, ref TArg argOnStack)
        {
            Invoke(self!, (nint) Unsafe.AsPointer(ref argOnStack));
        }
        public static void Invoke(TEvent self)
        {
            Invoke(self!, 0);
        }
    }
}

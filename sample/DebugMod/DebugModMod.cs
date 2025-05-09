﻿using Hashlink.Marshaling;
using Hashlink.Proxy.Clousre;
using Hashlink.Proxy.DynamicAccess;
using Hashlink.Proxy.Objects;
using Hashlink.Reflection.Types;
using Haxe.Marshaling;
using ModCore.Events.Interfaces.Game;
using ModCore.Mods;
using ModCore.Modules;

namespace DebugMod
{
    public class DebugModMod(ModInfo info) : ModBase(info),
        IOnBeforeGameInit
    {
        private void Hook_Console_ctor(HashlinkClosure orig, HashlinkObject self)
        {
            orig.DynamicInvoke(self);
            var s = self.AsDynamic();
            var ss = ((HashlinkObjectType)self.Type).GlobalValue.AsDynamic()!;
            ss.HIDE_UI = "FDMM_HIDE_UI";
            ss.HIDE_DEBUG = "FDMM_HIDE_DEBUG";
            ss.HIDE_CONSOLE = "FDMM_HIDE_CONSOLE";
            s.activateDebug();
        }
        private void Hook_Console_handleCommand(HashlinkClosure orig, HashlinkObject self,
            HashlinkString command)
        {
            Logger.Information("Handle Command: {cmd}", command.Value);
            orig.DynamicInvoke(self, command);
        }
        private void Hook_Console_log(HashlinkClosure orig, HashlinkObject self,
            HashlinkString text, object color)
        {
            Logger.Information(text.ToString() ?? "");
            var ct = (HashlinkObjectType)self.Type;
            ct.Super!.FindProto("log")!.Function.DynamicInvoke(self, text, color);
        }
        private void Hook_Console_updateUIVisibility(HashlinkClosure orig, HashlinkObject self)
        {
            orig.DynamicInvoke(self);
        }
        void IOnBeforeGameInit.OnBeforeGameInit()
        {
            var hh = HashlinkHooks.Instance;

            hh.CreateHook("ui.Console", "updateUIVisibility", Hook_Console_updateUIVisibility).Enable();
            hh.CreateHook("ui.Console", "log", Hook_Console_log).Enable();
            hh.CreateHook("ui.Console", "handleCommand", Hook_Console_handleCommand).Enable();
            hh.CreateHook("ui.$Console", "__constructor__", Hook_Console_ctor).Enable();
        }
    }
}

﻿using Hashlink;
using Hashlink.Marshaling;
using Hashlink.Proxy.Clousre;
using Hashlink.Proxy.Objects;
using Hashlink.Reflection.Types;
using Haxe;
using Haxe.Marshaling;
using ModCore.Events;
using ModCore.Events.Interfaces;
using ModCore.Events.Interfaces.Game;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Events.Interfaces.VM;

namespace ModCore.Modules
{
    [CoreModule]
    public unsafe class Game : CoreModule<Game>, IOnNativeEvent,
        IOnHashlinkVMReady
    {
        public override int Priority => ModulePriorities.Game;

        private void StartGame()
        {
            var entry = (HashlinkClosure)HashlinkMarshal.ConvertHashlinkObject(
                    &HashlinkVM.Instance.Context->c
                    )!;
            entry.CreateDelegate<Action>()();
        }

        void IOnNativeEvent.OnNativeEvent( IOnNativeEvent.Event ev )
        {
            if (ev.EventId == IOnNativeEvent.EventId.HL_EV_START_GAME)
            {
                EventSystem.BroadcastEvent<IOnBeforeGameStart>();
                StartGame();
            }
        }

        private void Hook_Boot_init( HashlinkClosure orig, HashlinkObject self)
        {
            var win = self.AsHaxe().Chain.engine.window.window;
            
            orig.DynamicInvoke(self);

            win.set_title("Dead Cells with Core Modding");

            EventSystem.BroadcastEvent<IOnGameInit>();
        }
        private void Hook_Boot_update( HashlinkClosure orig, HashlinkObject self, double dt )
        {
            orig.DynamicInvoke(self, dt);

            EventSystem.BroadcastEvent<IOnFrameUpdate, double>(dt);

            if (HeroInstance != null)
            {
                EventSystem.BroadcastEvent<IOnHeroUpdate, double>(dt);
            }
        }
        private void Hook_Boot_endInit( HashlinkClosure orig, HashlinkObject self)
        {
            orig.DynamicInvoke(self);

            EventSystem.BroadcastEvent<IOnGameEndInit>();
        }

        public HaxeObject? HeroInstance
        {
            get; private set;
        }
        private object? Hook_hero_init( HashlinkClosure orig, HashlinkObject self )
        {
            HeroInstance = self.AsHaxe();
            EventSystem.BroadcastEvent<IOnHeroInit>();
            return orig.DynamicInvoke(self);
        }
        private object? Hook_hero_dispose( HashlinkClosure orig, HashlinkObject self )
        {
            EventSystem.BroadcastEvent<IOnHeroUpdate>();
            HeroInstance = null;
            return orig.DynamicInvoke(self);
        }


        void IOnHashlinkVMReady.OnHashlinkVMReady()
        {

            HashlinkHooks.Instance.CreateHook("Boot", "endInit", Hook_Boot_endInit).Enable();
            HashlinkHooks.Instance.CreateHook("Boot", "init", Hook_Boot_init).Enable();
            HashlinkHooks.Instance.CreateHook("Boot", "update", Hook_Boot_update).Enable();

            HashlinkHooks.Instance.CreateHook("en.Hero", "init", Hook_hero_init).Enable();
            HashlinkHooks.Instance.CreateHook("en.Hero", "dispose", Hook_hero_dispose).Enable();
        }
    }
}

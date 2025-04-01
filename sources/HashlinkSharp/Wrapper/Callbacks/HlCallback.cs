﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hashlink.Wrapper.Callbacks
{
    public class HlCallback
    {
        private readonly nint routerPtr;
        private readonly Delegate router;
        private readonly HlCallbackInfo info;
        internal HlCallback(Delegate router, HlCallbackInfo info)
        {
            this.router = router;
            this.info = info;
            routerPtr = Marshal.GetFunctionPointerForDelegate(router);
        }

        public nint RedirectTarget
        {
            get => info.directRoute;
            set => info.directRoute = value;
        }

        public Delegate? Target
        {
            get => info.entry?.self;
            set => info.entry = new(value);
        }
        public nint RouterPointer => routerPtr;
        public Delegate RouterDelegate => router;
    }
}

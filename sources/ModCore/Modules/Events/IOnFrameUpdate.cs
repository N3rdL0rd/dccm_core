﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Modules.Events
{
    public interface IOnFrameUpdate
    {
        public void OnFrameUpdate(float dt);
    }
}

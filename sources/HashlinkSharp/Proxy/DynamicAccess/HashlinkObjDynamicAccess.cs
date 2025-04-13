﻿using Hashlink.Proxy.Clousre;
using Hashlink.Proxy.Objects;
using Hashlink.Reflection.Types;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashlink.Proxy.DynamicAccess
{
    public abstract class HashlinkObjDynamicAccess( HashlinkObj obj ) : DynamicObject, 
        IHashlinkPointer, IExtendData, IExtendDataItem
    {
        public HashlinkObj HashlinkObject
        {
            get;
        } = obj;
        public HashlinkType Type => HashlinkObject.Type;
        public nint HashlinkPointer => ((IHashlinkPointer)HashlinkObject).HashlinkPointer;

        public static object Create( HashlinkObj obj )
        {
            if (obj is HashlinkObject hobj)
            {
                return new HashlinkObjectDynamicAccess(hobj);
            }
            else if (obj is HashlinkArray harray)
            {
                return new HashlinkArrayDynamicAccess(harray);
            }
            else if (obj is HashlinkClosure hcl)
            {
                return new HashlinkClosureDynamicAccess(hcl);
            }
            throw new NotSupportedException();
        }

        public override bool TryConvert( ConvertBinder binder, out object? result )
        {
            if (binder.Type.IsAssignableTo(typeof(HashlinkObj)))
            {
                result = HashlinkObject;
                return true;
            }
            if (binder.Type == typeof(string))
            {
                result = ToString();
                return true;
            }
            return base.TryConvert( binder, out result );
        }

        public override string? ToString()
        {
            return HashlinkObject.ToString();
        }

        T IExtendData.GetData<T>()
        {
            return ((IExtendData)HashlinkObject).GetData<T>();
        }
    }
}

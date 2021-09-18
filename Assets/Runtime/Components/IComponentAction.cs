using System;
using System.Reflection;

namespace UIKit.Components
{
    public interface IComponentAction
    {
        //
    }

    public interface IComponentAction<TParameter>
    {
        //
    }

    internal interface IComponentActionBinder
    {
        void BindAction(UnityEngine.Object target, MethodInfo info);
    }

    internal interface IGenericComponentActionBinder
    {
        void BindAction(uint actionType, UnityEngine.Object target, MethodInfo info);
    }

    internal interface IComponentActionBinder<TActionType> : IGenericComponentActionBinder
        where TActionType : Enum
    { 
        //
    }
}

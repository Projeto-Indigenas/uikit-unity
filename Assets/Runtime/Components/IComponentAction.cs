using System;
using System.Reflection;

namespace UIKit.Components
{
    public interface IComponentAction
    {
        event Action action;
    }

    public interface IComponentAction<TParameter>
    {
        event Action<TParameter> action;
    }

    internal interface IComponentActionSetup
    {
        void SetupAction(UnityEngine.Object target, MethodInfo info);
    }
}

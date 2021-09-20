using System;
using System.Reflection;

namespace UIKit.Components
{
    public interface IComponentAction
    {
        //
    }

    public interface IComponentActionBinder
    {
        void BindAction(UnityEngine.Object target, MethodInfo info, EventInfo eventInfo);
        void UnbindActions();
    }
}

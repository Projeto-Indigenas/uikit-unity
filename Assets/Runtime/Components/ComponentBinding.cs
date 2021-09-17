using System;
using System.Reflection;
using UnityEngine;

namespace UIKit.Components
{
    [Serializable]
    public class ComponentBinding
    {
        [SerializeField] protected View _target;

        public View target
        {
            get => _target;
            private set => _target = value;
        }
    }

    [Serializable]
    public class ComponentBinding<TComponent> : ComponentBinding, ISerializationCallbackReceiver
        where TComponent : View
    {
        private const BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        [SerializeField] private UnityEngine.Object _methodTarget = default;
        [SerializeField] private string _methodName = default;

        public new TComponent target => (TComponent)_target;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            //
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_methodTarget == null || string.IsNullOrEmpty(_methodName)) return;

            if (target is IComponentActionSetup setup)
            {
                MethodInfo methodInfo = _methodTarget.GetType().GetMethod(_methodName, _bindingFlags);

                if (methodInfo == null) return;

                setup.SetupAction(_methodTarget, methodInfo);
            }
        }

        #endregion
    }
}

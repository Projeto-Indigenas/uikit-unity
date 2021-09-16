using System;
using System.Reflection;
using UnityEngine;

namespace UIKit.Components
{
    [Serializable]
    public class ComponentBinding
    {
        [SerializeField] protected View _view;

        public View view
        {
            get => _view;
            private set => _view = value;
        }
    }

    [Serializable]
    public class ComponentBinding<TComponent> : ComponentBinding, ISerializationCallbackReceiver
        where TComponent : View
    {
        private const BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        [SerializeField] private UnityEngine.Object _target = default;
        [SerializeField] private string _methodName = default;

        public new TComponent view => (TComponent)_view;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            //
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_target == null || string.IsNullOrEmpty(_methodName)) return;

            if (view is IComponentActionSetup setup)
            {
                MethodInfo methodInfo = _target.GetType().GetMethod(_methodName, _bindingFlags);

                if (methodInfo == null) return;

                setup.SetupAction(_target, methodInfo);
            }
        }

        #endregion
    }
}

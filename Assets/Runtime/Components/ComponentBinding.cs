using System;
using System.Collections.Generic;
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
        [SerializeField] private string _parameters = default;

        public new TComponent target => (TComponent)_target;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            //
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!_methodTarget || string.IsNullOrEmpty(_methodName)) return;

            if (_target is IComponentActionSetup setup)
            {
                Type targetType = _methodTarget.GetType();

                MethodInfo methodInfo = targetType.GetMethod(
                    _methodName, 
                    _bindingFlags, 
                    null, 
                    GetParametersTypes(),
                    null);

                if (methodInfo == null) return;

                setup.SetupAction(_methodTarget, methodInfo);
            }
        }

        #endregion

        private Type[] GetParametersTypes()
        {
            if (string.IsNullOrEmpty(_parameters)) return Array.Empty<Type>();

            string[] parameters = _parameters.Split(';');
            Type[] types = new Type[parameters.Length];
            for (int index = 0; index < parameters.Length; index++)
            {
                string current = parameters[index];
                types[index] = Type.GetType(current);
            }
            return types;
        }
    }
}

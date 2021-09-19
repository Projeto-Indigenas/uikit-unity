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

        [SerializeField] private ComponentActionData[] _componentActions = default;

        public new TComponent target => (TComponent)_target;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            //
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!(_target is IComponentActionBinder binder)) return;

            if (_componentActions == null || _componentActions.Length == 0) return;

            for (int index = 0; index < _componentActions.Length; index++)
            {
                ComponentActionData current = _componentActions[index];

                UnityEngine.Object methodTarget = current.methodTarget;
                string methodName = current.methodName;
                string parametersTypesNames = current.parameters;
                string returnTypeName = current.returnType;
                string eventName = current.eventName;

                Type[] parametersTypes = GetParametersTypes(parametersTypesNames);
                Type returnType = string.IsNullOrEmpty(returnTypeName) ? null : Type.GetType(returnTypeName);

                MethodInfo methodInfo = null;
                if (methodTarget)
                {
                    Type targetType = methodTarget.GetType();
    
                    methodInfo = targetType.GetMethod(
                        methodName,
                        _bindingFlags,
                        null,
                        parametersTypes,
                        null);
                }
                EventInfo eventInfo = binder.GetType().GetEvent(eventName, _bindingFlags);

                if (!methodTarget || methodInfo == null) continue;

                binder.BindAction(
                    methodTarget, 
                    methodInfo,
                    eventInfo);
            }
        }

        #endregion

        private Type[] GetParametersTypes(string parametersString)
        {
            if (string.IsNullOrEmpty(parametersString)) return Array.Empty<Type>();

            string[] parameters = parametersString.Split(';');
            Type[] types = new Type[parameters.Length];
            for (int index = 0; index < parameters.Length; index++)
            {
                string current = parameters[index];
                types[index] = Type.GetType(current);
            }
            return types;
        }
    }

    [Serializable]
    public class ComponentActionData
    {
        [SerializeField] private UnityEngine.Object _methodTarget = default;
        [SerializeField] private string _methodName = default;
        [SerializeField] private string _parameters = default;
        [SerializeField] private string _returnType = default;
        [SerializeField] private string _eventName = default;

        public UnityEngine.Object methodTarget => _methodTarget;
        public string methodName => _methodName;
        public string parameters => _parameters;
        public string returnType => _returnType;
        public string eventName => _eventName;
    }
}

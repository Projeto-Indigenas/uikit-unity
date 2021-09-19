using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIKit.Components.Attributes;
using UnityEditor;
using UnityEngine;

namespace UIKit.Editor.Drawers.Handlers
{
    internal class ComponentBindingMethodHandler
    {
        private const BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly Type _bindingGenericType = default;
        private readonly SerializedProperty _methodNameProperty = default;
        private readonly SerializedProperty _parametersProperty = default;
        private readonly SerializedProperty _eventNameProperty = default;

        private EventInfo[] _allEventInfos = default;
        private MethodData[] _methodsNamesAndTargets = default;
        
        public readonly SerializedProperty methodTargetProperty = default;

        public string[] actionEventsNames { get; private set; }
        public int selectedActionEventNameIndex { get; set; }

        public string[] allMethodsSignatures { get; private set; }
        public int selectedMethodIndex { get; set; }

        public ComponentBindingMethodHandler(
            Type bindingGenericType, 
            SerializedProperty componentActionItem)
        {
            _bindingGenericType = bindingGenericType;

            methodTargetProperty = componentActionItem.FindPropertyRelative("_methodTarget");
            _methodNameProperty = componentActionItem.FindPropertyRelative("_methodName");
            _parametersProperty = componentActionItem.FindPropertyRelative("_parameters");
            _eventNameProperty = componentActionItem.FindPropertyRelative("_eventName");
        }

        public void SetupMethods()
        {
            UnityEngine.Object targetObject = methodTargetProperty.objectReferenceValue;

            selectedMethodIndex = 0;

            List<MethodData> methodsNamesAndTargets = new List<MethodData> { new MethodData(null, "None", null) };
            List<string> methodsSignatures = new List<string> { "None" };
            
            FillEventInfos();
            ProcessEventInfos();

            if (targetObject)
            {
                MonoBehaviour[] monoBehaviours = GetMonoBehavioursFromTargetObject(targetObject);

                for (int index = 0; index < monoBehaviours.Length; index++)
                {
                    MonoBehaviour current = monoBehaviours[index];

                    MethodInfo[] methods = GetMethodsFromMonoBehaviour(current);

                    FilterMatchingMethods(methods, current, targetObject, methodsNamesAndTargets, methodsSignatures);
                }
            }

            _methodsNamesAndTargets = methodsNamesAndTargets.ToArray();
            allMethodsSignatures = methodsSignatures.ToArray();

            string currentMethodNameValue = GetMethodNameFieldValue();

            for (int index = 0; index < _methodsNamesAndTargets.Length; index++)
            {
                MethodData current = _methodsNamesAndTargets[index];

                if (!current.methodName.Equals(currentMethodNameValue)) continue;

                selectedMethodIndex = index;
            }
        }

        private void FillEventInfos()
        {
            Type actionBinderAttributeType = typeof(ComponentActionBinderAttribute);
            _allEventInfos = _bindingGenericType.GetEvents(_bindingFlags).Select(each =>
            {
                return Attribute.GetCustomAttribute(each, actionBinderAttributeType) == null ? null : each;
            }).ToArray();
        }

        private void FilterMatchingMethods(
            MethodInfo[] methods, 
            MonoBehaviour current, 
            UnityEngine.Object targetObject, 
            List<MethodData> methodsNamesAndTargets, 
            List<string> methodsSignatures)
        {
            Type customAttributeType = typeof(ComponentActionAttribute);
            for (int index = 0; index < methods.Length; index++)
            {
                MethodInfo info = methods[index];

                if (Attribute.GetCustomAttribute(info, customAttributeType) == null) continue;

                ParameterInfo[] parameters = info.GetParameters();
                Type[] parametersTypes = Array.ConvertAll(parameters, each => each.ParameterType);
                Type returnType = info.ReturnType;

                bool isValidMethod = IsSameSignatureFromEventInfo(
                    _allEventInfos[selectedActionEventNameIndex],
                    parametersTypes, returnType);

                if (!isValidMethod) continue;

                string methodSignature = GetFullMethodSignature(targetObject, info, parameters);
                string methodName = info.Name;
                string parametersNames = GetParametersString(parameters);

                methodsSignatures.Add(methodSignature);
                MethodData pair = new MethodData(current, methodName, parametersNames);
                methodsNamesAndTargets.Add(pair);
            }
        }

        private bool IsSameSignatureFromEventInfo(EventInfo eventInfo, Type[] parametersTypes, Type returnType)
        {
            Type eventType = eventInfo.EventHandlerType;
            Type[] genericTypeArguments = eventType.GenericTypeArguments;

            if (eventType == typeof(Action))
            {
                return parametersTypes.Length == 0 && returnType == typeof(void);
            }

            if (eventType.IsGenericType && eventType.GetGenericTypeDefinition() == typeof(Action<>))
            {
                return returnType.Equals(typeof(void)) && genericTypeArguments.SequenceEqual(parametersTypes);
            }
            
            if (eventType.IsGenericType && eventType.GetGenericTypeDefinition() == typeof(Func<,,,>) &&
                genericTypeArguments.Length == parametersTypes.Length + 1)
            {
                return genericTypeArguments[3].Equals(returnType) &&
                genericTypeArguments[0].Equals(parametersTypes[0]) &&
                genericTypeArguments[1].Equals(parametersTypes[1]) &&
                genericTypeArguments[2].Equals(parametersTypes[2]);
            }
            
            return false;
        }

        private static MethodInfo[] GetMethodsFromMonoBehaviour(MonoBehaviour current)
        {
            Type currentType = current.GetType();
            return currentType.GetMethods(_bindingFlags);
        }

        private static MonoBehaviour[] GetMonoBehavioursFromTargetObject(UnityEngine.Object targetObject)
        {
            MonoBehaviour[] monoBehaviours;
            if (targetObject is GameObject gameObject)
            {
                monoBehaviours = gameObject.GetComponents<MonoBehaviour>();
            }
            else
            {
                monoBehaviours = ((MonoBehaviour)targetObject).GetComponents<MonoBehaviour>();
            }

            return monoBehaviours;
        }

        private void ProcessEventInfos()
        {
            actionEventsNames = Array.ConvertAll(_allEventInfos, each => each.Name);

            for (int index = 0; index < actionEventsNames.Length; index++)
            {
                string current = actionEventsNames[index];
                
                if (current.Equals(_eventNameProperty.stringValue))
                {
                    selectedActionEventNameIndex = index;

                    break;
                }
            }
        }

        private string GetMethodNameFieldValue()
        {
            if (_methodNameProperty == null) return null;

            return _methodNameProperty.stringValue;
        }

        public void SetMethodNameFieldValue(bool setTarget = true)
        {
            if (_methodNameProperty == null) return;

            MethodData data = _methodsNamesAndTargets[selectedMethodIndex];

            if (setTarget) methodTargetProperty.objectReferenceValue = data.monoBehaviour;
            _methodNameProperty.stringValue = selectedMethodIndex == 0 ? null : data.methodName;
            _parametersProperty.stringValue = data.parameters;

            if (methodTargetProperty.serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(methodTargetProperty.serializedObject.targetObject);
            }
        }

        public void SetEventNameFieldValue()
        {
            if (_eventNameProperty == null) return;

            _eventNameProperty.stringValue = actionEventsNames[selectedActionEventNameIndex];
            if (!_eventNameProperty.serializedObject.ApplyModifiedProperties()) return;
            EditorUtility.SetDirty(_eventNameProperty.serializedObject.targetObject);
        }

        private string GetFullMethodSignature(UnityEngine.Object targetObject, MethodInfo info, ParameterInfo[] parameters)
        {
            string containingClassName = info.DeclaringType.Name;
            string methodName = info.Name;
            string returnTypeName = info.ReturnType.Name;
            if (parameters.Length > 0)
            {
                methodName += "(";
                for (int index = 0; index < parameters.Length; index++)
                {
                    ParameterInfo parameter = parameters[index];
                    methodName += parameter.ParameterType.Name;
                    if (index < parameters.Length - 1) methodName += ",";
                }
                methodName += ")";
            }
            else methodName += "(void)";
            return $"{targetObject.name} ({containingClassName})/{methodName} : {returnTypeName}";
        }

        private string GetParametersString(ParameterInfo[] parameters)
        {
            if (parameters.Length == 0) return null;

            string value = "";

            for (int index = 0; index < parameters.Length; index++)
            {
                ParameterInfo current = parameters[index];

                value += current.ParameterType.FullName;

                if (index < parameters.Length - 1)
                {
                    value += ";";
                }
            }

            return value;
        }
    }

    internal struct MethodData
    {
        public readonly MonoBehaviour monoBehaviour;
        public readonly string methodName;
        public readonly string parameters;

        public MethodData(MonoBehaviour monoBehaviour, string methodName, string parameters)
        {
            this.monoBehaviour = monoBehaviour;
            this.methodName = methodName;
            this.parameters = parameters;
        }
    }
}

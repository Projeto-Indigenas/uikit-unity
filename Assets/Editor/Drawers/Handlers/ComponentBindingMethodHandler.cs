using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIKit.Components;
using UIKit.Components.Attributes;
using UnityEditor;
using UnityEngine;

namespace UIKit.Editor.Drawers.Handlers
{
    internal class ComponentBindingMethodHandler
    {
        private readonly Type _bindingGenericType = default;
        private readonly SerializedProperty _methodNameProperty = default;
        private readonly SerializedProperty _parametersProperty = default;

        private MethodData[] _methodsNamesAndTargets = default;
        
        public readonly SerializedProperty methodTargetProperty = default;

        public string[] allMethodsSignatures { get; private set; }
        public int selectedMethodIndex { get; set; }

        public ComponentBindingMethodHandler(
            Type bindingGenericType, 
            SerializedProperty property)
        {
            _bindingGenericType = bindingGenericType;

            methodTargetProperty = property.FindPropertyRelative("_methodTarget");
            _methodNameProperty = property.FindPropertyRelative("_methodName");
            _parametersProperty = property.FindPropertyRelative("_parameters");
        }

        public void SetupMethods(bool isGenericComponentAction)
        {
            UnityEngine.Object targetObject = methodTargetProperty.objectReferenceValue;

            selectedMethodIndex = 0;

            List<MethodData> methodsNamesAndTargets = new List<MethodData> { new MethodData(null, "None", null) };
            List<string> methodsSignatures = new List<string> { "None" };

            if (targetObject)
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

                for (int indexBehaviours = 0; indexBehaviours < monoBehaviours.Length; indexBehaviours++)
                {
                    MonoBehaviour current = monoBehaviours[indexBehaviours];
                    Type currentType = current.GetType();

                    MethodInfo[] methods = currentType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    Type customAttributeType = typeof(ComponentActionAttribute);
                    for (int indexMethods = 0; indexMethods < methods.Length; indexMethods++)
                    {
                        MethodInfo info = methods[indexMethods];

                        foreach (CustomAttributeData data in info.CustomAttributes)
                        {
                            if (!customAttributeType.Name.Equals(data.AttributeType.Name)) continue;

                            ParameterInfo[] parameters = info.GetParameters();

                            if (parameters.Length > 0 && !IsSameComponentActionGenericType(parameters[0].ParameterType) ||
                                parameters.Length == 0 && isGenericComponentAction)
                            {
                                continue;
                            }

                            string methodSignature = GetFullMethodSignature(targetObject, info, parameters);
                            string methodName = info.Name;
                            string parametersNames = GetParametersString(parameters);

                            methodsSignatures.Add(methodSignature);
                            MethodData pair = new MethodData(current, methodName, parametersNames);
                            methodsNamesAndTargets.Add(pair);

                            break;
                        }
                    }
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

        private string GetFullMethodSignature(UnityEngine.Object targetObject, MethodInfo info, ParameterInfo[] parameters)
        {
            string containingClassName = info.DeclaringType.Name;
            string methodName = info.Name;
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
            return $"{targetObject.name} ({containingClassName})/{methodName}";
        }

        private bool IsSameComponentActionGenericType(Type genericType)
        {
            return _bindingGenericType.GetInterfaces().Any(i =>
            {
                bool isGeneric = i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IComponentAction<>);
                return isGeneric && i.GenericTypeArguments[0] == genericType;
            });
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

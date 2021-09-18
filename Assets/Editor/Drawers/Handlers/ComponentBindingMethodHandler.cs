using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIKit.Components;
using UIKit.Components.Attributes;
using UnityEditor;

namespace UIKit.Editor.Drawers.Handlers
{
    internal class ComponentBindingMethodHandler
    {
        private readonly ComponentBinding _binding = default;
        private readonly Type _bindingGenericType = default;
        private readonly FieldInfo _methodNameFieldInfo = default;
        
        public readonly SerializedProperty methodTargetProperty = default;

        public string[] allMethodsNames { get; private set; }
        public string[] allMethodsSignatures { get; private set; }
        public int selectedMethodIndex { get; set; }

        public ComponentBindingMethodHandler(
            ComponentBinding binding, 
            Type bindingGenericType, 
            SerializedProperty targetProperty)
        {
            _binding = binding;
            _bindingGenericType = bindingGenericType;
            Type componentBindingType = binding.GetType();

            methodTargetProperty = targetProperty;
            _methodNameFieldInfo = componentBindingType.GetField("_methodName", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public void SetupMethods(bool isGenericComponentAction)
        {
            UnityEngine.Object targetObject = methodTargetProperty.objectReferenceValue;

            selectedMethodIndex = 0;

            MethodInfo[] methods = Array.Empty<MethodInfo>();
            if (targetObject != null)
            {
                Type targetObjectType = targetObject.GetType();
                methods = targetObjectType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }

            List<string> methodsNames = new List<string>() { "None" };
            List<string> methodsSignatures = new List<string> { "None" };

            Type customAttributeType = typeof(ComponentActionAttribute);
            for (int index = 0; index < methods.Length; index++)
            {
                MethodInfo info = methods[index];

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

                    methodsSignatures.Add(methodSignature);
                    methodsNames.Add(methodName);

                    break;
                }
            }

            allMethodsNames = methodsNames.ToArray();
            allMethodsSignatures = methodsSignatures.ToArray();

            string currentMethodNameValue = GetMethodNameFieldValue();

            for (int index = 0; index < allMethodsNames.Length; index++)
            {
                string methodName = allMethodsNames[index];

                if (!methodName.Equals(currentMethodNameValue)) continue;

                selectedMethodIndex = index;
            }
        }

        private string GetMethodNameFieldValue()
        {
            if (_methodNameFieldInfo == null) return null;

            return (string)_methodNameFieldInfo.GetValue(_binding);
        }

        public void SetMethodNameFieldValue(string methodName)
        {
            _methodNameFieldInfo.SetValue(_binding, methodName);
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
    }
}

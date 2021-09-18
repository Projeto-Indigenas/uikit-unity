using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UIKit.Components;
using UIKit.Components.Attributes;
using UnityEditor;
using UnityEngine;

namespace UIKit.Editor.Drawers.Reflection
{
    internal class ComponentBindingMethodProvider
    {
        private readonly ComponentBinding _binding = default;
        private readonly Type _bindingGenericType = default;
        private readonly SerializedProperty _targetProperty = default;
        private readonly FieldInfo _methodNameFieldInfo = default;

        private string[] _allMethodsNames = default;
        private string[] _allMethodsSignatures = default;
        private int _selectedMethodIndex = default;

        public ComponentBindingMethodProvider(ComponentBinding binding, Type bindingGenericType, SerializedProperty targetProperty)
        {
            _binding = binding;
            _bindingGenericType = bindingGenericType;
            Type componentBindingType = binding.GetType();

            _targetProperty = targetProperty;
            _methodNameFieldInfo = componentBindingType.GetField("_methodName", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public void SetupMethods(UnityEngine.Object targetObject, bool isGenericComponentAction)
        {
            _selectedMethodIndex = 0;

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

                    string methodSignature = GetFullMethodSignature(info, parameters);
                    string methodName = info.Name;

                    methodsSignatures.Add(methodSignature);
                    methodsNames.Add(methodName);

                    break;
                }
            }

            _allMethodsNames = methodsNames.ToArray();
            _allMethodsSignatures = methodsSignatures.ToArray();

            string currentMethodNameValue = GetMethodNameFieldValue();

            for (int index = 0; index < _allMethodsNames.Length; index++)
            {
                string methodName = _allMethodsNames[index];

                if (!methodName.Equals(currentMethodNameValue)) continue;

                _selectedMethodIndex = index;
            }
        }

        public void OnGUI(Rect movingRect, Rect position, float columnX, float columnWidth, float height,
            SerializedProperty property, bool isGenericComponentAction)
        {
            movingRect.x = position.x + 2F;
            movingRect.y += height * .5F;
            movingRect.width = position.width - 4F;
            movingRect.height = 1F;
            EditorGUI.DrawRect(movingRect, Color.black);

            movingRect.x = columnX;
            movingRect.y += 4F;
            movingRect.width = columnWidth;
            movingRect.height = height * .5F;
            EditorGUI.LabelField(movingRect, "Target ViewController: ", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            movingRect.x += movingRect.width;
            movingRect.y += 2F;
            movingRect.width = position.width - movingRect.width - 6F;
            movingRect.height = height * 0.5F;
            EditorGUI.ObjectField(movingRect, _targetProperty, typeof(ViewController), GUIContent.none);

            if (EditorGUI.EndChangeCheck() && _targetProperty.serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);

                SetupMethods(_targetProperty.objectReferenceValue, isGenericComponentAction);
            }

            movingRect.y += height * .5F;
            movingRect.x = columnX;
            movingRect.width = columnWidth;
            EditorGUI.LabelField(movingRect, "Bound to: ", EditorStyles.boldLabel);

            movingRect.y += 4F;
            movingRect.x += movingRect.width;
            movingRect.width = position.width - movingRect.width - 6F;
            int newSelection = EditorGUI.Popup(movingRect, _selectedMethodIndex, _allMethodsSignatures);

            if (newSelection != _selectedMethodIndex)
            {
                _selectedMethodIndex = newSelection;

                if (newSelection == 0) SetMethodNameFieldValue(null);
                else SetMethodNameFieldValue(_allMethodsNames[newSelection]);

                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
        }

        private string GetMethodNameFieldValue()
        {
            if (_methodNameFieldInfo == null) return null;

            return (string)_methodNameFieldInfo.GetValue(_binding);
        }

        private void SetMethodNameFieldValue(string methodName)
        {
            _methodNameFieldInfo.SetValue(_binding, methodName);
        }

        private string GetFullMethodSignature(MethodInfo info, ParameterInfo[] parameters)
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
            return $"{containingClassName}::{methodName}";
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

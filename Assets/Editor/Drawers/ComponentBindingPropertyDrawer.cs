using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIKit.Components;
using UIKit.Editor.Attributes;
using UIKit.Editor.Extensions;
using UnityEditor;
using UnityEngine;

namespace UIKit.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ComponentBinding), true)]
    class ComponentBindingPropertyDrawer : PropertyDrawer
    {
        public static bool draw = default;

        private const float _height = 50F;

        private ComponentBinding _binding = default;
        private Type _bindingGenericType = default;
        private PropertyInfo _viewPropertyInfo = default;
        private FieldInfo _propertyFieldInfo = default;
        private Component[] _allComponents = default;
        private string[] _availableComponents = default;
        private int _selectedComponentIndex = 0;

        private SerializedProperty _targetProperty = default;
        private FieldInfo _methodNameFieldInfo = default;
        private string[] _allMethods = default;
        private int _selectedMethodIndex = 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!draw) return 0F;

            if (IsComponentAction())
            {
                return _height + 60;
            }

            return _height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!draw) return;

            if (_availableComponents == null)
            {
                InitialSetup(property);
            }

            EditorGUI.DrawRect(position, GetColor());

            float columnX = position.x + 2F;
            float columnWidth = position.width * .3F;

            Rect movingRect = new Rect(columnX, position.y, columnWidth, _height * .5F);
            EditorGUI.LabelField(movingRect, "Property Name: ", EditorStyles.boldLabel);
            
            movingRect.x += movingRect.width;
            movingRect.width = position.width - movingRect.width - 6F;
            EditorGUI.LabelField(movingRect, property.displayName);
            
            movingRect.y += _height * .5F;
            movingRect.x = columnX;
            movingRect.width = columnWidth;
            EditorGUI.LabelField(movingRect, "Bound to: ", EditorStyles.boldLabel);
            
            movingRect.x += movingRect.width;
            movingRect.width = position.width - movingRect.width - 6F;
            int newSelection = EditorGUI.Popup(movingRect, _selectedComponentIndex, _availableComponents);

            if (newSelection != _selectedComponentIndex)
            {
                _selectedComponentIndex = newSelection;

                if (newSelection == 0) SetViewPropertyValue(null);
                else SetViewPropertyValue(_allComponents[newSelection - 1]);

                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            if (IsComponentAction())
            {
                movingRect.x = position.x + 2F;
                movingRect.y += _height * .5F;
                movingRect.width = position.width - 4F;
                movingRect.height = 1F;
                EditorGUI.DrawRect(movingRect, Color.black);

                movingRect.x = columnX;
                movingRect.y += 4F;
                movingRect.width = columnWidth;
                movingRect.height = _height * .5F;
                EditorGUI.LabelField(movingRect, "Target ViewController: ", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();

                movingRect.x += movingRect.width;
                movingRect.y += 2F;
                movingRect.width = position.width - movingRect.width - 6F;
                movingRect.height = _height * 0.5F;
                _targetProperty.objectReferenceValue = EditorGUI.ObjectField(
                    movingRect, "", _targetProperty.objectReferenceValue, 
                    typeof(ViewController), true);
                
                if (EditorGUI.EndChangeCheck() && _targetProperty.serializedObject.ApplyModifiedProperties())
                {
                    EditorUtility.SetDirty(property.serializedObject.targetObject);

                    SetupMethods(_targetProperty.objectReferenceValue);
                }

                movingRect.y += _height * .5F;
                movingRect.x = columnX;
                movingRect.width = columnWidth;
                EditorGUI.LabelField(movingRect, "Bound to: ", EditorStyles.boldLabel);

                movingRect.y += 4F;
                movingRect.x += movingRect.width;
                movingRect.width = position.width - movingRect.width - 6F;
                newSelection = EditorGUI.Popup(movingRect, _selectedMethodIndex, _allMethods);

                if (newSelection != _selectedMethodIndex)
                {
                    _selectedMethodIndex = newSelection;

                    if (newSelection == 0) SetMethodNameFieldValue(null);
                    else SetMethodNameFieldValue(_allMethods[newSelection]);

                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            }
        }

        private void InitialSetup(SerializedProperty property)
        {
            _targetProperty = property.FindPropertyRelative("_target");
            if (_targetProperty != null) SetupMethods(_targetProperty.objectReferenceValue);

            MonoBehaviour targetObject = (MonoBehaviour)property.serializedObject.targetObject;

            Type parentType = targetObject.GetType();
            _propertyFieldInfo = parentType.GetField(property.propertyPath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (_propertyFieldInfo.FieldType.GenericTypeArguments.Length > 0)
            {
                _bindingGenericType = _propertyFieldInfo.FieldType.GenericTypeArguments[0];
            }

            _binding = (ComponentBinding)_propertyFieldInfo.GetValue(property.serializedObject.targetObject);
            Type componentBindingType = typeof(ComponentBinding);
            _viewPropertyInfo = componentBindingType.GetProperty("view", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            componentBindingType = _binding.GetType();
            _methodNameFieldInfo = componentBindingType.GetField("_methodName", BindingFlags.Instance | BindingFlags.NonPublic);

            Transform usedForChildren = targetObject.transform.parent;
            if (usedForChildren == null) usedForChildren = targetObject.transform;

            _allComponents = usedForChildren.GetComponentsInChildren(_bindingGenericType, true);
            
            List<string> availableOptions = new List<string> { "None" };

            for (int index = 0; index < _allComponents.Length; index++)
            {
                Component component = _allComponents[index];
                string path = GetComponentPath(component.transform, targetObject.transform);
                availableOptions.Add($"{path} ({component.GetType().Name})");

                if (_binding.view == component)
                {
                    _selectedComponentIndex = index + 1;
                }
            }

            _availableComponents = availableOptions.ToArray();
        }

        private void SetupMethods(UnityEngine.Object targetObject)
        {
            _selectedMethodIndex = 0;

            MethodInfo[] methods = Array.Empty<MethodInfo>();
            if (targetObject != null)
            {
                Type parentType = targetObject.GetType();
                methods = parentType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }

            List<string> availableOptions = new List<string>() { "None" };

            Type customAttributeType = typeof(ComponentActionAttribute);
            for (int index = 0; index < methods.Length; index++)
            {
                MethodInfo info = methods[index];
                foreach (CustomAttributeData data in info.CustomAttributes)
                {
                    if (!customAttributeType.Name.Equals(data.AttributeType.Name)) continue;

                    availableOptions.Add($"{info.DeclaringType.Name}::{info.Name}");

                    _selectedMethodIndex = index + 1;

                    break;
                }
            }

            _allMethods = availableOptions.ToArray();
        }

        private void SetViewPropertyValue(Component view)
        {
            _viewPropertyInfo.SetValue(_binding, view);
        }

        private void SetMethodNameFieldValue(string methodName)
        {
            _methodNameFieldInfo.SetValue(_binding, methodName);
        }

        private Color GetColor()
        {
            if (_selectedComponentIndex == 0)
            {
                return Color.red.WithAlpha(.2F);
            }

            return Color.cyan.WithAlpha(.2F);
        }

        private static string GetComponentPath(Transform transform, Transform root)
        {
            string path = transform.name;
            Transform parent = transform;
            while (parent != null && parent != root)
            {
                parent = parent.parent;
                path = $"{parent.name}/{path}";
            }
            return path;
        }

        private bool IsComponentAction()
        {
            if (_bindingGenericType == null) return false;
            bool isAction1 = _bindingGenericType.GetInterfaces().Contains(typeof(IComponentAction));
            bool isAction2 = _bindingGenericType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IComponentAction<>));
            return (isAction1 || isAction2) && _selectedComponentIndex > 0;
        }
    }
}

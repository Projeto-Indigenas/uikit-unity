using System;
using System.Collections.Generic;
using System.Reflection;
using UIKit.Components;
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
        private PropertyInfo _viewPropertyInfo = default;
        private FieldInfo _propertyFieldInfo = default;
        private Component[] _allComponents = default;
        private string[] _availableOptions = default;
        private int _selectedIndex = 0;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return draw ? _height : 0F;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!draw) return;

            if (_availableOptions == null)
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
            EditorGUI.LabelField(movingRect, "Bound to: ");
            
            movingRect.x += movingRect.width;
            movingRect.width = position.width - movingRect.width - 6F;
            int newSelection = EditorGUI.Popup(movingRect, _selectedIndex, _availableOptions);

            if (newSelection != _selectedIndex)
            {
                _selectedIndex = newSelection;

                if (newSelection == 0)
                {
                    SetNewPropertyValue(null, property);

                    return;
                }

                int realIndex = newSelection - 1;

                SetNewPropertyValue(_allComponents[realIndex], property);
            }
        }

        private void InitialSetup(SerializedProperty property)
        {
            MonoBehaviour targetObject = (MonoBehaviour)property.serializedObject.targetObject;

            Type parentType = targetObject.GetType();
            _propertyFieldInfo = parentType.GetField(property.propertyPath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Type genericType = _propertyFieldInfo.FieldType.GenericTypeArguments[0];

            _binding = (ComponentBinding)_propertyFieldInfo.GetValue(property.serializedObject.targetObject);
            _viewPropertyInfo = typeof(ComponentBinding).GetProperty("view", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            Transform usedForChildren = targetObject.transform.parent;
            if (usedForChildren == null) usedForChildren = targetObject.transform;

            _allComponents = usedForChildren.GetComponentsInChildren(genericType, true);
            
            List<string> availableOptions = new List<string> { "None" };

            for (int index = 0; index < _allComponents.Length; index++)
            {
                Component component = _allComponents[index];
                availableOptions.Add(GetComponentPath(component.transform, targetObject.transform));

                if (_binding.view == component)
                {
                    _selectedIndex = index + 1;
                }
            }

            _availableOptions = availableOptions.ToArray();
        }

        private void SetNewPropertyValue(Component view, SerializedProperty property)
        {
            _viewPropertyInfo.SetValue(_binding, view);

            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }

        private Color GetColor()
        {
            if (_selectedIndex == 0)
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
    }
}

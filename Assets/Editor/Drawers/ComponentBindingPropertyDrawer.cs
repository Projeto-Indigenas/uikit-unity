using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIKit.Components;
using UIKit.Editor.Drawers.Reflection;
using UIKit.Editor.Extensions;
using UnityEditor;
using UnityEngine;

namespace UIKit.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ComponentBinding), true)]
    class ComponentBindingPropertyDrawer : PropertyDrawer
    {
        private const float _height = 50F;

        private static GUIStyle _whiteLargeLabel = default;

        public static bool draw = default;

        private ComponentBinding _binding = default;
        private Type _bindingGenericType = default;
        private PropertyInfo _viewPropertyInfo = default;
        private FieldInfo _propertyFieldInfo = default;
        private Component[] _allComponents = default;
        private string[] _availableComponents = default;
        private int _selectedComponentIndex = default;

        private ComponentBindingMethodProvider _methodProvider = default;

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

            if (_whiteLargeLabel == null)
            {
                _whiteLargeLabel = new GUIStyle(EditorStyles.whiteLargeLabel);
                _whiteLargeLabel.fontStyle = FontStyle.Bold;
                _whiteLargeLabel.fontSize = 18;
            }

            if (_availableComponents == null)
            {
                InitialSetup(property);
            }

            EditorGUI.DrawRect(position, GetColor());

            float columnX = position.x + 2F;
            float columnWidth = position.width * .3F;

            Rect movingRect = new Rect(columnX, position.y, position.width - 6F, _height * .5F);
            EditorGUI.LabelField(movingRect, property.displayName, _whiteLargeLabel);
            
            movingRect.y += _height * .5F;
            movingRect.x = columnX;
            movingRect.width = columnWidth;
            EditorGUI.LabelField(movingRect, "Bound to: ", EditorStyles.boldLabel);
            
            movingRect.x += movingRect.width;
            movingRect.y += 4F;
            movingRect.width = position.width - movingRect.width - 6F;
            int newSelection = EditorGUI.Popup(movingRect, _selectedComponentIndex, _availableComponents);

            if (newSelection != _selectedComponentIndex)
            {
                _selectedComponentIndex = newSelection;

                if (newSelection == 0) SetViewPropertyValue(null);
                else SetViewPropertyValue(_allComponents[newSelection - 1]);

                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            _methodProvider?.OnGUI(
                    movingRect, position,
                    columnX, columnWidth, _height,
                    property, IsGenericComponentAction());
        }

        private void InitialSetup(SerializedProperty property)
        {
            MonoBehaviour targetObject = (MonoBehaviour)property.serializedObject.targetObject;

            Type parentType = targetObject.GetType();
            _propertyFieldInfo = parentType.GetField(property.propertyPath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            if (_propertyFieldInfo.FieldType.GenericTypeArguments.Length > 0)
            {
                _bindingGenericType = _propertyFieldInfo.FieldType.GenericTypeArguments[0];
            }
            else _bindingGenericType = typeof(View);

            _binding = (ComponentBinding)_propertyFieldInfo.GetValue(property.serializedObject.targetObject);
            Type componentBindingType = typeof(ComponentBinding);
            _viewPropertyInfo = componentBindingType.GetProperty("target", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            Transform usedForChildren = targetObject.transform.parent;
            if (usedForChildren == null) usedForChildren = targetObject.transform;

            _allComponents = usedForChildren.GetComponentsInChildren(_bindingGenericType, true);
            
            List<string> availableOptions = new List<string> { "None" };

            for (int index = 0; index < _allComponents.Length; index++)
            {
                Component component = _allComponents[index];
                string path = GetComponentPath(component.transform, targetObject.transform);
                availableOptions.Add($"{path} ({component.GetType().Name})");

                if (_binding.target == component)
                {
                    _selectedComponentIndex = index + 1;
                }
            }

            _availableComponents = availableOptions.ToArray();

            SerializedProperty targetProperty = property.FindPropertyRelative("_methodTarget");
            if (targetProperty == null) return;
            _methodProvider = new ComponentBindingMethodProvider(_binding, _bindingGenericType, targetProperty);
            _methodProvider.SetupMethods(targetProperty.objectReferenceValue, IsGenericComponentAction());
        }

        private bool IsComponentAction()
        {
            if (_bindingGenericType == null) return false;
            bool isAction1 = _bindingGenericType.GetInterfaces().Contains(typeof(IComponentAction));
            bool isAction2 = IsGenericComponentAction();
            return (isAction1 || isAction2) && _selectedComponentIndex > 0;
        }

        private bool IsGenericComponentAction()
        {
            return _bindingGenericType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IComponentAction<>));
        }

        private void SetViewPropertyValue(Component view)
        {
            _viewPropertyInfo.SetValue(_binding, view);
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
            Transform current = transform;
            string name = transform.name;
            do
            {
                current = current.parent;
                if (current == null) break;
                name = $"{current.name}/{name}";
            }
            while (current && current != root);
            return name;
        }
    }
}

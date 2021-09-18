using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIKit.Components;
using UIKit.Editor.Drawers.Handlers;
using UIKit.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIKit.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ComponentBinding), true)]
    class ComponentBindingPropertyDrawer : PropertyDrawer
    {
        private const float _height = 50F;

        private static GUIStyle _whiteLargeLabel = default;

        public static bool draw = default;

        private ComponentBindingViewHandler _viewHandler = default;
        private ComponentBindingMethodHandler _methodHandler = default;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!draw) return 0F;

            if (_viewHandler?.IsComponentAction() ?? false)
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

            if (_viewHandler == null || !_viewHandler.IsSameProperty(property))
            {
                _viewHandler = new ComponentBindingViewHandler(property);
                _viewHandler.Setup();

                SerializedProperty targetProperty = property.FindPropertyRelative("_methodTarget");
                if (targetProperty != null)
                {
                    _methodHandler = new ComponentBindingMethodHandler(
                        _viewHandler.binding,
                        _viewHandler.bindingGenericType,
                        targetProperty);
                    _methodHandler.SetupMethods(_viewHandler.IsGenericComponentAction());
                }
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

            int selectedComponentIndex = _viewHandler.selectedComponentIndex;
            string[] availableComponents = _viewHandler.availableComponents;
            Component[] allComponents = _viewHandler.allComponents;

            int newSelection = EditorGUI.Popup(movingRect, selectedComponentIndex, availableComponents);

            if (newSelection != selectedComponentIndex)
            {
                _viewHandler.selectedComponentIndex = newSelection;

                if (newSelection == 0) _viewHandler.SetViewPropertyValue(null);
                else _viewHandler.SetViewPropertyValue(allComponents[newSelection - 1]);

                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            if (_methodHandler == null) return;

            ComponentBindingMethodDrawer.OnGUI(
                property, _methodHandler,
                movingRect, position,
                columnX, columnWidth, _height,
                _viewHandler.IsGenericComponentAction());
        }

        private Color GetColor()
        {
            if (_viewHandler.selectedComponentIndex == 0)
            {
                return Color.red.WithAlpha(.2F);
            }

            return Color.cyan.WithAlpha(.2F);
        }
    }
}

using UIKit.Components;
using UIKit.Editor.Drawers.Handlers;
using UIKit.Editor.Extensions;
using UnityEditor;
using UnityEngine;

namespace UIKit.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ComponentBinding), true)]
    class ComponentBindingPropertyDrawer : PropertyDrawer
    {
        private const float _foldoutHeight = 20F;

        public const float height = 60F;

        private static bool _draw = default;
        private static GUIStyle _whiteLargeLabel = default;
        private static GUIStyle _foldoutStyle = default;

        private SerializedProperty _componentActionsProperty = default;
        private ComponentBindingViewHandler _viewHandler = default;
        private ComponentBindingMethodDrawer _methodDrawer = default;
        private bool _foldout = default;

        public static void EnableDrawing() => _draw = true;
        public static void DisableDrawing() => _draw = false;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!_draw) return 0F;

            if (_viewHandler?.IsComponentAction() ?? false)
            {
                if (!_foldout)
                {
                    return height + _foldoutHeight * 1.3F;
                }

                return height + _foldoutHeight + _methodDrawer.height;
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_draw) return;

            if (_whiteLargeLabel == null)
            {
                _whiteLargeLabel = new GUIStyle(EditorStyles.whiteLargeLabel);
                _whiteLargeLabel.fontStyle = FontStyle.Bold;
                _whiteLargeLabel.fontSize = 18;
            }

            if (_foldoutStyle == null)
            {
                _foldoutStyle = new GUIStyle(EditorStyles.foldout);
                _foldoutStyle.fontSize = 14;
                _foldoutStyle.fontStyle = FontStyle.Bold;
            }

            if (_componentActionsProperty == null)
            {
                _componentActionsProperty = property.FindPropertyRelative("_componentActions");
            }

            if (_viewHandler == null || !_viewHandler.IsSameProperty(property))
            {
                _viewHandler = new ComponentBindingViewHandler(property);
                _viewHandler.Setup();

                if (_componentActionsProperty != null)
                {
                    _methodDrawer = new ComponentBindingMethodDrawer(_viewHandler, _componentActionsProperty);
                }
            }

            EditorGUI.DrawRect(position, GetColor());

            float columnX = position.x + 2F;
            float columnWidth = position.width * .3F;

            Rect movingRect = new Rect(columnX, position.y, position.width - 6F, height * .5F);

            EditorGUI.LabelField(movingRect, property.displayName, _whiteLargeLabel);

            movingRect.y += height * .5F;
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

                if (newSelection == 0) _viewHandler.SetViewPropertyValue(null, _methodDrawer);
                else _viewHandler.SetViewPropertyValue(allComponents[newSelection - 1], _methodDrawer);
            }

            if (!_viewHandler.IsComponentAction()) return;

            movingRect.x = position.x;
            movingRect.y += height * .5F;
            movingRect.width = position.width;
            movingRect.height = _foldoutHeight;

            EditorGUI.DrawRect(movingRect, Color.black.WithAlpha(.5F));

            movingRect.x += 14F;

            _foldout = EditorGUI.Foldout(movingRect, _foldout, " Events", true, _foldoutStyle);

            if (_foldout)
            {
                movingRect.x = position.x + 2F;
                movingRect.y += height * .4F;
                movingRect.width = position.width - 4F;
                movingRect.height = _foldoutHeight;

                _methodDrawer.Draw(movingRect);
            }
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

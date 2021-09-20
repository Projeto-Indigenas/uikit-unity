using System;
using System.Collections.Generic;
using UIKit.Editor.Drawers.Handlers;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UIKit.Editor.Drawers
{
    internal class ComponentBindingMethodDrawer
    {
        private const float _elementHeight = 80F;
        private const float _propertyHeight = _elementHeight * 0.3F;

        private static GUIStyle _popupStyle = default;

        private readonly List<ComponentBindingMethodHandler> _methodHandlers = default;

        private ReorderableList _reorderableList = default;
        private ComponentBindingViewHandler _viewHandler = default;
        private SerializedProperty _componentActionsProperty = default;

        public float height => _reorderableList.GetHeight();

        public ComponentBindingMethodDrawer(
            ComponentBindingViewHandler viewHandler,
            SerializedProperty componentActionsProperty)
        {
            _viewHandler = viewHandler;
            _componentActionsProperty = componentActionsProperty;

            _methodHandlers = new List<ComponentBindingMethodHandler>();
            FillMethodHandlers(viewHandler, componentActionsProperty);

            _reorderableList = new ReorderableList(_methodHandlers, typeof(ComponentBindingMethodHandler));
            _reorderableList.onAddCallback = AddListElement;
            _reorderableList.onRemoveCallback = RemoveListElement;
            _reorderableList.drawElementCallback = DrawListElement;
            _reorderableList.drawHeaderCallback = DrawListHeader;
            _reorderableList.onReorderCallbackWithDetails = ReorderListElement;

            _reorderableList.elementHeight = _elementHeight;

            if (_popupStyle == null)
            {
                _popupStyle = new GUIStyle(EditorStyles.popup)
                {
                    fontStyle = FontStyle.Bold,
                };
            }
        }

        public void Draw(Rect movingRect)
        {
            movingRect.x += 10F;
            movingRect.width -= 10F;
            _reorderableList.DoList(movingRect);
        }

        public void Clear(bool applyAndSetDirty = true)
        {
            _componentActionsProperty.ClearArray();
            _methodHandlers.Clear();

            if (!applyAndSetDirty) return;

            if (!_componentActionsProperty.serializedObject.ApplyModifiedProperties()) return;

            EditorUtility.SetDirty(_componentActionsProperty.serializedObject.targetObject);
        }

        private void DrawListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Component Actions");
        }

        private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            ComponentBindingMethodHandler methodHandler = _methodHandlers[index];

            float columnX = rect.x + 2F;
            float columnWidth = rect.width * .2F;

            Rect movingRect = rect;

            movingRect.x = columnX;
            movingRect.y += 2F;
            movingRect.width = columnWidth;
            movingRect.height = _propertyHeight;

            EditorGUI.LabelField(movingRect, "Event name: ", EditorStyles.boldLabel);

            movingRect.x += columnWidth;
            movingRect.y += 2F;
            movingRect.width = rect.width - movingRect.width - 4F;

            int selectedActionEventIndex = methodHandler.selectedActionEventNameIndex;
            string[] actionEventsNames = methodHandler.actionEventsNames;

            int newSelection = EditorGUI.Popup(movingRect, 
                selectedActionEventIndex, 
                actionEventsNames,
                _popupStyle);

            if (newSelection != selectedActionEventIndex)
            {
                methodHandler.selectedActionEventNameIndex = newSelection;
                methodHandler.SetEventNameFieldValue();
                methodHandler.SetupMethods();
            }

            movingRect.x = columnX;
            movingRect.y += _propertyHeight - 2F;
            movingRect.width = columnWidth;
            movingRect.height = _propertyHeight;

            EditorGUI.LabelField(movingRect, "Target: ", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            {
                movingRect.x += movingRect.width;
                movingRect.width = rect.width - movingRect.width - 6F;
                movingRect.height = _propertyHeight;

                methodHandler.methodTargetProperty.objectReferenceValue =
                    EditorGUI.ObjectField(movingRect, 
                    methodHandler.methodTargetProperty.objectReferenceValue,
                    typeof(MonoBehaviour), 
                    true);
            }
            if (EditorGUI.EndChangeCheck() &&
                methodHandler.methodTargetProperty.serializedObject.ApplyModifiedProperties())
            {
                methodHandler.SetupMethods();
                methodHandler.SetMethodNameFieldValue(_viewHandler.viewProperty, false);
            }

            movingRect.y += _propertyHeight;
            movingRect.x = columnX;
            movingRect.width = columnWidth;
            EditorGUI.LabelField(movingRect, "Bound to: ", EditorStyles.boldLabel);

            movingRect.y += 4F;
            movingRect.x += movingRect.width;
            movingRect.width = rect.width - movingRect.width - 6F;

            int selectedMethodIndex = methodHandler.selectedMethodIndex;
            string[] allMethodsSignatures = methodHandler.allMethodsSignatures;

            newSelection = EditorGUI.Popup(
                movingRect, selectedMethodIndex, 
                allMethodsSignatures, _popupStyle);

            if (newSelection != selectedMethodIndex)
            {
                methodHandler.selectedMethodIndex = newSelection;
                methodHandler.SetMethodNameFieldValue(_viewHandler.viewProperty);
            }
        }

        private void AddListElement(ReorderableList list)
        {
            int index = list.count;
            _componentActionsProperty.InsertArrayElementAtIndex(index);
            if (!_componentActionsProperty.serializedObject.ApplyModifiedProperties()) return;
            EditorUtility.SetDirty(_componentActionsProperty.serializedObject.targetObject);

            AddItemToMethodHandlers(_viewHandler, _componentActionsProperty, index);
        }

        private void RemoveListElement(ReorderableList list)
        {
            int index = list.index;
            _componentActionsProperty.DeleteArrayElementAtIndex(index);
            if (!_componentActionsProperty.serializedObject.ApplyModifiedProperties()) return;
            EditorUtility.SetDirty(_componentActionsProperty.serializedObject.targetObject);

            _methodHandlers.Clear();
            FillMethodHandlers(_viewHandler, _componentActionsProperty);
        }

        private void ReorderListElement(ReorderableList list, int oldIndex, int newIndex)
        {
            ComponentBindingMethodHandler element = _methodHandlers[oldIndex];
            _methodHandlers.RemoveAt(oldIndex);
            _methodHandlers.Insert(newIndex, element);

            _componentActionsProperty.MoveArrayElement(oldIndex, newIndex);
            if (!_componentActionsProperty.serializedObject.ApplyModifiedProperties()) return;
            EditorUtility.SetDirty(_componentActionsProperty.serializedObject.targetObject);
        }

        private void FillMethodHandlers(ComponentBindingViewHandler viewHandler, SerializedProperty componentActionsProperty)
        {
            for (int index = 0; index < _componentActionsProperty.arraySize; index++)
            {
                AddItemToMethodHandlers(viewHandler, componentActionsProperty, index);
            }
        }

        private void AddItemToMethodHandlers(ComponentBindingViewHandler viewHandler, SerializedProperty componentActionsProperty, int index)
        {
            ComponentBindingMethodHandler item = new ComponentBindingMethodHandler(viewHandler.bindingGenericType, 
                componentActionsProperty.GetArrayElementAtIndex(index));

            item.SetupMethods();

            _methodHandlers.Add(item);
        }
    }
}

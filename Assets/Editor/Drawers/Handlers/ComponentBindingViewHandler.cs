using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIKit.Components;
using UnityEditor;
using UnityEngine;

namespace UIKit.Editor.Drawers.Handlers
{
    internal class ComponentBindingViewHandler
    {
        private readonly UnityEngine.Object _owner = default;
        private readonly SerializedProperty _componentBindingProperty = default;

        private FieldInfo _propertyFieldInfo = default;
        private bool _initialized = default;
        private SerializedProperty _ownerProperty = default;
        private SerializedProperty _propertyNameProperty = default;

        public SerializedProperty viewProperty { get; private set; }
        public ComponentBinding binding { get; private set; }
        public Type bindingGenericType { get; private set; }

        public Component[] allComponents = default;
        public string[] availableComponents = default;
        public int selectedComponentIndex { get; set; }

        public ComponentBindingViewHandler(UnityEngine.Object owner, SerializedProperty componentBindingProperty)
        {
            _owner = owner;
            _componentBindingProperty = componentBindingProperty;
        }

        public bool IsSameProperty(SerializedProperty property)
        {
            return _componentBindingProperty.Equals(property);
        }

        public void Setup()
        {
            if (_initialized) return;

            MonoBehaviour targetObject = (MonoBehaviour)_componentBindingProperty.serializedObject.targetObject;

            Type targetObjectType = targetObject.GetType();
            _propertyFieldInfo = targetObjectType.GetField(_componentBindingProperty.propertyPath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            binding = (ComponentBinding)_propertyFieldInfo.GetValue(targetObject);
            viewProperty = _componentBindingProperty.FindPropertyRelative("_target");
            _ownerProperty = _componentBindingProperty.FindPropertyRelative("_owner");
            _propertyNameProperty = _componentBindingProperty.FindPropertyRelative("_propertyName");

            if (viewProperty.objectReferenceValue)
            {
                _ownerProperty.objectReferenceValue = _owner;
                _propertyNameProperty.stringValue = _componentBindingProperty.name;
                if (_ownerProperty.serializedObject.ApplyModifiedProperties())
                { EditorUtility.SetDirty(_ownerProperty.serializedObject.targetObject); }
            }

            if (_propertyFieldInfo.FieldType.GenericTypeArguments.Length > 0)
            {
                bindingGenericType = _propertyFieldInfo.FieldType.GenericTypeArguments[0];
            }
            else bindingGenericType = typeof(View);

            Transform usedForChildren = targetObject.transform.parent;
            if (!usedForChildren) usedForChildren = targetObject.transform;

            allComponents = usedForChildren.GetComponentsInChildren(bindingGenericType, true);

            List<string> availableOptions = new List<string> { "None" };

            for (int index = 0; index < allComponents.Length; index++)
            {
                Component component = allComponents[index];
                string path = GetComponentPath(component.transform, targetObject.transform);
                availableOptions.Add($"{path} ({component.GetType().Name})");

                if (binding == component)
                {
                    selectedComponentIndex = index + 1;
                }
            }

            availableComponents = availableOptions.ToArray();

            _initialized = true;
        }

        public bool IsComponentAction()
        {
            if (bindingGenericType == null) return false;
            bool isAction1 = bindingGenericType.GetInterfaces().Contains(typeof(IComponentAction));
            return isAction1 && selectedComponentIndex > 0;
        }

        public void SetViewPropertyValue(View view, ComponentBindingMethodDrawer drawer)
        {
            if (view)
            {
                _ownerProperty.objectReferenceValue = _owner;
                _propertyNameProperty.stringValue = _componentBindingProperty.name;
            }
            else
            {
                _ownerProperty.objectReferenceValue = null;
                _propertyNameProperty.stringValue = null;
            }

            View oldViewValue = (View)viewProperty.objectReferenceValue;
            EditorNotifier.instance.NotifyRemoving(oldViewValue, binding);

            viewProperty.objectReferenceValue = view;

            drawer?.Clear(false);

            if (!_componentBindingProperty.serializedObject.ApplyModifiedProperties()) return;

            EditorUtility.SetDirty(_componentBindingProperty.serializedObject.targetObject);
        }

        private static string GetComponentPath(Transform transform, Transform root)
        {
            Transform current = transform;
            string name = transform.name;
            do
            {
                current = current.parent;
                if (!current) break;
                name = $"{current.name}/{name}";
            }
            while (current && current != root);
            return name;
        }
    }
}

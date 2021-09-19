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
        private readonly SerializedProperty _property = default;

        private PropertyInfo _viewPropertyInfo = default;
        private FieldInfo _propertyFieldInfo = default;
        private bool _initialized = default;

        public ComponentBinding binding { get; private set; }
        public Type bindingGenericType { get; private set; }

        public Component[] allComponents = default;
        public string[] availableComponents = default;
        public int selectedComponentIndex { get; set; }

        public ComponentBindingViewHandler(SerializedProperty property)
        {
            _property = property;
        }

        public bool IsSameProperty(SerializedProperty property)
        {
            return _property.Equals(property);
        }

        public void Setup()
        {
            if (_initialized) return;

            MonoBehaviour targetObject = (MonoBehaviour)_property.serializedObject.targetObject;

            Type targetObjectType = targetObject.GetType();
            _propertyFieldInfo = targetObjectType.GetField(_property.propertyPath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            binding = (ComponentBinding)_propertyFieldInfo.GetValue(targetObject);
            Type componentBindingType = typeof(ComponentBinding);
            _viewPropertyInfo = componentBindingType.GetProperty("target", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

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

                if (binding.target == component)
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

        public void SetViewPropertyValue(Component view, ComponentBindingMethodDrawer drawer)
        {
            _viewPropertyInfo.SetValue(binding, view);

            drawer?.Clear();

            EditorUtility.SetDirty(_property.serializedObject.targetObject);
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

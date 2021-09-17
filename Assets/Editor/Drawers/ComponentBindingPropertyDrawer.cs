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

        private SerializedProperty _targetProperty = default;
        private FieldInfo _methodNameFieldInfo = default;
        private string[] _allMethodsNames = default;
        private string[] _allMethodsSignatures = default;
        private int _selectedMethodIndex = default;

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
                EditorGUI.ObjectField(movingRect, _targetProperty, typeof(ViewController), GUIContent.none);
                
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
                newSelection = EditorGUI.Popup(movingRect, _selectedMethodIndex, _allMethodsSignatures);

                if (newSelection != _selectedMethodIndex)
                {
                    _selectedMethodIndex = newSelection;

                    if (newSelection == 0) SetMethodNameFieldValue(null);
                    else SetMethodNameFieldValue(_allMethodsNames[newSelection]);

                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            }
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

                if (_binding.target == component)
                {
                    _selectedComponentIndex = index + 1;
                }
            }

            _availableComponents = availableOptions.ToArray();

            _targetProperty = property.FindPropertyRelative("_methodTarget");
            if (_targetProperty != null) SetupMethods(_targetProperty.objectReferenceValue);
        }

        private void SetupMethods(UnityEngine.Object targetObject)
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
                        parameters.Length == 0 && IsGenericComponentAction())
                    {
                        continue;
                    }

                    string methodSignature = GetFullMethodSignature(info.Name, parameters);
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

        private void SetViewPropertyValue(Component view)
        {
            _viewPropertyInfo.SetValue(_binding, view);
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

        private string GetFullMethodSignature(string methodName, ParameterInfo[] parameters)
        {
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
            return methodName;
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

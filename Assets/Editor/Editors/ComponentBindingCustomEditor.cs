﻿using System.Collections.Generic;
using UIKit.Components;
using UIKit.Editor.Drawers;
using UnityEditor;
using UnityEngine;

namespace UIKit.Editor.CustomEditors
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    internal class ComponentBindingCustomEditor : UnityEditor.Editor
    {
        private SerializedProperty[] _componentBindings = default;

        private void OnEnable()
        {
            List<SerializedProperty> properties = new List<SerializedProperty>();

            SerializedProperty iterator = serializedObject.GetIterator();
            _ = iterator.Next(true);
            
            do
            {
                if (iterator.type.Contains(typeof(ComponentBinding).Name))
                {
                    properties.Add(iterator.Copy());
                }
            } 
            while (iterator.Next(false));

            _componentBindings = properties.ToArray();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_componentBindings.Length == 0) return;

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Component Bindings", EditorStyles.boldLabel);

            ComponentBindingPropertyDrawer.EnableDrawing(serializedObject.targetObject);
            for (int index = 0; index < _componentBindings.Length; index++)
            {
                EditorGUILayout.Separator();
                SerializedProperty property = _componentBindings[index];
                _ = EditorGUILayout.PropertyField(property);
            }
            ComponentBindingPropertyDrawer.DisableDrawing();
        }
    }
}

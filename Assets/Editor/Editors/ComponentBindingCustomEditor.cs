using System.Collections.Generic;
using UIKit.Components;
using UIKit.Editor.Drawers;
using UnityEditor;

namespace UIKit.Editor.Editors
{
    [CustomEditor(typeof(ViewController), true)]
    class ComponentBindingCustomEditor : UnityEditor.Editor
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

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Component Bindings", EditorStyles.boldLabel);

            ComponentBindingPropertyDrawer.draw = true;
            for (int index = 0; index < _componentBindings.Length; index++)
            {
                EditorGUILayout.Separator();
                SerializedProperty property = _componentBindings[index];
                _ = EditorGUILayout.PropertyField(property);
            }
            ComponentBindingPropertyDrawer.draw = false;
        }
    }
}

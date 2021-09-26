using System.Collections.Generic;
using System.Reflection;
using UIKit.Components;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace UIKit.Editor.CustomEditors
{
    [CustomEditor(typeof(View), true)]
    internal class ViewCustomEditor : ComponentBindingCustomEditor
    {
        private const BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        private const float _columnWidth = 100f;

        private static readonly Dictionary<View, List<ComponentBinding>> _viewBindingMap = new Dictionary<View, List<ComponentBinding>>();
        
        private static readonly FieldInfo _ownerFieldInfo = typeof(ComponentBinding).GetField("_owner", _bindingFlags);
        private static readonly FieldInfo _propertyNameFieldInfo = typeof(ComponentBinding).GetField("_propertyName", _bindingFlags);

        [DidReloadScripts]
        public static void Initialize()
        {
            EditorNotifier.instance.Register(
                addAction: (view, binding) =>
                {
                    if (_viewBindingMap.TryGetValue(view, out List<ComponentBinding> bindings))
                    {
                        if (!bindings.Contains(binding)) bindings.Add(binding);

                        return;
                    }

                    bindings = new List<ComponentBinding> { binding };
                    _viewBindingMap[view] = bindings;
                },
                removeAction: (view, binding) =>
                {
                    if (!view || !_viewBindingMap.TryGetValue(view, out List<ComponentBinding> bindings)) return;

                    bindings.Remove(binding);
                });
        }

        public override void OnInspectorGUI()
        {
            _doNotDrawInspectorGUI = true;

            base.OnInspectorGUI();

            _doNotDrawInspectorGUI = false;

            View view = (View)target;

            int bindingsCount = 0;
            if (_viewBindingMap.TryGetValue(view, out List<ComponentBinding> bindings))
            {
                bindingsCount = bindings.Count;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUIStyle labelStyle = EditorStyles.whiteLargeLabel;
                labelStyle.fontStyle = FontStyle.Bold;
                EditorGUILayout.LabelField("Bindings", labelStyle);

                if (bindingsCount > 0) DrawBindings(bindings);
                else EditorGUILayout.LabelField("  No bindings for this view.");
            }
            EditorGUILayout.EndVertical();

            base.ComponentBindingsInspectorGUI();
        }

        private static void DrawBindings(List<ComponentBinding> bindings)
        {
            for (int index = 0; index < bindings.Count; index++)
            {
                ComponentBinding binding = bindings[index];

                Object owner = (Object)_ownerFieldInfo.GetValue(binding);
                string propertyName = (string)_propertyNameFieldInfo.GetValue(binding);

                Color previousBgColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.green;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.backgroundColor = previousBgColor;
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Property name", GUILayout.Width(_columnWidth));
                        EditorGUILayout.LabelField(propertyName, EditorStyles.boldLabel, GUILayout.Height(20F));
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Bound to:", GUILayout.Width(_columnWidth));

                        GUI.enabled = false;
                        _ = EditorGUILayout.ObjectField(GUIContent.none, owner, typeof(Object), false);
                        GUI.enabled = true;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                if (index == bindings.Count - 1) continue;

                EditorGUILayout.Separator();
            }
        }
    }
}

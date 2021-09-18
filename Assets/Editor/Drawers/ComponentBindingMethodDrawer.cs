using UIKit.Editor.Drawers.Handlers;
using UnityEditor;
using UnityEngine;

namespace UIKit.Editor.Drawers
{
    internal static class ComponentBindingMethodDrawer
    {
        public static void OnGUI(
            SerializedProperty property, 
            ComponentBindingMethodHandler provider,
            Rect movingRect, 
            Rect position, 
            float columnX, 
            float columnWidth, 
            float height,
            bool isGenericComponentAction)
        {
            movingRect.x = position.x + 2F;
            movingRect.y += height * .5F;
            movingRect.width = position.width - 4F;
            movingRect.height = 1F;

            EditorGUI.DrawRect(movingRect, Color.black);

            movingRect.x = columnX;
            movingRect.y += 4F;
            movingRect.width = columnWidth;
            movingRect.height = height * .5F;

            EditorGUI.LabelField(movingRect, "Target ViewController: ", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            {
                movingRect.x += movingRect.width;
                movingRect.y += 2F;
                movingRect.width = position.width - movingRect.width - 6F;
                movingRect.height = height * 0.5F;

                EditorGUI.ObjectField(
                    movingRect,
                    provider.methodTargetProperty,
                    typeof(Object),
                    GUIContent.none);
            }
            if (EditorGUI.EndChangeCheck() && 
                provider.methodTargetProperty.serializedObject.ApplyModifiedProperties())
            {
                provider.SetupMethods(isGenericComponentAction);
                provider.SetMethodNameFieldValue(false);
            }

            movingRect.y += height * .5F;
            movingRect.x = columnX;
            movingRect.width = columnWidth;
            EditorGUI.LabelField(movingRect, "Bound to: ", EditorStyles.boldLabel);

            movingRect.y += 4F;
            movingRect.x += movingRect.width;
            movingRect.width = position.width - movingRect.width - 6F;

            int selectedMethodIndex = provider.selectedMethodIndex;
            string[] allMethodsSignatures = provider.allMethodsSignatures;

            int newSelection = EditorGUI.Popup(movingRect, selectedMethodIndex, allMethodsSignatures);

            if (newSelection != selectedMethodIndex)
            {
                provider.selectedMethodIndex = newSelection;
                provider.SetMethodNameFieldValue();
            }
        }
    }
}

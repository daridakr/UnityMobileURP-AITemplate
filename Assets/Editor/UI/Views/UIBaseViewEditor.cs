#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
using UI.Views;
using Utilities.Attributes;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace Editors.UI
{
    /// <summary>
    /// Custom editor for UIGeminiExampleView (and potentially other BaseUIView derivatives)
    /// to validate the [BindUIElement] bindings against the attached UIDocument's UXML.
    /// </summary>
    [CustomEditor(typeof(UIBaseView), true)]
    public class UIBaseViewEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("UI Element Binding Validation", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            var inspectedView = (UIBaseView)target;
            if (inspectedView == null)
            {
                 EditorGUI.indentLevel--;
                 return;
            }

            var uiDocument = inspectedView.GetComponent<UIDocument>();
            if (uiDocument == null || uiDocument.visualTreeAsset == null)
            {
                EditorGUILayout.HelpBox("Requires a UIDocument component with a Source Asset assigned on the same GameObject to validate bindings.", MessageType.Warning);
                EditorGUI.indentLevel--;
                return;
            }

            VisualElement rootForValidation = uiDocument.visualTreeAsset.CloneTree();
            bool allBindingsValid = true;
            var validatedFields = new HashSet<string>();

            var currentType = inspectedView.GetType();
            while (currentType != null && currentType != typeof(MonoBehaviour))
            {
                 var fields = currentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                 foreach (var field in fields)
                 {
                     if (validatedFields.Contains(field.Name)) continue;

                     var attribute = field.GetCustomAttribute<BindUIElementAttribute>(false);
                     if (attribute != null)
                     {
                         validatedFields.Add(field.Name);

                         var expectedType = field.FieldType;

                         if (!typeof(VisualElement).IsAssignableFrom(expectedType))
                         {
                             EditorGUILayout.HelpBox($"Field '{field.Name}' has [BindUIElement] but its type '{expectedType.Name}' is not derived from VisualElement.", MessageType.Warning);
                             allBindingsValid = false;
                             continue;
                         }

                         VisualElement foundElement = rootForValidation.Q(attribute.ElementName);

                         if (foundElement == null)
                         {
                             EditorGUILayout.HelpBox($"Binding Error (Field: {field.Name}): Element named '{attribute.ElementName}' not found in '{uiDocument.visualTreeAsset.name}.uxml'. Check UXML 'name' attribute and C# [BindUIElement] attribute.", MessageType.Error);
                             allBindingsValid = false;
                         }
                         else if (!expectedType.IsAssignableFrom(foundElement.GetType()))
                         {
                             EditorGUILayout.HelpBox($"Binding Error (Field: {field.Name}): Element '{attribute.ElementName}' found, but it is a '{foundElement.GetType().Name}', not the expected '{expectedType.Name}'. Check field type in C# or element type in UXML.", MessageType.Error);
                             allBindingsValid = false;
                         }
                         else { EditorGUILayout.LabelField($"Field '{field.Name}' -> '{attribute.ElementName}' ({foundElement.GetType().Name}) - OK"); }
                     }
                 }
                 currentType = currentType.BaseType;
            }

            if (allBindingsValid && validatedFields.Count > 0)
            {
                 EditorGUILayout.HelpBox("All [BindUIElement] bindings appear valid for the current UXML.", MessageType.Info);
            }
            else if (validatedFields.Count == 0)
            {
                 EditorGUILayout.HelpBox("No fields with [BindUIElement(\"...\")] attribute found in this script or its ancestors up to BaseUIView.", MessageType.None);
            }

            EditorGUI.indentLevel--;
        }
    }
}
#endif
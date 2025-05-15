using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
using Utilities.Attributes;
using System;

namespace UI.Views
{
    /// <summary>
    /// Base class for MonoBehaviour Views using UI Toolkit and automatic element binding via attributes.
    /// Handles finding the root element and initializing field bindings.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public abstract class UIBaseView : MonoBehaviour
    {
        private VisualElement _root;

        protected VisualElement RootVisualElement => _root;

        public void InitializeElements()
        {
            InitializeRootElement();
            BindElements();
            
            OnElementsInitialized();
        }

        protected virtual void OnElementsInitialized() {}

        private void InitializeRootElement()
        {
            if (!TryGetComponent<UIDocument>(out var uiDocument))
            {
                Debug.LogError($"Missing UIDocument on {this.gameObject.name}. Cannot initialize view.", this);
                return;
            }

            _root = uiDocument.rootVisualElement;
            if (_root == null)
                Debug.LogError($"UIDocument on {this.gameObject.name} has no RootVisualElement (Source Asset assigned?). Cannot initialize view.", this);
        }

        private void BindElements()
        {
            if (_root == null) return;

            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<BindUIElementAttribute>();

                if (attribute != null)
                {
                    Type elementType = field.FieldType;

                    if (typeof(VisualElement).IsAssignableFrom(elementType))
                    {
                        VisualElement visualElement = RootVisualElement.Q(attribute.ElementName, className: null);

                        if (visualElement == null)
                            Debug.LogError($"View '{this.GetType().Name}': Element with name '{attribute.ElementName}' not found in UXML for field '{field.Name}'.", this);
                        else if (!elementType.IsAssignableFrom(visualElement.GetType()))
                            Debug.LogError($"View '{this.GetType().Name}': Element with name '{attribute.ElementName}' was found, but it is a '{visualElement.GetType().Name}', not the expected '{elementType.Name}' for field '{field.Name}'.", this);
                        else
                            field.SetValue(this, visualElement);
                    }
                    else
                        Debug.LogWarning($"View '{this.GetType().Name}': Field '{field.Name}' has [BindUIElement] but is not a VisualElement type ({elementType.Name}).", this);
                }
            }
        }

        public void Dispose()
        {
            OnDisposed();
        }

        protected virtual void OnDisposed() {}
    }
}
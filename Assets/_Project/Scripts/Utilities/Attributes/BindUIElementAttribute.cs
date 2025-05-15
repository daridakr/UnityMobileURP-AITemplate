using System;
using UnityEngine.Scripting;

namespace Utilities.Attributes
{
    /// <summary>
    /// Attribute used to mark fields in a View class that should be automatically bound
    /// to a VisualElement found by name in the UXML loaded by a UIDocument.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [Preserve]
    public sealed class BindUIElementAttribute : Attribute
    {
        private string _elementName;

        public string ElementName => _elementName;

        public BindUIElementAttribute(string elementName)
        {
            if (string.IsNullOrWhiteSpace(elementName))
                throw new ArgumentException("Element name cannot be null or whitespace.", nameof(elementName));

            _elementName = elementName;
        }
    }
}
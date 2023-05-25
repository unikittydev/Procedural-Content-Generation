using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PCG.Generation.Editor
{
    [CustomPropertyDrawer(typeof(GeneratorReference<,>), true)]
    public class GeneratorReferencePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty reference = property.FindPropertyRelative("reference");
            var propertyField = new PropertyField(reference);

            return propertyField;
        }
    }
}

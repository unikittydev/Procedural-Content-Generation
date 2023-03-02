using UnityEditor;
using UnityEngine.UIElements;

namespace PCG.Generation.Editor
{
    [CustomPropertyDrawer(typeof(ObjectAlternative))]
    public class ObjectAlternativeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            property.serializedObject.Update();
            
            var alternative = (ObjectAlternative)property.managedReferenceValue;

            var dropdown = new DropdownField(alternative.choiceNames, alternative.GetCurrentName(), newValue =>
            {
                alternative.SetValueFromName(newValue);
                property.serializedObject.ApplyModifiedProperties();

                return newValue;
            });
            dropdown.label = property.displayName;

            return dropdown;
        }
    }
}

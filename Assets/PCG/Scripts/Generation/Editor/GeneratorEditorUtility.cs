using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PCG.Generation.Editor
{
    public static class GeneratorEditorUtility
    {
        public static void DrawAlternativeAndValue(SerializedProperty objectAlternative, SerializedProperty property, VisualElement content)
        {
            var alternativeField = DrawPropertyField(objectAlternative, content);
            alternativeField.RegisterValueChangeCallback(evt =>
            {
                CreateNewChoiceInstance(objectAlternative, property);
                RedrawPropertyField(property, content);
            });
            
            DrawPropertyField(property, content);
        }
        
        private static void CreateNewChoiceInstance(SerializedProperty objectAlternative, SerializedProperty property)
        {
            objectAlternative.serializedObject.Update();
            
            var choice = ((ObjectAlternative)objectAlternative.managedReferenceValue).choice;
            var value = TypeMapper.CreateInstanceFromName(choice);
                
            property.managedReferenceValue = value;
            property.serializedObject.ApplyModifiedProperties();
        }
        
        public static PropertyField RedrawPropertyField(SerializedProperty property, VisualElement container)
        {
            var oldProperty = container.Q<PropertyField>(property.displayName);
            container.Remove(oldProperty);
            return DrawPropertyField(property, container);
        }
        
        public static PropertyField DrawPropertyField(SerializedProperty property, VisualElement container)
        {
            var field = new PropertyField(property)
            {
                name = property.displayName
            };
            field.BindProperty(property);
            container.Add(field);
            return field;
        }
        
    }
}
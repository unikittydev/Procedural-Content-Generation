using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCG.Generation.Editor
{
    [CustomPropertyDrawer(typeof(GenerationSettings<>))]
    public class GenerationSettingsDrawer : PropertyDrawer
    {
        private const string cofPath = "Generators/CustomFieldTree.uxml";
        
        private VisualTreeAsset COFTree;
        private VisualElement parentContainer;
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            parentContainer = new VisualElement();
            COFTree = (VisualTreeAsset)EditorGUIUtility.Load(cofPath);
            Debug.Log(COFTree);
            DrawChildrenFields(property.FindPropertyRelative("fieldTree.children"), parentContainer);
            return parentContainer;
        }
        
        private void DrawChildrenFields(SerializedProperty children, VisualElement container)
        {
            for (int i = 0; i < children.arraySize; i++)
            {
                SerializedProperty child = children.GetArrayElementAtIndex(i);
                DrawField(child, container);
            }
        }

        private void DrawField(SerializedProperty field, VisualElement container)
        {
            // Creating field
            var fieldTree = COFTree.CloneTree();
            fieldTree.Bind(field.serializedObject);
            container.Add(fieldTree);
            
            // Field label and toggle
            var toggle = fieldTree.Q<Toggle>();
            toggle.BindProperty(field.FindPropertyRelative("generate"));
            toggle.label = field.displayName;

            var children = field.FindPropertyRelative("children");
            var content = fieldTree.Query<VisualElement>("content").First();
            var fields = fieldTree.Query<VisualElement>("fields").First();

            // Content display
            content.SetDisplay(toggle.value && children != null);
            toggle.RegisterValueChangedCallback(evt => content.SetDisplay(evt.newValue));
            
            if (children != null)
                DrawChildrenFields(children, fields);
            else
                GeneratorEditorUtility.DrawAlternativeAndValue(field.FindPropertyRelative("objectAlternative"), field.FindPropertyRelative("generator"), content);
        }
    }
}
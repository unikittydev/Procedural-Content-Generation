using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCG.Generation.Editor
{
    [CustomEditor(typeof(CustomObjectGenerator<>), true)]
    public class CustomObjectGeneratorEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset COGTree;

        [SerializeField] private VisualTreeAsset COFTree;

        private SerializedProperty rootChildren;

        private SerializedProperty initialSeed;
        private SerializedProperty currentSeed;
        
        private SerializedProperty providerAlternative;
        private SerializedProperty provider;

        private void OnEnable()
        {
            rootChildren = serializedObject.FindProperty("generationTree.fieldTree.children");

            initialSeed = serializedObject.FindProperty("seed.initialSeed");
            currentSeed = serializedObject.FindProperty("seed.random.state");
            
            providerAlternative = serializedObject.FindProperty("source.providerAlternative");
            provider = serializedObject.FindProperty("source.provider");
        }

        private void OnDestroy()
        {
            serializedObject.ApplyModifiedProperties();
        }

        public override VisualElement CreateInspectorGUI()
        {
            var tree = COGTree.CloneTree();

            // Field group
            var initialSeedField = new PropertyField(initialSeed);
            var currentSeedField = new PropertyField(currentSeed, "Current Seed");
            currentSeedField.SetEnabled(false);

            var seedContent = GetContentContainer<Foldout>(tree, "seed");
            seedContent.Add(initialSeedField);
            seedContent.Add(currentSeedField);

            // Source group
            var sourceContent = GetContentContainer<Foldout>(tree, "source");
            DrawAlternativeAndValue(providerAlternative, provider, sourceContent);
            
            // Generator group
            var generatorContent = GetContentContainer<Foldout>(tree, "generator");
            DrawChildrenFields(rootChildren, generatorContent);

            // "Generate" button
            var generateButton = tree.Q<Button>("generate");
            generateButton.clicked += (serializedObject.targetObject as IGenerateButtonCallback)!.Generate;

            return tree;
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
                DrawAlternativeAndValue(field.FindPropertyRelative("objectAlternative"), field.FindPropertyRelative("generator"), content);
        }

        private static VisualElement GetContentContainer<T>(VisualElement parent, string name) where T : VisualElement
        {
            return parent.Query<T>(name).First().contentContainer;
        }
        
        private static void DrawAlternativeAndValue(SerializedProperty objectAlternative, SerializedProperty property, VisualElement content)
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
        
        private static PropertyField RedrawPropertyField(SerializedProperty property, VisualElement container)
        {
            var oldProperty = container.Q<PropertyField>(property.displayName);
            container.Remove(oldProperty);
            return DrawPropertyField(property, container);
        }
        
        private static PropertyField DrawPropertyField(SerializedProperty property, VisualElement container)
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

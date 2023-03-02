using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCG.Generation.Editor
{
    [CustomEditor(typeof(CustomGenerator<,>), true)]
    public class CustomObjectGeneratorEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset COGTree;

        private SerializedProperty initialSeed;
        private SerializedProperty currentSeed;
        
        private SerializedProperty providerAlternative;
        private SerializedProperty provider;

        private void OnEnable()
        {
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
            GeneratorEditorUtility.DrawAlternativeAndValue(providerAlternative, provider, sourceContent);
            
            // Generator group
            var generatorContent = GetContentContainer<Foldout>(tree, "generator");
            var generatorProperty = new PropertyField(serializedObject.FindProperty("generationTree"));
            generatorContent.Add(generatorProperty);

            // "Generate" button
            var generateButton = tree.Q<Button>("generate");
            generateButton.clicked += (serializedObject.targetObject as IGenerateButtonCallback)!.Generate;

            return tree;
        }

        private static VisualElement GetContentContainer<T>(VisualElement parent, string name) where T : VisualElement
        {
            return parent.Query<T>(name).First().contentContainer;
        }
    }
}

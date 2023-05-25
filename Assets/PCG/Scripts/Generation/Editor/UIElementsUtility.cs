using UnityEngine.UIElements;

namespace PCG.Generation.Editor
{
    public static class UIElementsUtility
    {
        public static void SetDisplay(this VisualElement element, bool display)
        {
            element.style.display = new StyleEnum<DisplayStyle>(display ? DisplayStyle.Flex : DisplayStyle.None);
        }
    }
}

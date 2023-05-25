using System;

namespace PCG.Generation
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class DisplayNameAttribute : Attribute
    {
        public string DisplayName { get; }
        
        public DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
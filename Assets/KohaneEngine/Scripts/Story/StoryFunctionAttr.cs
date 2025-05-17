using System;

namespace KohaneEngine.Scripts.Story
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class StoryFunctionAttr : Attribute
    
    {
        public string Name { get; }
        public string Description { get; }
        
        public StoryFunctionAttr(string name, string description = "") => (Name, Description) = (name, description);
    }
}
using System;
using System.Collections.Generic;

namespace KohaneEngine.Scripts.Structure
{
    [Serializable]
    public class KohaneStruct
    {
        public string version;
        public List<Scene> scenes = new();
    }
    
    [Serializable]
    public class Block
    {
        public string type;
        public List<object> args;
    }

    [Serializable]
    public class Scene
    {
        public string label;
        public List<Block> blocks = new();
    }
}
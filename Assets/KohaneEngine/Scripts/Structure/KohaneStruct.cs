using System;
using System.Collections.Generic;

namespace KohaneEngine.Scripts.Structure
{
    [Serializable]
    public class KohaneStruct
    {
        public string version;
        public Story story;
    }
    
    [Serializable]
    public class Block
    {
        public string type;
        public string op;
        public string name;
        public string text;
        public string audio;
        public float volume;
        public bool click;
        public string _;
        public string img;
        public int alpha;
        public List<int> pos;
        public string target;
    }

    [Serializable]
    public class Scene
    {
        public string label;
        public List<Block> blocks;
        public string inherit;
        public string _;
    }

    [Serializable]
    public class Story
    {
        public List<Scene> scenes;
    }
}
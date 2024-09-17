using System;
using System.Collections.Generic;

namespace KohaneEngine.Scripts.Structure
{
    public class YukimiStruct
    {
        [Serializable]
        public class Block
        {
            public string call;
            public List<object> args;
        }

        [Serializable]
        public class Root
        {
            public string scene;
            public List<Block> block;
        }
    }
    
}
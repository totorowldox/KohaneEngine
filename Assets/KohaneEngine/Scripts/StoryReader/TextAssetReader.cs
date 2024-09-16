using System;
using KohaneEngine.Scripts.Serializer;
using KohaneEngine.Scripts.Structure;
using UnityEngine;

namespace KohaneEngine.Scripts.StoryReader
{
    /// <summary>
    /// Read story from Unity's TextAsset
    /// </summary>
    public class TextAssetReader : IStoryReader
    {
        private readonly IKohaneRuntimeStructSerializer _serializer;
        
        public TextAssetReader(IKohaneRuntimeStructSerializer serializer)
        {
            _serializer = serializer;
        }
        
        public KohaneStruct ReadFrom(object src) => _serializer.Deserialize((src as TextAsset)?.text);
    }
}
using KohaneEngine.Scripts.Structure;
using UnityEngine;

namespace KohaneEngine.Scripts.Serializer
{
    public class JsonSerializer : IKohaneRuntimeStructSerializer
    {
        public string Serialize(KohaneStruct obj) => JsonUtility.ToJson(obj);

        public KohaneStruct Deserialize(string obj) => JsonUtility.FromJson<KohaneStruct>(obj);
    }
}
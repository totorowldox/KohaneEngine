using System;
using KohaneEngine.Scripts.Structure;
using Newtonsoft.Json;
using YukimiScript = System.Collections.Generic.List<KohaneEngine.Scripts.Structure.YukimiStruct.Root>;

namespace KohaneEngine.Scripts.Serializer
{
    public class YukimiJsonSerializer : IKohaneRuntimeStructSerializer
    {
        public string Serialize(KohaneStruct obj)
        {
            throw new InvalidOperationException("Json other than KohaneStruct cannot be serialized");
        }

        public KohaneStruct Deserialize(string obj)
        {
            var ykmScript = JsonConvert.DeserializeObject<YukimiScript>(obj);
            var ret = new KohaneStruct
            {
                version = "YukimiScript for KohaneEngine"
            };

            foreach (var scene in ykmScript)
            {
                var tempScene = new Scene()
                {
                    label = scene.scene
                };

                foreach (var block in scene.block)
                {
                    tempScene.blocks.Add(new Block()
                    {
                        type = block.call,
                        args = block.args
                    });
                }
                
                ret.scenes.Add(tempScene);
            }

            return ret;
        }
    }
}
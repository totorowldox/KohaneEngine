using KohaneEngine.Scripts.ResourceManager;
using KohaneEngine.Scripts.Structure;
using UnityEngine;

namespace KohaneEngine.Scripts.Story.Resolvers
{
    public class AudioResolver : Resolver
    {
        private readonly AudioSource _bgmSource;
        private readonly IResourceManager _resourceManager;
        
        public AudioResolver(IResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
            _bgmSource = KohaneEngine.GetComponent<AudioSource>();
            _bgmSource.playOnAwake = false;
        }
        
        public override ResolveResult Resolve(Block block)
        {
            switch (block.type)
            {
                case "bgm":
                    if (block.op == "stop")
                    {
                        Debug.Log("[Audio] Stopping BGM");
                        StopBGM();
                    }
                    else
                    {
                        PlayBGM(block.audio, block.volume);
                        Debug.Log($"[Audio] Playing BGM {block.audio}");
                    }
                    break;
                case "sfx":
                    Debug.Log($"[Audio] Playing SFX {block.audio}");
                    PlayFX(block.audio, block.volume);
                    break;
            }

            return ResolveResult.SuccessResult();
        }

        private async void PlayBGM(string id, float volume = 1)
        {
            _bgmSource.volume = volume;
            _bgmSource.clip = await _resourceManager.LoadResourceAsync<AudioClip>(string.Format(Constants.BGMPath, id));
            _bgmSource.Play();
        }

        private void StopBGM()
        {
            _bgmSource.Stop();
        }

        private async void PlayFX(string id, float volume = 1)
        {
            var sfx = await _resourceManager.LoadResourceAsync<AudioClip>(string.Format(Constants.FXPath, id));
            AudioSource.PlayClipAtPoint(sfx, Camera.main!.transform.position, volume);
        }
    }
}
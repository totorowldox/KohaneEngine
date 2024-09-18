using KohaneEngine.Scripts.ResourceManager;
using KohaneEngine.Scripts.Structure;
using KohaneEngine.Scripts.Utils;
using UnityEngine;

namespace KohaneEngine.Scripts.Story.Resolvers
{
    public class AudioResolver : Resolver
    {
        private readonly AudioSource _bgmSource;
        private readonly IResourceManager _resourceManager;
        
        public AudioResolver(IResourceManager resourceManager, KohaneBinder binder)
        {
            _resourceManager = resourceManager;
            _bgmSource = binder.bgmSource;
            _bgmSource.playOnAwake = false;
        }
        
        public override ResolveResult Resolve(Block block)
        {
            var audio = block.GetArg<string>(0);
            var volume = block.GetArg<float>(1);
            var op = block.GetArg<string>(2);
            
            switch (block.type)
            {
                case "bgm":
                    if (op == "stop")
                    {
                        Debug.Log("[Audio] Stopping BGM");
                        StopBGM();
                    }
                    else
                    {
                        PlayBGM(audio, volume);
                        Debug.Log($"[Audio] Playing BGM {audio}");
                    }
                    break;
                case "sfx":
                    Debug.Log($"[Audio] Playing SFX {audio}");
                    PlayFX(audio, volume);
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
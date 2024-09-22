using KohaneEngine.Scripts.Framework;
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
        private readonly KohaneAnimator _animator;
        
        public AudioResolver(IResourceManager resourceManager, KohaneBinder binder, KohaneAnimator animator)
        {
            _resourceManager = resourceManager;
            _animator = animator;
            _bgmSource = binder.bgmSource;
            _bgmSource.playOnAwake = false;
            _bgmSource.loop = true;
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
                        _animator.AppendCallback(StopBGM);
                    }
                    else
                    {
                        _animator.AppendCallback(() => PlayBGM(audio, volume));
                    }
                    break;
                case "sfx":
                    _animator.AppendCallback(() => PlayFX(audio, volume));
                    break;
            }

            return ResolveResult.SuccessResult();
        }

        private async void PlayBGM(string id, float volume = 1)
        {
            _bgmSource.volume = volume;
            _bgmSource.clip = await _resourceManager.LoadResourceAsync<AudioClip>(string.Format(Constants.BGMPath, id));
            _bgmSource.Play();
            Debug.Log($"[Audio] Playing BGM {id}");
        }

        private void StopBGM()
        {
            _bgmSource.Stop();
            Debug.Log("[Audio] Stopping BGM");
        }

        private async void PlayFX(string id, float volume = 1)
        {
            var sfx = await _resourceManager.LoadResourceAsync<AudioClip>(string.Format(Constants.SfxPath, id));
            AudioSource.PlayClipAtPoint(sfx, Camera.main!.transform.position, volume);
            Debug.Log($"[Audio] Playing SFX {id}");
        }
    }
}
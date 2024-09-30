using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.ResourceManager;
using KohaneEngine.Scripts.Serializer;
using KohaneEngine.Scripts.Story;
using KohaneEngine.Scripts.Story.Resolvers;
using KohaneEngine.Scripts.StoryReader;
using UnityEngine;

namespace KohaneEngine.Scripts
{
    [RequireComponent(typeof(KohaneBinder))]
    public class KohaneEngine : MonoBehaviour
    {
        public static DependencyResolver Resolver;
        public static StoryResolver StoryResolver;
        public static KohaneEngine Instance;

        [SerializeField] private TextAsset storyAsset;

        private void Awake()
        {
            Instance = this;
            Resolver = new DependencyResolver();
            StoryResolver = new StoryResolver();
            InitializeResolvers();

            // Test
            ReadStory();
        }

        private void InitializeResolvers()
        {
            Resolver.RegisterInstance(GetComponent<KohaneBinder>());
            Resolver.RegisterInstance(gameObject.AddComponent<KohaneInteractManager>());
            Resolver.Register<KohaneStateManager>();
            Resolver.Register<KohaneStoryManager>();
            Resolver.Register<KohaneAutoPlayManager>();
            Resolver.Register<KohaneAnimator>();
            Resolver.Register<IStoryReader, TextAssetReader>();
            Resolver.Register<IResourceManager, LegacyResourceManager>();

            UseYukimiScript();
        }

        private void UseYukimiScript()
        {
            Resolver.Register<IKohaneRuntimeStructSerializer, YukimiJsonSerializer>();
            
            // Register types
            StoryResolver.Register<AudioResolver>("bgm", "sfx");
            StoryResolver.Register<TextResolver>("__text_begin", "__text_type", "__text_end");
            StoryResolver.Register<CharacterResolver>("__charDefine", "__charDelete",
                "charSwitch", "charMove", "charAlpha", "charScale");
            StoryResolver.Register<BackgroundResolver>("__bgSwitch", "__bgRemove",
                "bgAlpha", "bgMove", "bgScale");
            StoryResolver.Register<EtcResolver>("startAsync", "endAsync", "wait", "waitForClick", "canSkip", "at");
            StoryResolver.Register<UIResolver>("blackScreen", "showDialogBox");
        }

        private void ReadStory()
        {
            var story = Resolver.Resolve<IStoryReader>().ReadFrom(storyAsset);
            Debug.Log($"[KohaneEngine] Read KohaneStruct! Version: {story.version}");

            Resolver.Resolve<KohaneStoryManager>().StartStory(story);
        }
    }
}
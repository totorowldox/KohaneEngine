using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.ResourceManager;
using KohaneEngine.Scripts.Serializer;
using KohaneEngine.Scripts.Story;
using KohaneEngine.Scripts.Story.Resolvers;
using KohaneEngine.Scripts.StoryReader;
using KohaneEngine.Scripts.Structure;
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
            Resolver.RegisterInstance(gameObject.AddComponent<KohaneInputManager>());
            Resolver.Register<KohaneStateManager>();
            Resolver.Register<KohaneStoryManager>();
            Resolver.Register<IKohaneRuntimeStructSerializer, YukimiJsonSerializer>();
            Resolver.Register<IStoryReader, TextAssetReader>();
            Resolver.Register<IResourceManager, LegacyResourceManager>();
            
            // Register types
            StoryResolver.Register<AudioResolver>("bgm");
            StoryResolver.Register<AudioResolver>("sfx");
            StoryResolver.Register<TextResolver>("__text_begin");
            StoryResolver.Register<TextResolver>("__text_type");
            StoryResolver.Register<TextResolver>("__text_end");
        }

        private void ReadStory()
        {
            var story = Resolver.Resolve<IStoryReader>().ReadFrom(storyAsset);
            Debug.Log($"[KohaneEngine] Read KohaneStruct! Version: {story.version}");

            Resolver.Resolve<KohaneStoryManager>().StartStory(story);
        }
    }
}
using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.Graphic.TypewriterAnimation;
using KohaneEngine.Scripts.ResourceManager;
using KohaneEngine.Scripts.Serializer;
using KohaneEngine.Scripts.Story;
using KohaneEngine.Scripts.Story.Resolvers;
using KohaneEngine.Scripts.StoryReader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KohaneEngine.Scripts
{
    [RequireComponent(typeof(KohaneBinder))]
    public class KohaneEngine : MonoBehaviour
    {
        public static DependencyResolver Resolver;
        public static StoryResolver StoryResolver;

        [SerializeField] private TextAsset storyAsset;
        [SerializeField] private bool isEditor;

        private static string _scriptFile = "";
        private static int _jumpToSceneIndex;
        private static int _jumpToLine;

        private void Awake()
        {
            Resolver = new DependencyResolver();
            StoryResolver = new StoryResolver();
            InitializeResolvers();
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
            if (!isEditor)
            {
                Resolver.Register<IStoryReader, TextAssetReader>();
            }
            else
            {
                Resolver.Register<IStoryReader, LocalFileReader>();
            }
            Resolver.Register<IResourceManager, LegacyResourceManager>();
            Resolver.Register<TypewriterAnimation, FadeDownTypewriterAnimation>();

            UseYukimiScript();
        }

        private void UseYukimiScript()
        {
            Resolver.Register<IKohaneRuntimeStructSerializer, YukimiJsonSerializer>();
            
            // Register story functions 
            StoryResolver.RegisterAllOf("KohaneEngine.Scripts.Story.Resolvers");
        }

        private void ReadStory()
        {
            var story = Resolver.Resolve<IStoryReader>().ReadFrom(isEditor ? _scriptFile : storyAsset);
            Debug.Log($"[KohaneEngine] Read KohaneStruct! Version: {story.version}");
            var storyManager = Resolver.Resolve<KohaneStoryManager>();
            storyManager.StartStory(story);
            if (_jumpToSceneIndex != -1)
            {
                storyManager.JumpToLine(_jumpToSceneIndex, _jumpToLine);
            }
        }

        public static void SetScriptFileName(string filename)
        {
            _scriptFile = filename;
        }

        public static void Restart(bool jumpingToCurrentLine = false)
        {
            if (jumpingToCurrentLine)
            {
                var storyManager = Resolver.Resolve<KohaneStoryManager>();
                _jumpToSceneIndex = storyManager.CurrentSceneIndex;
                _jumpToLine = storyManager.CurrentBlockIndex;
            }
            else
            {
                _jumpToSceneIndex = _jumpToLine = -1;
            }
            var currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}
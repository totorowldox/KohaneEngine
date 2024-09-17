using System;
using System.Collections.Generic;
using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.ResourceManager;
using KohaneEngine.Scripts.Serializer;
using KohaneEngine.Scripts.Story.Resolvers;
using KohaneEngine.Scripts.StoryReader;
using KohaneEngine.Scripts.Structure;
using UnityEngine;

namespace KohaneEngine.Scripts
{
    [RequireComponent(typeof(KohaneUIBinder))]
    public class KohaneEngine : MonoBehaviour
    {
        public static DependencyResolver Resolver;
        public static StoryResolver StoryResolver;
        public static KohaneStruct Story;
        public static KohaneEngine Instance;
        public static readonly Dictionary<Type, object> Components = new();

        [SerializeField] private TextAsset storyAsset;
        
        private void Awake()
        {
            Instance = this;
            Components.Clear();
            Resolver = new DependencyResolver();
            StoryResolver = new StoryResolver();
            InitializeResolvers();
            
            // Test
            ReadStory();
        }

        private static void InitializeResolvers()
        {
            Resolver.Register<IKohaneRuntimeStructSerializer, YukimiJsonSerializer>();
            Resolver.Register<IStoryReader, TextAssetReader>();
            Resolver.Register<IResourceManager, ResourceManager.ResourceManager>();
            
            // Register types
            StoryResolver.Register<AudioResolver>("bgm");
            StoryResolver.Register<AudioResolver>("sfx");
            StoryResolver.Register<TextResolver>("__text_begin");
            StoryResolver.Register<TextResolver>("__text_type");
            StoryResolver.Register<TextResolver>("__text_end");
        }

        public new static T GetComponent<T>() where T: Component
        {
            if (Components.ContainsKey(typeof(T))) return (T)Components[typeof(T)];
            var newComponent = Instance.gameObject.AddComponent<T>();
            Components.Add(typeof(T), newComponent);
            return newComponent;
        }

        public static KohaneUIBinder GetUIBinder()
        {
            return Instance.gameObject.GetComponent<KohaneUIBinder>();
        }

        private void ReadStory()
        {
            Story = Resolver.Resolve<IStoryReader>().ReadFrom(storyAsset);
            Debug.Log($"[KohaneEngine] Read KohaneStruct! Version: {Story.version}");

            foreach (var block in Story.scenes[0].blocks)
            {
                StoryResolver.Resolve(block);
                // try
                // {
                //     StoryResolver.Resolve(block);
                // }
                // catch(Exception ex)
                // {
                //     Debug.Log(ex);
                // }
            }
            
        }
    }
}
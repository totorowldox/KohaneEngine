using KohaneEngine.Scripts.Story;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KohaneEngine.Scripts
{
    public class KohaneBinder : MonoBehaviour
    {
        [Header("Audio")]
        public AudioSource bgmSource;
        
        [Space(10)]
        [Header("Dialog")]
        public TMP_Text speaker;
        public TMP_Text text;
        
        [Space(10)]
        [Header("Character")]
        public RawImage characterPrototype;
        public RectTransform characterParentTransform;
        
        [Space(10)]
        [Header("Image")]
        public RawImage imagePrototype;
        public RectTransform imageParentTransform;


        [Space(10)]
        [Header("Background")]
        public Image backgroundImage;

        [Space(10)]
        [Header("UI")]
        public Image blackScreenImage;
        public CanvasGroup dialogCanvasGroup;
        
        [Space(10)]
        [Header("System")]
        public Button touchArea;
        public Button autoPlay;

        private void Start()
        {
            touchArea.onClick.AddListener(() => KohaneEngine.Resolver.Resolve<KohaneInteractManager>().PerformNextStep());
            autoPlay.onClick.AddListener(() => KohaneEngine.Resolver.Resolve<KohaneAutoPlayManager>().ToggleAutoPlay());
        }

        public RawImage CreateCharacterImage() => Instantiate(characterPrototype, characterParentTransform);
        
        public RawImage CreateImage() => Instantiate(imagePrototype, imageParentTransform);
    }
}
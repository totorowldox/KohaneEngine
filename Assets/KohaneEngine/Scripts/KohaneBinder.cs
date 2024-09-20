using KohaneEngine.Scripts.Story;
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
        public Text speaker;
        public Text text;
        
        [Space(10)]
        [Header("Character")]
        public Image characterPrototype;
        public RectTransform characterParentTransform;

        [Space(10)]
        [Header("System")]
        public Button touchArea;
        public Button autoPlay;

        private void Start()
        {
            touchArea.onClick.AddListener(() => KohaneEngine.Resolver.Resolve<KohaneInteractManager>().PerformNextStep());
            autoPlay.onClick.AddListener(() => KohaneEngine.Resolver.Resolve<KohaneAutoPlayManager>().ToggleAutoPlay());
        }

        public Image CreateCharacterImage() => Instantiate(characterPrototype, characterParentTransform);
    }
}
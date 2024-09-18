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
    }
}
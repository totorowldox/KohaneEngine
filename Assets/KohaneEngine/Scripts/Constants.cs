
using UnityEngine;

namespace KohaneEngine.Scripts
{
    /// <summary>
    /// Store constants
    /// </summary>
    public static class Constants
    {
        public const string BGMPath = "KohaneEngine/BGM/{0}";
        public const string SfxPath = "KohaneEngine/SFX/{0}";

        public const string CharacterPath = "KohaneEngine/Character/{0}";

        public const float CanvasWidth = 1920;
        public const float CanvasHeight = 1080;
        
        // Delay per character
        public const float TypeAnimationSpeed = 0.02f;
        
        // Delay per block
        public const float AutoSpeed = 1f;
        public const float SkipSpeed = 0.2f;
    }
}
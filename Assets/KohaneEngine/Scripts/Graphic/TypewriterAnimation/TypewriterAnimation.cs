using TMPro;

namespace KohaneEngine.Scripts.Graphic.TypewriterAnimation
{
    public abstract class TypewriterAnimation
    {
        protected TMP_Text TextContainer;
        protected TMP_Text SpeakerContainer;
        
        /// <summary>
        /// Initialize the animation with the given speaker and text
        /// </summary>
        /// <param name="speaker">Speaker</param>
        /// <param name="text">Dialogue</param>
        public abstract void InitializeAnimation(string speaker, string text);
        
        /// <summary>
        /// Update the animation at the given phase(time)
        /// </summary>
        /// <param name="phase">Given phase</param>
        public abstract void UpdateAnimation(float phase);

        /// <summary>
        /// Evaluate the duration of the animation
        /// </summary>
        /// <param name="text">Text to type</param>
        /// <returns></returns>
        public abstract float GetDuration(string text);
    }
}
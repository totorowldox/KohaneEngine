using UnityEngine;
using UnityEngine.EventSystems;

namespace KohaneEngine.Scripts.UI
{
    public class PointerClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private int _downFrame = -1;
        private int _upFrame = -1;
        public bool IsPressed { get; private set; }

        public bool IsDown => _downFrame == Time.frameCount;

        public bool IsUp => _upFrame == Time.frameCount;

        private void OnDisable()
        {
            IsPressed = false;
            _downFrame = -1;
            _upFrame = -1;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsPressed = true;
            _downFrame = Time.frameCount;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsPressed = false;
            _upFrame = Time.frameCount;
        }
    }
}
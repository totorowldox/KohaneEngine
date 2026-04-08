using System.Collections.Generic;
using System.Linq;
using KohaneEngine.Scripts.UI;
using UnityEngine;

namespace KohaneEngine.Scripts.Framework
{
    public class KohaneInputManager : MonoBehaviour
    {
        private Dictionary<string, List<KeyCode>> _keyMappings;
        private Dictionary<string, List<PointerClick>> _pointerMappings;

        private void Awake()
        {
            _keyMappings = new Dictionary<string, List<KeyCode>>
            {
                ["next_step"] = new() {KeyCode.Space, KeyCode.Return},
                ["skip"] = new() {KeyCode.LeftControl, KeyCode.RightControl}
            };
            var binder = KohaneEngine.Resolver.Resolve<KohaneBinder>();
            _pointerMappings = new Dictionary<string, List<PointerClick>>
            {
                ["next_step"] = new() {binder.touchArea},
                ["auto"] = new() {binder.autoPlay}
            };
        }

        public bool GetInputUp(string key) => _keyMappings[key].Any(Input.GetKeyUp);
        public bool GetInputDown(string key) => _keyMappings[key].Any(Input.GetKeyDown);

        public bool GetInputPressed(string key) => _keyMappings[key].Any(Input.GetKey);

        public bool GetPointerUp(string key) => _pointerMappings[key].Any(x => x.IsUp);
    }
}
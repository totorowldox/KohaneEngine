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
            _pointerMappings = new Dictionary<string, List<PointerClick>>
            {
                ["next_step"] = new() {KohaneEngine.Resolver.Resolve<KohaneBinder>().touchArea},
                ["auto"] = new() {KohaneEngine.Resolver.Resolve<KohaneBinder>().autoPlay}
            };
        }

        public bool GetInputUp(string key) => _keyMappings[key].Any(Input.GetKeyUp);
        public bool GetInputDown(string key) => _keyMappings[key].Any(Input.GetKeyDown);

        public bool GetInputPressed(string key) => _keyMappings[key].Any(Input.GetKey);

        public bool GetPointerInput(string key) => _pointerMappings[key].Any(x => x.IsUp);
    }
}
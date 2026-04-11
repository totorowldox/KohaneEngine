using System;
using KohaneEngine.Scripts.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace KohaneEngine.Editor
{
    [RequireComponent(typeof(Graphic))]
    public class ImageInfoFetcher : MonoBehaviour
    {
        private Graphic _graphic;

        private void Awake()
        {
            _graphic = GetComponent<Graphic>();
        }

        public void LogInfo()
        {
            // get transform position/alpha/scale
            Debug.Log(
                $"RT Position: {UIUtils.CanvasPositionToScriptPosition(_graphic.rectTransform.anchoredPosition).ToString()}\n" +
                $"RT Scale: {_graphic.transform.localScale}\n" +
                $"Alpha: {_graphic.color.a}\n");
        }
    }

    [CustomEditor(typeof(ImageInfoFetcher))]
    public class ImageInfoFetcherEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
           DrawDefaultInspector();
           var fetcher = (ImageInfoFetcher)target;
           if (GUILayout.Button("Fetch"))
           {
               fetcher.LogInfo();
           }
        }
    }
}
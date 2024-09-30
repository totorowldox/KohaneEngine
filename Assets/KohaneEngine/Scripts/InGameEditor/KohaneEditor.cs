using System;
using System.Diagnostics;
using System.IO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace KohaneEngine.Scripts.InGameEditor
{
    public class KohaneEditor : MonoBehaviour
    {
        [SerializeField] private bool isEditor;
        [SerializeField] private GameObject editorCanvas;
        [SerializeField] private CanvasGroup editorPanel;
        [SerializeField] private Button editPanelExpandButton;

        [SerializeField] private InputField ykmcPathInput;
        [SerializeField] private InputField scriptPathInput;
        [SerializeField] private Button compileAndRunButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Toggle jumpToCurrentLineToggle;
        [SerializeField] private Text outputLog;
        
        private static string _ykmcPath = "";
        private static string _scriptPath = "";
        private bool _isExpanded;

        private void Start()
        {
            editorCanvas.SetActive(isEditor);
            ykmcPathInput.text = _ykmcPath;
            scriptPathInput.text = _scriptPath;
            editPanelExpandButton.onClick.AddListener(OnClick);
            compileAndRunButton.onClick.AddListener(CompileAndRun);
            restartButton.onClick.AddListener(Restart);
        }

        private void OnClick()
        {
            _isExpanded = !_isExpanded;
            editorPanel.DOFade(_isExpanded ? 1 : 0, 0.5f);
            editorPanel.interactable = _isExpanded;
            editorPanel.blocksRaycasts = _isExpanded;
        }

        private void CompileAndRun()
        {
            outputLog.text = "";
            
            if (string.IsNullOrEmpty(ykmcPathInput.text) || string.IsNullOrEmpty(scriptPathInput.text))
            {
                outputLog.text = "<color=red>Paths cannot be empty.</color>";
                return;
            }

            _ykmcPath = ykmcPathInput.text;
            _scriptPath = scriptPathInput.text;

            var outputJsonPath = Path.Combine(
                Path.GetDirectoryName(_scriptPath) ?? string.Empty,
                $"{Path.GetFileNameWithoutExtension(_scriptPath)}.json"
            );

            var processStartInfo = new ProcessStartInfo
            {
                FileName = _ykmcPath,
                Arguments = $"\"{_scriptPath}\" --target-json \"{outputJsonPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using var process = Process.Start(processStartInfo);
                var output = process!.StandardOutput.ReadToEnd();
                var errors = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                {
                    outputLog.text = output;
                }

                if (!string.IsNullOrEmpty(errors))
                {
                    outputLog.text += $"<color=red>{errors}</color>";
                }
            }
            catch (Exception ex)
            {
                outputLog.text = $"<color=red>An error occurred while executing the process: {ex.Message}</color>";
                return;
            }

            KohaneEngine.SetScriptFileName(outputJsonPath);
            KohaneEngine.Restart(jumpToCurrentLineToggle.isOn);
        }

        private void Restart()
        {
            KohaneEngine.Restart(jumpToCurrentLineToggle.isOn);
        }
    }
}
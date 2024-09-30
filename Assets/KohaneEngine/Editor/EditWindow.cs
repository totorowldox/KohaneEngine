using System;
using System.IO;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace KohaneEngine.Editor
{
    public class EditWindow : EditorWindow
    {
        private static string ykmcPath;
        private static string scriptPath;

        [MenuItem("KohaneEngine/Edit")]
        public static void ShowWindow()
        {
            var window = GetWindow<EditWindow>("KohaneEngine");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            // Ykmc Path Input and Browse Button
            ykmcPath = TextFieldWithBrowseButton("Ykmc path: ", ykmcPath, "Select Ykmc binary", "exe");

            // Script Path Input and Browse Button
            scriptPath = TextFieldWithBrowseButton("Ykm Script path: ", scriptPath, "Select Ykm Script", "ykm");

            // Compile and Run Button
            if (GUILayout.Button("Compile and Run", GUILayout.Width(500), GUILayout.Height(30)))
            {
                CompileAndRun();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private static string TextFieldWithBrowseButton(string label, string path, string dialogTitle, string extension)
        {
            GUILayout.BeginHorizontal();
            path = EditorGUILayout.TextField(label, path);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                var selectedPath = EditorUtility.OpenFilePanel(dialogTitle, "", extension);
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    path = selectedPath;
                }
            }

            GUILayout.EndHorizontal();
            return path;
        }


        private static void CompileAndRun()
        {
            if (string.IsNullOrEmpty(ykmcPath) || string.IsNullOrEmpty(scriptPath))
            {
                Debug.LogError("Paths cannot be empty.");
                return;
            }

            var outputJsonPath = Path.Combine(
                Path.GetDirectoryName(scriptPath) ?? string.Empty,
                $"{Path.GetFileNameWithoutExtension(scriptPath)}.json"
            );

            var processStartInfo = new ProcessStartInfo
            {
                FileName = ykmcPath,
                Arguments = $"\"{scriptPath}\" --target-json \"{outputJsonPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using var process = Process.Start(processStartInfo);
                var output = process.StandardOutput.ReadToEnd();
                var errors = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                {
                    Debug.Log(output);
                }

                if (!string.IsNullOrEmpty(errors))
                {
                    Debug.LogError(errors);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while executing the process: {ex.Message}");
                return;
            }
            
            Scripts.KohaneEngine.SetScriptFileName(outputJsonPath);
            EditorApplication.isPlaying = true;
        }
    }
}
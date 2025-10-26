#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class WebGLBuilder
{
    private const string OutputFolder = "Builds/WebGL";

    [MenuItem("Build/Build WebGL" , false, 2000)]
    public static void BuildWebGL()
    {
        string outputPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", OutputFolder));
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            Debug.LogError("No scenes are enabled in Build Settings.");
            return;
        }

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"WebGL build succeeded. Output path: {outputPath}");
        }
        else
        {
            Debug.LogError($"WebGL build failed: {report.summary.result}");
        }
    }
}
#endif

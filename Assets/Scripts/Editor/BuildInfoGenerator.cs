using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System;
using System.Globalization;
using System.IO;
using UnityEditor;

public sealed class BuildInfoGenerator : IPreprocessBuildWithReport
{
    private const string OutputPath = "Assets/Resources/BuildInfo.txt";

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string buildTime = DateTime.Now.ToString("MMM dd yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        string commit = ReadGitShortHash();
        string platform = report.summary.platform.ToString();

        string info = $"{Application.productName} {Application.version}  {buildTime}  {commit}  {platform}";

        Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));
        File.WriteAllText(OutputPath, info);
        AssetDatabase.ImportAsset(OutputPath);
    }

    private static string ReadGitShortHash()
    {
        try
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo("git", "rev-parse --short HEAD")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = System.Diagnostics.Process.Start(startInfo);
            string hash = process!.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
            return string.IsNullOrEmpty(hash) ? "unknown" : hash;
        }
        catch
        {
            return "unknown";
        }
    }
}

// -------------------------------------------------------------------------------------------------
// Assets/Editor/JenkinsBuild.cs
// -------------------------------------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Build.Reporting;

// ------------------------------------------------------------------------
// https://docs.unity3d.com/Manual/CommandLineArguments.html
// ------------------------------------------------------------------------
public class JenkinsBuild
{
    static string[] EnabledScenes = FindEnabledEditorScenes();

    // ------------------------------------------------------------------------
    // Build IOS Script called from Jenkins
    // ------------------------------------------------------------------------
    public static void BuildIOS()
    {
        string appName = "AppName";
        string targetDir = "~/Desktop";

        // find: -executeMethod
        //   +1: JenkinsBuild.BuildIOS
        //   +2: BombEscort
        //   +3: /Users/Shared/Jenkins/Home/jobs/BombEscort/builds/47/output
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-executeMethod")
            {
                try
                {
                    appName = args[i + 2];
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(
                        "[JenkinsBuild] Incorrect Parameters for -executeMethod Format: -executeMethod BuildIOS <app_name> <output_dir>");
                    Debug.Log("Bug editor: " + ex.Message);
                }

                try
                {
                    targetDir = args[i + 3];
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(
                        "[JenkinsBuild] Incorrect Parameters for -executeMethod Format: -executeMethod BuildIOS <app_name> <output_dir>");
                    Debug.Log("Bug editor: " + ex.Message);
                }

                System.Console.WriteLine("AppName: " + appName + ", target: " + targetDir);
            }
        }

        // e.g. // /Users/Shared/Jenkins/Home/jobs/BombEscort/builds/47/output/BombEscort.app
        // string fullPathAndName = targetDir + System.IO.Path.DirectorySeparatorChar + appName + ".app";
        string fullPathAndName = targetDir + System.IO.Path.DirectorySeparatorChar + appName;
        BuildProject(EnabledScenes, fullPathAndName, BuildTargetGroup.iOS, BuildTarget.iOS, BuildOptions.None);
    }

    // ------------------------------------------------------------------------
    // Build Android Script called from Jenkins
    // ------------------------------------------------------------------------
    public static void BuildAndroid()
    {
        string appName = "AppName";
        string targetDir = "~/Desktop";

        // find: -executeMethod
        //   +1: JenkinsBuild.BuildAndroid
        //   +2: BombEscort
        //   +3: /Users/Shared/Jenkins/Home/jobs/BombEscort/builds/47/output
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-executeMethod")
            {
                try
                {
                    appName = args[i + 2];
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(
                        "[JenkinsBuild] Incorrect Parameters for -executeMethod Format: -executeMethod BuildAndroid <app_name> <output_dir>");
                    Debug.Log("Bug editor: " + ex.Message);
                }

                try
                {
                    targetDir = args[i + 3];
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(
                        "[JenkinsBuild] Incorrect Parameters for -executeMethod Format: -executeMethod BuildAndroid <app_name> <output_dir>");
                    Debug.Log("Bug editor: " + ex.Message);
                }

                System.Console.WriteLine("AppName: " + appName + ", target: " + targetDir);
            }
        }

        // e.g. // /Users/Shared/Jenkins/Home/jobs/BombEscort/builds/47/output/BombEscort.app
        // string fullPathAndName = targetDir + System.IO.Path.DirectorySeparatorChar + appName + ".app";
        string fullPathAndName = targetDir + System.IO.Path.DirectorySeparatorChar + appName;
        BuildProject(EnabledScenes, fullPathAndName, BuildTargetGroup.Android, BuildTarget.Android,
            BuildOptions.None);
    }

    // ------------------------------------------------------------------------
    // Build WebGL Script called from Jenkins
    // ------------------------------------------------------------------------
    public static void BuildWebGL()
    {
        string appName = "AppName";
        string targetDir = "~/WebGL";
        BuildOptions buildOptions = BuildOptions.None;
        // find: -executeMethod
        //   +1: JenkinsBuild.WebGL
        //   +2: BombEscort
        //   +3: /Users/Shared/Jenkins/Home/jobs/BombEscort/builds/47/output
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-executeMethod")
            {
                try
                {
                    appName = args[i + 2];
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(
                        "[JenkinsBuild] Incorrect Parameters for -executeMethod Format: -executeMethod BuildWebGL <app_name> <output_dir>");
                    Debug.Log("Bug editor: " + ex.Message);
                }

                try
                {
                    targetDir = args[i + 3];
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(
                        "[JenkinsBuild] Incorrect Parameters for -executeMethod Format: -executeMethod BuildWebGL <app_name> <output_dir>");
                    Debug.Log("Bug editor: " + ex.Message);
                }

                System.Console.WriteLine("AppName: " + appName + ", target: " + targetDir);
            }
            if (args[i] == "-development")
            {
                buildOptions = BuildOptions.Development;
                System.Console.WriteLine("[JenkinBuild] Make a development build");
            }    
        }
        // e.g. // /Users/Shared/Jenkins/Home/jobs/BombEscort/builds/47/output/BombEscort.app
        // string fullPathAndName = targetDir + System.IO.Path.DirectorySeparatorChar + appName + ".app";
        string fullPathAndName = targetDir + System.IO.Path.DirectorySeparatorChar + appName;
        BuildProject(EnabledScenes, fullPathAndName, BuildTargetGroup.WebGL, BuildTarget.WebGL, buildOptions);
    }

    // ------------------------------------------------------------------------
    // ------------------------------------------------------------------------
    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                EditorScenes.Add(scene.path);
            }
        }

        return EditorScenes.ToArray();
    }

    // ------------------------------------------------------------------------
    // e.g. BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX
    // ------------------------------------------------------------------------
    private static void BuildProject(string[] scenes, string targetDir, BuildTargetGroup buildTargetGroup,
        BuildTarget buildTarget, BuildOptions buildOptions)
    {
        System.Console.WriteLine("[JenkinsBuild] Building:" + targetDir + " buildTargetGroup:" +
                                 buildTargetGroup.ToString() + " buildTarget:" + buildTarget.ToString());

        // https://docs.unity3d.com/ScriptReference/EditorUserBuildSettings.SwitchActiveBuildTarget.html
        bool switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
        if (switchResult)
        {
            System.Console.WriteLine("[JenkinsBuild] Successfully changed Build Target to: " +
                                     buildTarget.ToString());
        }
        else
        {
            System.Console.WriteLine("[JenkinsBuild] Unable to change Build Target to: " + buildTarget.ToString() +
                                     " Exiting...");
            return;
        }

        // Execute default AddressableAsset content build, if the package is installed.
        // Version defines would be the best solution here, but Unity 2018 doesn't support that,
        // so we fall back to using reflection instead.
        var addressableAssetSettingsType = Type.GetType(
            "UnityEditor.AddressableAssets.Settings.AddressableAssetSettings,Unity.Addressables.Editor");
        if (addressableAssetSettingsType != null)
        {
            // ReSharper disable once PossibleNullReferenceException, used from try-catch
            try
            {
                System.Console.WriteLine("[JenkinsBuild] Building addressable: " +
                                         buildTarget.ToString());
                addressableAssetSettingsType.GetMethod("CleanPlayerContent", BindingFlags.Static | BindingFlags.Public)
                    .Invoke(null, new object[] { null });
                addressableAssetSettingsType.GetMethod("BuildPlayerContent", new Type[0]).Invoke(null, new object[0]);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to run default addressables build:\n{e}");
            }
        }

        // https://docs.unity3d.com/ScriptReference/BuildPipeline.BuildPlayer.html
        BuildReport buildReport = BuildPipeline.BuildPlayer(scenes, targetDir, buildTarget, buildOptions);
        BuildSummary buildSummary = buildReport.summary;
        if (buildSummary.result == BuildResult.Succeeded)
        {
            System.Console.WriteLine("[JenkinsBuild] Build Success: Time:" + buildSummary.totalTime + " Size:" +
                                     buildSummary.totalSize + " bytes");
        }
        else
        {
            System.Console.WriteLine("[JenkinsBuild] Build Failed: Time:" + buildSummary.totalTime +
                                     " Total Errors:" + buildSummary.totalErrors);
        }
    }
}

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;

public class iOSBuildPostProcess
{
#if UNITY_IOS
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string buildPath)
    {
        if (target != BuildTarget.iOS) return;

        string projectPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
        PBXProject project = new PBXProject();
        project.ReadFromFile(projectPath);

        string mainTarget = project.GetUnityMainTargetGuid();
        string frameworkTarget = project.GetUnityFrameworkTargetGuid();

        project.SetBuildProperty(mainTarget, "CODE_SIGN_STYLE", "Automatic");
        project.SetBuildProperty(frameworkTarget, "CODE_SIGN_STYLE", "Automatic");
        project.SetBuildProperty(mainTarget, "DEVELOPMENT_TEAM", "D96LNKD98X");
        project.SetBuildProperty(frameworkTarget, "DEVELOPMENT_TEAM", "D96LNKD98X");
        project.SetBuildProperty(mainTarget, "CODE_SIGN_IDENTITY", "Apple Development");
        project.SetBuildProperty(frameworkTarget, "CODE_SIGN_IDENTITY", "Apple Development");

        project.WriteToFile(projectPath);
        UnityEngine.Debug.Log("iOS Signing settings applied!");
    }
#endif
}
#endif

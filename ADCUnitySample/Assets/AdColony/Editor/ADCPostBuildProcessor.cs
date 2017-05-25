#if UNITY_IOS || UNITY_ANDROID

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.IO;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace AdColony.Editor {
    public class ADCPostBuildProcessor : MonoBehaviour {

#if UNITY_CLOUD_BUILD
        public static void OnPostprocessBuildiOS(string exportPath) {
            ProcessPostBuild(BuildTarget.iOS, exportPath);
        }
#endif

        [PostProcessBuildAttribute(1)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath) {
            if (buildTarget == BuildTarget.iOS) {
#if !UNITY_CLOUD_BUILD
#if UNITY_IOS
                Debug.Log("AdColony: OnPostprocessBuild");
                UpdateProject(buildTarget, buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj");
                UpdateProjectPlist(buildTarget, buildPath + "/Info.plist");
#endif
#endif
            }
        }

        private static void UpdateProject(BuildTarget buildTarget, string projectPath) {
#if UNITY_IOS
            PBXProject project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projectPath));

            string targetId = project.TargetGuidByName(PBXProject.GetUnityTargetName());

            // Required Frameworks
            project.AddFrameworkToProject(targetId, "AudioToolbox.framework", false);
            project.AddFrameworkToProject(targetId, "AVFoundation.framework", false);
            project.AddFrameworkToProject(targetId, "CoreGraphics.framework", false);
            project.AddFrameworkToProject(targetId, "CoreTelephony.framework", false);
            project.AddFrameworkToProject(targetId, "CoreMedia.framework", false);
            project.AddFrameworkToProject(targetId, "EventKit.framework", false);
            project.AddFrameworkToProject(targetId, "EventKitUI.framework", false);
            project.AddFrameworkToProject(targetId, "MediaPlayer.framework", false);
            project.AddFrameworkToProject(targetId, "MessageUI.framework", false);
            project.AddFrameworkToProject(targetId, "QuartzCore.framework", false);
            project.AddFrameworkToProject(targetId, "SystemConfiguration.framework", false);
            project.AddFrameworkToProject(targetId, "Security.framework", false);
            project.AddFrameworkToProject(targetId, "JavaScriptCore.framework", false);

            project.AddFileToBuild(targetId, project.AddFile("usr/lib/libz.1.2.5.dylib", "Frameworks/libz.1.2.5.dylib", PBXSourceTree.Sdk));

            // Optional Frameworks
            project.AddFrameworkToProject(targetId, "AdSupport.framework", true);
            project.AddFrameworkToProject(targetId, "Social.framework", true);
            project.AddFrameworkToProject(targetId, "StoreKit.framework", true);
            project.AddFrameworkToProject(targetId, "Webkit.framework", true);

            // For 3.0 MP classes
            project.AddBuildProperty(targetId, "OTHER_LDFLAGS", "-ObjC");

            File.WriteAllText(projectPath, project.WriteToString());
#endif
        }

        private static void UpdateProjectPlist(BuildTarget buildTarget, string plistPath) {
#if UNITY_IOS
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));


            // NO special plist modifications in this version


            File.WriteAllText(plistPath, plist.WriteToString());
#endif
        }
    }
}

#endif

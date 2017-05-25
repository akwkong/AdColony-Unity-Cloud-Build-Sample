#if UNITY_IOS

using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class ADCUnityPostBuildProcessor {

	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath) {
		if (buildTarget == BuildTarget.iOS) {
			UpdateProject(buildTarget, buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj");
			UpdateProjectPList(buildTarget, buildPath + "/Info.plist");
		}
	}

	private static void UpdateProject(BuildTarget buildTarget, string projectPath) {
		PBXProject project = new PBXProject();
		project.ReadFromString(File.ReadAllText(projectPath));

		string targetId = project.TargetGuidByName(PBXProject.GetUnityTargetName());

		// Other Linker Flags - The SDK in the sample app needs this to work.
		project.AddBuildProperty(targetId, "OTHER_LDFLAGS", "-ObjC");

		File.WriteAllText(projectPath, project.WriteToString());
	}

	private static void UpdateProjectPList(BuildTarget buildTarget, string plistPath) {
		PlistDocument plist = new PlistDocument();
		plist.ReadFromString(File.ReadAllText(plistPath));

		PlistElementDict rootDict = plist.root;

		// Setup ATS (App Transport Security) in the plist file
		PlistElementDict atsDict = rootDict.CreateDict("NSAppTransportSecurity");
		atsDict.SetBoolean("NSAllowsLocalNetworking", true);

		File.WriteAllText(plistPath, plist.WriteToString());
	}
}

#endif
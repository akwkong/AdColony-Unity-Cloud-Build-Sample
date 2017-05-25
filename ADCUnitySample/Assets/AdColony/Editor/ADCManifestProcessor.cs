using System;
using System.Text;
using UnityEditor;
using System.Linq;
using UnityEngine;
using System.IO;

namespace AdColony.Editor {
    [InitializeOnLoad]
    public static class ADCManifestProcessor {
        private const string templateManifest = "AndroidManifestTemplate.xml";
        private const string manifest = "AndroidManifest.xml";

        static ADCManifestProcessor() {
            Process();
        }

        public static void CheckMinSDKVersion() {
            if (PlayerSettings.Android.minSdkVersion < ADCPluginInfo.RequiredAndroidVersion) {
                UnityEngine.Debug.LogError("AdColony requires " + ADCPluginInfo.RequiredAndroidVersion + " in PlayerSettings");
            }
        }

        public static void Process() {
            CheckMinSDKVersion();

            string outputPath = Path.Combine(Application.dataPath, "Plugins/Android/AdColony");
            string inputPath = Path.Combine(Application.dataPath, "AdColony/Editor");

            string original = Path.Combine(inputPath, ADCManifestProcessor.templateManifest);
            string manifest = Path.Combine(outputPath, ADCManifestProcessor.manifest);

            if (!File.Exists(original)) {
                UnityEngine.Debug.Log("AdColony manifest template missing in folder: " + inputPath);
                return;
            }

            if (File.Exists(manifest)) {
                File.Delete(manifest);
            }

            File.Copy(original, manifest);

            StreamReader sr = new StreamReader(manifest);
            string body = sr.ReadToEnd();
            sr.Close();


            // No template manipulations needed in this version


            using (var wr = new StreamWriter(manifest, false)) {
                wr.Write(body);
            }
        }
    }
}

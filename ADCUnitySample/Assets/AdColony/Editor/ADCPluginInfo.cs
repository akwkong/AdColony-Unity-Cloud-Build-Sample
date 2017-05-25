namespace AdColony.Editor {
    public class ADCPluginInfo {
        public const string Version = "3.1.0";
        public const string AndroidSDKVersion = "3.1.2";
        public const string iOSSDKVersion = "3.1.1";
        #if UNITY_5_6
        public const UnityEditor.AndroidSdkVersions RequiredAndroidVersion = UnityEditor.AndroidSdkVersions.AndroidApiLevel16;
        #else
        public const UnityEditor.AndroidSdkVersions RequiredAndroidVersion = UnityEditor.AndroidSdkVersions.AndroidApiLevel14;
        #endif
        public const string Name = "AdColony";
    }
}

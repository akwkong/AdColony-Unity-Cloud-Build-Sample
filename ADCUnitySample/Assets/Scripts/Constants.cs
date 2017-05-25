using UnityEngine;
using System.Collections;

public class Constants : MonoBehaviour {

	// AdColony App ID and Zone IDs.
#if UNITY_IOS
	public static string AppID = "appbdee68ae27024084bb334a";
	public static string InterstitialZoneID = "vzf8fb4670a60e4a139d01b5";
	public static string CurrencyZoneID = "vzf8e4e97704c4445c87504e";
#elif UNITY_ANDROID
	public static string AppID = "app185a7e71e1714831a49ec7";
	public static string InterstitialZoneID = "vz06e8c32a037749699e7050";
	public static string CurrencyZoneID = "vz1fd5a8b2bf6841a0a4b826";
#else
	public static string AppID = "dummyAppID";
	public static string InterstitialZoneID = "dummyInterstitialZoneID";
	public static string CurrencyZoneID = "dummyCurrencyZoneID";
#endif

	// Game Object Tags.
	public static string GameControllerTag = "GameController";
	public static string AsteroidConfigureTag = "Asteroid Configure";
	public static string AsteroidRequestTag = "Asteroid Request";
	public static string AsteroidPlayTag = "Asteroid Play";
	public static string RocketTag = "Rocket";
}

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class BuildScript {

	static string[] SCENES = FindEnabledEditorScenes();
	static string APP_NAME = "TeddyPlanet";
	static string TARGET_DIR = "build";

	[MenuItem("Custom/CI/Staging Build - Android")]
	static void PerformAndroidStagingBuild() {

		Debug.Log("-- 안드로이드 스테이징 빌드를 실행합니다.");

		string target_filename = APP_NAME + " Staging.apk";

		char sep = Path.DirectorySeparatorChar;
		string buildDirectory = Path.GetFullPath(".") + sep + TARGET_DIR;
		Directory.CreateDirectory(buildDirectory);

		string BUILD_TARGET_PATH = buildDirectory + "/android";
		Directory.CreateDirectory(BUILD_TARGET_PATH);

		BUILD_TARGET_PATH = BUILD_TARGET_PATH + sep + target_filename;

		Debug.Log("-- 빌드 PATH: " + BUILD_TARGET_PATH);

		GenericBuild(SCENES, BUILD_TARGET_PATH, BuildTarget.Android, BuildOptions.None);
	}

	[MenuItem("Custom/CI/Build IOS Debug")]
	static void PerformIOSDebugBuild() {

		BuildOptions opt = BuildOptions.SymlinkLibraries | BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging | BuildOptions.AcceptExternalModificationsToPlayer;

		PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
		PlayerSettings.iOS.targetOSVersion = iOSTargetOSVersion.iOS_4_3;
		PlayerSettings.statusBarHidden = true;

		char sep = Path.DirectorySeparatorChar;
		string buildDirectory = Path.GetFullPath(".") + sep + TARGET_DIR;
		Directory.CreateDirectory(buildDirectory);

		string BUILD_TARGET_PATH = buildDirectory + "/ios";
		Directory.CreateDirectory(BUILD_TARGET_PATH);

		GenericBuild(SCENES, BUILD_TARGET_PATH, BuildTarget.iPhone, opt);
	}

	private static string[] FindEnabledEditorScenes() {

		Debug.Log("-- Enabled 되어진 씬 오브젝트 검색을 시작합니다.");

		List<string> EditorScenes = new List<string>();
		foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {

			if (!scene.enabled)
				continue;

			Debug.Log("-- 씬 오브젝트 발견: " + scene.path);
			EditorScenes.Add(scene.path);
		}

		Debug.Log("--" + EditorScenes.Capacity + "개의 씬 오브젝트를 발견하였습니다.");
		return EditorScenes.ToArray();
	}

	static void GenericBuild(string[] scenes, string target_filename, BuildTarget build_target, BuildOptions build_options) {

		EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
		Debug.Log("-- 빌드 과정을 시작합니다...");
		string res = BuildPipeline.BuildPlayer(scenes, target_filename, build_target, build_options);
		if (res.Length > 0) {

			throw new Exception("빌드 실패: " + res);
		}
	}
}

using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.SceneManagement;

using UnityEngine;

namespace SceneLoadingWindow
{
   public partial class SceneWindow : EditorWindow
   {
      private string startScenePath;
      private SceneAsset startSceneAsset;
      //private IList<SceneAsset> allScenes;
      private IList<(string name, string path)> scenesNotInBuild = new List<(string, string)>();

      private string startScenePath_SavedKey => $"{Application.productName}.SceneWindow.startScene";

      private bool firstOpen = true;
      private Vector2 scrollView_BuildScenes;
      private Vector2 scrollView_NotInBuildScenes;
      private AnimBool animBool_NotInBuild = new AnimBool();

      [MenuItem("Tools/Scene Loading Window &#e")]
      //----------------------------------------------------------------------------------------------------
      private static void Init()
      {
         var window = GetWindow<SceneWindow>("Scene Loading Window");
         window.Show();
      }

      ///---------------------------------------------------------------------------------------------------
      private void OnDisable()
      {
         firstOpen = true;
      }

      ///---------------------------------------------------------------------------------------------------
      private void OnGUI()
      {
         if (firstOpen)
         {
            firstOpen = false;

            GenerateGUIContent();
            GenerateGUIStyles();
            FillData();

#if UNITY_2019_3_OR_NEWER
            animBool_NotInBuild = new AnimBool(false);
            animBool_NotInBuild.valueChanged.AddListener(Repaint);
#else
            animBool_NotInBuild = new AnimBool(true);
#endif
         }

         using (new GUILayout.HorizontalScope(EditorStyles.helpBox)) //-----
         {
            EditorGUILayout.LabelField(playModeScene_Content);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
               startSceneAsset = (SceneAsset)EditorGUILayout.ObjectField(startSceneAsset, typeof(SceneAsset), false);

               if (check.changed)
               {
                  EditorPrefs.SetString(startScenePath_SavedKey, AssetDatabase.GetAssetPath(startSceneAsset));
                  EditorSceneManager.playModeStartScene = startSceneAsset;
               }
            }
         } //-----

         DrawScenes();

         DrawScenesNotInBuild();
      }

      //----------------------------------------------------------------------------------------------------
      private void DrawScenes()
      {
         EditorGUILayout.LabelField("Scenes In Build", EditorStyles.boldLabel);

         using (var scroll = new GUILayout.ScrollViewScope(scrollView_BuildScenes, GUILayout.MaxHeight(Screen.height * 0.66f))) //-----
         {
            scrollView_BuildScenes = scroll.scrollPosition;

            int index = 0;
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
               EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
               DrawEditorBuildScene(scene, i, index);
               if (scene.enabled) index++;
            }
         } //-----
      }

      //----------------------------------------------------------------------------------------------------
      private void DrawScenesNotInBuild()
      {
#if UNITY_2019_3_OR_NEWER
         animBool_NotInBuild.target = EditorGUILayout.BeginFoldoutHeaderGroup(animBool_NotInBuild.target, "Scenes Not In Build");
#else
         EditorGUILayout.LabelField("Scenes Not In Build", EditorStyles.boldLabel);
#endif
         if (EditorGUILayout.BeginFadeGroup(animBool_NotInBuild.faded))
         {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollView_NotInBuildScenes))
            {
               scrollView_NotInBuildScenes = scrollView.scrollPosition;
               for (int i = 0; i < scenesNotInBuild.Count; i++)
               {
                  var scene = scenesNotInBuild[i];
                  DrawSceneAsset(scene.name, scene.path);
               }
            } //-----
         }
         EditorGUILayout.EndFadeGroup();
#if UNITY_2019_3_OR_NEWER
         EditorGUILayout.EndFoldoutHeaderGroup();
#endif
      }

      //----------------------------------------------------------------------------------------------------
      private void DrawEditorBuildScene(EditorBuildSettingsScene scene, int index, int enabledIndex)
      {
         using (new GUILayout.HorizontalScope("box")) //-----
         {
            string color = (scene.enabled ? "lime" : "red");
            string spacing = enabledIndex >= 10 ? "" : " ";
            string indexText = $"{{{spacing}<color={color}>{enabledIndex}</color>{spacing}}}";

            if (GUILayout.Button(indexText, richText_Style))
            {
               var temp = EditorBuildSettings.scenes;
               temp[index].enabled = !scene.enabled;
               EditorBuildSettings.scenes = temp;
            }

            if (GUILayout.Button(Path.GetFileNameWithoutExtension(scene.path), EditorStyles.label))
               EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
         } //-----
      }

      //----------------------------------------------------------------------------------------------------
      private void DrawSceneAsset(string name, string path)
      {
         using (new GUILayout.HorizontalScope("box")) //-----
         {
            if (GUILayout.Button($"{name} ({path})", EditorStyles.label))
            {
               var tempArray = EditorBuildSettings.scenes.ToList();
               tempArray.Add(new EditorBuildSettingsScene(path, true));
               EditorBuildSettings.scenes = tempArray.ToArray();

               RefreshNotInBuildSceneList();
            }
         } //-----
      }

      //----------------------------------------------------------------------------------------------------
      private void FillData()
      {
         if (string.IsNullOrEmpty(startScenePath))
            startScenePath = EditorPrefs.GetString(startScenePath_SavedKey);

         if (startSceneAsset == null && string.IsNullOrEmpty(startScenePath) == false)
            startSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(startScenePath);

         RefreshNotInBuildSceneList();
      }

      //----------------------------------------------------------------------------------------------------
      private void RefreshNotInBuildSceneList()
      {
         var editorScenes = new List<string>();
         for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            editorScenes.Add(EditorBuildSettings.scenes[i].path);

         scenesNotInBuild = AssetDatabase.FindAssets("t:Scene")
            .Select(guid => (name: Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid)), path: AssetDatabase.GUIDToAssetPath(guid)))
               .Where(path => editorScenes.Contains(path.path) == false)
                  .ToList();
      }
   }
}

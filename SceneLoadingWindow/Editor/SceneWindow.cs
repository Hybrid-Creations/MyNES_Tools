using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;

namespace SceneLoadingWindow
{
   public partial class SceneWindow : EditorWindow
   {
      private string startScenePath;
      private SceneAsset startSceneAsset;
      //private IList<SceneAsset> allScenes;
      private IList<SceneAsset> scenesNotInBuild = new List<SceneAsset>();

      private string startScenePath_SavedKey => $"{Application.productName}.SceneWindow.startScene";

      private Vector2 scrollView_BuildScenes;
      private Vector2 scrollView_NotInBuildScenes;
      private bool foldout_NotInBuild;

      [MenuItem("Tools/Scene Loading Window &#e")]
      //----------------------------------------------------------------------------------------------------
      private static void Init()
      {
         var window = GetWindow<SceneWindow>("Scene Loading Window");
         window.Show();
      }

      ///---------------------------------------------------------------------------------------------------
      private void OnEnable()
      {
         GenerateGUIContent();
         GenerateGUIStyles();
         FillData();
      }

      //----------------------------------------------------------------------------------------------------
      [UnityEditor.Callbacks.DidReloadScripts]
      private static void OnScriptsReloaded()
      {
         var window = GetWindow<SceneWindow>("Scene Loading Window");
         EditorApplication.delayCall += () => window.OnEnable();
      }

      ///---------------------------------------------------------------------------------------------------
      private void OnGUI()
      {
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

         using (var scroll = new GUILayout.ScrollViewScope(scrollView_BuildScenes, GUILayout.MaxHeight(Screen.height))) //-----
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
#if UNITY_2019_OR_NEWER
         // An absolute-positioned example: We make foldout header group and put it in a small rect on the screen.
         foldout_NotInBuild = EditorGUI.BeginFoldoutHeaderGroup(new Rect(10, 10, 200, 100), showPosition, status);

         if (foldout_NotInBuild)
            if (Selection.activeTransform)
            {
               Selection.activeTransform.position =
                   EditorGUI.Vector3Field(new Rect(10, 30, 200, 100), "Position", Selection.activeTransform.position);
               status = Selection.activeTransform.name;
            }
#else
         EditorGUILayout.LabelField("Scenes Not In Build", EditorStyles.boldLabel);
#endif

         using (var scroll = new GUILayout.ScrollViewScope(scrollView_NotInBuildScenes)) //-----
         {
            scrollView_NotInBuildScenes = scroll.scrollPosition;

            for (int i = 0; i < scenesNotInBuild.Count; i++)
            {
               var scene = scenesNotInBuild[i];
               DrawSceneAsset(scene, i);
            }
         } //-----
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
      private void DrawSceneAsset(SceneAsset scene, int index)
      {
         using (new GUILayout.HorizontalScope("box")) //-----
         {
            if (GUILayout.Button(scene.name, EditorStyles.label))
            { }//EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
         } //-----
      }

      //----------------------------------------------------------------------------------------------------
      private void FillData()
      {
         if (string.IsNullOrEmpty(startScenePath))
            startScenePath = EditorPrefs.GetString(startScenePath_SavedKey);

         if (startSceneAsset == null && string.IsNullOrEmpty(startScenePath) == false)
            startSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(startScenePath);

         var editorScenes = new List<string>();
         for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            editorScenes.Add(EditorBuildSettings.scenes[i].path);

         scenesNotInBuild = AssetDatabase.FindAssets("t:Scene")
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
               .Where(path => editorScenes.Contains(path) == false)
                  .Select(path => AssetDatabase.LoadAssetAtPath<SceneAsset>(path)).ToList();
      }
   }
}

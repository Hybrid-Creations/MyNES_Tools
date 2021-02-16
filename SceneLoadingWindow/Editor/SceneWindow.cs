using System.IO;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;

namespace SceneLoadingWindow
{
   public partial class SceneWindow : EditorWindow
   {
      private string startScenePath;
      private SceneAsset startSceneAsset;

      private string startScenePath_SavedKey => $"{Application.productName}.SceneWindow.startScene";

      [MenuItem("Tools/Scene Loading Window &#e")]
      //----------------------------------------------------------------------------------------------------
      private static void Init()
      {
         var window = GetWindow<SceneWindow>("Scene Loading Window");
         window.Show();
      }

      //----------------------------------------------------------------------------------------------------
      private void OnEnable()
      {
         GenerateGUIContent();
         GenerateGUIStyles();
         FillData();
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

         EditorGUILayout.LabelField("Scenes", EditorStyles.boldLabel);

         using (new GUILayout.VerticalScope("box")) //-----
         {
            int index = 0;
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
               EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
               DrawScene(scene, i, index);
               if (scene.enabled) index++;
            }
         } //-----
      }

      //----------------------------------------------------------------------------------------------------
      private void DrawScene(EditorBuildSettingsScene scene, int index, int enabledIndex)
      {
         using (new GUILayout.HorizontalScope("box")) //-----
         {
            string color = (scene.enabled ? "lime" : "red");
            string spacing = enabledIndex >= 10 ? "" : " ";
            string text = $"{{{spacing}<color={color}>{enabledIndex}</color>{spacing}}}";

            if (GUILayout.Button(text, richText_Style))
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
      private void FillData()
      {
         if (string.IsNullOrEmpty(startScenePath))
            startScenePath = EditorPrefs.GetString(startScenePath_SavedKey);

         if (startSceneAsset == null && string.IsNullOrEmpty(startScenePath) == false)
            startSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(startScenePath);
      }
   }
}

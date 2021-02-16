using UnityEditor;

using UnityEngine;

namespace ExtraPrefabUtilities.Scripts
{
   public static class ReplaceWithPrefab
   {
      private static GameObject selectedPrefab;

      [MenuItem("GameObject/Replace/WithPrefab", priority = -1)]
      public static void Replace(MenuCommand menuCommand)
      {
         if (ExecuteOnce(menuCommand))
         {
            foreach (var go in Selection.gameObjects)
            {
               var newgo = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab, go.transform.parent);

               newgo.transform.position = go.transform.position;
               newgo.transform.rotation = go.transform.rotation;

               go.SetActive(false);
            }
         }

         selectedPrefab = null;
      }

      private static bool ExecuteOnce(MenuCommand menuCommand)
      {
         if (menuCommand.context == null) return true;

         if (menuCommand.context == Selection.activeObject) return true;

         return false;
      }

      [MenuItem("GameObject/Replace/WithPrefab", true)]
      public static bool ReplaceValidate()
      {
         return selectedPrefab != null && Selection.gameObjects != null && Selection.gameObjects.Length > 0;
      }

      [MenuItem("Assets/Replace/SelectPrefab")]
      public static void SelectPrefab()
      {
         selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GetAssetPath(Selection.activeObject));
      }

      [MenuItem("Assets/Replace/SelectPrefab", true)]
      public static bool SelectPrefabValidate()
      {
         return Selection.activeObject != null && AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GetAssetPath(Selection.activeObject)) != null;
      }
   }
}

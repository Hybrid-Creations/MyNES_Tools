using UnityEditor;

using UnityEngine;

namespace ExtraSceneUtilities
{
   public class CreateSeparator
   {
      [MenuItem("GameObject/CreateSeparator", priority = -1)]
      public static void Create(/*MenuCommand menuCommand*/)
      {
         var go = new GameObject("----- ----- ----- ----- -----")
         {
            tag = "EditorOnly"
         };

         if (Selection.activeGameObject != null)
            go.transform.SetSiblingIndex(Selection.activeGameObject.transform.GetSiblingIndex() + 1);
      }

      [MenuItem("GameObject/CreateSeparator", true)]
      public static bool CreateValidate()
      {
         return true;// selectedPrefab != null && Selection.gameObjects != null && Selection.gameObjects.Length > 0;
      }
   }
}

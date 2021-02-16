using UnityEditor;

using UnityEngine;

namespace SceneLoadingWindow
{
   public partial class SceneWindow : EditorWindow
   {
      private GUIStyle richText_Style;

      private GUIContent playModeScene_Content;
      private const string playModeScene_Label = "Play Mode Start Scene";
      private const string playModeScene_Tooltip = "This is the scene that will be loaded when entering Play Mode, no matter which scene is open in the Editor";

      //----------------------------------------------------------------------------------------------------
      private void GenerateGUIContent()
      {
         playModeScene_Content = new GUIContent(playModeScene_Label, playModeScene_Tooltip);
      }

      //----------------------------------------------------------------------------------------------------
      private void GenerateGUIStyles()
      {
         if (GenerateIfNull(richText_Style, out richText_Style, EditorStyles.label))
            richText_Style.richText = true;
      }

      //----------------------------------------------------------------------------------------------------
      private bool GenerateIfNull(GUIStyle style, out GUIStyle newStyle, GUIStyle blueprint = null)
      {
         bool boo = style == null;
         newStyle = null;

         if (boo)
            newStyle = new GUIStyle(blueprint);

         return boo;
      }
   }
}

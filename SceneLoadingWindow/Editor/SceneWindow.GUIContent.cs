﻿using UnityEditor;

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
         if (GenerateContentIfNull(playModeScene_Content, out playModeScene_Content))
         {
            playModeScene_Content.text = playModeScene_Label;
            playModeScene_Content.tooltip = playModeScene_Tooltip;
         }
      }

      //----------------------------------------------------------------------------------------------------
      private void GenerateGUIStyles()
      {
         if (GenerateStyleIfNull(richText_Style, out richText_Style, EditorStyles.label))
            richText_Style.richText = true;
      }

      //----------------------------------------------------------------------------------------------------
      private bool GenerateStyleIfNull(GUIStyle style, out GUIStyle newStyle, GUIStyle blueprint = null)
      {
         bool boo = style == null;
         newStyle = null;

         if (boo)
         {
            if (blueprint == null)
               newStyle = new GUIStyle();
            else
               newStyle = new GUIStyle(blueprint);
         }

         return boo;
      }

      //----------------------------------------------------------------------------------------------------
      private bool GenerateContentIfNull(GUIContent style, out GUIContent newStyle, GUIContent blueprint = null)
      {
         bool boo = style == null;
         newStyle = null;

         if (boo)
         {
            if (blueprint == null)
               newStyle = new GUIContent();
            else
               newStyle = new GUIContent(blueprint);
         }

         return boo;
      }
   }
}

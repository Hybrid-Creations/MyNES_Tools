using System.IO;
using System.Linq;
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace StaticGenerators
{
   public static class StaticTagGenerator
   {
      private static readonly string defaultDirectoryName = "ProjectDefaults";
      private static readonly string generatedClassName = "Tags";
      private static string DefaultFileName => $"{generatedClassName}.cs";

      static string DefaultDirectory => Application.dataPath + "/" + defaultDirectoryName;
      static string DefaultPath => DefaultDirectory + "/" + DefaultFileName;

      private static string[] editorTags;

      [InitializeOnLoadMethod]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Automatically called by Unity.")]
      private static void OnScriptsReloaded()
      {
         if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

         if (Directory.Exists(DefaultDirectory) == false)
            Directory.CreateDirectory(DefaultDirectory);

         editorTags = UnityEditorInternal.InternalEditorUtility.tags;

         if (TagScriptNeedsUpdate())
         {
            //Debug.LogWarning("Generated Tags Script Needs An Update.");
            GenerateTagsScript();
         }
      }

      private static bool TagScriptNeedsUpdate()
      {
         if (Exists(out var tagsType))
         {
            PropertyInfo[] props = tagsType.GetProperties();
            var propSelect = props.Select(item => item.Name);

            foreach (var tag in editorTags)
               if (propSelect.Contains(tag) == false)
               {
                  Debug.LogWarning("Tags Script Contained More Tags Than The Editor, Generating New One.");
                  return true;
               }

            foreach (var prop in props)
               if (editorTags.Contains(prop.Name) == false)
               {
                  Debug.LogWarning("Tags Script Did Not Contain All The Tags, Generating New One.");
                  return true;
               }

            return false;
         }

         Debug.LogWarning("Tags Script Did Not Exist, Generating New One.");
         return true;
      }

      private static bool Exists(out System.Type tagsType)
      {
         foreach (Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
         {
            foreach (System.Type type in assembly.GetTypes())
            {
               if (type.Name == generatedClassName)
               {
                  tagsType = type;
                  return true;
               }
            }
         }

         tagsType = null;
         return false;
      }

      private static void GenerateTagsScript()
      {
         var finfo = new FileInfo(GetPath());

         using (var fs = new StreamWriter(finfo.FullName))
         {
            fs.Write("public class Tags\n{\n");

            foreach (var tag in editorTags)
               fs.WriteLine($"public static string {tag}" + @"{ get=>" + "\"" + $"{tag}" + "\"" + @"; }");

            fs.Write("\n}");
         }

         Debug.Log("<color=lime>Generated Tags Script Successfully Updated.</color>");
      }

      private static string GetPath()
      {
         if (Exists(out _))
         {
            foreach (var item in AssetDatabase.FindAssets($"t:script {DefaultFileName.Replace(".cs", "")}"))
            {
               string path = AssetDatabase.GUIDToAssetPath(item);
               if (Path.GetFileName(path) == DefaultFileName)
                  return path;
            }
         }

         return DefaultPath;
      }
   }
}

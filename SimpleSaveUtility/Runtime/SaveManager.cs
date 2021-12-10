using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor.Graphs;
using UnityEngine;

namespace SimpleSaveUtility
{
   //-//----------------------------------------------------------------------------------------------------
   public static class SaveManager
   {
      private static readonly Dictionary<string, string> saveData = new Dictionary<string, string>();
      private static string CurrentSaveIdentifier { get; set; }
      private static readonly string savesDirectoryPath = Application.persistentDataPath + "/Save Games/";
      private static string SaveGameName => CurrentSaveIdentifier + ".txt";
      private static string SaveGamePath => savesDirectoryPath + SaveGameName;

      //----------------------------------------------------------------------------------------------------
      public static void SetCurrentSaveIdentifier(string saveIdentifier)
      {
         CurrentSaveIdentifier = saveIdentifier;
      }

      //----------------------------------------------------------------------------------------------------
      internal static bool TryGetData<T>(string key, out T newValue)
      {
         throw new System.NotImplementedException();
      }

      //----------------------------------------------------------------------------------------------------
      internal static void LoadDataInto<T>(SavedData<T> data)
      {
         if (saveData.Count == 0)
            LoadGameData();

         if (saveData.TryGetValue(data.Key, out var value))
            data.Deserialize(value);
      }

      //----------------------------------------------------------------------------------------------------
      internal static void SaveDataFrom<T>(SavedData<T> data)
      {
         data.Serialize();
         saveData[data.Key] = data.savedValue;
      }

      //----------------------------------------------------------------------------------------------------
      private static void LoadGameData()
      {
         if (string.IsNullOrEmpty(CurrentSaveIdentifier))
            return;

         if (Directory.Exists(savesDirectoryPath) == false)
            Directory.CreateDirectory(savesDirectoryPath);
         if (File.Exists(SaveGamePath) == false)
            File.Create(SaveGamePath);
         else
         {
            foreach (var line in File.ReadLines(SaveGamePath))
            {
               var split = line.Split("||");
               saveData[split[0]] = split[1];
            }
         }
      }

      //----------------------------------------------------------------------------------------------------
      public static void SaveGameData()
      {
         if (Directory.Exists(savesDirectoryPath) == false)
            Directory.CreateDirectory(savesDirectoryPath);
         using (var sw = new StreamWriter(SaveGamePath))
         {
            foreach (var data in saveData)
               sw.WriteLine($"{data.Key}||{data.Value}");
         }
      }

      //----------------------------------------------------------------------------------------------------
      public static void SaveGameDataAsync()
      {
         if (Directory.Exists(savesDirectoryPath) == false)
            Directory.CreateDirectory(savesDirectoryPath);
         using (var sw = new StreamWriter(SaveGamePath))
         {
            foreach (var data in saveData)
               sw.WriteLineAsync($"{data.Key}||{data.Value}");
         }
      }

      // [01] ----------------------------------------------------------------------------------------------
      public static Vector3 Vector3FromString(string split) => Vector3FromString(split.Split(','));

      //----------------------------------------------------------------------------------------------------
      public static Vector3 Vector3FromString(string[] split)
      {
         Vector3 vector = new Vector3();
         for (int i = 0; i < split.Length; i++)
            split[i] = Regex.Replace(split[i], @"[^0-9.,]", string.Empty);

         if (float.TryParse(split[0], out var x))
            vector.x = x;
         if (float.TryParse(split[1], out var y))
            vector.y = y;
         if (float.TryParse(split[2], out var z))
            vector.z = z;

         return vector;
      }

      //----------------------------------------------------------------------------------------------------
      public static string StringFromVector3(Vector3 vec)
      {
         var sb = new StringBuilder();
         for (int i = 0; i < 3; i++)
            sb.Append(Regex.Replace($"{vec[i]}", @"[^0-9.]", string.Empty));

         return sb.ToString();
      }
   }

   //-//----------------------------------------------------------------------------------------------------
   /// <summary>
   /// Can only be used with classes that implement <see cref="ISaveable"/>, or used with default types.
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public class SavedData<T>
   {
      private static readonly Dictionary<string, T> map = new Dictionary<string, T>();

      private bool loaded = false;
      private readonly string key;
      private T MapValue
      {
         get => map.TryGetValue(key, out var val) ? val : map[key];
         set => map[key] = value;
      }

      internal string savedValue;
      private readonly T defaultValue;

      public string Key => key;
      public T Value
      {
         get
         {
            if (!loaded)
            {
               SaveManager.LoadDataInto(this);
               loaded = true;
            }

            return MapValue;
         }
         set
         {
            MapValue = value;
            SaveManager.SaveDataFrom(this);
         }
      }

      //-||-------------------------------------------------------------------------------------------------
      public SavedData(string key, T defaultValue)
      {
         this.key = key;
         MapValue = this.defaultValue = defaultValue;
      }

      //----------------------------------------------------------------------------------------------------
      internal void Deserialize(string value)
      {
         if (MapValue is ISaveable s && s != null)
         {
            ((ISaveable)MapValue).Deserialize(value);
            return;
         }

         object newValue = null;
         switch (MapValue)
         {
            default:
               Debug.LogError($"Cannot Deserialize type: \"{typeof(T)}\". It must implement \"{nameof(ISaveable)}\", or be a default type.");
               break;

            case int:
               if (int.TryParse(value, out var i))
                  newValue = i;
               break;

            case double:
               if (double.TryParse(value, out var d))
                  newValue = d;
               break;

            case string:
               newValue = value;
               break;

            case bool:
               if (bool.TryParse(value, out var b))
                  newValue = b;
               break;

            case Vector3:
               value = Regex.Replace(value, @"[^0-9.,]", string.Empty);
               newValue = SaveManager.Vector3FromString(value.Split(','));
               break;
         }

         MapValue = newValue != null ? (T)newValue : defaultValue;
      }

      //----------------------------------------------------------------------------------------------------
      internal void Serialize()
      {
         if (MapValue is ISaveable saveable)
            savedValue = saveable.Serialize();
         else
            savedValue = MapValue.ToString();
      }

      //----------------------------------------------------------------------------------------------------
      public override string ToString()
      {
         return $"Key: \"{Key}\", Value: \"{Value}\"";
      }
   }

   //----------------------------------------------------------------------------------------------------
   /// <summary>
   /// Custom way to save data.
   /// </summary>
   public interface ISaveable
   {
      /// <summary>
      /// Used to convert all of your values into a string.
      /// </summary>
      /// <returns></returns>
      public string Serialize();

      /// <summary>
      /// Used to break apart the string to set all of your values.
      /// </summary>
      /// <param name="savedValue"></param>
      public void Deserialize(string savedValue);
   }
}

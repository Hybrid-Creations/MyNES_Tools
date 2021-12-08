using System.Collections.Generic;
using UnityEngine;

namespace SimpleSaveUtility
{
   //-//----------------------------------------------------------------------------------------------------
   public static class SaveManager
   {
      private static readonly Dictionary<string, string> saveData = new Dictionary<string, string>();
      private static string CurrentSaveIdentifier { get; set; }

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
         if (saveData.TryGetValue(data.Key, out var value))
         {
            data.Deserialize(value);
         }
      }

      //----------------------------------------------------------------------------------------------------
      internal static void SaveDataFrom<T>(SavedData<T> data)
      {
         data.Serialize();
         saveData[data.Key] = data.savedValue;
      }
   }

   //-//----------------------------------------------------------------------------------------------------
   /// <summary>
   /// Can only be used with classes that implement <see cref="ISaveable"/>, or used with default types.
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public class SavedData<T>
   {
      private bool loaded = false;
      private readonly string key;
      private T value;

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

            return value;
         }
         set
         {
            this.value = value;
            SaveManager.SaveDataFrom(this);
         }
      }

      //-||-------------------------------------------------------------------------------------------------
      public SavedData(string key, T defaultValue)
      {
         this.key = key;
         this.value = this.defaultValue = defaultValue;
      }

      //----------------------------------------------------------------------------------------------------
      internal void Deserialize(string value)
      {
         if (this.value is ISaveable s && s != null)
         {
            ((ISaveable)this.value).Deserialize(value);
            return;
         }

         object newValue = null;
         switch (this.value)
         {
            default:
               Debug.LogError($"Cannot Deserialize type: \"{typeof(T)}\". It must implement \"{nameof(ISaveable)}\", or be a default type.");
               break;

            case int:
               int.TryParse(value, out var i);
               newValue = i;
               break;

            case double:
               double.TryParse(value, out var d);
               newValue = d;
               break;

            case string:
               newValue = value;
               break;
         }

         if (newValue != null)
            this.value = (T)newValue;
      }

      //----------------------------------------------------------------------------------------------------
      internal void Serialize()
      {
         if (value is ISaveable saveable)
            savedValue = saveable.Serialize();
         else
            savedValue = value.ToString();
      }

      //----------------------------------------------------------------------------------------------------
      public override string ToString()
      {
         return $"Key: \"{key}\", Value: \"{value}\"";
      }
   }

   //----------------------------------------------------------------------------------------------------
   public interface ISaveable
   {
      public string Serialize();

      public void Deserialize(string savedValue);
   }
}

using Codice.CM.WorkspaceServer;
using UnityEngine;

namespace SimpleSaveUtility
{
   //-//----------------------------------------------------------------------------------------------------
   public static class SaveManager
   {
      //private static readonly List<SavedData> saveData = new List<SavedData>();
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
      internal static void LoadDataInto<T>(Data<T> data)
      {
         throw new System.NotImplementedException();
      }

      //----------------------------------------------------------------------------------------------------
      internal static void SaveDataFrom<T>(Data<T> data)
      {
         throw new System.NotImplementedException();
      }

      //-//----------------------------------------------------------------------------------------------------
      internal class Data<T>
      {
         internal bool loaded = false;
         internal string key;
         internal string savedValue;
         private T value;
         internal T defaultValue;

         //-||-------------------------------------------------------------------------------------------------
         public Data(string key, T defaultValue)
         {
            this.key = key;
            this.value = this.defaultValue = defaultValue;
         }

         public T Value
         {
            get
            {
               if (!loaded)
               {
                  SaveManager.LoadDataInto(this);
                  Deserialize(savedValue);
               }
               return value;
            }
            set
            {
               this.value = value;
               Serialize(value);
               SaveManager.SaveDataFrom(this);
            }
         }

         //----------------------------------------------------------------------------------------------------
         private void Deserialize(string value)
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
         private void Serialize(T value)
         {
            if (this.value is ISaveable s && s != null)
            {
               savedValue = s.Serialize();
               return;
            }

            savedValue = value.ToString();
         }

         public override string ToString()
         {
            return $"Key: \"{key}\", Value: \"{value}\"";
         }
      }
   }

   //----------------------------------------------------------------------------------------------------
   public interface ISaveable
   {
      public string Serialize();

      public void Deserialize(string savedValue);
   }

   //-//----------------------------------------------------------------------------------------------------
   public class SavedString
   {
      private readonly SaveManager.Data<string> data;

      public string Value => data.Value;

      //-||-------------------------------------------------------------------------------------------------
      public SavedString(string key, string defaultValue)
      {
         data = new SaveManager.Data<string>(key, defaultValue);
      }

      public override string ToString()
      {
         return data.ToString();
      }
   }

   //-//----------------------------------------------------------------------------------------------------
   public class SavedDouble
   {
      private readonly SaveManager.Data<double> data;

      public double Value => data.Value;

      //-||-------------------------------------------------------------------------------------------------
      public SavedDouble(string key, double defaultValue)
      {
         data = new SaveManager.Data<double>(key, defaultValue);
      }

      public override string ToString()
      {
         return data.ToString();
      }
   }

   //-||----------------------------------------------------------------------------------------------------
   public class SavedData<T> where T : ISaveable
   {
      private readonly SaveManager.Data<T> data;

      public T Value => data.Value;

      //-||-------------------------------------------------------------------------------------------------
      public SavedData(string key, T defaultValue)
      {
         data = new SaveManager.Data<T>(key, defaultValue);
      }

      public override string ToString()
      {
         return data.ToString();
      }
   }
}

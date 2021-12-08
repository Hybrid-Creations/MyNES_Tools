using System;

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
      internal static void LoadDataInto(Data data)
      {
         throw new NotImplementedException();
      }
   }

   //----------------------------------------------------------------------------------------------------
   public interface ISaveable
   {
      public string Serialize();

      public void Deserialize(string savedValue);
   }

   //-//----------------------------------------------------------------------------------------------------
   internal class Data
   {
      internal bool loaded = false;
      internal string key;
      internal string savedValue;
   }

   //-//----------------------------------------------------------------------------------------------------
   public class SavedString
   {
      private readonly Data data;

      public string Value
      {
         get
         {
            if (!data.loaded)
               SaveManager.LoadDataInto(data);
            return data.savedValue;
         }
      }

      //-||-------------------------------------------------------------------------------------------------
      public SavedString(string key, string defaultValue)
      {
         data = new Data();
         data.key = key;
         data.savedValue = defaultValue;
      }

      public override string ToString()
      {
         return $"Key: \"{data.key}\", Value: \"{data.savedValue}\"";
      }
   }

   //-//----------------------------------------------------------------------------------------------------
   public class SavedDouble
   {
      private readonly Data data;
      private double value;

      public double Value
      {
         get
         {
            if (!data.loaded)
            {
               SaveManager.LoadDataInto(data);
               value = Deserialize(data.savedValue);
            }

            return value;
         }
      }

      //-||-------------------------------------------------------------------------------------------------
      public SavedDouble(string key, double defaultValue)
      {
         data = new Data();
         data.key = key;
         data.savedValue = Serialize(defaultValue);
      }

      //----------------------------------------------------------------------------------------------------
      private double Deserialize(string value)
      {
         double.TryParse(value, out double result);
         return result;
      }

      //----------------------------------------------------------------------------------------------------
      private string Serialize(double value)
      {
         return value.ToString();
      }

      public override string ToString()
      {
         return $"Key: \"{data.key}\", Value: \"{data.savedValue}\"";
      }
   }

   //-||----------------------------------------------------------------------------------------------------
   public class SavedData<T> where T : ISaveable
   {
      private readonly Data data;
      private T value;

      public T Value
      {
         get
         {
            if (!data.loaded)
            {
               SaveManager.LoadDataInto(data);
               value.Deserialize(data.savedValue);
            }
            return value;
         }
      }

      //-||-------------------------------------------------------------------------------------------------
      public SavedData(string key, T defaultValue = default)
      {
         data = new Data();
         data.key = key;
         value = defaultValue;
      }
   }
}

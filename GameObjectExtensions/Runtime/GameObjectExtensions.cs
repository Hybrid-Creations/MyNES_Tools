using UnityEngine;

namespace GameObjectExtensions.Scripts
{
   public static class GameObjectExtensions
   {
      public static t GetOrAddComponent<t>(this GameObject go) where t : Component
      {
         if (go.GetComponent<t>())
            return go.GetComponent<t>();
         else
            return go.AddComponent<t>();
      }
   }
}

using UnityEditor;

using UnityEngine.Events;

#if UNITY_ENGINE

namespace CallOnMainThreadUtility
{
   [InitializeOnLoad]
   public static class CallOnMainThread_Editor
   {
      private static readonly ThreadRunner tr = new ThreadRunner();

      static CallOnMainThread_Editor()
      {
         EditorApplication.update += LocalUpdate;
      }

      //----------------------------------------------------------------------------------------------------
      internal static void LocalUpdate() => tr.LocalUpdate();

      //----------------------------------------------------------------------------------------------------
      public static void EnqueueAction(UnityAction action) => tr.EnqueueAction(action);
   }
}

#endif

using UnityEngine;
using UnityEngine.Events;

namespace CallOnMainThreadUtility
{
   public static class CallOnMainThread_Runtime
   {
      private static GameObject threadRunner;

      private static readonly ThreadRunner tr = new ThreadRunner();

      //----------------------------------------------------------------------------------------------------
      [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
      [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called Automatically By Unity.")]
      private static void Initialize()
      {
         if (threadRunner == null)
         {
            threadRunner = new GameObject("MainThreadRunner");
            GameObject.DontDestroyOnLoad(threadRunner);

            threadRunner.AddComponent<RuntimeThreadRunner>();
         }
      }

      //----------------------------------------------------------------------------------------------------
      internal static void LocalUpdate() => tr.LocalUpdate();

      //----------------------------------------------------------------------------------------------------
      public static void EnqueueAction(UnityAction action) => tr.EnqueueAction(action);
   }

   internal class RuntimeThreadRunner : MonoBehaviour
   {
      [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called Automatically By Unity.")]
      private void Update() => CallOnMainThread_Runtime.LocalUpdate();
   }
}

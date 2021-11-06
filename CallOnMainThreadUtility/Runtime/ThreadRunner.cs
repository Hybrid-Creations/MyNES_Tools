using System.Collections.Concurrent;

using UnityEngine.Events;
using UnityEngine.Profiling;

namespace CallOnMainThreadUtility
{
   internal class ThreadRunner
   {
      private readonly ConcurrentQueue<UnityAction> queuedActions = new ConcurrentQueue<UnityAction>();

      //----------------------------------------------------------------------------------------------------
      internal void LocalUpdate()
      {
         Profiler.BeginSample("Call On Main Thread Update");

         while (queuedActions.Count > 0)
         {
            if (queuedActions.TryDequeue(out var action))
               action.Invoke();
         }

         Profiler.EndSample();
      }

      //----------------------------------------------------------------------------------------------------
      internal void EnqueueAction(UnityAction action) => queuedActions.Enqueue(action);
   }
}

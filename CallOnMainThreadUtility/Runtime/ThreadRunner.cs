using System.Collections.Concurrent;

using UnityEngine.Events;

namespace CallOnMainThreadUtility
{
   internal class ThreadRunner
   {
      private readonly ConcurrentQueue<UnityAction> queuedActions = new ConcurrentQueue<UnityAction>();

      //----------------------------------------------------------------------------------------------------
      internal void LocalUpdate()
      {
         while (queuedActions.Count > 0)
         {
            if (queuedActions.TryDequeue(out var action))
               action.Invoke();
         }
      }

      //----------------------------------------------------------------------------------------------------
      internal void EnqueueAction(UnityAction action) => queuedActions.Enqueue(action);
   }
}

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace SimpleTimerUtility
{
   //-//----------------------------------------------------------------------------------------------------
   public static class TimerUtility
   {
      internal static readonly Queue<SimpleTimer> timerPool = new Queue<SimpleTimer>();
      internal static readonly Queue<SimpleTimer> timersToAdd = new Queue<SimpleTimer>();
      internal static readonly HashSet<SimpleTimer> timers = new HashSet<SimpleTimer>();
      private static bool createdRunner = false;

      internal static float currentTime;

      //----------------------------------------------------------------------------------------------------
      public static SimpleTimer StartTimer(double duration)
      {
         SimpleTimer newTimer;
         if (timerPool.Count > 0)
         {
            newTimer = timerPool.Dequeue();
            newTimer.ClearAllListeners();
            newTimer.Setup(duration);
         }
         else
            newTimer = new SimpleTimer(duration);

         newTimer.IsPoolable = true;
         newTimer.Start();
         return newTimer;
      }

      //----------------------------------------------------------------------------------------------------
      private static void UpdateTimers()
      {
         currentTime = Time.time;

         var list = new List<SimpleTimer>();
         foreach (var timer in timers)
         {
            if (timer.IsComplete)
               list.Add(timer);
            else
               timer.Update();
         }

         foreach (var timer in list)
         {
            timers.Remove(timer);
            if (timer.IsPoolable)
               timerPool.Enqueue(timer);
         }

         while (timersToAdd.Count > 0)
            timers.Add(timersToAdd.Dequeue());
      }

      //----------------------------------------------------------------------------------------------------
      internal static void CheckRunner()
      {
         if (createdRunner == false)
         {
            var go = new GameObject("Timer Master");
            go.AddComponent<TimerRunner>();
            GameObject.DontDestroyOnLoad(go);

            createdRunner = true;
         }
      }

      private class TimerRunner : MonoBehaviour
      {
         ///------------------------------------------------------------------------------------------------
         [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity automatic call.")]
         private void Update() => UpdateTimers();
      }
   }

   //-//----------------------------------------------------------------------------------------------------
   public class SimpleTimer
   {
      public double Duration { get; private set; }
      public double EndTime { get; private set; }
      public double StartTime { get; private set; }
      public bool IsComplete { get; private set; }
      public double RemainingTime => EndTime - Time.time;
      public double RemainingTimeNormalized => RemainingTime / Duration;
      public double ElapsedTime => Time.time - StartTime;
      public double ElapsedTimeNormalized => ElapsedTime / Duration;

      internal bool IsPoolable { get; set; } = false;

      private readonly TimerUnityEvent myEvent = new TimerUnityEvent();
      private readonly TimerUnityEvent myUpdateEvent = new TimerUnityEvent();

      public SimpleTimer(double duration)
      {
         Duration = duration;
      }

      internal void Setup(double duration)
      {
         Duration = duration;
      }

      ///---------------------------------------------------------------------------------------------------
      internal void Update()
      {
         if (myUpdateEvent.HasListeners)
            myUpdateEvent.Invoke(this);

         if (EndTime <= TimerUtility.currentTime)
            Stop(true);
      }

      //----------------------------------------------------------------------------------------------------
      public void Start()
      {
         TimerUtility.CheckRunner();

         StartTime = Time.time;
         EndTime = StartTime + Duration;

         IsComplete = false;

         TimerUtility.timersToAdd.Enqueue(this);
      }

      //----------------------------------------------------------------------------------------------------
      public void Restart() => Start();

      //----------------------------------------------------------------------------------------------------
      public void Stop(bool runEvent)
      {
         IsComplete = true;

         if (runEvent && myUpdateEvent.HasListeners)
            myUpdateEvent.Invoke(this);
         if (runEvent && myEvent.HasListeners)
            myEvent.Invoke(this);
      }

      // [01] ----------------------------------------------------------------------------------------------
      public SimpleTimer AddListener(UnityAction action) => AddListener((_) => action.Invoke());

      //----------------------------------------------------------------------------------------------------
      public SimpleTimer AddListener(UnityAction<SimpleTimer> action)
      {
         myEvent.AddListener(action);
         return this;
      }

      // [01] ----------------------------------------------------------------------------------------------
      public SimpleTimer AddUpdateListener(UnityAction action) => AddUpdateListener((_) => action.Invoke());

      //----------------------------------------------------------------------------------------------------
      public SimpleTimer AddUpdateListener(UnityAction<SimpleTimer> action)
      {
         myUpdateEvent.AddListener(action);
         return this;
      }

      //----------------------------------------------------------------------------------------------------
      public void ClearAllListeners()
      {
         ClearListeners();
         ClearUpdateListeners();
      }

      //----------------------------------------------------------------------------------------------------
      public void ClearListeners()
      {
         myEvent.ClearListeners();
      }

      //----------------------------------------------------------------------------------------------------
      public void ClearUpdateListeners()
      {
         myUpdateEvent.ClearListeners();
      }

      //-//-------------------------------------------------------------------------------------------------
      [System.Serializable]
      internal class TimerUnityEvent
      {
         private readonly List<UnityAction<SimpleTimer>> actionList = new List<UnityAction<SimpleTimer>>();

         internal bool HasListeners { get; private set; }

         //-------------------------------------------------------------------------------------------------
         internal void AddListener(UnityAction<SimpleTimer> action)
         {
            actionList.Add(action);
            HasListeners = true;
         }

         //-------------------------------------------------------------------------------------------------
         internal void Invoke(SimpleTimer timer)
         {
            foreach (var action in actionList)
            {
               try
               {
                  action.Invoke(timer);
               }
               catch (System.Exception e)
               {
                  Debug.LogError(e);
               }
            }
         }

         //-------------------------------------------------------------------------------------------------
         internal void ClearListeners()
         {
            if (HasListeners)
            {
               actionList.Clear();
               HasListeners = false;
            }
         }
      }
   }
}

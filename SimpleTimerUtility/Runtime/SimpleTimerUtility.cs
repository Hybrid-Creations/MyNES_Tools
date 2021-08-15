using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace SimpleTimerUtility
{
   //-//----------------------------------------------------------------------------------------------------
   public static class TimerUtility
   {
      internal static readonly List<SimpleTimer> timers = new List<SimpleTimer>();
      private static TimerRunner runner;

      //----------------------------------------------------------------------------------------------------
      public static SimpleTimer StartTimer(double duration)
      {
         var newTimer = new SimpleTimer(duration);
         newTimer.Start();

         return newTimer;
      }

      //----------------------------------------------------------------------------------------------------
      private static void UpdateTimers()
      {
         for (int i = 0; i < timers.Count; i++)
            timers[i].Update();
      }

      //----------------------------------------------------------------------------------------------------
      internal static void CheckRunner()
      {
         if (runner == null)
         {
            var go = new GameObject("Timer Master");
            runner = go.AddComponent<TimerRunner>();
            GameObject.DontDestroyOnLoad(go);
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
      private bool HasListeners { get; set; }
      private bool HasUpdateListeners { get; set; }
      public double RemainingTime => EndTime - Time.time;
      public double RemainingTimeNormalized => RemainingTime / Duration;
      public double ElapsedTime => Time.time - StartTime;
      public double ElapsedTimeNormalized => ElapsedTime / Duration;

      private TimerUnityEvent myEvent = new TimerUnityEvent();
      private TimerUnityEvent myUpdateEvent = new TimerUnityEvent();

      public SimpleTimer(double duration)
      {
         Duration = duration;
      }

      ///---------------------------------------------------------------------------------------------------
      internal void Update()
      {
         if (HasUpdateListeners)
            myUpdateEvent.Invoke(this);

         if (EndTime <= Time.time)
            Stop(true);
      }

      //----------------------------------------------------------------------------------------------------
      public void Start()
      {
         TimerUtility.CheckRunner();

         StartTime = Time.time;
         EndTime = Time.time + Duration;

         TimerUtility.timers.Add(this);
      }

      //----------------------------------------------------------------------------------------------------
      public void Stop(bool runEvent)
      {
         IsComplete = true;

         if (runEvent && HasUpdateListeners)
            myUpdateEvent.Invoke(this);
         if (runEvent && HasListeners)
            myEvent.Invoke(this);

         TimerUtility.timers.Remove(this);
      }

      //----------------------------------------------------------------------------------------------------
      public SimpleTimer AddListener(UnityAction<SimpleTimer> action)
      {
         myEvent.AddListener(action);
         HasListeners = true;
         return this;
      }

      //----------------------------------------------------------------------------------------------------
      public SimpleTimer AddUpdateListener(UnityAction<SimpleTimer> action)
      {
         myUpdateEvent.AddListener(action);
         HasUpdateListeners = true;
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
         HasListeners = false;
         myEvent = new TimerUnityEvent();
      }

      //----------------------------------------------------------------------------------------------------
      public void ClearUpdateListeners()
      {
         HasUpdateListeners = false;
         myUpdateEvent = new TimerUnityEvent();
      }

      //-//-------------------------------------------------------------------------------------------------
      [System.Serializable]
      private class TimerUnityEvent : UnityEvent<SimpleTimer>
      { }
   }
}

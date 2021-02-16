using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace SimpleTimerUtility
{
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
      internal static void R()
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

   public class SimpleTimer
   {
      public double Duration { get; private set; }
      public double EndTime { get; private set; }
      public double StartTime { get; private set; }
      public double RemainingTime { get => EndTime - Time.time; }
      public bool IsComplete { get; private set; }
      public bool HasListeners { get; private set; }
      public bool HasUpdateListeners { get; private set; }

      private UnityEvent myEvent = new UnityEvent();
      private UnityEvent myUpdateEvent = new UnityEvent();

      public SimpleTimer(double duration)
      {
         Duration = duration;
      }

      internal void Update()
      {
         if (HasUpdateListeners)
            myUpdateEvent.Invoke();

         if (EndTime <= Time.time)
            End(true);
      }

      //----------------------------------------------------------------------------------------------------
      public void Start()
      {
         TimerUtility.R();

         StartTime = Time.time;
         EndTime = Time.time + Duration;

         TimerUtility.timers.Add(this);
      }

      //----------------------------------------------------------------------------------------------------
      public void End(bool runEvent)
      {
         IsComplete = true;

         if (runEvent && HasListeners)
            myEvent.Invoke();

         TimerUtility.timers.Remove(this);
      }

      //----------------------------------------------------------------------------------------------------
      public SimpleTimer AddListener(UnityAction action)
      {
         myEvent.AddListener(action);
         HasListeners = true;
         return this;
      }

      //----------------------------------------------------------------------------------------------------
      public SimpleTimer AddUpdateListener(UnityAction action)
      {
         myUpdateEvent.AddListener(action);
         HasUpdateListeners = true;
         return this;
      }

      //----------------------------------------------------------------------------------------------------
      public void ClearListeners()
      {
         HasListeners = false;
         myEvent = new UnityEvent();
      }

      //----------------------------------------------------------------------------------------------------
      public void ClearUpdateListeners()
      {
         HasUpdateListeners = false;
         myUpdateEvent = new UnityEvent();
      }
   }
}

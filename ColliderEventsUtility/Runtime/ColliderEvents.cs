using UnityEngine;
using UnityEngine.Events;

namespace CollisionEvents
{
   public class ColliderEvents : MonoBehaviour
   {
      [SerializeField] private EnabledEvents defaultEnabledEvents = EnabledEvents.EITHER;

      //private Collider myCollider;
      public EnabledEvents Mode { get; private set; }

      private CollderEvent _OnTriggerEnter = new CollderEvent();
      private CollderEvent _OnTriggerExit = new CollderEvent();
      private CollisionEvent _OnCollisionEnter = new CollisionEvent();

      [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called Automatically By Unity.")]
      private void Awake()
      {
         //myCollider = GetComponent<Collider>();
         Mode = defaultEnabledEvents;
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called Automatically By Unity.")]
      private void OnCollisionEnter(Collision collision)
      {
         if (Mode == EnabledEvents.COLLLISION || Mode == EnabledEvents.EITHER)
            _OnCollisionEnter.Invoke(collision);
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called Automatically By Unity.")]
      private void OnTriggerEnter(Collider other)
      {
         if (Mode == EnabledEvents.TRIGGER || Mode == EnabledEvents.EITHER)
            _OnTriggerEnter.Invoke(other);
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called Automatically By Unity.")]
      private void OnTriggerExit(Collider other)
      {
         if (Mode == EnabledEvents.TRIGGER || Mode == EnabledEvents.EITHER)
            _OnTriggerExit.Invoke(other);
      }

      public void SetMode(EnabledEvents newMode)
      {
         Mode = newMode;
      }

      public void AddCollisionListener(UnityAction<Collision> action)
      {
         _OnCollisionEnter.AddListener(action);
      }

      public void RemoveCollisionListener(UnityAction<Collision> action)
      {
         _OnCollisionEnter.RemoveListener(action);
      }

      public void AddTriggerEnterListener(UnityAction<Collider> action)
      {
         _OnTriggerEnter.AddListener(action);
      }

      public void RemoveTriggerEnterListener(UnityAction<Collider> action)
      {
         _OnTriggerEnter.RemoveListener(action);
      }

      public void AddTriggerExitListener(UnityAction<Collider> action)
      {
         _OnTriggerExit.AddListener(action);
      }

      public void RemoveTriggerExitListener(UnityAction<Collider> action)
      {
         _OnTriggerExit.RemoveListener(action);
      }

      public enum EnabledEvents
      {
         COLLLISION,
         TRIGGER,
         EITHER,
         NONE
      }

      public class CollisionEvent : UnityEvent<Collision> { }

      public class CollderEvent : UnityEvent<Collider> { }
   }
}

using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace TimelineTools
{
    namespace Events
    {
        public enum ParameterType
        {
            None,
            Bool,
            Int,
            Float,
            String,
            Object
        }

        [Serializable]
        public class Argument
        {
            // Argument type
            public ParameterType parameterType;
            // argument properties
            public bool Bool;
            public int Int;
            public string String;
            public float Float;
            public ExposedReference<Object> Object;
        }

        [Serializable]
        public class Callback
        {
            // Names
            public string assemblyName;
            public string methodName;
            public string fullMethodName;
            public Argument[] arguments;
        }

        [CustomStyle("EventMarkerStyle")]
        [Serializable, DisplayName("Event")]
        public class EventMarkerNotification : Marker, INotification, INotificationOptionProvider
        {
            public Callback[] callbacks;
            public bool retroactive = true;
            public bool emitOnce;
            public bool emitInEditor = true;
            public Color color = new(1.0f, 1.0f, 1.0f, 0.5f);
            public bool showLineOverlay = true;

            PropertyName INotification.id { get { return new PropertyName(); } }

            NotificationFlags INotificationOptionProvider.flags
            {
                get
                {
                    return (retroactive ? NotificationFlags.Retroactive : default) |
                        (emitOnce ? NotificationFlags.TriggerOnce : default) |
                        (emitInEditor ? NotificationFlags.TriggerInEditMode : default);
                }
            }
        }
    }
}

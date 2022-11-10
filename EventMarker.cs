using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace TimelineTools
{
    public partial class Events
    {
        public enum ParameterType
        {
            Int,
            Float,
            String,
            Object,
            None
        }

        [Serializable]
        public struct Method
        {
            public string name;
            public ParameterType parameterType;
            public int Int;
            public string String;
            public float Float;
            public ExposedReference<Object> Object;

        }

        [Serializable, DisplayName("Event Marker")]
        public class EventMarker : Marker, INotification, INotificationOptionProvider
        {
            public Method[] methods;
            public bool retroactive;
            public bool emitOnce;
            public bool emitInEditor;

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

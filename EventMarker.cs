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
        public class Method
        {
            public string name;
            public ParameterType parameterType;
            public int Int;
            public string String;
            public float Float;
            public ExposedReference<Object> Object;
        }

        [CustomStyle("EventMarkerStyle")]
        [Serializable, DisplayName("Event Marker")]
        public class EventMarker : Marker, INotification, INotificationOptionProvider
        {
            public Method[] methods;
            public bool retroactive;
            public bool emitOnce;
            public bool emitInEditor;
            public string tooltip;
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

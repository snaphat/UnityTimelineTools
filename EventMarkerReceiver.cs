using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;

namespace TimelineTools
{
    namespace Events
    {
        public class EventMarkerReceiver : MonoBehaviour, INotificationReceiver
        {
            public void OnNotify(Playable origin, INotification notification, object context)
            {
                //An INotificationReceiver will receive all the triggered notifications. We need to 
                //have a filter to use only the notifications that we can process.
                var message = notification as EventMarkerNotification;
                if (message == null || message.callbacks == null) return;

                foreach (var callback in message.callbacks)
                {

                    // Setup argument
                    object[] argument = new object[1];
                    if (callback.parameterType == ParameterType.Int)
                        argument[0] = callback.Int;
                    else if (callback.parameterType == ParameterType.Float)
                        argument[0] = callback.Float;
                    else if (callback.parameterType == ParameterType.Object)
                        argument[0] = callback.Object.Resolve(origin.GetGraph().GetResolver());
                    else if (callback.parameterType == ParameterType.String)
                        argument[0] = callback.String;

                    // Call method
                    var behaviour = gameObject.GetComponent(Type.GetType(callback.assemblyName)) as MonoBehaviour;
                    MethodInfo methodInfo = behaviour.GetType().GetMethod(callback.methodName);
                    methodInfo.Invoke(behaviour, callback.parameterType == ParameterType.None ? null : argument);
                }
            }
        }
    }
}

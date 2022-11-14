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
                    // Setup arguments
                    object[] arguments = new object[callback.arguments.Length];
                    Type[] types = new Type[callback.arguments.Length];
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        var argument = callback.arguments[i];
                        if (argument.parameterType == ParameterType.Bool)
                            arguments[i] = argument.Bool;
                        else if (argument.parameterType == ParameterType.Int)
                            arguments[i] = argument.Int;
                        else if (argument.parameterType == ParameterType.Float)
                            arguments[i] = argument.Float;
                        else if (argument.parameterType == ParameterType.Object)
                            arguments[i] = argument.Object.Resolve(origin.GetGraph().GetResolver());
                        else if (argument.parameterType == ParameterType.String)
                            arguments[i] = argument.String;
                        else if (argument.parameterType == ParameterType.Enum)
                            arguments[i] = Enum.ToObject(Type.GetType(argument.String), argument.Int);

                        types[i] = arguments[i].GetType();
                    }

                    // Call method
                    var behaviour = gameObject.GetComponent(Type.GetType(callback.assemblyName)) as MonoBehaviour;
                    MethodInfo methodInfo = behaviour.GetType().GetMethod(callback.methodName, types);
                    methodInfo.Invoke(behaviour, arguments);
                }
            }
        }
    }
}

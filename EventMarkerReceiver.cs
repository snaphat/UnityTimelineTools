using System;
using UnityEditor;
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
                if (message == null || message.methods == null) return;

                foreach (var method in message.methods)
                {
                    object argument = null;
                    if (method.parameterType == Events.ParameterType.Int)
                        argument = method.Int;
                    else if (method.parameterType == Events.ParameterType.Float)
                        argument = method.Float;
                    else if (method.parameterType == Events.ParameterType.Object)
                        argument = method.Object.Resolve(origin.GetGraph().GetResolver());
                    else if (method.parameterType == Events.ParameterType.String)
                        argument = method.String;

                    SendMessage(method.name, argument);
                    // Invoke(method.name, 2);
                    // StartCoroutine(DeleteDelayed(test, 0f));
                    // IEnumerator DeleteDelayed(GameObject objectToDestroy, float delayTime)
                    // {
                    //     yield return new WaitForSeconds(delayTime);
                    //     DestroyImmediate(objectToDestroy);
                    // }
                }
            }
        }
    }
}

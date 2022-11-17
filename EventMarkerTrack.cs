using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineTools
{
    namespace Events
    {
        [TrackBindingType(typeof(GameObject)), DisplayName("Event Marker Track")]
        public class EventMarkerTrack : TrackAsset
        {
#if UNITY_EDITOR
            public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
            {
                var binding = director.GetGenericBinding(this);
                var gameObject = binding as GameObject;
                if (gameObject != null)
                {
                    foreach (var spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
                        driver.AddFromName<SpriteRenderer>(spriteRenderer.gameObject, "m_Sprite");
                    foreach (var animator in gameObject.GetComponentsInChildren<Animator>())
                        driver.AddFromName<Animator>(animator.gameObject, "m_Enabled");
                }
            }
#endif
        }
    }
}

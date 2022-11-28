using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.U2D.Animation;

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
                    foreach (var animator in gameObject.GetComponentsInChildren<Animator>())
                        driver.AddFromName<Animator>(animator.gameObject, "m_Enabled");
                    foreach (var spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
                    {
                        driver.AddFromName<SpriteRenderer>(spriteRenderer.gameObject, "m_Enabled");
                        driver.AddFromName<SpriteRenderer>(spriteRenderer.gameObject, "m_FlipX");
                        driver.AddFromName<SpriteRenderer>(spriteRenderer.gameObject, "m_FlipY");
                        driver.AddFromName<SpriteRenderer>(spriteRenderer.gameObject, "m_SortingLayer");
                        driver.AddFromName<SpriteRenderer>(spriteRenderer.gameObject, "m_SortingOrder");//
                        driver.AddFromName<SpriteRenderer>(spriteRenderer.gameObject, "m_SpriteSortPoint");
                        driver.AddFromName<SpriteRenderer>(spriteRenderer.gameObject, "m_Sprite");
                    }
                    foreach (var spriteResolver in gameObject.GetComponentsInChildren<SpriteResolver>())
                        driver.AddFromName<SpriteResolver>(spriteResolver.gameObject, "m_SpriteHash");
                    foreach (var audioSource in gameObject.GetComponentsInChildren<AudioSource>()) 
                    {
                        driver.AddFromName<AudioSource>(audioSource.gameObject, "m_audioClip");
                        driver.AddFromName<AudioSource>(audioSource.gameObject, "m_Enabled");
                        driver.AddFromName<AudioSource>(audioSource.gameObject, "m_Volume");
                        driver.AddFromName<AudioSource>(audioSource.gameObject, "m_Pitch");
                        driver.AddFromName<AudioSource>(audioSource.gameObject, "Loop");
                    }
                }
            }
#endif
        }
    }
}

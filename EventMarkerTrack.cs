using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine.Playables;
using UnityEditor;
using UnityEditor.Timeline;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;
#endif

namespace TimelineTools
{
    namespace Events
    {
        [TrackBindingType(typeof(GameObject)), DisplayName("Event Marker Track")]
        public class EventMarkerTrack : TrackAsset
        {
#if UNITY_EDITOR
            // Duplicate version of Unity's DrivenPropertyManager.bindings.cs:TryPropertyModification because it is an internal API
            private static void TryPropertyModification(Object driver, Object target, string propertyPath)
            {
                // Deviate from Unity's DrivenPropertyManager.bindings.cs:TryPropertyModification Internals because we need to retrieve private APIs:
                // TryRegisterPropertyPartial(driver, target, propertyPath);

                // Retrieve DrivenPropertyManager.TryRegisterProperty() API driver to directly call the silent version
                var drivenPropertyManager = Type.GetType("UnityEngine.DrivenPropertyManager,UnityEngine.CoreModule");
                var tryRegisterProperty = drivenPropertyManager.GetMethod("TryRegisterProperty"); // Silent (ignores property not found)
                //var registerProperty = drivenPropertyManager.GetMethod("RegisterProperty"); // Vocal (complains if property not found)

                // Setup arguments for DrivenPropertyManager.TryRegisterProperty()
                object[] arguments = new object[3];
                arguments[0] = driver; // always the same every call
                arguments[1] = target;
                arguments[2] = propertyPath;

                // Invoke the silent API
                tryRegisterProperty.Invoke(null, arguments);
            }

            // Duplicate version of Unity's PropertyCollector.cs:AddPropertyModification API which is silent instead of displaying errors for non-existent properties
            private static void AddPropertyModification(Component comp, string name)
            {
                if (comp == null)
                    return;

                // var driver = WindowState.previewDriver; // Deviate from Unity's  AddPropertyModification Internals:
                var windowState = typeof(TimelineEditor).GetProperty("state", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                var previewDriver = (AnimationModeDriver)windowState.GetType().GetProperty("previewDriver", BindingFlags.Public | BindingFlags.Static).GetValue(null);
                
                var driver = previewDriver;
                if (driver == null || !AnimationMode.InAnimationMode(driver))
                    return;

                // Register Property will display an error if a property doesn't exist (wanted behaviour)
                // However, it also displays an error on Monobehaviour m_Script property, since it can't be driven. (not wanted behaviour)
                // case 967026
                if (name == "m_Script" && (comp as MonoBehaviour) != null) 
                    return;

                // Deviate from Unity's PropertyCollector.cs:AddPropertyModification API that doesn't display errors instead:
                // DrivenPropertyManager.RegisterProperty(driver, comp, name);
                TryPropertyModification(driver, comp, name); // silent registration
            }


            // Duplicate working version of Unity's PropertyCollector.cs:AddFromComponent API.
            public void AddFromComponent(GameObject obj, Component component)
            {
                if (Application.isPlaying)
                    return;

                if (obj == null || component == null)
                    return;

                var serializedObject = new SerializedObject(component);
                SerializedProperty property = serializedObject.GetIterator();

                // Deviate from Unity's PropertyCollector.cs:AddFromComponent API because we want all children to be recorded:
                //while (property.NextVisible(true))
                while (property.Next(true)) 
                { 
                    // Deviate from Unity's PropertyCollector.cs:AddFromComponent API, because we want to register animatable types:
                    //if (property.hasVisibleChildren || !AnimatedParameterUtility.IsTypeAnimatable(property.propertyType))
                    if (property.hasVisibleChildren) 
                        continue;

                    AddPropertyModification(component, property.propertyPath);
                }
            }

            public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
            {
                // Grab game object from director
                var binding = director.GetGenericBinding(this);
                var gameObject = binding as GameObject;

                // Iterate each component of game object
                if (gameObject != null)
                    foreach (var component in gameObject.GetComponentsInChildren<Component>()) 
                        AddFromComponent(gameObject, component);
            }
#endif
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline;
using UnityEditor.Timeline.Actions;
using UnityEditorInternal;
// Adds support for inserting and cutting frames in all tracks and infinite clips within the timeline.
namespace TimelineTools
{
    class InsertCutExtensions
    {
        // Menu entries
        const string MenuPath_Insert_1 = "Tools/Timeline/Insert Frames/1 Frame";
        const string MenuPath_Insert_5 = "Tools/Timeline/Insert Frames/5 Frames";
        const string MenuPath_Insert_10 = "Tools/Timeline/Insert Frames/10 Frames";
        const string MenuPath_Insert_15 = "Tools/Timeline/Insert Frames/15 Frames";
        const string MenuPath_Insert_20 = "Tools/Timeline/Insert Frames/20 Frames";
        const string MenuPath_Insert_25 = "Tools/Timeline/Insert Frames/25 Frames";
        const string MenuPath_Insert_30 = "Tools/Timeline/Insert Frames/30 Frames";
        const string MenuPath_Insert_50 = "Tools/Timeline/Insert Frames/50 Frames";
        const string MenuPath_Insert_60 = "Tools/Timeline/Insert Frames/60 Frames";
        const string MenuPath_Insert_100 = "Tools/Timeline/Insert Frames/100 Frames";
        const string MenuPath_Insert_120 = "Tools/Timeline/Insert Frames/120 Frames";
        const string MenuPath_Insert_180 = "Tools/Timeline/Insert Frames/180 Frames";
        const string MenuPath_Insert_200 = "Tools/Timeline/Insert Frames/200 Frames";
        const string MenuPath_Insert_240 = "Tools/Timeline/Insert Frames/240 Frames";
        const string MenuPath_Insert_300 = "Tools/Timeline/Insert Frames/300 Frames";
        const string MenuPath_Insert_360 = "Tools/Timeline/Insert Frames/360 Frames";
        const string MenuPath_Insert_400 = "Tools/Timeline/Insert Frames/400 Frames";
        const string MenuPath_Insert_420 = "Tools/Timeline/Insert Frames/420 Frames";
        const string MenuPath_Insert_480 = "Tools/Timeline/Insert Frames/480 Frames";
        const string MenuPath_Insert_500 = "Tools/Timeline/Insert Frames/500 Frames";
        const string MenuPath_Insert_1000 = "Tools/Timeline/Insert Frames/1000 Frames";
        const string MenuPath_Cut_1 = "Tools/Timeline/Cut Frames/1 Frame";
        const string MenuPath_Cut_5 = "Tools/Timeline/Cut Frames/5 Frames";
        const string MenuPath_Cut_10 = "Tools/Timeline/Cut Frames/10 Frames";
        const string MenuPath_Cut_15 = "Tools/Timeline/Cut Frames/15 Frames";
        const string MenuPath_Cut_20 = "Tools/Timeline/Cut Frames/20 Frames";
        const string MenuPath_Cut_25 = "Tools/Timeline/Cut Frames/25 Frames";
        const string MenuPath_Cut_30 = "Tools/Timeline/Cut Frames/30 Frames";
        const string MenuPath_Cut_50 = "Tools/Timeline/Cut Frames/50 Frames";
        const string MenuPath_Cut_60 = "Tools/Timeline/Cut Frames/60 Frames";
        const string MenuPath_Cut_100 = "Tools/Timeline/Cut Frames/100 Frames";
        const string MenuPath_Cut_120 = "Tools/Timeline/Cut Frames/120 Frames";
        const string MenuPath_Cut_180 = "Tools/Timeline/Cut Frames/180 Frames";
        const string MenuPath_Cut_200 = "Tools/Timeline/Cut Frames/200 Frames";
        const string MenuPath_Cut_240 = "Tools/Timeline/Cut Frames/240 Frames";
        const string MenuPath_Cut_300 = "Tools/Timeline/Cut Frames/300 Frames";
        const string MenuPath_Cut_360 = "Tools/Timeline/Cut Frames/360 Frames";
        const string MenuPath_Cut_400 = "Tools/Timeline/Cut Frames/400 Frames";
        const string MenuPath_Cut_420 = "Tools/Timeline/Cut Frames/420 Frames";
        const string MenuPath_Cut_480 = "Tools/Timeline/Cut Frames/480 Frames";
        const string MenuPath_Cut_500 = "Tools/Timeline/Cut Frames/500 Frames";
        const string MenuPath_Cut_1000 = "Tools/Timeline/Cut Frames/1000 Frames";

        // Class to specify the number of frames to insert/cut
        public class Frames : Attribute
        {
            public float frames;
            public Frames(float frames) { this.frames = frames; }
        }

        // Class to call insert/cut method
        abstract class Action<T> : TimelineAction
        {
            public override ActionValidity Validate(ActionContext actionContext) { return ActionValidity.Valid; }
            public override bool Execute(ActionContext actionContext)
            { Insert(); return true; }
            public static void Insert() { InsertCutExtensions.Insert(((Frames)Attribute.GetCustomAttribute(typeof(T), typeof(Frames))).frames); }
        };

        // Add insert menu items
        [Frames(1)][MenuEntry(MenuPath_Insert_1, 0)] class Insert_1 : Action<Insert_1> { [MenuItem(MenuPath_Insert_1, priority = 0)] public static void F() { Insert(); } };
        [Frames(5)][MenuEntry(MenuPath_Insert_5, 1)] class Insert_5 : Action<Insert_5> { [MenuItem(MenuPath_Insert_5, priority = 1)] public static void F() { Insert(); } };
        [Frames(10)][MenuEntry(MenuPath_Insert_10, 2)] class Insert_10 : Action<Insert_10> { [MenuItem(MenuPath_Insert_10, priority = 2)] public static void F() { Insert(); } };
        [Frames(15)][MenuEntry(MenuPath_Insert_15, 3)] class Insert_15 : Action<Insert_15> { [MenuItem(MenuPath_Insert_15, priority = 3)] public static void F() { Insert(); } };
        [Frames(20)][MenuEntry(MenuPath_Insert_20, 4)] class Insert_20 : Action<Insert_20> { [MenuItem(MenuPath_Insert_20, priority = 4)] public static void F() { Insert(); } };
        [Frames(25)][MenuEntry(MenuPath_Insert_25, 5)] class Insert_25 : Action<Insert_25> { [MenuItem(MenuPath_Insert_25, priority = 5)] public static void F() { Insert(); } };
        [Frames(30)][MenuEntry(MenuPath_Insert_30, 6)] class Insert_30 : Action<Insert_30> { [MenuItem(MenuPath_Insert_30, priority = 6)] public static void F() { Insert(); } };
        [Frames(50)][MenuEntry(MenuPath_Insert_50, 7)] class Insert_50 : Action<Insert_50> { [MenuItem(MenuPath_Insert_50, priority = 7)] public static void F() { Insert(); } };
        [Frames(60)][MenuEntry(MenuPath_Insert_60, 8)] class Insert_60 : Action<Insert_60> { [MenuItem(MenuPath_Insert_60, priority = 8)] public static void F() { Insert(); } };
        [Frames(100)][MenuEntry(MenuPath_Insert_100, 9)] class Insert_100 : Action<Insert_100> { [MenuItem(MenuPath_Insert_100, priority = 9)] public static void F() { Insert(); } };
        [Frames(120)][MenuEntry(MenuPath_Insert_120, 10)] class Insert_120 : Action<Insert_120> { [MenuItem(MenuPath_Insert_120, priority = 10)] public static void F() { Insert(); } };
        [Frames(180)][MenuEntry(MenuPath_Insert_180, 11)] class Insert_180 : Action<Insert_180> { [MenuItem(MenuPath_Insert_180, priority = 11)] public static void F() { Insert(); } };
        [Frames(200)][MenuEntry(MenuPath_Insert_200, 12)] class Insert_200 : Action<Insert_200> { [MenuItem(MenuPath_Insert_200, priority = 12)] public static void F() { Insert(); } };
        [Frames(240)][MenuEntry(MenuPath_Insert_240, 13)] class Insert_240 : Action<Insert_240> { [MenuItem(MenuPath_Insert_240, priority = 13)] public static void F() { Insert(); } };
        [Frames(300)][MenuEntry(MenuPath_Insert_300, 14)] class Insert_300 : Action<Insert_300> { [MenuItem(MenuPath_Insert_300, priority = 14)] public static void F() { Insert(); } };
        [Frames(360)][MenuEntry(MenuPath_Insert_360, 15)] class Insert_360 : Action<Insert_360> { [MenuItem(MenuPath_Insert_360, priority = 15)] public static void F() { Insert(); } };
        [Frames(400)][MenuEntry(MenuPath_Insert_400, 16)] class Insert_400 : Action<Insert_400> { [MenuItem(MenuPath_Insert_400, priority = 16)] public static void F() { Insert(); } };
        [Frames(420)][MenuEntry(MenuPath_Insert_420, 17)] class Insert_420 : Action<Insert_420> { [MenuItem(MenuPath_Insert_420, priority = 17)] public static void F() { Insert(); } };
        [Frames(480)][MenuEntry(MenuPath_Insert_480, 18)] class Insert_480 : Action<Insert_480> { [MenuItem(MenuPath_Insert_480, priority = 18)] public static void F() { Insert(); } };
        [Frames(500)][MenuEntry(MenuPath_Insert_500, 19)] class Insert_500 : Action<Insert_500> { [MenuItem(MenuPath_Insert_500, priority = 19)] public static void F() { Insert(); } };
        [Frames(1000)][MenuEntry(MenuPath_Insert_1000, 20)] class Insert_1000 : Action<Insert_1000> { [MenuItem(MenuPath_Insert_1000, priority = 20)] public static void F() { Insert(); } };

        // Add cut menu items
        [Frames(-1)][MenuEntry(MenuPath_Cut_1, 0)] class Cut_1 : Action<Cut_1> { [MenuItem(MenuPath_Cut_1, priority = 0)] public static void F() { Insert(); } };
        [Frames(-5)][MenuEntry(MenuPath_Cut_5, 1)] class Cut_5 : Action<Cut_5> { [MenuItem(MenuPath_Cut_5, priority = 1)] public static void F() { Insert(); } };
        [Frames(-10)][MenuEntry(MenuPath_Cut_10, 2)] class Cut_10 : Action<Cut_10> { [MenuItem(MenuPath_Cut_10, priority = 2)] public static void F() { Insert(); } };
        [Frames(-15)][MenuEntry(MenuPath_Cut_15, 3)] class Cut_15 : Action<Cut_15> { [MenuItem(MenuPath_Cut_15, priority = 3)] public static void F() { Insert(); } };
        [Frames(-20)][MenuEntry(MenuPath_Cut_20, 4)] class Cut_20 : Action<Cut_20> { [MenuItem(MenuPath_Cut_20, priority = 4)] public static void F() { Insert(); } };
        [Frames(-25)][MenuEntry(MenuPath_Cut_25, 5)] class Cut_25 : Action<Cut_25> { [MenuItem(MenuPath_Cut_25, priority = 5)] public static void F() { Insert(); } };
        [Frames(-30)][MenuEntry(MenuPath_Cut_30, 6)] class Cut_30 : Action<Cut_30> { [MenuItem(MenuPath_Cut_30, priority = 6)] public static void F() { Insert(); } };
        [Frames(-50)][MenuEntry(MenuPath_Cut_50, 7)] class Cut_50 : Action<Cut_50> { [MenuItem(MenuPath_Cut_50, priority = 7)] public static void F() { Insert(); } };
        [Frames(-60)][MenuEntry(MenuPath_Cut_60, 8)] class Cut_60 : Action<Cut_60> { [MenuItem(MenuPath_Cut_60, priority = 8)] public static void F() { Insert(); } };
        [Frames(-100)][MenuEntry(MenuPath_Cut_100, 9)] class Cut_100 : Action<Cut_100> { [MenuItem(MenuPath_Cut_100, priority = 9)] public static void F() { Insert(); } };
        [Frames(-120)][MenuEntry(MenuPath_Cut_120, 10)] class Cut_120 : Action<Cut_120> { [MenuItem(MenuPath_Cut_120, priority = 10)] public static void F() { Insert(); } };
        [Frames(-180)][MenuEntry(MenuPath_Cut_180, 11)] class Cut_180 : Action<Cut_180> { [MenuItem(MenuPath_Cut_180, priority = 11)] public static void F() { Insert(); } };
        [Frames(-200)][MenuEntry(MenuPath_Cut_200, 12)] class Cut_200 : Action<Cut_200> { [MenuItem(MenuPath_Cut_200, priority = 12)] public static void F() { Insert(); } };
        [Frames(-240)][MenuEntry(MenuPath_Cut_240, 13)] class Cut_240 : Action<Cut_240> { [MenuItem(MenuPath_Cut_240, priority = 13)] public static void F() { Insert(); } };
        [Frames(-300)][MenuEntry(MenuPath_Cut_300, 14)] class Cut_300 : Action<Cut_300> { [MenuItem(MenuPath_Cut_300, priority = 14)] public static void F() { Insert(); } };
        [Frames(-360)][MenuEntry(MenuPath_Cut_360, 15)] class Cut_360 : Action<Cut_360> { [MenuItem(MenuPath_Cut_360, priority = 15)] public static void F() { Insert(); } };
        [Frames(-400)][MenuEntry(MenuPath_Cut_400, 16)] class Cut_400 : Action<Cut_400> { [MenuItem(MenuPath_Cut_400, priority = 16)] public static void F() { Insert(); } };
        [Frames(-420)][MenuEntry(MenuPath_Cut_420, 17)] class Cut_420 : Action<Cut_420> { [MenuItem(MenuPath_Cut_420, priority = 17)] public static void F() { Insert(); } };
        [Frames(-480)][MenuEntry(MenuPath_Cut_480, 18)] class Cut_480 : Action<Cut_480> { [MenuItem(MenuPath_Cut_480, priority = 18)] public static void F() { Insert(); } };
        [Frames(-500)][MenuEntry(MenuPath_Cut_500, 19)] class Cut_500 : Action<Cut_500> { [MenuItem(MenuPath_Cut_500, priority = 19)] public static void F() { Insert(); } };
        [Frames(-1000)][MenuEntry(MenuPath_Cut_1000, 20)] class Cut_1000 : Action<Cut_1000> { [MenuItem(MenuPath_Cut_1000, priority = 20)] public static void F() { Insert(); } };

        // Undo group name
        static readonly string undoKey = "Insert Frames";

        private static void Insert(float frames)
        {
            // Grab timeline asset
            var timelineAsset = TimelineEditor.inspectedAsset;
            if (timelineAsset == null) return;

            // Grab playable director
            var playableDirector = TimelineEditor.inspectedDirector;
            if (playableDirector == null) return;

            // Register undo for any changes directly to timeline asset
            UndoExtensions.RegisterCompleteTimeline(timelineAsset, undoKey);

            // Grab the current time of the playhead
            var currentTime = playableDirector.time;

            // filter only unlocked tracks
            var unlockedTracks = timelineAsset.GetOutputTracks().Where(e => !e.lockedInHierarchy);

            // Compute tolerance for determining whether to shift a track or not
            double kTimeEpsilon = 1e-14; // from com.unity.timeline/Runtime/Utilities/TimeUtility.cs 
            var tolerance = Math.Max(Math.Abs(currentTime), 1) * timelineAsset.editorSettings.frameRate * kTimeEpsilon;

            // Handle infinite animation clips (really tracks)
            // filter infinite animation tracks
            var infiniteTracks = unlockedTracks.OfType<AnimationTrack>().Where(e => !e.inClipMode && e.infiniteClip != null);
            foreach (var track in infiniteTracks)
            {
                // Grab the infinite clip in track
                var clip = track.infiniteClip;

                // Register undo for any changes in infinite clip 
                Undo.RegisterCompleteObjectUndo(clip, undoKey);

                // Get amount of time to insert/cut in seconds using the clip framerate.
                var amount = frames / clip.frameRate;

                // Update events in clip: insert/cut time by amount.
                List<AnimationEvent> updatedEvents = new();
                foreach (var evnt in clip.events)
                {
                    if (evnt.time > currentTime) evnt.time += amount;
                    updatedEvents.Add(evnt);
                }
                AnimationUtility.SetAnimationEvents(clip, updatedEvents.ToArray());

                // update float curves: insert/cut time by amount.
                var floatBindings = AnimationUtility.GetCurveBindings(clip);
                foreach (var bind in floatBindings)
                {
                    var curve = AnimationUtility.GetEditorCurve(clip, bind);
                    var keys = curve.keys;
                    for (var i = 0; i < keys.Length; i++)
                        if ((keys[i].time - currentTime) >= -tolerance) keys[i].time += amount;
                    curve.keys = keys;
                    AnimationUtility.SetEditorCurve(clip, bind, curve);
                }

                // update the PPtr curves: insert/cut time by amount.
                var objectBindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
                foreach (var bind in objectBindings)
                {
                    var curve = AnimationUtility.GetObjectReferenceCurve(clip, bind);
                    for (var i = 0; i < curve.Length; i++)
                        if ((curve[i].time - currentTime) >= -tolerance) curve[i].time += amount;
                    AnimationUtility.SetObjectReferenceCurve(clip, bind, curve);
                }

                // Grab markers to modify in track based off of current time and tolerance
                foreach (var marker in track.GetMarkers())
                {
                    if ((marker.time - currentTime) >= -tolerance) marker.time += amount;
                }

                // Mark clip as dirty
                EditorUtility.SetDirty(clip);
            }

            // Handle other tracks (all unlocked non-infinite animation or non-animation clips)
            {
                // Get other tracks
                var otherTracks = unlockedTracks.Where(e => (e is AnimationTrack a && (a.inClipMode == true || a.infiniteClip == null)) || e is not AnimationTrack).ToList();

                // Get amount of time to insert/cut in seconds for tracks using the timeline assets frame rate
                var amount = frames / timelineAsset.editorSettings.frameRate;

                // Grab clips to modify in track based off of current time and tolerance
                var clips = otherTracks.SelectMany(x => x.GetClips()).Where(x => (x.start - currentTime) >= -tolerance).ToList();
                foreach (var clip in clips)
                {
                    clip.start += amount;
                }

                // Grab markers to modify in track based off of current time and tolerance
                var markers = otherTracks.SelectMany(x => x.GetMarkers()).Where(x => (x.time - currentTime) >= -tolerance).ToList();
                foreach (var marker in markers)
                {
                    marker.time += amount;
                }
            }

            // Refresh editor
            TimelineEditor.Refresh(RefreshReason.ContentsModified);
        }
    }

    // Adds support for Synchronizing timeline with animation view
    public class AnimationViewSynchronizer
    {
        private static bool enabled = false;
        private const string menuPath = "Tools/Timeline/Sync Timeline && Animation Views";

        [MenuItem(menuPath, priority = 0)]
        public static void Sync()
        {
            enabled = !enabled;
            Menu.SetChecked(menuPath, enabled);

            // Remove and add update callback
            EditorApplication.update -= OnUpdate;
            if (enabled) EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            // Get timeline window ruler range
            var visibleTimeRange = (Vector2)typeof(TimelineEditor).GetProperty("visibleTimeRange", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            if (visibleTimeRange == null) return;

            // Get Animation Window horizontal range setter
            var animationWindow = EditorWindow.GetWindow<AnimationWindow>(false, null, false);
            if (animationWindow == null) return;
            var m_AnimEditor = animationWindow.GetType().GetField("m_AnimEditor", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(animationWindow);
            if (m_AnimEditor == null) return;
            var m_State = m_AnimEditor.GetType().GetField("m_State", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(m_AnimEditor);
            if (m_State == null) return;
            var m_TimeArea = m_State.GetType().GetField("m_TimeArea", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(m_State);
            if (m_TimeArea == null) return;
            var SetShownHRangeInsideMargins = m_TimeArea.GetType().GetMethod("SetShownHRangeInsideMargins");
            if (SetShownHRangeInsideMargins == null) return;

            // Call Range Updater on Animation view
            var parametersArray = new object[] { visibleTimeRange.x, visibleTimeRange.y };
            SetShownHRangeInsideMargins.Invoke(m_TimeArea, parametersArray);

            // Force repaint
            animationWindow.Repaint();
        }
    }

    // Adds Editor support for Timeline Event Markers for calling GameObject methods
    namespace Events
    {
        [CustomTimelineEditor(typeof(EventMarkerTrack))]
        public class EventTrackEditor : TrackEditor
        {
            readonly Texture2D iconTexture;

            public EventTrackEditor()
            {
                iconTexture = Resources.Load<Texture2D>("EventMarkerIcon");
            }

            public override TrackDrawOptions GetTrackOptions(TrackAsset track, Object binding)
            {
                var options = base.GetTrackOptions(track, binding);
                options.icon = iconTexture;
                return options;
            }
        }

        // Add event handler for detecting timeline marker events during timeline preview scrubbing - fixes scrubbing not calling events
        public class TimelineEditorEventHandler
        {
            [InitializeOnLoadMethod]
            public static void OnLoad()
            {
                EditorApplication.update -= OnUpdate;
                EditorApplication.update += OnUpdate;
            }

            // Handle pushing marker event notifications for timeline preview scrubbing
            static double previousTime = 0;
            static readonly HashSet<EventMarkerNotification> firedEvents = new(); // store fired events
            static bool inTimeline = false;
            public static void OnUpdate()
            {

                // Do nothing if no inspected director (not in timeline)
                if (TimelineEditor.inspectedDirector == null)
                {
                    // Clear all events if we were previously in the timeline
                    if (inTimeline)
                    {
                        inTimeline = false;
                        firedEvents.Clear();
                        previousTime = 0;
                    }
                    return;
                }

                var director = TimelineEditor.inspectedDirector;

                // Check if scrubbing
                var graph = director.playableGraph;
                var isScrub = !Application.isPlaying && graph.IsValid() && !graph.IsPlaying() && previousTime != director.time;
                if (!isScrub) return;

                // Keep track of the fact that we entered the timeline to clear state info later
                inTimeline = true;

                // Clear all events if timeline moved in reverse (and refire them if marked as retroactive)
                if (previousTime > director.time) firedEvents.Clear();

                // Loop each track
                for (int i = 0; i < graph.GetOutputCount(); i++)
                {
                    SortedList<double, EventMarkerNotification> sortedMarkers = new();
                    // Get track and continue if null
                    var output = graph.GetOutput(i);
                    var playable = output.GetSourcePlayable().GetInput(i);
                    var track = output.GetReferenceObject() as TrackAsset;
                    if (track == null) continue;

                    // Loop each marker of type INotification and sort
                    var markers = track.GetMarkers().OfType<Marker>().OfType<EventMarkerNotification>();
                    foreach (var marker in markers) sortedMarkers.Add(marker.time, marker);

                    // Iterate the sorted marker list to check for whether events need to be processed
                    foreach (var marker in sortedMarkers.Values)
                    {
                        if (marker.emitInEditor && !firedEvents.Contains(marker))
                        {
                            // Push notification if current time matches notification time
                            if (director.time == 0 && marker.time == 0 || Math.Abs(director.time - marker.time) < 1e-14)
                            {
                                // Add to list of fired events 
                                firedEvents.Add(marker);

                                // Push event
                                output.PushNotification(playable, marker);
                            }
                            // Push notification if time after notification time and notification hasn't been previously pushed
                            else if (director.time >= marker.time)
                            {
                                // Add to list of fired events 
                                firedEvents.Add(marker);

                                // Don't emit if timeline reversed and notification not marked as retroactive
                                if (previousTime > director.time && !marker.retroactive) continue;

                                // Push event
                                output.PushNotification(playable, marker);
                            }
                        }
                    }
                }

                // Record current time
                previousTime = director.time;
            }
        }

        // For storing parsed method information in the editor
        public class CallbackDescription
        {
            public string assemblyName;         // Object type of the method
            public string methodName;           // The short name of the method
            public string fullMethodName;       // The name of the method with parameters: e.g.: Foo(arg_type)
            public string qualifiedMethodName;  // The name of the class + method + parameters: e.g. Bar.Foo(arg_type)
            public List<Type> parameterTypes;   // none, bool, int, float, string, Object, Enum
            public ArrayList defaultParameters; // default value for the given parameter
        }

        // For uniquely identifying Stored SerializedProperty methods with found CallbackDescription methods using assembly name, method name, and argument types
        public static class ListExtensions
        {
            public static int FindMethod(this IList<CallbackDescription> callbacks, SerializedProperty assemblyName, SerializedProperty methodName, SerializedProperty arguments)
            {
                // Iterate each callback method in list
                for (int id = 0; id < callbacks.Count; id++)
                {
                    var callback = callbacks[id];

                    // if num params, assembly name, or method name don't match continue to next callback
                    if (arguments.arraySize != callback.parameterTypes.Count || assemblyName.stringValue.Split(",")[0] != callback.assemblyName.Split(",")[0] || methodName.stringValue != callback.methodName)
                        continue;

                    // Iterate each param type
                    bool isMatch = true;
                    int i;
                    for (i = 0; i < callback.parameterTypes.Count; i++)
                    {
                        // Grab types
                        var type = callback.parameterTypes[i];
                        var argumentProperty = arguments.GetArrayElementAtIndex(i);
                        SerializedProperty m_ParameterType = argumentProperty.FindPropertyRelative("parameterType");
                        var type2 = m_ParameterType.enumValueIndex;

                        // break early if no match
                        if (type == typeof(bool) && type2 == (int)ParameterType.Bool)
                            continue;
                        else if (type == typeof(int) && type2 == (int)ParameterType.Int)
                            continue;
                        else if (type == typeof(float) && type2 == (int)ParameterType.Float)
                            continue;
                        else if (type == typeof(string) && type2 == (int)ParameterType.String)
                            continue;
                        else if (type == typeof(Playable) && type2 == (int)ParameterType.Playable) // handle before object
                            continue;
                        else if (type == typeof(EventMarkerNotification) && type2 == (int)ParameterType.EventMarkerNotification) // handle before object
                            continue;
                        else if ((type == typeof(object) || type.IsSubclassOf(typeof(Object))) && type2 == (int)ParameterType.Object)
                            continue;
                        else if (type.IsEnum && (type2 == (int)ParameterType.Enum || argumentProperty.FindPropertyRelative("String").stringValue.Split(",")[0] == type.FullName))
                            continue;
                        isMatch = false;
                        break;
                    }

                    // if count match then method matches so return id
                    if (isMatch) return id;
                }
                return -1;
            }
        }

        // Custom Inspector for creating EventMarkers
        [CustomEditor(typeof(EventMarkerNotification)), CanEditMultipleObjects]
        public class EventMarkerInspector : Editor
        {
            // Cached data for speeding up editor
            class EditorCache
            {
                public int selectedMethodId; // selected id for a given reorderable list entry
                public string[] dropdown; // dropdown for for a given reorderable list entry
            }
            Dictionary<int, EditorCache> editorCache;
            GameObject cachedGameObject; // bound game object
            List<CallbackDescription> cachedSupportedMethods; // supported methods for the given game object
            ReorderableList cachedMethodList; // selected methods in a reorderable list
            int cachedMethodListCount; // element count in cachedMethodList

            // Properties
            SerializedProperty m_Time;
            SerializedProperty m_Callbacks;
            SerializedProperty m_Retroactive;
            SerializedProperty m_EmitOnce;
            SerializedProperty m_EmitInEditor;
            SerializedProperty m_Color;
            SerializedProperty m_ShowLineOverlay;

            // Get serialized object properties (for UI)
            public void OnEnable()
            {
                // Functional properties
                m_Time = serializedObject.FindProperty("m_Time");
                m_Callbacks = serializedObject.FindProperty("callbacks");
                m_Retroactive = serializedObject.FindProperty("retroactive");
                m_EmitOnce = serializedObject.FindProperty("emitOnce");
                m_EmitInEditor = serializedObject.FindProperty("emitInEditor");

                // Style properties
                m_Color = serializedObject.FindProperty("color");
                m_ShowLineOverlay = serializedObject.FindProperty("showLineOverlay");
            }

            // Draw inspector GUI
            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                var marker = target as Marker;

                // Make sure there is an instance of all objects before attempting to show anything in inspector
                if (marker == null || marker.parent == null || TimelineEditor.inspectedDirector == null) return;

                // Get bound scene object
                var boundObj = TimelineEditor.inspectedDirector.GetGenericBinding(marker.parent);
                {
                    using var changeScope = new EditorGUI.ChangeCheckScope();
                    EditorGUILayout.PropertyField(m_Time);
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Event Properties");
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(m_Retroactive);
                    EditorGUILayout.PropertyField(m_EmitOnce);
                    EditorGUILayout.PropertyField(m_EmitInEditor);

                    EditorGUILayout.Space();
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField("Marker Style");
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(m_Color);
                    EditorGUILayout.PropertyField(m_ShowLineOverlay);

                    EditorGUILayout.Space();

                    GameObject curGameObject = null;
                    if (boundObj as GameObject != null) curGameObject = (GameObject)boundObj;
                    else if (boundObj as Component != null) curGameObject = ((Component)boundObj).gameObject;

                    // Workaround Unity Timeline bug where active context is lost on save which breaks the inspector for
                    // Object Fields with Exposed References
                    if (Selection.activeContext == null)
                        Selection.SetActiveObjectWithContext(target, TimelineEditor.inspectedDirector); // Re-set the context

                    // Only rebuild list if something as changed (it isn't draggable otherwise)
                    if (cachedMethodList == null || cachedMethodListCount != cachedMethodList.count || cachedGameObject != curGameObject)
                    {
                        // Warning -- event markers should only be used in event marker tracks for correct timeline preview behaviour
                        if (marker.parent is not EventMarkerTrack)
                            Debug.LogWarning("<color=red>TimelineTools: Add Event Marker to an Event Marker Track</color>");

                        cachedGameObject = curGameObject;
                        cachedSupportedMethods = CollectSupportedMethods(cachedGameObject).ToList();

                        cachedMethodList = new ReorderableList(serializedObject, m_Callbacks, true, true, true, true)
                        {
                            elementHeightCallback = GetElementHeight,
                            drawElementCallback = DrawMethodAndArguments,
                            drawHeaderCallback = delegate (Rect rect) { EditorGUI.LabelField(rect, "GameObject Methods"); }
                        };
                        cachedMethodListCount = cachedMethodList.count;
                        editorCache = new();
                    }

                    // Layout reorderable list
                    cachedMethodList.DoLayoutList();

                    // apply changes
                    if (changeScope.changed) serializedObject.ApplyModifiedProperties();
                }
            }

            // Height determiner for a given element
            float GetElementHeight(int index)
            {
                // Retrieve element (elements are added when + is clicked in reorderable list UI)
                SerializedProperty element = cachedMethodList.serializedProperty.GetArrayElementAtIndex(index);

                // Retrieve element properties
                SerializedProperty m_Arguments = element.FindPropertyRelative("arguments");

                // Determine height
                if (m_Arguments.arraySize == 0) return EditorGUIUtility.singleLineHeight + 10;
                else return EditorGUIUtility.singleLineHeight * 2 + 10;
            }

            // Draw drawer entry for given element
            void DrawMethodAndArguments(Rect rect, int index, bool isActive, bool isFocused)
            {
                // Compute first field position
                Rect line = new(rect.x, rect.y + 4, rect.width, EditorGUIUtility.singleLineHeight);

                // Retrieve element (elements are added when + is clicked in reorderable list UI)
                SerializedProperty element = cachedMethodList.serializedProperty.GetArrayElementAtIndex(index);

                // Retrieve element properties
                SerializedProperty m_AssemblyName = element.FindPropertyRelative("assemblyName");
                SerializedProperty m_MethodName = element.FindPropertyRelative("methodName");
                SerializedProperty m_Arguments = element.FindPropertyRelative("arguments");

                // Initialize cache for this index if not initialized
                if (!editorCache.ContainsKey(index)) editorCache.Add(index, new());
                var cache = editorCache[index];

                // Generate dropdown if the the cache is empty
                if (cache.dropdown == null)
                {
                    // Get current method ID based off of stored name (index really)
                    cache.selectedMethodId = cachedSupportedMethods.FindMethod(m_AssemblyName, m_MethodName, m_Arguments);

                    // Create dropdown
                    var qualifiedMethodNames = cachedSupportedMethods.Select(i => i.qualifiedMethodName);
                    var dropdownList = new List<string>() { "", "" }; // Add 2x blank line entries
                    dropdownList.AddRange(qualifiedMethodNames);
                    cache.dropdown = dropdownList.ToArray();
                }

                // Draw popup (dropdown box)
                var previousMixedValue = EditorGUI.showMixedValue;
                {
                    GUIStyle style = EditorStyles.popup;
                    style.richText = true;

                    // Create dropdownlist with 'pseudo entry' for the currently selected method at the top of the list
                    CallbackDescription selectedMethod = cache.selectedMethodId > -1 ? cachedSupportedMethods[cache.selectedMethodId] : null;
                    cache.dropdown[0] = cache.selectedMethodId > -1 ? selectedMethod.assemblyName.Split(",")[0] + "." + selectedMethod.fullMethodName : "No method";

                    // Store old selected method id case it isn't changed
                    var oldSelectedMethodId = cache.selectedMethodId;
                    cache.selectedMethodId = EditorGUI.Popup(line, 0, cache.dropdown, style);

                    // Update field position
                    line.y += EditorGUIUtility.singleLineHeight + 2;

                    // Normalize selection
                    if (cache.selectedMethodId == 0)
                        cache.selectedMethodId = oldSelectedMethodId; // No selection so restore actual method id
                    else
                        cache.selectedMethodId -= 2; // normalize to get actual Id (-2 for the two 'pseudo entries'
                }

                EditorGUI.showMixedValue = previousMixedValue;

                // If selected method is valid then try to draw parameters
                if (cache.selectedMethodId > -1 && cache.selectedMethodId < cachedSupportedMethods.Count)
                {
                    var callbackDescription = cachedSupportedMethods.ElementAt(cache.selectedMethodId);

                    // Detect method change in order to initialize default parameters
                    var methodChanged = m_MethodName.stringValue != callbackDescription.methodName;

                    // Fillout assembly and method name properties using the selected id
                    m_AssemblyName.stringValue = callbackDescription.assemblyName;
                    m_MethodName.stringValue = callbackDescription.methodName;

                    // Draw each argument
                    DrawArguments(line, element, callbackDescription, methodChanged);
                }
            }

            // Create UI elements for the given parameter types of a methods arguments
            void DrawArguments(Rect rect, SerializedProperty element, CallbackDescription callbackDescription, bool initialize)
            {
                // Find the amount of user enterable arguments to compute UI entry box sizes
                int enterableArgCount = 0;
                foreach (var type in callbackDescription.parameterTypes)
                    if (type != typeof(Playable) && type != typeof(EventMarkerNotification)) enterableArgCount++;

                // Compute the rect for the method parameters based off of the count
                var paramWidth = rect.width / enterableArgCount;
                rect.width = paramWidth - 5;

                // Grab the arguments property
                SerializedProperty m_Arguments = element.FindPropertyRelative("arguments");

                // Resize the arguments array
                m_Arguments.arraySize = callbackDescription.parameterTypes.Count;

                // Iterate and display each argument by type
                for (var i = 0; i < m_Arguments.arraySize; i++)
                {
                    var type = callbackDescription.parameterTypes[i];
                    var defaultValue = callbackDescription.defaultParameters[i];
                    var argumentProperty = m_Arguments.GetArrayElementAtIndex(i);
                    SerializedProperty m_ParameterType = argumentProperty.FindPropertyRelative("parameterType");

                    // Assign Param type and generate field. The Field style is determined by the serialized property type
                    if (type == typeof(bool))
                    {
                        m_ParameterType.enumValueIndex = (int)ParameterType.Bool;
                        var property = argumentProperty.FindPropertyRelative("Bool");
                        if (initialize && defaultValue.GetType() != typeof(DBNull)) property.boolValue = (bool)defaultValue;
                        EditorGUI.PropertyField(rect, property, GUIContent.none);
                    }
                    else if (type == typeof(int))
                    {
                        m_ParameterType.enumValueIndex = (int)ParameterType.Int;
                        var property = argumentProperty.FindPropertyRelative("Int");
                        if (initialize && defaultValue.GetType() != typeof(DBNull)) property.intValue = (int)defaultValue;
                        EditorGUI.PropertyField(rect, property, GUIContent.none);
                    }
                    else if (type == typeof(float))
                    {
                        m_ParameterType.enumValueIndex = (int)ParameterType.Float;
                        var property = argumentProperty.FindPropertyRelative("Float");
                        if (initialize && defaultValue.GetType() != typeof(DBNull)) property.floatValue = (float)defaultValue;
                        EditorGUI.PropertyField(rect, property, GUIContent.none);
                    }
                    else if (type == typeof(string))
                    {
                        m_ParameterType.enumValueIndex = (int)ParameterType.String;
                        var property = argumentProperty.FindPropertyRelative("String");
                        if (initialize && defaultValue.GetType() != typeof(DBNull)) property.stringValue = (string)defaultValue;
                        EditorGUI.PropertyField(rect, property, GUIContent.none);
                    }
                    else if (type == typeof(Playable)) // handle before object
                    {
                        m_ParameterType.enumValueIndex = (int)ParameterType.Playable;
                        continue;
                    }
                    else if (type == typeof(EventMarkerNotification)) // handle before object
                    {
                        m_ParameterType.enumValueIndex = (int)ParameterType.EventMarkerNotification;
                        continue;
                    }
                    else if (type == typeof(object) || type.IsSubclassOf(typeof(Object)))
                    {
                        var property = argumentProperty.FindPropertyRelative("Object");
                        var exposedName = property.FindPropertyRelative("exposedName");
                        var defaultExposedValue = property.FindPropertyRelative("defaultValue");

                        m_ParameterType.enumValueIndex = (int)ParameterType.Object;
                        if (initialize && defaultValue.GetType() != typeof(DBNull)) property.exposedReferenceValue = (Object)defaultValue;
                        var obj = EditorGUI.ObjectField(rect, property.exposedReferenceValue, type, true);
                        if (property.exposedReferenceValue != obj)
                        {
                            TimelineEditor.inspectedDirector.ClearReferenceValue(exposedName.stringValue);
                            exposedName.stringValue = "";
                            defaultExposedValue.objectReferenceValue = null;
                            if (obj is GameObject x && x.scene.name != null)
                                property.exposedReferenceValue = obj;           // scene / exposed object
                            else
                                defaultExposedValue.objectReferenceValue = obj; // prefab-nonscene object
                        }
                    }
                    else if (type.IsEnum)
                    {
                        m_ParameterType.enumValueIndex = (int)ParameterType.Enum;
                        var intProperty = argumentProperty.FindPropertyRelative("Int");
                        var stringProperty = argumentProperty.FindPropertyRelative("String");
                        if (initialize && defaultValue.GetType() != typeof(DBNull)) intProperty.intValue = (int)defaultValue;
                        intProperty.intValue = (int)(object)EditorGUI.EnumPopup(rect, (Enum)Enum.ToObject(type, intProperty.intValue)); // Parse as enum type
                        stringProperty.stringValue = type.FullName; // store full type name in string
                    }

                    // Update field position
                    rect.x += paramWidth;
                }
            }

            // Helper method for retrieving method signatures from an Object
            public static IEnumerable<CallbackDescription> CollectSupportedMethods(Object obj)
            {
                // return if object is null
                if (obj == null) return Enumerable.Empty<CallbackDescription>();

                // Create a list to fill with supported methods
                List<CallbackDescription> supportedMethods = new();

                // Create a list of objects to search methods for (include base object)
                List<Object> objectList = new() { obj };

                // Get components if object is a game object
                var components = (obj as GameObject)?.GetComponentsInChildren<Component>();
                if (components != null) objectList.AddRange(components);

                // Iterate over base Object and all components
                foreach (var item in objectList)
                {
                    // Get item type. If the type is a monoscript then get the class type directly
                    var itemType = item is MonoScript monoScript ? monoScript.GetClass() : item.GetType();

                    // Loop over type for derived type up the entire inheritence hierarchy 
                    while (itemType != null)
                    {
                        // Get methods for class type. Include instance methods if the type is a game object or component
                        var methods = itemType.GetMethods((item is GameObject || item is Component ? BindingFlags.Instance : 0) | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                        foreach (var method in methods)
                        {
                            // don't support adding built in method names
                            if (method.Name == "Main" && method.Name == "Start" && method.Name == "Awake" && method.Name == "Update") continue;

                            var parameters = method.GetParameters();    // get parameters
                            List<Type> parameterTypes = new();          // create empty parameter list
                            ArrayList defaultValues = new();            // create empty default arguments list
                            string fullMethodName = method.Name + "(";  // start full method name signature
                            bool validMethod = true;                    // mark the method as valid until proven otherwise

                            // Parse parameter types
                            for (int i = 0; i < parameters.Length; i++)
                            {
                                if (i > 0) fullMethodName += ", ";
                                var parameter = parameters[i];
                                var strType = "";
                                if (parameter.ParameterType == typeof(bool))
                                    strType = "bool";
                                else if (parameter.ParameterType == typeof(int))
                                    strType = "int";
                                else if (parameter.ParameterType == typeof(float))
                                    strType = "float";
                                else if (parameter.ParameterType == typeof(string))
                                    strType = "string";
                                else if (parameter.ParameterType == typeof(Playable)) // handle before object
                                    strType = "Playable";
                                else if (parameter.ParameterType == typeof(EventMarkerNotification)) // handle before object
                                    strType = "EventMarkerNotification";
                                else if (parameter.ParameterType == typeof(object) || parameter.ParameterType.IsSubclassOf(typeof(Object)))
                                    strType = "Object";
                                else if (parameter.ParameterType.IsEnum && Enum.GetUnderlyingType(parameter.ParameterType) == typeof(int))
                                    strType = parameter.ParameterType.Name; // use underlying typename for fullanme string
                                else
                                    validMethod = false;

                                // Add parameter and update full method name with parameter type and name
                                parameterTypes.Add(parameter.ParameterType);
                                defaultValues.Add(parameter.DefaultValue);
                                fullMethodName += strType + " " + parameter.Name;
                            }

                            // one or more argument types was not supported so don't add method.
                            if (validMethod == false) continue;

                            // Finish the full name signature
                            fullMethodName += ")";

                            // Collect the first two pieces of the FQN
                            var assemblyName = itemType.FullName + "," + itemType.Module.Assembly.GetName().Name;

                            // Create method description object
                            var supportedMethod = new CallbackDescription
                            {
                                methodName = method.Name,
                                fullMethodName = fullMethodName,
                                qualifiedMethodName = itemType + "/" + fullMethodName[0] + "/" + fullMethodName,
                                parameterTypes = parameterTypes,
                                defaultParameters = defaultValues,
                                assemblyName = assemblyName
                            };
                            supportedMethods.Add(supportedMethod);
                        }

                        // Get base type to check it for methods as well
                        itemType = itemType.BaseType;
                    }
                }

                return supportedMethods.OrderBy(x => x.fullMethodName, StringComparer.Ordinal).ToList();
            }
        }

        // Editor used by the Timeline window to customize the appearance of a marker
        [CustomTimelineEditor(typeof(EventMarkerNotification))]
        public class EventMarkerOverlay : MarkerEditor
        {
            const float k_LineOverlayWidth = 6.0f;

            static readonly Texture2D iconTexture;
            static readonly Texture2D overlayTexture;
            static readonly Texture2D overlaySelectedTexture;
            static readonly Texture2D overlayCollapsedTexture;

            static EventMarkerOverlay()
            {
                iconTexture = Resources.Load<Texture2D>("EventMarkerIcon");
                overlayTexture = Resources.Load<Texture2D>("EventMarker");
                overlaySelectedTexture = Resources.Load<Texture2D>("EventMarker_Selected");
                overlayCollapsedTexture = Resources.Load<Texture2D>("EventMarker_Collapsed");
            }

            // Draws a vertical line on top of the Timeline window's contents.
            public override void DrawOverlay(IMarker marker, MarkerUIStates uiState, MarkerOverlayRegion region)
            {
                // The `marker argument needs to be cast as the appropriate type, usually the one specified in the `CustomTimelineEditor` attribute
                var annotation = marker as EventMarkerNotification;
                if (annotation == null) return;

                if (annotation.showLineOverlay) DrawLineOverlay(annotation.color, region);

                DrawColorOverlay(region, annotation.color, uiState);
            }

            // Sets the marker's tooltip based on its title.
            public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
            {
                // The `marker argument needs to be cast as the appropriate type, usually the one specified in the `CustomTimelineEditor` attribute
                var eventMarker = marker as EventMarkerNotification;
                if (eventMarker == null) return base.GetMarkerOptions(marker);

                // Set marker icon
                EditorGUIUtility.SetIconForObject(eventMarker, iconTexture);

                // Tooltip format
                string richMethodFormat = EditorGUIUtility.isProSkin ?
                "<b><color=#569cd6>{0}</color><color=#c586c0>.</color><color=#dcdcaa>{1}</color><color=#ffd700>(</color>{2}<color=#ffd700>)</color></b>\n" : "<b><color=#0000ff>{0}</color><color=#319331>.</color><color=#795e26>{1}</color><color=#0431fa>(</color>{2}<color=#0431fa>)</color></b>\n";
                string richArgumentFormat = EditorGUIUtility.isProSkin ?
                "<color={2}>{0}</color> <color=#c586c0>(</color><color=#569cd6>{1}</color><color=#c586c0>)</color>" : "<color={2}>{0}</color> <color=#319331>(</color><color=#0000ff>{1}</color><color=#319331>)</color>";

                // Create tooltip
                string tooltip = "";

                if (eventMarker.callbacks != null)
                {
                    foreach (var callback in eventMarker.callbacks)
                    {
                        // if no method name, give up
                        if (callback.methodName.Length == 0) continue;

                        string arg = "", type = "", color = "#000000", argumentText = "";
                        for (int i = 0; i < callback.arguments.Length; i++)
                        {
                            if (i > 0) argumentText += ", ";

                            // Supports int, float, Object, string, enum, and none types. The Field style is determined by the serialized property type
                            var argument = callback.arguments[i];
                            var objectValue = argument.Object.defaultValue.ToString().Split('(', ')');
                            if (argument.parameterType == ParameterType.Bool)
                                (arg, type, color) = (argument.Bool.ToString(), "bool", argument.Bool ? "#009900" : "#ff2222");
                            else if (argument.parameterType == ParameterType.Int)
                                (arg, type, color) = (argument.Int.ToString(), "int", EditorGUIUtility.isProSkin ? "#b5cea8" : "#098658");
                            else if (argument.parameterType == ParameterType.Float)
                                (arg, type, color) = (argument.Float.ToString(), "float", EditorGUIUtility.isProSkin ? "#b5cea8" : "#098658");
                            else if (argument.parameterType == ParameterType.String)
                                (arg, type, color) = ("\"" + argument.String + "\"", "string", EditorGUIUtility.isProSkin ? "#ce9178" : "#a31515");
                            else if (argument.parameterType == ParameterType.Playable)
                                (arg, type, color) = ("playable", "Playable", EditorGUIUtility.isProSkin ? "#4ec9b0" : "#267f99");
                            else if (argument.parameterType == ParameterType.EventMarkerNotification)
                                (arg, type, color) = ("notification", "EventMarkerNotification", EditorGUIUtility.isProSkin ? "#4ec9b0" : "#267f99");
                            else if (argument.parameterType == ParameterType.Object)
                                (arg, type, color) = objectValue[0] == "null" ? ("None", "Object", "#ff0000") :
                                                   objectValue[0].Length > 48 ? ("...", "Object", EditorGUIUtility.isProSkin ? "#4ec9b0" : "#267f99") :
                                                                                (objectValue[0], objectValue[1], EditorGUIUtility.isProSkin ? "#4ec9b0" : "#267f99");
                            else if (argument.parameterType == ParameterType.Enum)
                                (arg, type, color) = (Enum.ToObject(Type.GetType(argument.String + ",Assembly-CSharp"), argument.Int).ToString(), "Enum", EditorGUIUtility.isProSkin ? "#4ec9b0" : "#267f99");

                            argumentText += string.Format(richArgumentFormat, arg, type, color);
                        }

                        // Format and trim string if no args
                        tooltip += string.Format(richMethodFormat, callback.assemblyName.Split(",")[0], callback.methodName, argumentText);
                    }
                }

                tooltip = tooltip.Length == 0 ? "No method" : tooltip.TrimEnd();
                return new MarkerDrawOptions { tooltip = tooltip };
            }

            static void DrawLineOverlay(Color color, MarkerOverlayRegion region)
            {
                // Calculate markerRegion's center on the x axis
                float markerRegionCenterX = region.markerRegion.xMin + (region.markerRegion.width - k_LineOverlayWidth) / 2.0f;

                // Calculate a rectangle that uses the full timeline region's height
                Rect overlayLineRect = new(markerRegionCenterX, region.timelineRegion.y, k_LineOverlayWidth, region.timelineRegion.height);

                Color overlayLineColor = new(color.r, color.g, color.b, color.a * 0.5f);
                EditorGUI.DrawRect(overlayLineRect, overlayLineColor);
            }

            static void DrawColorOverlay(MarkerOverlayRegion region, Color color, MarkerUIStates state)
            {
                // Save the Editor's overlay color before changing it
                Color oldColor = GUI.color;

                if (state.HasFlag(MarkerUIStates.Selected))
                {
                    GUI.color = color;
                    GUI.DrawTexture(region.markerRegion, overlayTexture);
                    GUI.color = new(1.0f, 1.0f, 1.0f, 1.0f);
                    GUI.DrawTexture(region.markerRegion, overlaySelectedTexture);
                }
                else if (state.HasFlag(MarkerUIStates.Collapsed))
                {
                    GUI.color = color;
                    GUI.DrawTexture(region.markerRegion, overlayCollapsedTexture);
                }
                else if (state.HasFlag(MarkerUIStates.None))
                {
                    GUI.color = color;
                    GUI.DrawTexture(region.markerRegion, overlayTexture);
                }

                // Restore the previous Editor's overlay color
                GUI.color = oldColor;
            }
        }
    }
}
#endif

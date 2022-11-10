using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEditor.Timeline.Actions;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
// Adds support for inserting and cutting frames in all tracks and infinite clips within the timeline.
namespace TimelineTools
{
    class InsertCutExtensions
    {
        // Menu entries
        const string MenuPath_Insert_1 = "Tools/Timeline/Insert Frames/1 Frame";
        const string MenuPath_Insert_5 = "Tools/Timeline/Insert Frames/5 Frames";
        const string MenuPath_Insert_10 = "Tools/Timeline/Insert Frames/10 Frames";
        const string MenuPath_Insert_25 = "Tools/Timeline/Insert Frames/25 Frames";
        const string MenuPath_Insert_30 = "Tools/Timeline/Insert Frames/30 Frames";
        const string MenuPath_Insert_50 = "Tools/Timeline/Insert Frames/50 Frames";
        const string MenuPath_Insert_60 = "Tools/Timeline/Insert Frames/60 Frames";
        const string MenuPath_Insert_100 = "Tools/Timeline/Insert Frames/100 Frames";
        const string MenuPath_Cut_1 = "Tools/Timeline/Cut Frames/1 Frame";
        const string MenuPath_Cut_5 = "Tools/Timeline/Cut Frames/5 Frames";
        const string MenuPath_Cut_10 = "Tools/Timeline/Cut Frames/10 Frames";
        const string MenuPath_Cut_25 = "Tools/Timeline/Cut Frames/25 Frames";
        const string MenuPath_Cut_30 = "Tools/Timeline/Cut Frames/30 Frames";
        const string MenuPath_Cut_50 = "Tools/Timeline/Cut Frames/50 Frames";
        const string MenuPath_Cut_60 = "Tools/Timeline/Cut Frames/60 Frames";
        const string MenuPath_Cut_100 = "Tools/Timeline/Cut Frames/100 Frames";

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
        [Frames(25)][MenuEntry(MenuPath_Insert_25, 3)] class Insert_25 : Action<Insert_25> { [MenuItem(MenuPath_Insert_25, priority = 3)] public static void F() { Insert(); } };
        [Frames(30)][MenuEntry(MenuPath_Insert_30, 4)] class Insert_30 : Action<Insert_30> { [MenuItem(MenuPath_Insert_30, priority = 4)] public static void F() { Insert(); } };
        [Frames(50)][MenuEntry(MenuPath_Insert_50, 5)] class Insert_50 : Action<Insert_50> { [MenuItem(MenuPath_Insert_50, priority = 5)] public static void F() { Insert(); } };
        [Frames(60)][MenuEntry(MenuPath_Insert_60, 6)] class Insert_60 : Action<Insert_60> { [MenuItem(MenuPath_Insert_60, priority = 6)] public static void F() { Insert(); } };
        [Frames(100)][MenuEntry(MenuPath_Insert_100, 7)] class Insert_100 : Action<Insert_100> { [MenuItem(MenuPath_Insert_100, priority = 7)] public static void F() { Insert(); } };

        // Add cut menu items
        [Frames(-1)][MenuEntry(MenuPath_Cut_1, 0)] class Cut_1 : Action<Cut_1> { [MenuItem(MenuPath_Cut_1, priority = 0)] public static void F() { Insert(); } };
        [Frames(-5)][MenuEntry(MenuPath_Cut_5, 1)] class Cut_5 : Action<Cut_5> { [MenuItem(MenuPath_Cut_5, priority = 1)] public static void F() { Insert(); } };
        [Frames(-10)][MenuEntry(MenuPath_Cut_10, 2)] class Cut_10 : Action<Cut_10> { [MenuItem(MenuPath_Cut_10, priority = 2)] public static void F() { Insert(); } };
        [Frames(-25)][MenuEntry(MenuPath_Cut_25, 3)] class Cut_25 : Action<Cut_25> { [MenuItem(MenuPath_Cut_25, priority = 3)] public static void F() { Insert(); } };
        [Frames(-30)][MenuEntry(MenuPath_Cut_30, 4)] class Cut_30 : Action<Cut_30> { [MenuItem(MenuPath_Cut_30, priority = 4)] public static void F() { Insert(); } };
        [Frames(-50)][MenuEntry(MenuPath_Cut_50, 5)] class Cut_50 : Action<Cut_50> { [MenuItem(MenuPath_Cut_50, priority = 5)] public static void F() { Insert(); } };
        [Frames(-60)][MenuEntry(MenuPath_Cut_60, 6)] class Cut_60 : Action<Cut_60> { [MenuItem(MenuPath_Cut_60, priority = 6)] public static void F() { Insert(); } };
        [Frames(-100)][MenuEntry(MenuPath_Cut_100, 7)] class Cut_100 : Action<Cut_100> { [MenuItem(MenuPath_Cut_100, priority = 7)] public static void F() { Insert(); } };

        // Undo group name
        static readonly String undoKey = "Insert Frames";

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
            var infiniteTracks = unlockedTracks.OfType<AnimationTrack>().Where(e => !e.inClipMode);
            foreach (var track in infiniteTracks)
            {
                // Grab the infinite clip in track
                var clip = track.infiniteClip;

                // Register undo for any changes in infinite clip 
                Undo.RegisterCompleteObjectUndo(clip, undoKey);

                // Get amount of time to insert/cut in seconds using the clip framerate.
                var amount = frames / clip.frameRate;

                // Update events in clip: insert/cut time by amount.
                var updatedEvents = new List<AnimationEvent>();
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
                var otherTracks = unlockedTracks.Where(e => (e is AnimationTrack a && a.inClipMode == true) || e is not AnimationTrack).ToList();

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
            var m_AnimEditor = animationWindow.GetType().GetField("m_AnimEditor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(animationWindow);
            if (m_AnimEditor == null) return;
            var m_State = m_AnimEditor.GetType().GetField("m_State", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(m_AnimEditor);
            if (m_State == null) return;
            var m_TimeArea = m_State.GetType().GetField("m_TimeArea", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(m_State);
            if (m_TimeArea == null) return;
            var SetShownHRangeInsideMargins = m_TimeArea.GetType().GetMethod("SetShownHRangeInsideMargins");
            if (SetShownHRangeInsideMargins == null) return;

            // Call Range Updater on Animation view
            object[] parametersArray = new object[] { visibleTimeRange.x, visibleTimeRange.y };
            SetShownHRangeInsideMargins.Invoke(m_TimeArea, parametersArray);

            // Force repaint
            animationWindow.Repaint();
        }
    }

    // Adds Editor support for Timeline Event Markers for calling GameObject methods
    public partial class Events
    {
        // Add event handler for detecting timeline marker events during timeline preview scrubbing
        [InitializeOnLoadMethod]
        public static void OnLoad()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        // Handle pushing marker event notifications for timeline preview scrubbing
        static double previousTime = 0.0f;
        public static void OnUpdate()
        {
            var director = TimelineEditor.inspectedDirector;

            // Check if scrubbing
            var isScrub = director != null && director.playableGraph.IsValid() && !director.playableGraph.IsPlaying() && previousTime != director.time;
            if (!isScrub) return;

            // Loop each track
            for (int i = 0; i < director.playableGraph.GetOutputCount(); i++)
            {
                // Get track and continue if null
                var output = director.playableGraph.GetOutput(i);
                var playable = output.GetSourcePlayable().GetInput(i);
                var track = output.GetReferenceObject() as TrackAsset;
                if (track == null) continue;

                // Loop each marker of type INotification
                var notifications = track.GetMarkers().OfType<Marker>().OfType<INotification>();
                foreach (var notification in notifications)
                {
                    // Push notification if time change in range
                    double time = (notification as Marker).time;
                    bool fire = (time >= previousTime && time < director.time) || (time > director.time && time <= previousTime);
                    if (fire) output.PushNotification(playable, notification);
                }
            }

            // Record current time
            previousTime = director.time;
        }

        // For storing parased method information in the editor
        public class MethodDesc
        {
            public string fullname;    // Foo(arg_type)
            public string name;        // Foo
            public ParameterType type; // None, int, float, string, Object
            public bool isOverload;    // Overloads not handable so need detection
        }

        // Helper method for retrieving method signatures from a game object
        public static IEnumerable<MethodDesc> CollectSupportedMethods(GameObject gameObject)
        {
            if (gameObject == null)
                return Enumerable.Empty<MethodDesc>();

            var supportedMethods = new List<MethodDesc>();
            var behaviours = gameObject.GetComponents<MonoBehaviour>();

            foreach (var behaviour in behaviours)
            {
                if (behaviour == null)
                    continue;

                var type = behaviour.GetType();
                while (type != typeof(MonoBehaviour) && type != null)
                {
                    var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                    foreach (var method in methods)
                    {
                        var name = method.Name;

                        if (name == "Main" && name == "Start" && name == "Awake" && name == "Update")
                            continue;

                        var parameters = method.GetParameters();
                        if (parameters.Length > 1) continue; // methods with multiple parameters are not supported

                        // Parse parameter type and create fullname
                        string fullname = null;
                        ParameterType parameterType = ParameterType.None;
                        var paramType = parameters.Length == 1 ? parameters[0].ParameterType : null;
                        if (paramType == null) fullname = name + "()";
                        else if (paramType == typeof(string))
                            (parameterType, fullname) = (ParameterType.String, name + "(string)");
                        else if (paramType == typeof(float))
                            (parameterType, fullname) = (ParameterType.Float, name + "(float)");
                        else if (paramType == typeof(int))
                            (parameterType, fullname) = (ParameterType.Int, name + "(int)");
                        else if (paramType == typeof(object) || paramType.IsSubclassOf(typeof(Object)))
                            (parameterType, fullname) = (ParameterType.Object, name + "(Object)");
                        else continue;

                        // Create method description object
                        var supportedMethod = new MethodDesc { fullname = fullname, name = name, type = parameterType };

                        // Since AnimationEvents only stores method name, it can't handle functions with multiple overloads.
                        // Only retrieve first found function, but discard overloads.
                        var existingMethodIndex = supportedMethods.FindIndex(m => m.name == name);
                        if (existingMethodIndex != -1)
                        {
                            // The method is only ambiguous if it has a different signature to the one we saw before
                            var existingMethod = supportedMethods[existingMethodIndex];
                            existingMethod.isOverload = existingMethod.type != parameterType;
                        }
                        else
                            supportedMethods.Add(supportedMethod);
                    }
                    type = type.BaseType;
                }
            }

            return supportedMethods;
        }

        // Custom Inspector for creating EventMarkers
        [CustomEditor(typeof(EventMarker)), CanEditMultipleObjects]
        public class EventMarkerInspector : Editor
        {
            SerializedProperty m_Time;
            SerializedProperty m_Methods;
            SerializedProperty m_Retroactive;
            SerializedProperty m_EmitOnce;
            SerializedProperty m_EmitInEditor;

            public ReorderableList list;
            List<MethodDesc> supportedMethods;

            // Get serialized object properties (for UI)
            public void OnEnable()
            {
                m_Time = serializedObject.FindProperty("m_Time");
                m_Methods = serializedObject.FindProperty("methods");
                m_Retroactive = serializedObject.FindProperty("retroactive");
                m_EmitOnce = serializedObject.FindProperty("emitOnce");
                m_EmitInEditor = serializedObject.FindProperty("emitInEditor");
            }

            // Draw inspector GUI
            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                var marker = target as Marker;
                var parent = marker.parent;
                var boundObj = TimelineEditor.inspectedDirector.GetGenericBinding(parent);

                var changeScope = new EditorGUI.ChangeCheckScope();
                EditorGUILayout.PropertyField(m_Time);

                GameObject gameObject = null;
                if (boundObj as GameObject != null) gameObject = (GameObject)boundObj;
                else if (boundObj as Component != null) gameObject = ((Component)boundObj).gameObject;

                supportedMethods = CollectSupportedMethods(gameObject).ToList();

                list = new ReorderableList(serializedObject, m_Methods, true, true, true, true)
                {
                    drawElementCallback = DrawMethodAndArguments,
                    drawHeaderCallback = delegate (Rect rect) { EditorGUI.LabelField(rect, "Method"); }
                };
                list.DoLayoutList();

                EditorGUILayout.PropertyField(m_Retroactive);
                EditorGUILayout.PropertyField(m_EmitOnce);
                EditorGUILayout.PropertyField(m_EmitInEditor);

                if (changeScope.changed)
                    serializedObject.ApplyModifiedProperties();
            }

            // Draw drawer entry for given element
            void DrawMethodAndArguments(Rect rect, int index, bool isActive, bool isFocused)
            {
                // Retrieve element (elements are added when + is clicked in reorderable list UI)
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

                // Retrieve name for element
                SerializedProperty m_Method = element.FindPropertyRelative("name");

                // Create dropdown list
                var dropdown = supportedMethods.Select(i => i.fullname).ToList();
                dropdown.Add("No method");

                // Get current method ID based off of stored name (index really)
                var selectedMethodId = supportedMethods.FindIndex(i => i.name == m_Method.stringValue);
                if (selectedMethodId == -1)
                    selectedMethodId = supportedMethods.Count;

                // Draw popup (dropdown box)
                var previousMixedValue = EditorGUI.showMixedValue;
                {
                    if (m_Method.hasMultipleDifferentValues)
                        EditorGUI.showMixedValue = true;
                    selectedMethodId = EditorGUI.Popup(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
                                                       selectedMethodId, dropdown.ToArray());
                }

                rect = new Rect(rect.x + 220, rect.y, 200, EditorGUIUtility.singleLineHeight);

                EditorGUI.showMixedValue = previousMixedValue;

                // If perform checks and if method valid draw arguments
                if (selectedMethodId < supportedMethods.Count)
                {
                    var method = supportedMethods.ElementAt(selectedMethodId);
                    m_Method.stringValue = method.name;
                    DrawArguments(rect, element, method);
                    if (supportedMethods.Any(i => i.isOverload == true))
                        EditorGUI.HelpBox(rect, "Some functions were overloaded in MonoBehaviour components and may not work as intended if used with Animation Events!", MessageType.Warning);
                }
                else EditorGUI.HelpBox(rect, "Method is not valid", MessageType.Warning);
            }

            // Create UI elements for the given parameter type
            void DrawArguments(Rect rect, SerializedProperty element, MethodDesc method)
            {
                SerializedProperty m_ArgumentType = element.FindPropertyRelative("parameterType");
                m_ArgumentType.enumValueIndex = (int)method.type;

                // Supports int, float, Object, string, and none types. The Field style is determined by the serialized property type
                if (method.type == ParameterType.Int)
                {
                    SerializedProperty m_IntArg = element.FindPropertyRelative("Int");
                    EditorGUI.PropertyField(rect, m_IntArg, GUIContent.none);
                }
                else if (method.type == ParameterType.Float)
                {
                    SerializedProperty m_FloatArg = element.FindPropertyRelative("Float");
                    EditorGUI.PropertyField(rect, m_FloatArg, GUIContent.none);
                }
                else if (method.type == ParameterType.Object)
                {
                    SerializedProperty m_ObjectArg = element.FindPropertyRelative("Object");
                    EditorGUI.PropertyField(rect, m_ObjectArg, GUIContent.none);
                }
                else if (method.type == ParameterType.String)
                {
                    SerializedProperty m_StringArg = element.FindPropertyRelative("String");
                    EditorGUI.PropertyField(rect, m_StringArg, GUIContent.none);
                }
            }
        }
    }
}
#endif

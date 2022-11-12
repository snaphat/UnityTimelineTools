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
            var parametersArray = new object[] { visibleTimeRange.x, visibleTimeRange.y };
            SetShownHRangeInsideMargins.Invoke(m_TimeArea, parametersArray);

            // Force repaint
            animationWindow.Repaint();
        }
    }

    // Adds Editor support for Timeline Event Markers for calling GameObject methods
    namespace Events
    {
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
                    var notifications = track.GetMarkers().OfType<Marker>().OfType<EventMarkerNotification>();
                    foreach (var notification in notifications)
                    {
                        if (notification.emitInEditor)
                        {
                            // Push notification if time change in range
                            double time = notification.time;
                            bool fire = (time >= previousTime && time < director.time) || (time > director.time && time <= previousTime);
                            if (fire) output.PushNotification(playable, notification);
                        }
                    }
                }

                // Record current time
                previousTime = director.time;
            }
        }

        // For storing parsed method information in the editor
        public class MethodDescription
        {
            public string assemblyName; // Object type of the method
            public string name;         // the short name of the method
            public string fullName;     // the name of the method with parameters: e.g.: Foo(arg_type)
            public ParameterType type;  // None, int, float, string, Object
            public bool isOverload;     // Overloads not handable so need detection
        }

        // Custom Inspector for creating EventMarkers
        [CustomEditor(typeof(EventMarkerNotification)), CanEditMultipleObjects]
        public class EventMarkerInspector : Editor
        {
            SerializedProperty m_Time;
            SerializedProperty m_Callbacks;
            SerializedProperty m_Retroactive;
            SerializedProperty m_EmitOnce;
            SerializedProperty m_EmitInEditor;
            SerializedProperty m_Color;
            SerializedProperty m_ShowLineOverlay;

            GameObject storedGameObject;

            ReorderableList list;
            List<MethodDescription> supportedMethods;
            List<string> fullNames;
            float dropDownComputedSize;

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
                var parent = marker.parent;
                if (marker.parent == null) return; // In case of looking at timeline asset directly.

                var boundObj = TimelineEditor.inspectedDirector.GetGenericBinding(parent);

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

                    // Only rebuild list if something as changed (it isn't draggable otherwise)
                    if (list == null || storedGameObject != curGameObject)
                    {
                        storedGameObject = curGameObject;
                        supportedMethods = CollectSupportedMethods(storedGameObject).ToList();
                        fullNames = supportedMethods.Select(i => i.fullName).ToList();

                        list = new ReorderableList(serializedObject, m_Callbacks, true, true, true, true)
                        {
                            drawElementCallback = DrawMethodAndArguments,
                            drawHeaderCallback = delegate (Rect rect) { EditorGUI.LabelField(rect, "GameObject Methods"); }
                        };
                    }

                    // Find longest method name for computing space needed for reorderable list visual layout
                    var longestMethodName = "";
                    for (int i = 0; i < list.serializedProperty.arraySize; i++)
                    {
                        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(i);
                        SerializedProperty m_MethodName = element.FindPropertyRelative("methodName");
                        var index = supportedMethods.FindIndex(i => i.name == m_MethodName.stringValue);
                        var fullName = index < 0 ? "" : fullNames[index]; // no method is a default blank string
                        if (longestMethodName.Length < fullName.Length) longestMethodName = fullName;
                    }

                    // Compute how large the button needs to be.
                    GUIContent content = new(longestMethodName);
                    GUIStyle style = EditorStyles.popup;
                    style.richText = true;
                    Vector2 size = style.CalcSize(content);
                    dropDownComputedSize = size.x + 5;

                    // Layout reorderable list
                    list.DoLayoutList();

                    // apply changes
                    if (changeScope.changed) serializedObject.ApplyModifiedProperties();
                }
            }

            // Draw drawer entry for given element
            void DrawMethodAndArguments(Rect rect, int index, bool isActive, bool isFocused)
            {
                // Retrieve element (elements are added when + is clicked in reorderable list UI)
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

                // Retrieve name for element
                SerializedProperty m_AssemblyName = element.FindPropertyRelative("assemblyName");
                SerializedProperty m_MethodName = element.FindPropertyRelative("methodName");

                // Get current method ID based off of stored name (index really)
                var selectedMethodId = supportedMethods.FindIndex(i => i.name == m_MethodName.stringValue);
                if (selectedMethodId == -1) selectedMethodId = supportedMethods.Count;

                // Draw popup (dropdown box)
                var previousMixedValue = EditorGUI.showMixedValue;
                {
                    if (m_MethodName.hasMultipleDifferentValues) EditorGUI.showMixedValue = true;
                    GUIStyle style = EditorStyles.popup;
                    style.richText = true;
                    selectedMethodId = EditorGUI.Popup(new Rect(rect.x, rect.y, dropDownComputedSize, EditorGUIUtility.singleLineHeight),
                                                       selectedMethodId, fullNames.ToArray(), style);
                }

                rect = new Rect(rect.x + dropDownComputedSize + 5, rect.y, rect.width - dropDownComputedSize - 5, EditorGUIUtility.singleLineHeight);

                EditorGUI.showMixedValue = previousMixedValue;

                // If perform checks and if method valid draw arguments
                if (selectedMethodId < supportedMethods.Count)
                {
                    var method = supportedMethods.ElementAt(selectedMethodId);
                    m_AssemblyName.stringValue = method.assemblyName;
                    m_MethodName.stringValue = method.name;
                    DrawArguments(rect, element, method);
                    if (supportedMethods.Any(i => i.isOverload == true))
                        EditorGUI.HelpBox(rect, "Some functions were overloaded in MonoBehaviour components and may not work as intended if used with Animation Events!", MessageType.Warning);
                }
                else EditorGUI.HelpBox(rect, "Method is not valid", MessageType.Warning);
            }

            // Create UI elements for the given parameter type
            void DrawArguments(Rect rect, SerializedProperty element, MethodDescription method)
            {
                SerializedProperty m_ArgumentType = element.FindPropertyRelative("parameterType");
                m_ArgumentType.enumValueIndex = (int)method.type;

                // Supports int, float, Object, string, and none types. The Field style is determined by the serialized property typ
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
                else if (method.type == ParameterType.String)
                {
                    SerializedProperty m_StringArg = element.FindPropertyRelative("String");
                    EditorGUI.PropertyField(rect, m_StringArg, GUIContent.none);
                }
                else if (method.type == ParameterType.Object)
                {
                    SerializedProperty m_ObjectArg = element.FindPropertyRelative("Object");
                    EditorGUI.PropertyField(rect, m_ObjectArg, GUIContent.none);
                }
            }

            // Helper method for retrieving method signatures from a game object
            public static IEnumerable<MethodDescription> CollectSupportedMethods(GameObject gameObject)
            {
                if (gameObject == null)
                    return Enumerable.Empty<MethodDescription>();

                List<MethodDescription> supportedMethods = new();
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
                            string fullName = null;
                            ParameterType parameterType = ParameterType.None;
                            var paramType = parameters.Length == 1 ? parameters[0].ParameterType : null;
                            if (paramType == null)
                                fullName = name + "()";
                            else if (paramType == typeof(int))
                                (parameterType, fullName) = (ParameterType.Int, name + "(int)");
                            else if (paramType == typeof(float))
                                (parameterType, fullName) = (ParameterType.Float, name + "(float)");
                            else if (paramType == typeof(string))
                                (parameterType, fullName) = (ParameterType.String, name + "(string)");
                            else if (paramType == typeof(object) || paramType.IsSubclassOf(typeof(Object)))
                                (parameterType, fullName) = (ParameterType.Object, name + "(Object)");
                            else continue;

                            // Create method description object
                            var supportedMethod = new MethodDescription { name = name, fullName = fullName, type = parameterType, assemblyName = type.AssemblyQualifiedName };

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
        }

        // Editor used by the Timeline window to customize the appearance of an TestMarker
        [CustomTimelineEditor(typeof(EventMarkerNotification))]
        public class EventMarkerOverlay : MarkerEditor
        {
            const float k_LineOverlayWidth = 6.0f;

            const string k_OverlayPath = "EventMarker";
            const string k_OverlayCollapsedPath = "EventMarker_Collapsed";

            static readonly Texture2D s_OverlayTexture;
            static readonly Texture2D s_OverlayCollapsedTexture;

            static EventMarkerOverlay()
            {
                s_OverlayTexture = Resources.Load<Texture2D>(k_OverlayPath);
                s_OverlayCollapsedTexture = Resources.Load<Texture2D>(k_OverlayCollapsedPath);
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

                string richFormat = EditorGUIUtility.isProSkin ?
                "<b><color=cyan>{0}</color><color=yellow>(</color><color=magenta>{1}</color><color=yellow>)</color></b>\n" :
                "<b><color=blue>{0}</color><color=green>(</color><color=red>{1}</color><color=green>)</color></b>\n";

                string tooltip = "";
                foreach (var callback in eventMarker.callbacks)
                {
                    if (callback.methodName.Length == 0) continue;
                    // Supports int, float, Object, string, and none types. The Field style is determined by the serialized property type
                    if (callback.parameterType == ParameterType.None)
                        tooltip += string.Format(richFormat, callback.methodName, "");
                    else if (callback.parameterType == ParameterType.Int)
                        tooltip += string.Format(richFormat, callback.methodName, callback.Int + " (int)");
                    else if (callback.parameterType == ParameterType.Float)
                        tooltip += string.Format(richFormat, callback.methodName, callback.Float + " (float)");
                    else if (callback.parameterType == ParameterType.String)
                        tooltip += string.Format(richFormat, callback.methodName, "\"" + callback.String + "\" (string)");
                    else if (callback.parameterType == ParameterType.Object)
                        tooltip += string.Format(richFormat, callback.methodName, callback.Object.ToString().Length > 48 ? "(Object)" : callback.Object);
                    else
                        tooltip += "Error: Unsupported Method";
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
                    GUI.color = new(1.0f - color.r, 1.0f - color.g, 1.0f - color.b, color.a * 1.1f);
                    GUI.DrawTexture(region.markerRegion, s_OverlayTexture);
                }
                else if (state.HasFlag(MarkerUIStates.Collapsed))
                {
                    GUI.color = color;
                    GUI.DrawTexture(region.markerRegion, s_OverlayCollapsedTexture);
                }
                else if (state.HasFlag(MarkerUIStates.None))
                {
                    GUI.color = color;
                    GUI.DrawTexture(region.markerRegion, s_OverlayTexture);
                }

                // Restore the previous Editor's overlay color
                GUI.color = oldColor;
            }
        }
    }
}
#endif

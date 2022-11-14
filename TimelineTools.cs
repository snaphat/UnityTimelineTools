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
        public class CallbackDescription
        {
            public string assemblyName;        // Object type of the method
            public string methodName;          // The short name of the method
            public string fullMethodName;      // The name of the method with parameters: e.g.: Foo(arg_type)
            public string qualifiedMethodName; // The name of the class + method + parameters: e.g. Bar.Foo(arg_type)
            public List<Type> parameterTypes;  // none, bool, int, float, string, Object, Enum
            public bool isOverload;            // Overloads not handable so need detection
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
            List<CallbackDescription> supportedMethods;
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

                    // Only rebuild list if something as changed (it isn't draggable otherwise)
                    if (list == null || storedGameObject != curGameObject)
                    {
                        storedGameObject = curGameObject;
                        supportedMethods = CollectSupportedMethods(storedGameObject).ToList();

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
                        // Retrieve element (elements are added when + is clicked in reorderable list UI)
                        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(i);

                        // Retrieve name for element
                        SerializedProperty m_AssemblyName = element.FindPropertyRelative("assemblyName");
                        SerializedProperty m_FullMethodName = element.FindPropertyRelative("fullMethodName");

                        // Get current method ID based off of stored name (index really)
                        var selectedMethodId = supportedMethods.FindIndex(i => i.assemblyName == m_AssemblyName.stringValue && i.fullMethodName == m_FullMethodName.stringValue);

                        // If no method use default otherwise use method name + parameters
                        var fullName = selectedMethodId < 0 ? "No method" : supportedMethods[selectedMethodId].fullMethodName;

                        // Update longest match
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
                SerializedProperty m_FullMethodName = element.FindPropertyRelative("fullMethodName");

                // Get current method ID based off of stored name (index really)
                var selectedMethodId = supportedMethods.FindIndex(i => i.assemblyName == m_AssemblyName.stringValue && i.fullMethodName == m_FullMethodName.stringValue);

                // Draw popup (dropdown box)
                var previousMixedValue = EditorGUI.showMixedValue;
                {
                    if (m_FullMethodName.hasMultipleDifferentValues) EditorGUI.showMixedValue = true;
                    GUIStyle style = EditorStyles.popup;
                    style.richText = true;

                    // Create dropdownlist with 'pseudo entries' for the currently selected method at the top of the list followed by a blank line
                    var dropdownList = supportedMethods.Select(i => i.qualifiedMethodName).ToList();
                    dropdownList.Insert(0, ""); // insert line
                    dropdownList.Insert(0, selectedMethodId > -1 ? supportedMethods[selectedMethodId].fullMethodName : "No method");

                    // Store old selected method id case it isn't changed
                    var oldSelectedMethodId = selectedMethodId;
                    selectedMethodId = EditorGUI.Popup(new Rect(rect.x, rect.y, dropDownComputedSize, EditorGUIUtility.singleLineHeight), 0, dropdownList.ToArray(), style);

                    // Normalize selection
                    if (selectedMethodId == 0)
                        selectedMethodId = oldSelectedMethodId; // No selection so restore actual method id
                    else
                        selectedMethodId -= 2; // normalize to get actual Id (-2 for the two 'pseudo entries'
                }

                EditorGUI.showMixedValue = previousMixedValue;

                // If selected method is valid then try to draw parameters
                if (selectedMethodId > -1 && selectedMethodId < supportedMethods.Count)
                {
                    var callbackDescription = supportedMethods.ElementAt(selectedMethodId);
                    SerializedProperty m_methodName = element.FindPropertyRelative("methodName");

                    // Fillout assembly and method name propetries using the selected id
                    m_AssemblyName.stringValue = callbackDescription.assemblyName;     // used for unique iding
                    m_FullMethodName.stringValue = callbackDescription.fullMethodName; // used for unique iding
                    m_methodName.stringValue = callbackDescription.methodName;         // used for name lookup in receiver

                    // Draw each argument
                    DrawArguments(rect, element, callbackDescription);
                }
            }

            // Create UI elements for the given parameter types of a methods arguments
            void DrawArguments(Rect rect, SerializedProperty element, CallbackDescription callbackDescription)
            {
                // Compute the rect for the method parameters based off of the count
                var paramWidth = (rect.width - dropDownComputedSize - 10) / callbackDescription.parameterTypes.Count;
                rect = new Rect(rect.x + dropDownComputedSize + 5, rect.y, paramWidth, EditorGUIUtility.singleLineHeight);

                // Grab the arguments property
                SerializedProperty m_Arguments = element.FindPropertyRelative("arguments");

                // Resize the arguments array
                m_Arguments.arraySize = callbackDescription.parameterTypes.Count;

                // Iterate and display each argument by type
                for (var i = 0; i < m_Arguments.arraySize; i++)
                {
                    var type = callbackDescription.parameterTypes[i];
                    var argumentProperty = m_Arguments.GetArrayElementAtIndex(i);
                    SerializedProperty m_ParameterType = argumentProperty.FindPropertyRelative("parameterType");

                    // Assign Param type and generate field. The Field style is determined by the serialized property type
                    if (type == typeof(bool))
                    {
                        m_ParameterType.enumValueIndex = (int)ParameterType.Bool;
                        EditorGUI.PropertyField(rect, argumentProperty.FindPropertyRelative("Bool"), GUIContent.none);
                    }
                    else if (type == typeof(int))
                    {
                        m_ParameterType.enumValueIndex = (int)ParameterType.Int;
                        EditorGUI.PropertyField(rect, argumentProperty.FindPropertyRelative("Int"), GUIContent.none);
                    }
                    else if (type == typeof(float))
                    {
                        m_ParameterType.enumValueIndex = (int)ParameterType.Float;
                        EditorGUI.PropertyField(rect, argumentProperty.FindPropertyRelative("Float"), GUIContent.none);
                    }
                    else if (type == typeof(string))
                    {
                        m_ParameterType.enumValueIndex = (int)ParameterType.String;
                        EditorGUI.PropertyField(rect, argumentProperty.FindPropertyRelative("String"), GUIContent.none);
                    }
                    else if (type == typeof(object) || type.IsSubclassOf(typeof(Object)))
                    {
                        m_ParameterType.enumValueIndex = (int)ParameterType.Object;
                        EditorGUI.PropertyField(rect, argumentProperty.FindPropertyRelative("Object"), GUIContent.none);
                    }
                    else if (type.IsEnum)
                    {
                        m_ParameterType.enumValueIndex = (int)ParameterType.Enum;
                        var intProperty = argumentProperty.FindPropertyRelative("Int");
                        var stringProperty = argumentProperty.FindPropertyRelative("String");
                        intProperty.intValue = (int)(object)EditorGUI.EnumPopup(rect, (Enum)Enum.ToObject(type, intProperty.intValue)); // Parse as enum type
                        stringProperty.stringValue = type.AssemblyQualifiedName; // store full type name
                    }

                    // Update field position
                    rect = new Rect(rect.x + paramWidth + 5, rect.y, paramWidth, EditorGUIUtility.singleLineHeight);
                }
            }

            // Helper method for retrieving method signatures from a game object
            public static IEnumerable<CallbackDescription> CollectSupportedMethods(GameObject gameObject)
            {
                if (gameObject == null)
                    return Enumerable.Empty<CallbackDescription>();

                List<CallbackDescription> supportedMethods = new();
                var behaviours = gameObject.GetComponents<MonoBehaviour>();

                foreach (var behaviour in behaviours)
                {
                    if (behaviour == null)
                        continue;

                    var methodType = behaviour.GetType();
                    while (methodType != typeof(MonoBehaviour) && methodType != null)
                    {
                        var methods = methodType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                        foreach (var method in methods)
                        {
                            // don't support adding built in method names
                            if (method.Name == "Main" && method.Name == "Start" && method.Name == "Awake" && method.Name == "Update") continue;

                            var parameters = method.GetParameters();    // get parameters
                            List<Type> parameterTypes = new(); // create empty parameter list
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
                                else if (parameter.ParameterType == typeof(object) || parameter.ParameterType.IsSubclassOf(typeof(Object)))
                                    strType = "Object";
                                else if (parameter.ParameterType.IsEnum && Enum.GetUnderlyingType(parameter.ParameterType) == typeof(int))
                                    strType = parameter.ParameterType.Name; // use underlying typename for fullanme string
                                else
                                    validMethod = false;

                                // Add parameter and update full method name with parameter type and name
                                parameterTypes.Add(parameter.ParameterType);
                                fullMethodName += strType + " " + parameter.Name;
                            }

                            // one or more argument types was not supported so don't add method.
                            if (validMethod == false) continue;

                            // Finish the full name signature
                            fullMethodName += ")";

                            // Create method description object
                            var supportedMethod = new CallbackDescription { methodName = method.Name, fullMethodName = fullMethodName, qualifiedMethodName = methodType + "/" + fullMethodName, parameterTypes = parameterTypes, assemblyName = methodType.AssemblyQualifiedName };
                            supportedMethods.Add(supportedMethod);
                        }
                        methodType = methodType.BaseType;
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
            const string k_OverlaySelectedPath = "EventMarker_Selected";
            const string k_OverlayCollapsedPath = "EventMarker_Collapsed";

            static readonly Texture2D s_OverlayTexture;
            static readonly Texture2D s_OverlaySelectedTexture;
            static readonly Texture2D s_OverlayCollapsedTexture;

            static EventMarkerOverlay()
            {
                s_OverlayTexture = Resources.Load<Texture2D>(k_OverlayPath);
                s_OverlaySelectedTexture = Resources.Load<Texture2D>(k_OverlaySelectedPath);
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

                // Tooltip format
                string richMethodFormat = EditorGUIUtility.isProSkin ?
                "<b><color=#dcdcaa>{0}</color><color=#ffd700>(</color>{1}<color=#ffd700>)</color></b>\n" : "<b><color=#795e26>{0}</color><color=#0431fa>(</color>{1}<color=#0431fa>)</color></b>\n";
                string richArgumentFormat = EditorGUIUtility.isProSkin ?
                "<color={2}>{0}</color> <color=#c586c0>(</color><color=#569cd6>{1}</color><color=#c586c0>)</color>" : "<color={2}>{0}</color> <color=#319331>(</color><color=#0000ff>{1}</color><color=#319331>)</color>";

                // Create tooltip
                string tooltip = "";

                if (eventMarker.callbacks != null)
                {
                    foreach (var callback in eventMarker.callbacks)
                    {
                        string arg = "", type = "", color = "#000000";
                        if (callback.fullMethodName.Length == 0) continue;

                        string argumentText = "";
                        for (int i = 0; i < callback.arguments.Length; i++)
                        {
                            if (i > 0) argumentText += ", ";

                            // Supports int, float, Object, string, and none types. The Field style is determined by the serialized property type
                            var argument = callback.arguments[i];
                            if (argument.parameterType == ParameterType.Bool)
                                (arg, type, color) = (argument.Bool.ToString(), "bool", argument.Bool ? "#009900" : "#ff2222");
                            else if (argument.parameterType == ParameterType.Int)
                                (arg, type, color) = (argument.Int.ToString(), "int", EditorGUIUtility.isProSkin ? "#b5cea8" : "#098658");
                            else if (argument.parameterType == ParameterType.Float)
                                (arg, type, color) = (argument.Float.ToString(), "float", EditorGUIUtility.isProSkin ? "#b5cea8" : "#098658");
                            else if (argument.parameterType == ParameterType.String)
                                (arg, type, color) = ("\"" + argument.String + "\"", "string", EditorGUIUtility.isProSkin ? "#ce9178" : "#a31515");
                            else if (argument.parameterType == ParameterType.Object)
                            {
                                // Retrieve the exposed reference
                                if (TimelineEditor.inspectedDirector.playableGraph.IsValid())
                                {
                                    var temp = argument.Object.Resolve(TimelineEditor.inspectedDirector.playableGraph.GetResolver()).ToString().Split('(', ')');
                                    (arg, type, color) = temp[0] == "null" ? ("None", "Object", "#ff0000") :
                                        temp[0].Length > 48 ? ("...", "Object", EditorGUIUtility.isProSkin ? "#4ec9b0" : "#267f99") :
                                        (temp[0], temp[1], EditorGUIUtility.isProSkin ? "#4ec9b0" : "#267f99");
                                }
                            }
                            else if (argument.parameterType == ParameterType.Enum)
                                (arg, type, color) = (Enum.ToObject(Type.GetType(argument.String), argument.Int).ToString(), "Enum", EditorGUIUtility.isProSkin ? "#4ec9b0" : "#267f99");
                            argumentText += string.Format(richArgumentFormat, arg, type, color);
                        }

                        // Format and trim string if no args
                        tooltip += string.Format(richMethodFormat, callback.methodName, argumentText);
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
                    GUI.DrawTexture(region.markerRegion, s_OverlayTexture);
                    GUI.color = new(1.0f, 1.0f, 1.0f, 1.0f);
                    GUI.DrawTexture(region.markerRegion, s_OverlaySelectedTexture);
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

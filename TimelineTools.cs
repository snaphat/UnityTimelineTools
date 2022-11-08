using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEditor.Timeline.Actions;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Timeline;

#if UNITY_EDITOR
// Adds support for inserting and cutting frames in all tracks and infinite clips within the timeline.
public class TimelineTools
{
    class TimelineInsertCutExtensions
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
            public static void Insert() { TimelineInsertCutExtensions.Insert(((Frames)Attribute.GetCustomAttribute(typeof(T), typeof(Frames))).frames); }
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

            // Handle infinite animation clips (really tracks)
            // filter infinite animation tracks
            var infiniteTracks = unlockedTracks.Where(e => e is AnimationTrack a && !a.inClipMode).Select(e => (AnimationTrack)e);
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
                        if (keys[i].time > currentTime) keys[i].time += amount;
                    curve.keys = keys;
                    AnimationUtility.SetEditorCurve(clip, bind, curve);
                }

                // update the PPtr curves: insert/cut time by amount.
                var objectBindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
                foreach (var bind in objectBindings)
                {
                    var curve = AnimationUtility.GetObjectReferenceCurve(clip, bind);
                    for (var i = 0; i < curve.Length; i++)
                        if (curve[i].time > currentTime) curve[i].time += amount;
                    AnimationUtility.SetObjectReferenceCurve(clip, bind, curve);
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

                // Compute tolerance for determining whether to shift a track or not
                double kTimeEpsilon = 1e-14; // from com.unity.timeline/Runtime/Utilities/TimeUtility.cs 
                var tolerance = Math.Max(Math.Abs(currentTime), 1) * timelineAsset.editorSettings.frameRate * kTimeEpsilon;

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

    public class TimelineAnimationViewSynchronizer
    {
        private static bool enabled = false;

        private const string menuPath = "Tools/Timeline/Sync Timeline && Animation Views";

        [MenuItem(menuPath, priority = 0)]
        public static void Sync()
        {
            enabled = !enabled;
            Menu.SetChecked(menuPath, enabled);

            if (enabled)
                EditorApplication.update += OnUpdate;
            else
                EditorApplication.update -= OnUpdate;
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
}
#endif

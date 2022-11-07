using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Timeline;
using System.Reflection;

#if UNITY_EDITOR
public class TimelineAnimationViewSynchronizer
{
    private static bool enabled = false;

    private const string menuPath = "Tools/Sync Timeline && Animation Views";

    [MenuItem(menuPath)]
    private static void Sync(MenuCommand menuCommand)
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

        object[] parametersArray = new object[] { visibleTimeRange.x, visibleTimeRange.y };
        SetShownHRangeInsideMargins.Invoke(m_TimeArea, parametersArray);
        animationWindow.Repaint();
    }
}
#endif

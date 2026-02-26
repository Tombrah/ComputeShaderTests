using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScreenshotTool))]
public class ScreenshotToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        if (GUILayout.Button("Take Screenshot"))
        {
            ScreenshotTool capture = (ScreenshotTool)target;
            capture.SaveScreenshot();
        }
    }
}

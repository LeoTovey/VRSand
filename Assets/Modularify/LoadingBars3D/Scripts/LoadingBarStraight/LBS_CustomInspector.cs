#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Modularify.LoadingBars3D
{
    /// <summary>
    /// Custom inspector for the LoadingBarStraight Gameobject
    /// Adds a button to the script inspector in order to call the Initialize() method of the script
    /// </summary>
    [CustomEditor(typeof(LoadingBarStraight))]
    public class LBS_CustomInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LoadingBarStraight lbs = (LoadingBarStraight)target;
            if (GUILayout.Button("Refresh Values"))
            {
                lbs.Initialize();
            }
        }
    }
}
#endif

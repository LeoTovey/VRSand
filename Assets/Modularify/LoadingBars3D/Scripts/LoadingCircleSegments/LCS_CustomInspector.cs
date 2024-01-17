#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
namespace Modularify.LoadingBars3D
{
    /// <summary>
    /// Custom inspector for the LoadingBarSegments Gameobject
    /// Adds a button to the script inspector in order to call the Initialize() method of the script
    /// </summary>
    [CustomEditor(typeof(LoadingBarSegments))]
    public class LCS_CustomInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LoadingBarSegments lbs = (LoadingBarSegments)target;
            if (!Application.isPlaying || Application.isPlaying)
            {
                if (GUILayout.Button("Refresh Values"))
                {
                    lbs.Initialize();
                }
            }
        }
    }
}

#endif
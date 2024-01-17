#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Modularify.LoadingBars3D
{
    /// <summary>
    /// Custom inspector for the LoadingBarParts Gameobject
    /// Adds a button to the script inspector in order to call the Initialize() method of the script
    /// </summary>
    [CustomEditor(typeof(LoadingBarParts))]
    public class LBP_CustomInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LoadingBarParts lbp = (LoadingBarParts)target;
            if (GUILayout.Button("Refresh Values"))
            {
                lbp.Initialize();
            }
        }
    }
}
#endif


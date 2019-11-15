using UnityEditor;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Editor for the grid generator
    /// Author : Mike Bédard
    /// </summary>
    [CustomEditor(typeof(GridGenerator))]
    public class GridGeneratorEditor : Editor
    {
        public const string GENERATE_GRID_BUTTON_LABEL = "Generate Grid";
        public const string CLEAR_GRID_BUTTON_LABEL = "Clear Grid";
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            GridGenerator gridGenerator = (GridGenerator)target;
            
            if(GUILayout.Button(GENERATE_GRID_BUTTON_LABEL))
            {
                gridGenerator.GenerateGrid();
            }
            
            if(GUILayout.Button(CLEAR_GRID_BUTTON_LABEL))
            {
                gridGenerator.ClearGrid();
            }
        }
    }
}
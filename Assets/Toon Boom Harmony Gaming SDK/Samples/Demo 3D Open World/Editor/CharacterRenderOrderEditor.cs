#if ENABLE_UNITY_2D_ANIMATION && ENABLE_UNITY_COLLECTIONS

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TBG3DOpenWorldExample
{
    [CustomEditor(typeof(CharacterRenderOrder))]
    public class CharacterRenderOrderEditor : Editor
    {
        private SerializedProperty sortingLayerProperty;

        private void OnEnable()
        {
            sortingLayerProperty = serializedObject.FindProperty("sortingLayerName");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Display other properties as needed.
            DrawDefaultInspector();

            // Display the Sorting Layer dropdown.
            EditorGUI.BeginChangeCheck();

            // Collect all the sorting layer names and store them in a string array, then get the index of the currently selected sorting layer, this is used to select the correct item in the popup.
            {
                var sortingLayerNames = SortingLayer.layers;
                var sortingLayerName = sortingLayerProperty.stringValue;
                var sortingLayerIndex = -1;
                for (int i = 0; i < sortingLayerNames.Length; i++)
                {
                    if (sortingLayerNames[i].name == sortingLayerName)
                    {
                        sortingLayerIndex = i;
                        break;
                    }
                }
                sortingLayerIndex = EditorGUILayout.Popup("Sorting Layer", sortingLayerIndex, sortingLayerNames.Select(layer => layer.name).ToArray());
                if (sortingLayerIndex >= 0 && sortingLayerIndex < sortingLayerNames.Length)
                {
                    sortingLayerProperty.stringValue = sortingLayerNames[sortingLayerIndex].name;
                }
                else {
                    sortingLayerProperty.stringValue = "Default";
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

#endif

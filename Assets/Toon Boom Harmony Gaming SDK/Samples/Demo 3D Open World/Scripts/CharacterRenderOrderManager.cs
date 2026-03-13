#if ENABLE_UNITY_2D_ANIMATION && ENABLE_UNITY_COLLECTIONS

using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using ToonBoom.TBGRenderer;
using UnityEngine;

namespace TBG3DOpenWorldExample
{
    [ExecuteAlways]
    [DefaultExecutionOrder(-1000)] // Needs to start before anything registers to it.
    public class CharacterRenderOrderManager : MonoBehaviour
    {
        private List<CharacterRenderOrder> characters = new List<CharacterRenderOrder>();

        public static CharacterRenderOrderManager Instance { get; private set; }

        public void OnEnable()
        {
            if (Instance != null)
            {
                Debug.LogError("There should only be one CharacterRenderOrderManager in the scene.");
                DestroyImmediate(this);
                return;
            }
            Instance = this;
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        public void RegisterCharacterRenderer(CharacterRenderOrder renderer)
        {
            if (!characters.Contains(renderer))
            {
                characters.Add(renderer);
            }
        }

        public void UnregisterCharacterRenderer(CharacterRenderOrder renderer)
        {
            characters.Remove(renderer);
        }

        struct SortableCharacter
        {
            public CharacterRenderOrder character;
            public float zDepth;
        }
        private List<SortableCharacter> sortableCharacters = new List<SortableCharacter>();

        private void LateUpdate()
        {
            var cameraMatrix = Camera.main.transform.worldToLocalMatrix;
            
            // Fill sortable characters with the characters and their z-depth relative to camera
            sortableCharacters.Clear();
            for (int i = 0; i < characters.Count; i++)
            {
                var character = characters[i];
                var cameraSpace = cameraMatrix * character.transform.localToWorldMatrix;
                sortableCharacters.Add(new SortableCharacter()
                {
                    character = character,
                    zDepth = cameraSpace.MultiplyPoint(Vector3.zero).z,
                });
            }

            // Sort the characters based on camera z-depth.
            sortableCharacters.Sort((a, b) =>
            {
                return b.zDepth.CompareTo(a.zDepth);
            });

            // Call each character to sort, which has the opportunity to increment the total order.
            var totalOrder = 0;
            for (int i = 0; i < sortableCharacters.Count; i++)
            {
                var character = sortableCharacters[i].character;
                character.Sort(ref totalOrder);
            }
        }
    }
}

#endif

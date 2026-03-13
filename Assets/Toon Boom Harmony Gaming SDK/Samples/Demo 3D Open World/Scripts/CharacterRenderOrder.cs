#if ENABLE_UNITY_2D_ANIMATION && ENABLE_UNITY_COLLECTIONS

using System;
using System.Collections.Generic;
using System.Linq;
using ToonBoom.TBGRenderer;
using UnityEngine;
using UnityEngine.Rendering;

namespace TBG3DOpenWorldExample
{
    [ExecuteAlways]
    public class CharacterRenderOrder : MonoBehaviour
    {
        [HideInInspector]
        public string sortingLayerName = "Default";
        private TBGRenderer tbgRenderer;
        private SpriteRenderer[] otherRenderers;

        private void OnEnable()
        {
            if (CharacterRenderOrderManager.Instance == null)
            {
                Debug.LogError("There should be a CharacterRenderOrderManager in the scene.");
            }

            // If we don't have a TBGRenderer, we can still sort the SpriteRenderers on this object.
            if (!TryGetComponent<TBGRenderer>(out tbgRenderer))
            {
                otherRenderers = GetComponentsInChildren<SpriteRenderer>();
            }

            if (CharacterRenderOrderManager.Instance != null)
            {
                CharacterRenderOrderManager.Instance.RegisterCharacterRenderer(this);
            }
        }

        private void OnDisable()
        {
            if (CharacterRenderOrderManager.Instance != null)
            {
                CharacterRenderOrderManager.Instance.UnregisterCharacterRenderer(this);
            }
        }

        struct SortableRenderer
        {
            public SpriteRenderer[] spriteRenderers;
            public float zOffset;
        }

        private static int zPositionScale = 1000;

        public void Sort(ref int totalOrder)
        {
            if (tbgRenderer == null)
            {
                // Simple path for non-TBG characters.
                for (int i = 0; i < otherRenderers.Length; i++)
                {
                    var spriteRenderer = otherRenderers[i];
                    if (spriteRenderer == null)
                    {
                        continue;
                    }
                    spriteRenderer.sortingLayerName = sortingLayerName;
                    spriteRenderer.sortingOrder = totalOrder; // For multiple SpriteRenderers, this is left to how Unity determines render order.
                }
                totalOrder++;
            }
            else
            {
                // For TBG animations, we can assume that sprite renderers are already in a definitive order and we can just assign the sorting order as a scale of the localPosition.z.
                var maxTotalOrder = totalOrder;
                var maxLocalOrder = tbgRenderer.ReadToSpriteRenderers.Length;
                for (int i = 0; i < maxLocalOrder; i++)
                {
                    var entry = tbgRenderer.ReadToSpriteRenderers[i];
                    for (int j = 0; j < entry.spriteRenderers.Length; j++)
                    {
                        var spriteRenderer = entry.spriteRenderers[j];
                        if (spriteRenderer == null)
                        {
                            continue;
                        }
                        var localOrder = Mathf.RoundToInt(spriteRenderer.transform.localPosition.z * zPositionScale);
                        var order = totalOrder + (maxLocalOrder - localOrder);
                        maxTotalOrder = Math.Max(maxTotalOrder, order);
                        spriteRenderer.sortingLayerName = sortingLayerName;
                        spriteRenderer.sortingOrder = order;
                    }
                }
                totalOrder = maxTotalOrder;
            }
        }
    }
}

#endif

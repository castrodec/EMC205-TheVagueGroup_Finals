#if ENABLE_UNITY_2D_ANIMATION && ENABLE_UNITY_COLLECTIONS

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace ToonBoom.TBGRenderer
{
    [ExecuteAlways]
    [DefaultExecutionOrder(-2)]
    public class TBGRenderer : MonoBehaviour
    {
        [Serializable]
        public class SpriteRenderersEntry
        {
            public SpriteRenderer[] spriteRenderers;
        }

        [Serializable]
        public struct Skin
        {
            public SpriteRenderer[] SpriteRenderers;
        }

        [Serializable]
        public struct SkinGroup
        {
            public Skin[] Skins;
        }
        private TBGStore _LastStore;
        public TBGStore Store;
        public TBGStore ReferenceStore;
        public ushort PaletteID;
        public ushort ResolutionID;
        public ushort[] GroupToSkinID;
        public SpriteRenderersEntry[] ReadToSpriteRenderers;
        public SkinGroup[] SkinGroups;
        public Transform[] CutterToTransform;
        public CutterManager CutterManager;

        private ushort[] _LastGroupToSkinID;
        private ushort _LastResolutionID;
        private ushort _LastPaletteID;
        private Dictionary<SpriteRenderer, int> SpriteRendererToReadIndex;

        public void SetSkinForGroup(int groupID, ushort skinID)
        {
            GroupToSkinID[groupID] = skinID;
        }

        public void HideGroup(string groupName)
        {
            var groupID = GroupNameToID(groupName);
            HideGroup(groupID);
        }

        public void HideGroup(int groupID)
        {
            SetSkinForGroup(groupID, 0);
        }

        public void SetSkinForGroup(string groupName, string skinName)
        {
            var groupID = GroupNameToID(groupName);
            var skinID = SkinNameToID(groupID, skinName);
            SetSkinForGroup(groupID, skinID);
        }

        public void SetMaterial(Material material)
        {
            for (int readIndex = 0; readIndex < ReadToSpriteRenderers.Length; readIndex++)
            {
                var spriteRenderers = ReadToSpriteRenderers[readIndex].spriteRenderers;
                for (int rendererIndex = 0; rendererIndex < spriteRenderers.Length; rendererIndex++)
                {
                    spriteRenderers[rendererIndex].sharedMaterial = material;
                }
            }
        }

        public void SetColor(Color color)
        {
            for (int readIndex = 0; readIndex < ReadToSpriteRenderers.Length; readIndex++)
            {
                var spriteRenderers = ReadToSpriteRenderers[readIndex].spriteRenderers;
                for (int rendererIndex = 0; rendererIndex < spriteRenderers.Length; rendererIndex++)
                {
                    spriteRenderers[rendererIndex].color = color;
                }
            }
        }

        public int GroupNameToID(string groupName)
        {
            var groupID = 0;
            for (; groupID < Store.SkinGroups.Length; groupID++)
            {
                if (Store.SkinGroups[groupID].GroupName == groupName)
                    break;
            }
            if (groupID >= Store.SkinGroups.Length)
                throw new ArgumentException($"groupName of {groupName} not found");
            return groupID;
        }

        public ushort SkinNameToID(int groupID, string skinName)
        {
            ushort skinID = 0;
            var skinNames = Store.SkinGroups[groupID].SkinNames;
            for (; skinID < skinNames.Length; skinID++)
            {
                if (skinNames[skinID] == skinName)
                    break;
            }
            if (skinID >= skinNames.Length)
                throw new ArgumentException($"skinName of {skinName} not found for groupID of {groupID}");
            return skinID;
        }

        public void ResolutionPaletteToSpriteSheetID(string resolutionName, string paletteName, out ushort resolutionID, out ushort paletteID)
        {
            for (ushort rID = 0; rID < Store.Resolutions.Length; rID++)
            {
                if (Store.Resolutions[rID].ResolutionName != resolutionName)
                    continue;
                for (ushort pID = 0; pID < Store.Resolutions[rID].Palettes.Length; pID++)
                    if (Store.Resolutions[rID].Palettes[pID].PaletteName == paletteName)
                    {
                        resolutionID = rID;
                        paletteID = pID;
                        return;
                    }
            }
            throw new ArgumentException($"resolutionName of {resolutionName} and paletteName of {paletteName} was not found");
        }

        public ushort ResolutionNameToID(string resolutionName)
        {
            for (ushort resolutionID = 0; resolutionID < Store.Resolutions.Length; resolutionID++)
            {
                if (Store.Resolutions[resolutionID].ResolutionName == resolutionName)
                    return resolutionID;
            }
            throw new ArgumentException($"resolutionName of {resolutionName} was not found");
        }

        public ushort PaletteNameToID(string paletteName)
        {
            var palettes = Store.Resolutions[ResolutionID].Palettes;
            for (ushort paletteID = 0; paletteID < palettes.Length; paletteID++)
            {
                if (palettes[paletteID].PaletteName == paletteName)
                    return paletteID;
            }
            throw new ArgumentException($"paletteName of {paletteName} was not found");
        }

        public void SetResolutionPalette(string resolutionName, string paletteName)
        {
            ResolutionPaletteToSpriteSheetID(resolutionName, paletteName, out ResolutionID, out PaletteID);
        }

        public void SetPalette(string paletteName)
        {
            PaletteID = PaletteNameToID(paletteName);
        }

        public void SetResolution(string resolutionName)
        {
            ResolutionID = ResolutionNameToID(resolutionName);
        }

        private bool _Started = false;
        void Start()
        {
            _Started = true;
            OnEnable();
        }

        void OnEnable()
        {
            // Don't run if we haven't started yet (in import process especially)
            if (!_Started)
                return;

            SpriteRendererToReadIndex = new Dictionary<SpriteRenderer, int>();
            for (var readIndex = 0; readIndex < ReadToSpriteRenderers.Length; readIndex++)
            {
                var entry = ReadToSpriteRenderers[readIndex];
                for (int i = entry.spriteRenderers.Length - 1; i >= 0; i--)
                {
                    SpriteRendererToReadIndex.Add(entry.spriteRenderers[i], readIndex);
                    RegisterOnSpriteChanged(entry.spriteRenderers[i]);
                }
            }

            CutterManager.Init(Store, this);

            _LastStore = Store;
            _LastGroupToSkinID = new ushort[GroupToSkinID.Length];
            for (int i = 0; i < _LastGroupToSkinID.Length; i++)
                _LastGroupToSkinID[i] = ushort.MaxValue;
            for (int groupIndex = 0; groupIndex < SkinGroups.Length; groupIndex++)
            {
                var skinGroup = SkinGroups[groupIndex];
                for (int skinIndex = 0; skinIndex < skinGroup.Skins.Length; skinIndex++)
                {
                    var skin = skinGroup.Skins[skinIndex];
                    for (int rendererIndex = 0; rendererIndex < skin.SpriteRenderers.Length; rendererIndex++)
                    {
                        skin.SpriteRenderers[rendererIndex].enabled = false;
                    }
                }
            }
            ReregisterSprites();
        }

        List<Action> UnregisterActions = new();

        void ReregisterSprites()
        {
            UnregisterActions.Clear();
            for (int readIndex = 0; readIndex < ReadToSpriteRenderers.Length; readIndex++)
            {
                var spriteRenderers = ReadToSpriteRenderers[readIndex].spriteRenderers;
                for (int rendererIndex = 0; rendererIndex < spriteRenderers.Length; rendererIndex++)
                {
                    UnregisterActions.Add(RegisterOnSpriteChanged(spriteRenderers[rendererIndex]));
                }
            }
        }

        void OnDisable()
        {
            for (int i = 0; i < UnregisterActions.Count; i++)
            {
                UnregisterActions[i]();
            }
            UnregisterActions.Clear();
        }

        void LateUpdate()
        {
            if (_LastStore != Store)
            {
                _LastStore = Store;
                ReregisterSprites();
            }
            UpdateRenderer();
        }

        public void UpdateRenderer()
        {
            UpdateSkins();
            UpdatePaletteResolution();
            CutterManager.LateUpdate(Store, this, true);
        }

        public Action RegisterOnSpriteChanged(SpriteRenderer spriteRenderer)
        {
            UpdateSprite(spriteRenderer);

            spriteRenderer.RegisterSpriteChangeCallback(UpdateSprite);
            void Unregister()
            {
                spriteRenderer.UnregisterSpriteChangeCallback(UpdateSprite);
            }

            return Unregister;
        }

        private void UpdateSprite(SpriteRenderer spriteRenderer)
        {
            var sprite = spriteRenderer.sprite;
            if (sprite == null)
                return;
            if (Store.SpriteNameToIndex.TryGetValue(sprite.name, out var spriteIndex))
            {
                var spriteSheet = Store.Resolutions[ResolutionID].Palettes[PaletteID].Sprites;
                var replacementSprite = spriteSheet[spriteIndex];
                var readIndex = SpriteRendererToReadIndex[spriteRenderer];
                if (Store.ReadToDeformedSprite.TryGetValue(readIndex, out var deformedSprites))
                {
                    replacementSprite = deformedSprites[replacementSprite];
                }
                if (replacementSprite == spriteRenderer.sprite)
                {
                    return;
                }
                spriteRenderer.sprite = replacementSprite;
            }
        }

        private void UpdatePaletteResolution()
        {
            if (_LastPaletteID != PaletteID || _LastResolutionID != ResolutionID)
            {
                _LastPaletteID = PaletteID;
                _LastResolutionID = ResolutionID;
                for (int readIndex = 0; readIndex < ReadToSpriteRenderers.Length; readIndex++)
                {
                    var spriteRenderers = ReadToSpriteRenderers[readIndex].spriteRenderers;
                    for (int rendererIndex = 0; rendererIndex < spriteRenderers.Length; rendererIndex++)
                    {
                        var spriteRenderer = spriteRenderers[rendererIndex];
                        UpdateSprite(spriteRenderer);
                    }
                }
            }
        }

        private void UpdateSkins()
        {
            if (_LastGroupToSkinID.Length != GroupToSkinID.Length)
                _LastGroupToSkinID = new ushort[GroupToSkinID.Length];

            if (SkinGroups.Length != 0)
            {
                for (int i = 0; i < _LastGroupToSkinID.Length; i++)
                {
                    var skinID = GroupToSkinID[i];
                    var previousSkinID = _LastGroupToSkinID[i];
                    if (skinID == previousSkinID)
                        continue;
                    var skinGroup = SkinGroups[i];
                    if (previousSkinID != ushort.MaxValue)
                    {
                        foreach (var spriteRenderer in skinGroup.Skins[previousSkinID].SpriteRenderers)
                        {
                            spriteRenderer.enabled = false;
                        }
                    }
                    if (skinID != ushort.MaxValue)
                    {
                        foreach (var spriteRenderer in skinGroup.Skins[skinID].SpriteRenderers)
                        {
                            spriteRenderer.enabled = true;
                            RegisterOnSpriteChanged(spriteRenderer);
                        }
                    }
                    _LastGroupToSkinID[i] = skinID;
                }
            }
        }
    }

    public struct CutterManager
    {
        private Matrix4x4[] _LocalToSpriteUV;
        private Matrix4x4[] _SpriteUVToLocal;
        private static MaterialPropertyBlock _PropertyBlock = new MaterialPropertyBlock();
        public unsafe void Init(TBGStore store, TBGRenderer renderer)
        {
            _LocalToSpriteUV = new Matrix4x4[renderer.CutterToTransform.Length];
            _SpriteUVToLocal = new Matrix4x4[renderer.CutterToTransform.Length];

            for (int cutterIdx = 0; cutterIdx < renderer.CutterToTransform.Length; cutterIdx++)
            {
                var spriteRenderers = renderer.ReadToSpriteRenderers[store.CutterToCutteeReadIndex[cutterIdx]].spriteRenderers;
                for (int rendererIdx = 0; rendererIdx < spriteRenderers.Length; rendererIdx++)
                {
                    spriteRenderers[rendererIdx].GetPropertyBlock(_PropertyBlock);
                    _PropertyBlock.SetInt(CutterInverse, store.CutterToInverse[cutterIdx] ? 1 : 0);
                    spriteRenderers[rendererIdx].SetPropertyBlock(_PropertyBlock);
                }
            }
        }

        private static readonly int CutterInverse = Shader.PropertyToID("_CutterInverse");
        private static readonly int _CutterEnabledPropertyID = Shader.PropertyToID("_CutterEnabled");
        private static readonly int _CutterRectPropertyID = Shader.PropertyToID("_CutterRect");
        private static readonly int _TexCoordToCutterCoordPropertyID = Shader.PropertyToID("_TexCoordToCutterCoord");
        private static readonly int _CutterTexPropertyID = Shader.PropertyToID("_CutterTex");

        public void LateUpdate(TBGStore store, TBGRenderer renderer, bool spriteSheetChanged)
        {
            Profiler.BeginSample("CutterManager.LateUpdate");
            for (int cutterIdx = 0; cutterIdx < renderer.CutterToTransform.Length; cutterIdx++)
            {
                Profiler.BeginSample("Get Sprites");
                var cuttees = renderer.ReadToSpriteRenderers[store.CutterToCutteeReadIndex[cutterIdx]].spriteRenderers;
                var cutters = renderer.ReadToSpriteRenderers[store.CutterToMatteReadIndex[cutterIdx]].spriteRenderers;
                for (int cutterRendererIdx = 0; cutterRendererIdx < cutters.Length; cutterRendererIdx++)
                {
                    for (int cutteeRendererIdx = 0; cutteeRendererIdx < cuttees.Length; cutteeRendererIdx++)
                    {
                        var cuttee = cuttees[cutteeRendererIdx];
                        var cutter = cutters[cutterRendererIdx];
                        if (cuttee.enabled == false || cutter.enabled == false)
                            continue;
                        Sprite cutteeSprite = cuttee.sprite;
                        Sprite matteSprite = cutter.sprite;
                        var cutteeChanged = true;
                        var matteChanged = true;

                        if (cutteeChanged || spriteSheetChanged)
                        {
                            Profiler.BeginSample("Calculate SpriteUVToLocal");
                            _SpriteUVToLocal[cutterIdx] = SpriteUVToLocal(cutteeSprite);
                            Profiler.EndSample();
                        }

                        Profiler.BeginSample("GetPropertyBlock");
                        cuttee.GetPropertyBlock(_PropertyBlock);
                        Profiler.EndSample();

                        _PropertyBlock.SetFloat(_CutterEnabledPropertyID, 1);
                        _PropertyBlock.SetInt(CutterInverse, store.CutterToInverse[cutterIdx] ? 1 : 0);

                        if (matteChanged || spriteSheetChanged)
                        {
                            Profiler.BeginSample("Calculate SpriteUVToLocal");
                            _LocalToSpriteUV[cutterIdx] = LocalToSpriteUV(matteSprite, out var cutterRect);
                            Profiler.EndSample();
                            Profiler.BeginSample("SetVector");
                            _PropertyBlock.SetVector(_CutterRectPropertyID, cutterRect);
                            Profiler.EndSample();
                        }

                        var cutterTransform = renderer.CutterToTransform[cutterIdx];

                        Profiler.BeginSample("Retrieve Transform Matrices");
                        var cutterWorldToLocal = cutterTransform.worldToLocalMatrix;
                        var rendererLocalToWorld = cuttee.transform.localToWorldMatrix;
                        Profiler.EndSample();

                        Profiler.BeginSample("Calculate Matrix");
                        var texCoordToCutterCoord = _LocalToSpriteUV[cutterIdx]
                            * cutterWorldToLocal
                            * rendererLocalToWorld
                            * _SpriteUVToLocal[cutterIdx];
                        Profiler.EndSample();

                        Profiler.BeginSample("SetMatrix/Texture");
                        _PropertyBlock.SetMatrix(_TexCoordToCutterCoordPropertyID, texCoordToCutterCoord);
                        if (matteSprite != null)
                            _PropertyBlock.SetTexture(_CutterTexPropertyID, matteSprite.texture);
                        Profiler.EndSample();

                        Profiler.BeginSample("SetPropertyBlock");
                        cuttee.SetPropertyBlock(_PropertyBlock);
                        Profiler.EndSample();
                    }
                }
                Profiler.EndSample();
            }
            Profiler.EndSample();
        }
        static Matrix4x4 SpriteUVToLocal(Sprite sprite)
        {
            if (sprite == null) return Matrix4x4.identity;
            var pixelToUV = new Vector2(1.0f / sprite.texture.width, 1.0f / sprite.texture.height);
            var spriteSheetUVToSpriteUV = Matrix4x4.Scale(new Vector2(
                    1 / sprite.textureRect.size.x / pixelToUV.x,
                    1 / sprite.textureRect.size.y / pixelToUV.y))
                * Matrix4x4.Translate(-sprite.textureRect.min * pixelToUV);
            var uvToLocalPosition = Matrix4x4.TRS(sprite.bounds.min, Quaternion.identity, sprite.bounds.size);
            return uvToLocalPosition * spriteSheetUVToSpriteUV;
        }
        static Matrix4x4 LocalToSpriteUV(Sprite sprite, out Vector4 rect)
        {
            if (sprite == null)
            {
                rect = Vector4.zero;
                return Matrix4x4.identity;
            }
            var pixelToUV = new Vector2(1.0f / sprite.texture.width, 1.0f / sprite.texture.height);
            var min = sprite.textureRect.min * pixelToUV;
            var size = sprite.textureRect.size * pixelToUV;
            var spriteUVToSpriteSheetUV = Matrix4x4.TRS(min, Quaternion.identity, size);
            var localPositionToUV = Matrix4x4.Scale(new Vector2(1.0f / sprite.bounds.size.x, 1.0f / sprite.bounds.size.y))
                * Matrix4x4.Translate(-sprite.bounds.min);
            rect = new Vector4(min.x, min.y, size.x, size.y);
            return spriteUVToSpriteSheetUV * localPositionToUV;
        }
    }
}

#endif

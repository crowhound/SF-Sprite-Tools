using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace SFEditor.SpritesData
{
    public enum AutoSlicingMethod
    {
        DeleteAll = 0,
        Smart = 1,
        Safe = 2
    }

    /// <summary>
    /// Helper class used to calculate slice operations on SpriteRects. This is used to do slice operations inside of Sprite Editor tools.
    /// </summary>
    public class SpriteSlicer
    {
        /// <summary>
        /// A cached copy of the texture we are doing operations on.
        /// </summary>
        private Texture2D _textureToSlice;
        private Texture2D _readableTextureCopy;


        public List<Rect> SlicedRects = new();
        
        private int TextureWidth => _textureToSlice.width;
        private int TextureHeight => _textureToSlice.height;
        private Vector2 _gridSpriteSize;

        public bool KeepEmptyRects = false;
        public SliceGridOperationType SlicingOperation;
        public AutoSlicingMethod AutoSlicingMethod;
        public SpriteAlignment SpriteAlignment;

        public Vector2 SpritePivot;
        public Vector2 GridSpriteOffset;
        public Vector2 GridSpritePadding;
        public Vector2 GridCellCount;

        public SpriteSlicer(Texture2D textureToSlice) 
        {
            if(textureToSlice == null)
                return;

            _textureToSlice = textureToSlice;
            _readableTextureCopy = CreateTemporaryCopy(_textureToSlice);
        }

        public void DoGridSlicing()
        {
            switch(SlicingOperation)
            {
                case SliceGridOperationType.Auatomatic:
                    DoAutomaticSlicing(4);
                    break;
                case SliceGridOperationType.GridByCellCount:
                    DoGridByCellCountSlicing();
                    break;
            }
        }

        public void DoGridSlicing(Vector2 size, Vector2 offset, Vector2 padding, int alignment, Vector2 pivot, AutoSlicingMethod slicingMethod = AutoSlicingMethod.Smart, bool keepEmptyRects = false)
        {
            SlicedRects = InternalSpriteUtility.GenerateGridSpriteRectangles(_readableTextureCopy,GridSpriteOffset, _gridSpriteSize, GridSpritePadding, keepEmptyRects).ToList();
        }
        /// <summary>
        /// Calls Unity's auomatic Sprite Slicing command that is used in the built in Sprite Editor.
        /// 
        /// </summary>
        /// <param name="minimumSpriteSize"></param>
        public void DoAutomaticSlicing(int minimumSpriteSize)
        {
            if(_textureToSlice == null)
            {
                Debug.Log("Texture not set in Sprite Slicer yet");
                return;
            }
            SlicedRects = InternalSpriteUtility.GenerateAutomaticSpriteRectangles((Texture2D)_readableTextureCopy, minimumSpriteSize, 0).ToList();
            if(SlicedRects.Count == 0)
                Debug.LogWarning("There were no Sprite Rects returned during the automatic slicing operation. If you are doing a manual call to the Sprite Slicing functions remember that you first have to generate mipmaps on a copy of the Sprite Texture first and pass that copied texture as the texture to slice. Automatic Sprite Slicing only works on textures that have mipmaps generated.");
        }
        private void DoGridByCellCountSlicing()
        {
            DetermineGridCellSizeWithCellCount(out var cellsize);
            _gridSpriteSize = cellsize;
            DoGridSlicing(_gridSpriteSize,GridSpriteOffset,GridSpritePadding, (int)SpriteAlignment, SpritePivot, AutoSlicingMethod, KeepEmptyRects);
        }
        private void DetermineGridCellSizeWithCellCount(out Vector2 cellSize)
        {
            cellSize.x = (TextureWidth - GridSpriteOffset.x - (GridSpritePadding.x * Math.Max(0, GridCellCount.x - 1))) / GridCellCount.x;
            cellSize.y = (TextureHeight - GridSpriteOffset.y - (GridSpritePadding.y * Math.Max(0, GridCellCount.y - 1))) / GridCellCount.y;

            cellSize.x = Mathf.Floor(cellSize.x);
            cellSize.y = Mathf.Floor(cellSize.y);

            cellSize.x = Mathf.Clamp(cellSize.x, 1, TextureWidth);
            cellSize.y = Mathf.Clamp(cellSize.y, 1, TextureHeight);
        }

        /// <summary>
        /// Creates a temp copy of the texture so we can use it for Sprite Slicing without manipulating the original.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private Texture2D CreateTemporaryCopy(Texture2D original)
        {
            RenderTexture active = RenderTexture.active;
            RenderTexture temporary = RenderTexture.GetTemporary(TextureWidth, TextureHeight, 0, SystemInfo.GetGraphicsFormat(DefaultFormat.LDR));
            Graphics.Blit(original, temporary);
            RenderTexture.active = temporary;
            bool flag = TextureWidth >= SystemInfo.maxTextureSize || TextureHeight >= SystemInfo.maxTextureSize;
            Texture2D texture2D = new Texture2D(TextureWidth, TextureHeight, TextureFormat.RGBA32, original.mipmapCount > 1 || flag);
            texture2D.ReadPixels(new Rect(0f, 0f, TextureWidth, TextureHeight), 0, 0);
            texture2D.Apply();
            RenderTexture.ReleaseTemporary(temporary);
            texture2D.alphaIsTransparency = original.alphaIsTransparency;
            return texture2D;
        }
    }
}

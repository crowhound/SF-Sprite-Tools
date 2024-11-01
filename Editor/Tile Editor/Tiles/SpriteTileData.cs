using UnityEditor;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace SFEditor.SpritesData
{

	// TODO: Add a Collection of this to the SFSpriteEditorSettings to allow for easily seeing and editing the SpriteTileRenderLayers.
	[System.Serializable]
	public class SpriteTileRenderLayer
	{
		/// <summary>
		/// This is used in URP for ligh, shadow, and other render layer checks.
		/// </summary>
		public uint RenderLayerMask;
		public SortingLayer SortingLayer;
		public int SortingOrder;

	}

	public class SpriteTileData
    {
		/// <summary>
		/// This is used to keep track of the tiles linked to a Sprite.
		/// </summary>
		public GUID SpriteTileID;

		public SpriteTileRenderLayer TileRenderLayer;
		public TileBase DefaultTile;
    }
}

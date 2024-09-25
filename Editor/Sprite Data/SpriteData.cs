using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace SFEditor.SpritesData
{
	/// <summary>
	/// A custom class that can be used to edit, add, remove, or read data from sprites in the Editor. 
	/// </summary>
	/// <remarks>This is used inside of the SF Sprite Editor tools to make updates to Sprite Assets.</remarks>
	[System.Serializable]
	public class SpriteData : SpriteRect
	{
		public SpriteData(SpriteRect spriteRect)
		{
			this.name = spriteRect.name;
			this.rect = spriteRect.rect;
			this.spriteID = spriteRect.spriteID;
			this.pivot = spriteRect.pivot;
			this.alignment = spriteRect.alignment;
			this.border = spriteRect.border;
			this.customData = spriteRect.customData;
		}

		// TODO: Find a way to pull out of Editor only assemblies. This could be useful for some shader, sprite vfx, and more effects.
		// This might require making a class that doesn't inherit from SpriteRect and instead has an implicity/explicit operator for converting it over

		// The animation clip the sprite in the sprite rect is used for.
		// Note this won't initially support multiple animation clips using the same sprite from the SF Sprite Editor, but normal animation work flow works like normal.
		public AnimationClip AnimationClip;

		// The list of tiles the sprite is used in.
		public List<Tile> Tiles;
	}
}

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
		/// <summary>
		/// This is the ID that is used to keep track of all data that is not kept inside of the base Sprite Rect class. This is used to keep track of linked animation clips, tiles, and other types of Sprite Data needing saved to the actual project.
		/// </summary>
		public GUID SpriteDataPairID;
		public AnimationClip AnimationClip;

		// The list of tiles the sprite is used in.
		public List<SpriteTileData> SpriteTileDatas;

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

		/// <summary>
		/// Converts the custom data in the Sprite Data class to a string so unity can save it to a Sprite meta file.
		/// </summary>
		/// <returns></returns>
		private void SetCustomMetaData()
		{
			customData = SpriteDataPairID.ToString();
		}

		/// <summary>
		/// Gets the custom data saved to a Sprite's meta data file, allowing us to take it and convert it into the format needed for SpriteData class.
		/// </summary>
		/// <remarks>
		///	The custom data in the Sprite's meta file is where we store the information for linked <seealso cref="SpriteTileData"/>.
		///	We only keep track of the GUI identifier for the SpriteTileData allowing us to only need to save minimal data to each Sprite's meta data file. We than recover the actually Sprite meta data file. 
		///	The cached SpriteTileData is saved in the <seealso cref="SpriteTileDataCache"/> which is stored inside of the SFEditorProjectSettings.
		/// </remarks>
		private string GetCustomMetaData()
		{
			return SpriteDataPairID.ToString();
		}


		// TODO: Find a way to pull out of Editor only assemblies. This could be useful for some shader, sprite vfx, and more effects.
		// This might require making a class that doesn't inherit from SpriteRect and instead has an implicity/explicit operator for converting it over


	}
}

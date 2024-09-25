using System.Collections.Generic;
using System.Linq;

using SFEditor.Utilities;

using UnityEditor;
using UnityEditor.U2D.Sprites;

using UnityEngine;

namespace SFEditor.SpritesData
{
    public class SpriteDataCache : ScriptableObject
    {
		/// <summary>
		/// The collection of Sprite Data that have been caches for Sprite operations.
		/// </summary>
		public List<SpriteData> SpriteDataRects
		{
			get { return _spriteDataRects; }
			private set { _spriteDataRects = value; }
		}
		[SerializeField] private List<SpriteData> _spriteDataRects = new List<SpriteData>();

		private IReadOnlyCollection<SpriteRect> _spriteRectCache = new List<SpriteRect>();
		public List<SpriteData> SelectedSpriteRects = new();
		public SpriteData LastSelectedSprite;

		public List<Sprite> SelectedSprites = new();
		public int Count 
		{ 
			get { return _spriteDataRects != null ? _spriteDataRects.Count : 0; } 
		}

		public List<string> SpriteNames;
		public StringGUIDList SpriteFileIDs;
		public HashSet<string> NamesInUse;
		public HashSet<GUID> SpriteIDsInUse;
		public List<SpriteNameFileIdPair> SpriteNameFileIDPairs = new();

		public SpriteDataCache()
		{
			SpriteNames = new List<string>();
			SpriteFileIDs = new StringGUIDList();

			NamesInUse = new HashSet<string>();
			SpriteIDsInUse = new HashSet<GUID>();
		}

		/// <summary>
		/// Gets the Sprite Data at the passed in index inside the SpriteDataCache.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public SpriteData SpriteDataAt(int index)
		{
			// Check to make sure the index is actual valid. If not return null SpriteData.
			return index >= Count || index < 0 ? null : _spriteDataRects[index];
		}

		public List<SpriteData> GetSpriteRects()
		{
			// TODO: Add some error checks and some safety initalization for grabbing custom Sprite Data.
			// The custom SpriteData will be used for animations, tile map, and 2D Natvigation.
			return SpriteDataRects;
		}

		public void SetSpriteDataRects(List<SpriteData> newSpriteDataRects)
		{
			//TODO: Remove the clear/insert range and use the newer LINQ commands without allocations.
			SpriteDataRects.Clear();
			SpriteDataRects.InsertRange(0, newSpriteDataRects);
			NamesInUse = new();
			SpriteIDsInUse = new();
			for(int i = 0; i < SpriteDataRects.Count; i++)
			{
				NamesInUse.Add(SpriteDataRects[i].name);
				SpriteIDsInUse.Add(SpriteDataRects[i].spriteID);
			}
		}

		public void SetFileNameIdPairs(IEnumerable<SpriteNameFileIdPair> pairs)
		{
			SpriteNames.Clear();
			SpriteFileIDs.Clear();
			SpriteNameFileIDPairs.Clear();

			foreach(var pair in pairs)
			{
				AddFileNameIdPair(pair.name, pair.GetFileGUID());
				SpriteNameFileIDPairs.Add(pair);
			}
		}

		protected void AddFileNameIdPair(string spriteName, GUID fileId)
		{
			SpriteNames.Add(spriteName);
			SpriteFileIDs.Add(fileId);
		}

		public void AddData(SpriteData spriteData)
		{
			if(_spriteDataRects != null)
				_spriteDataRects.Add(spriteData);
		}
		public void RemoveData(SpriteData spriteData)
		{
			if(_spriteDataRects != null)
				_spriteDataRects.RemoveAll(x => x.spriteID == spriteData.spriteID);
		}
		public void SaveSpriteDataToAsset(ISpriteEditorDataProvider dataProvider)
		{
			_spriteRectCache = SpriteDataRects;

			//Write the updated data back to the data provider
			dataProvider.SetSpriteRects(_spriteRectCache.ToArray());

			// This part is required for changing names post Unity 2021.2 and newer
			var nameFileIDProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();

			// Create a new list of the last updated values names and ids in the Sprite Data Cache.
			List<SpriteNameFileIdPair> pairs = new();

			// Add the Sprite Data pairs to the newly made list.
			foreach(SpriteData spriteData in SpriteDataRects)
			{
				pairs.Add(new SpriteNameFileIdPair(spriteData.name, spriteData.spriteID));
			}

			// This updates the Sprite Data Caches copy of the Sprite name/id pairs.
			// The cached copy is what is read from when getting the SpriteFileNameID pairs for Sprite Frame modules to use.
			SetFileNameIdPairs(pairs);

			// Pass in the newly updated Sprite Data Caches copy of the SpriteFileNameIDPairs.
			nameFileIDProvider.SetNameFileIdPairs(SpriteNameFileIDPairs);

			// Apply the changes. Most of the time this is to a TextureImporterDataProvider class object.
			dataProvider.Apply();
			
			// Force a reimport because some Unity versions bug out and doesn't reimport the asset automatically. You either have to right click reimport or reopen the project after closing it.
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(dataProvider.targetObject));
		}

		public void ClearAll()
		{
			if(_spriteDataRects != null)
				_spriteDataRects.Clear();
		}

		public int GetIndex(SpriteData spriteData)
		{
			if(_spriteDataRects != null && spriteData != null)
				return _spriteDataRects.FindIndex(p => p.spriteID == spriteData.spriteID);

			return -1;
		}

		public bool Contains(SpriteData SpriteData)
		{
			if(_spriteDataRects != null && SpriteData != null)
				return _spriteDataRects.Find(x => x.spriteID == SpriteData.spriteID) != null;

			return false;
		}

		void OnEnable()
		{
			if(_spriteDataRects == null)
				_spriteDataRects = new List<SpriteData>();
		}
	}
}

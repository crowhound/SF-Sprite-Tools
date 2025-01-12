using System.Collections.Generic;
using System.Linq;

using SFEditor.SpritesData.Utilities;
using SFEditor.Utilities;

using UnityEditor;
using UnityEditor.U2D.Sprites;

using UnityEngine;

namespace SFEditor.SpritesData
{
    public class SpriteDataCache : ScriptableObject
    {
		/// <summary>
		/// Keeps track if the data has changed since last time the SpriteRects were applied and save to the currently selected sprite sheet texture.
		/// </summary>
		public bool HasDataChanged = false;
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
		public HashSet<GUID> SpriteIDInUse;
		public List<SpriteNameFileIdPair> SpriteNameFileIDPairs = new();

		/// <summary>
		/// The matrix for the currently loaded sprite sheet view if there is one being used.
		/// </summary>
		public Matrix4x4 TextureHandlesMatrix;

        public SpriteDataCache()
		{
			SpriteNames = new List<string>();
			SpriteFileIDs = new StringGUIDList();

			NamesInUse = new HashSet<string>();
			SpriteIDInUse = new HashSet<GUID>();
		}


        /// <summary>
        /// This function is used to do the full initialization of a SpriteDataCache 
		/// and any ISpriteEditorDataProvider interfaces needed for it.
		/// The created ISpriteEditorDataProvider and ISpriteNameFileIdDataProvider are passed out using an out argument for use.
        /// </summary>
        /// <returns></returns>
        public static SpriteDataCache FullInitialization(
			SpriteDataCache spriteDataCache, 
			Object targetObject,
			out ISpriteEditorDataProvider spriteDataProvider,
			out ISpriteNameFileIdDataProvider nameFileDataProvider)
		{
            // Create a new clean instance of ISpriteEditorDataProvider
            spriteDataProvider = SFSpriteEditorUtilities.CreateInitSpriteEditorDataProvider(targetObject);

            /*	Create a new instance of ISpriteNameFileIdDataProvider 
			*	A new set of SpriteIdNamePairs is created during the call to DestroyAndCreateNewCache 
			*	so we don't have to do anything else here after setting a new DefaultSpriteNameFileIdProvider
			*/
            var spriteList = spriteDataProvider.GetSpriteRects().ToSpriteData();
            nameFileDataProvider = new DefaultSpriteNameFileIdProvider(spriteList);

            // Using the newly created instances of ISpriteEditorDataProvider and ISpriteNameFileIdDataProvider
			// create a new SpriteDataCache and return it.
            spriteDataCache = DestroyAndCreateNewCache(spriteDataCache);

			if(spriteDataCache == null)
				Debug.Log("WE FUCKED IT SON!!!");

			return spriteDataCache;
        }

        /// <summary>
        /// Destroys the SpriteDataCache this is being passed in if it isn't null, than runs through the process of creating a new SpriteDataCache one with new ISpriteNameFileIdDataProvider keys and returns the newly created one.
        /// </summary>
        /// <returns></returns>
        public static SpriteDataCache DestroyAndCreateNewCache(SpriteDataCache spriteDataCache,
			ISpriteEditorDataProvider spriteDataProvider = null,
            ISpriteNameFileIdDataProvider nameFileDataProvider = null)
		{
			if(spriteDataCache != null)
				DestroyImmediate(spriteDataCache);

			return CreateSpriteDataCache(spriteDataProvider,nameFileDataProvider);
        }

        /// <summary>
        /// This initializes a new SpriteDataCache with the proper editor required flags
		/// and than returns it. 
        /// </summary>
		/// <remarks>
		///		This basically takes the place of a SpriteDataCache Factory class.
		/// </remarks>
        /// <returns></returns>
        public static SpriteDataCache CreateSpriteDataCache()
		{
			SpriteDataCache spriteData = CreateInstance<SpriteDataCache>();
            spriteData.hideFlags = HideFlags.HideAndDontSave;

			return spriteData;
        }
        /// <summary>
        /// Initializes a new SpriteDataCache and also sets up the NameFile if the passed in ISpriteEditorDataProvider is not null. If the passed in ISpriteNameFileIdDataProvider
		/// is null a new one will be created and linked to the passed in ISpriteEditorDataProvider.
        /// </summary>
        /// <param name="spriteDataProvider"></param>
        /// <param name="nameFileDataProvider"></param>
        /// <returns></returns>
        public static SpriteDataCache CreateSpriteDataCache(
			ISpriteEditorDataProvider spriteDataProvider,
            ISpriteNameFileIdDataProvider nameFileDataProvider = null)
        {
            SpriteDataCache spriteData = CreateInstance<SpriteDataCache>();
            spriteData.hideFlags = HideFlags.HideAndDontSave;


			if(spriteDataProvider != null)
			{
				var spriteList = spriteDataProvider.GetSpriteRects().ToSpriteData();

				if(nameFileDataProvider == null)
					nameFileDataProvider = new DefaultSpriteNameFileIdProvider(spriteList);

				var nameFileIDPairs = nameFileDataProvider.GetNameFileIdPairs();

                spriteData.SetSpriteDataRects(spriteList);
                spriteData.SetFileNameIdPairs(nameFileIDPairs);
            }

            return spriteData;
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
			SpriteIDInUse = new();
			for(int i = 0; i < SpriteDataRects.Count; i++)
			{
				NamesInUse.Add(SpriteDataRects[i].name);
				SpriteIDInUse.Add(SpriteDataRects[i].spriteID);
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


		public bool AddSpriteData(SpriteData spriteData, bool shouldReplaceInTable = false)
		{
            if(spriteData.spriteID.Empty())
            {
                spriteData.spriteID = GUID.Generate();
            }
            else
            {
                if(IsSpriteIdInUsed(spriteData.spriteID))
                    return false;
            }

			// TODO: Check if the Sprite exists and if it needs replaced.

			if(shouldReplaceInTable)
			{

			}
			else
			{

			}

			if(_spriteDataRects != null)
			{
				_spriteDataRects.Add(spriteData);
			}

			return true;
		}
		public void RemoveSpriteData(SpriteData spriteData)
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

		public void ClearAllData()
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

		public bool IsSpriteIdInUsed(GUID spriteID)
		{
			return SpriteIDInUse.Contains(spriteID);
		}
		public bool IsNameUsed(string name)
		{
			return NamesInUse.Contains(name);
		}

        #region Externally callled functions
		// The functions here are called from classing that have a reference to a SpriteDataCache instance.

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		///		This needs to be called from functions that can properly evaluate the current 
		///		Event.Current during editoe lifetime cycles.
		///		
		///		Example OnGUI, 
		/// </remarks>
		/// <returns></returns>
        public bool ShouldHandleSpriteSelection()
        {
            bool changed = false;

            var oldSelectedRect = LastSelectedSprite;

            var triedSelectedRect = TrySelectSprite(Event.current.mousePosition);

            if(triedSelectedRect != oldSelectedRect)
            {
                Undo.RegisterCompleteObjectUndo(this, "Sprite Selection");
                LastSelectedSprite = (SpriteData)triedSelectedRect;

                if(LastSelectedSprite != null)
                {
                    if(Event.current.control)
                    {
                        SelectedSpriteRects.Add(LastSelectedSprite);
                    }
                    else
                    {
                        SelectedSpriteRects.Clear();
                        SelectedSpriteRects.Add(LastSelectedSprite);
                    }
                }
                else
                {
                    SelectedSpriteRects.Clear();
                }

                changed = true;
            }

            return changed;
        }

        public SpriteRect TrySelectSprite(Vector2 mousePosition)
        {
            float selectionSize = float.MaxValue;
            SpriteRect currentRect = null;

            // We have to multiply the mouse position by the handles matrix being used by the texture view to properly get the right position in texture coordintes.
            mousePosition = TextureHandlesMatrix.inverse.MultiplyPoint(mousePosition);

            // Need to convert the mouse position over to the flipped view port matrix to match the texture sprite rect coordinates
            // mousePosition = Handles.inverseMatrix.MultiplyPoint(mousePosition);

            for(int i = 0; i < SpriteDataRects.Count; i++)
            {
                var spriteRect = SpriteDataRects[i];
                if(spriteRect.rect.Contains(mousePosition))
                {
                    // If the current sprite was the one being clicked just return the same sprite.
                    if(spriteRect == LastSelectedSprite)
                        return LastSelectedSprite;

                    float width = spriteRect.rect.width;
                    float height = spriteRect.rect.height;
                    float newSize = width * height;
                    if(width > 0f && height > 0f && newSize < selectionSize)
                    {
                        currentRect = spriteRect;
                        selectionSize = newSize;
                    }
                }
            }
            return currentRect;
        }
        #endregion

        void OnEnable()
		{
			if(_spriteDataRects == null)
				_spriteDataRects = new List<SpriteData>();
		}
	}
}

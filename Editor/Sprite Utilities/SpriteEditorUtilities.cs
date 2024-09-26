using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.U2D.Sprites;

using UnityEngine;

namespace SFEditor.SpritesData.Utilities
{
    public static class SFSpriteEditorUtilities
    {
		/// <summary>
		/// Initializes a new instance of SpriteDataProviderFactories and return the newly initialized instance to be instantly used.
		/// </summary>
		/// <returns></returns>
		public static SpriteDataProviderFactories InitSpriteDataPrivoderFactory()
        {
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            return factory;
        }

        /// <summary>
        /// Creates and initializes a Sprite Data Provider and returns in an our value the factory generated with it. 
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="factory"></param>
        /// <returns>Retyrns a ISpriteEditorDataProvider,outs SpriteDataProviderFactories </returns>
        public static ISpriteEditorDataProvider CreateInitSpriteEditorDataProvider(Object sprite, out SpriteDataProviderFactories factory)
        {
            factory = new SpriteDataProviderFactories();
            factory.Init();

            var DataProvider = factory.GetSpriteEditorDataProviderFromObject(sprite);
            DataProvider.InitSpriteEditorDataProvider();
            return DataProvider;
        }

		/// <summary>
		/// Creates and initializes a Sprite Data Provider and returns in an our value the factory generated with it. 
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="factory"></param>
		/// <returns>Retyrns a ISpriteEditorDataProvider,outs SpriteDataProviderFactories </returns>
		public static ISpriteEditorDataProvider CreateInitSpriteEditorDataProvider(Object sprite)
		{
			SpriteDataProviderFactories factory = new SpriteDataProviderFactories();
			factory.Init();

			var DataProvider = factory.GetSpriteEditorDataProviderFromObject(sprite);
			DataProvider.InitSpriteEditorDataProvider();
			return DataProvider;
		}

		public static ISpriteEditorDataProvider CreateInitSpriteEditorDataProvider(SpriteDataProviderFactories factory, Object sprite)
        {
            var DataProvider = factory.GetSpriteEditorDataProviderFromObject(sprite);
            DataProvider.InitSpriteEditorDataProvider();
            return DataProvider;
        }
        public static void CreateInitSpriteEditorDataProvider(SpriteDataProviderFactories factory, Object sprite, out ISpriteEditorDataProvider dataProvider)
        {
            dataProvider = factory.GetSpriteEditorDataProviderFromObject(sprite);
            dataProvider.InitSpriteEditorDataProvider();
        }

		// TODO: Implement the ITextureDataProvider is we can't use the ISpriteEditorDataProvider.

		/// <summary>
		/// Returns a filtered IEnmerable collection of Sprite Rects from the passed in Sprites and the sprite rects contained in the passed in data provider.
		/// </summary>
		/// <param name="sprites"></param>
		/// <param name="spriteRects"></param>
		/// <returns></returns>
		public static List<SpriteRect> FilterSelectedSpriteRects(List<Sprite> sprites, ISpriteEditorDataProvider dataProvider, out List<SpriteRect> spriteRects)
		{
			spriteRects = dataProvider.GetSpriteRects().ToList();

			return FilterSelectedSpriteRects(sprites, spriteRects).ToList();
		}

		/// <summary>
		/// Returns a filtered IEnumerable collection of Sprite Rects from the passed in Sprites and the sprite rects contained in the passed in data provider.
		/// </summary>
		/// <param name="sprites"></param>
		/// <param name="spriteRects"></param>
		/// <returns></returns>
		public static IEnumerable<SpriteRect> FilterSelectedSpriteRects(List<Sprite> sprites, ISpriteEditorDataProvider dataProvider, out SpriteRect[] spriteRects)
        {
            spriteRects = dataProvider.GetSpriteRects();

            return FilterSelectedSpriteRects(sprites, spriteRects);
        }
		/// <summary>
		/// Returns a filtered IEnumerable collection of Sprite Rects from the passed in Sprites and Sprite Rects.
		/// </summary>
		/// <param name="sprites"></param>
		/// <param name="spriteRects"></param>
		/// <returns></returns>
		public static IEnumerable<SpriteRect> FilterSelectedSpriteRects(List<Sprite> sprites, List<SpriteRect> spriteRects)
        {
            return (from sprite in sprites
                    join spriteRect in spriteRects on sprite.name equals spriteRect.name
                    select spriteRect).OrderByDescending(sprite => sprite.name);
        }
		/// <summary>
		/// Returns a filtered IEnumerable collection of Sprite Rects from the passed in Sprites and Sprite Rects.
		/// </summary>
		/// <param name="sprites"></param>
		/// <param name="spriteRects"></param>
		/// <returns></returns>
		public static IEnumerable<SpriteRect> FilterSelectedSpriteRects(List<Sprite> sprites, SpriteRect[] spriteRects)
        {
            return (from sprite in sprites
                    join spriteRect in spriteRects on sprite.name equals spriteRect.name
                    select spriteRect).OrderByDescending(sprite => sprite.name);
        }

		/// <summary>
		/// Gets a specific Sprite from the passed in ISpriteEditorDataProvider that matches the passed in name.
		/// </summary>
		/// <param name="dataProvider"></param>
		/// <exception cref="System.Exception"></exception>
		public static void GetSprite(ISpriteEditorDataProvider dataProvider, string spriteName)
        {
			throw new System.Exception("Not Yet Implemented");
        }

		/// <summary>
		/// Retreives the first sprite from the selected sprites in the Editor Selection API calls.
		/// Used to grab the actual first selected Sprite asset during asset selection.
		/// </summary>
		/// <returns></returns>
        public static Sprite GetFirstSelectedSprite()
        {
            List<Sprite> sprites = Selection.GetFiltered<Sprite>(SelectionMode.Unfiltered).ToList();
            if(sprites.Count == 0)
                return null;
            else
                return sprites[0];
        }

		public static bool GetFirstSelectedSprite(out Sprite sprite)
		{
			List<Sprite> sprites = Selection.GetFiltered<Sprite>(SelectionMode.Unfiltered).ToList();
			if(sprites.Count == 0)
			{
				sprite = null;
				return false;
			}

			sprite = sprites[0];
			return true;
		}

		/// <summary>
		/// Gets only the Sprite asets from all currently selected assets in the editor.  
		/// </summary>
		/// <param name="sprites"></param>
		public static void GetSelectedSprites(ref List<Sprite> sprites)
        {
            sprites = Selection.GetFiltered<Sprite>(SelectionMode.Unfiltered).ToList();
        }

		public static Sprite GetSpriteAssetFromRect(ISpriteEditorDataProvider IspriteEditorDataProvider, SpriteRect spriteRect)
		{
			string path = (IspriteEditorDataProvider.targetObject as TextureImporter).assetPath;
			var asset = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>();

			return asset.Where(sprite => sprite.name == spriteRect.name).First();
		}

		public static List<Sprite> GetAllSpriteAssetsFromRects(ISpriteEditorDataProvider IspriteEditorDataProvider, List<SpriteData > spriteRects)
		{
			string path = (IspriteEditorDataProvider.targetObject as TextureImporter).assetPath;
			List<Sprite> asset = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToList();

			List<Sprite> sprites = new();

			for(int i = 0; i < asset.Count(); i++)
			{
				for(int j = 0; j < spriteRects.Count; j ++)
				{
					if(asset[i].name == spriteRects[j].name)
						sprites.Add(asset[i]);
				}
			}
			return sprites;
		}

		/// <summary>
		/// Updates the name of Sprites in a List<Sprite> and saves the data using the DataProviderAPI.
		/// For sequence of sprites the names have a space than the index of the list added to prevent naming conflicts.
		/// This will reimport the Sprite Sheet to make sure the asset is properly refreshed for the AssetDatabase.
		/// </summary>
		/// <param name="sprites"></param>
		/// <param name="newName"></param>
		public static void UpdateSpriteNames(this List<Sprite> sprites, string newName)
        {
            if(sprites.Count < 1) return;

            var dataProvider = CreateInitSpriteEditorDataProvider(sprites[0], out SpriteDataProviderFactories factory);
            var selectedRects = FilterSelectedSpriteRects(sprites, dataProvider, out SpriteRect[] spriteRects);

            int index = 0;
				
            foreach(var rect in selectedRects)
            {
                rect.name = newName + " " + index;
                index++;
            }

            dataProvider.SetAndSaveSpriteRects(spriteRects);
        }

		/// <summary>
		/// Updates the name of Sprites in the Selection API List<Sprite> and saves the data using the DataProviderAPI.
		/// For sequence of sprites the names have a space than the index of the list added to prevent naming conflicts.
		/// This will reimport the Sprite Sheet to make sure the asset is properly refreshed for the AssetDatabase.
		/// </summary>
		/// <param name="sprites"></param>
		/// <param name="newName"></param>
		public static void UpdateSelectedSpriteNames(string newName)
        {
            List<Sprite> sprites = Selection.GetFiltered<Sprite>(SelectionMode.Unfiltered).ToList();
            UpdateSpriteNames(sprites, newName);
        }

		/// <summary>
		/// Sets the SpriteRect data for the ISpriteEditorDataProvider than applies and saves the data updates.
		/// This will reimport the Sprite Sheet to make sure the asset is properly refreshed for the AssetDatabase.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataProvider"></param>
		/// <param name="spriteRects"></param>
        public static void SetAndSaveSpriteRects<T>(this T dataProvider, SpriteRect[] spriteRects) where T : ISpriteEditorDataProvider
        {
            dataProvider.SetSpriteRects(spriteRects);
            dataProvider.ApplyAndSave();
        }

		/// <summary>
		/// Applies the data changes to the Sprite Editor Data Provider than saves the asset. After saving the asset is than reimported.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataProvider"></param>
		public static void ApplyAndSave<T>(this T dataProvider) where T : ISpriteEditorDataProvider
        {
            dataProvider.Apply();
            SaveAndReimport(dataProvider);
        }
        /// <summary>
        /// Saves the data provider changes to the asset and reimports the asset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataProvider"></param>
        public static void SaveAndReimport<T>(this T dataProvider) where T : ISpriteEditorDataProvider
        {
            var assetImporter = dataProvider.targetObject as AssetImporter;
            assetImporter.SaveAndReimport();
        }

		public static List<SpriteData> ToSpriteData(this List<SpriteRect> spriteRects)
		{
			// TODO: Improve performance of this later by making a ConvertAll TOutPut Extension down the line.

			List<SpriteData> spriteDatas = new List<SpriteData>();
			foreach(var spriteRect in spriteRects)
			{
				spriteDatas.Add((SpriteData)spriteRect);
			}
			return spriteDatas;
		}

		public static List<SpriteData> ToSpriteData(this SpriteRect[] spriteRects)
		{
			// TODO: Improve performance of this later by making a ConvertAll TOutPut Extension down the line.

			List<SpriteData> spriteDatas = new List<SpriteData>();
			foreach(var spriteRect in spriteRects)
			{
				spriteDatas.Add(new SpriteData(spriteRect));
			}
			return spriteDatas;
		}

		public static Color SpriteFrameColor => new Color(.55f,.55f,.55f,1);
		public static Color SelectedSpriteFrameColor => new Color(0, 0.1f,.7f,1);
		public static Color MultiSelectedSpriteFrameColor => new Color(0, .7f,0,1);
	}
}

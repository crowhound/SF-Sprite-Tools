using System;
using System.Linq;

using SFEditor.SpritesData.Utilities;

using UnityEditor;
using UnityEditor.U2D.Sprites;

using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace SFEditor.SpritesData
{
	// This is in the SFSpriteEditorDataProvider file. This is the implementation for the data providers.
	public partial class SFSpriteEditor
	{
		/// <summary>
		/// The texture in the currently loaded ITextureDataProvider/
		/// </summary>
		public Texture2D Texture 
		{
			get {	return  _texture; } 
			protected set
			{
				_texture = value;
                OnTextureChanged();
            }
		}
		private Texture2D _texture;
		/// <summary>
		/// The currently loaded Sprite Tool 
		/// </summary>
		public static ISpriteFrameModule CurrentSpriteFrameModule;
		public Action<ISpriteFrameModule> OpenSpriteFrameModule;

		private ISpriteEditorDataProvider _spriteEditorDataProvider;
		public ISpriteEditorDataProvider SpriteDataProvider
		{
			get
			{
				// TODO: Do checks ot make sure this isn't null. If it is than use the Factory class to set it.
				return _spriteEditorDataProvider;
			}
			set { _spriteEditorDataProvider = value; }
		}
		#region Data Providers
		protected ISpriteNameFileIdDataProvider _nameFileDataProvider;
		/// <summary>
		/// The ITextureDataProvider from the currently loaded SpriteDataProvider.
		/// </summary>
		public ITextureDataProvider TextureDataProvider;
		#endregion

		/// <summary>
		/// The currently selected Unity asset in the project folder. This can be different than the TargetObject since the TargetObject can also select the AssetImporters.
		/// </summary>
		/// <remarks> If you are needed to get information on the currently loaded Texture2D that is holding the Sprites it is better to use the TargetObject and cast it as a TextureImporter to grab the Texture Information.</remarks>
		public UnityObject SelectedObject;
		/// <summary>
		/// This is the targetted Unity Object of the currently loaded sprite data provider being read.
		/// In a lot of cases this is the asset importers that is having it's data read into the data providers. For example TextureImporter is the common target type for the SpriteDataProvider when loading a Texture2D from the assetdatabase.
		/// </summary>
		/// <remarks>This is useful for getting the asset path and the Texture Importer settings like mipmaps, compression, texture sizes, filtermode, texture swizzles for color channels, amd </remarks>
		public UnityObject TargetObject => SpriteDataProvider.targetObject;

		/// <summary>
		/// Returns the asset path in the project of the currently loaded sprite data provider if it is a type of Texture Imported. If the current target object is not a Texture Importer than it returns an empty string.
		/// </summary>
		public string TextureAssetPath
		{ 
			get 
			{ 
				if(TargetObject as TextureImporter != null)
					return (TargetObject as TextureImporter).assetPath;

				Debug.LogError($"The currently loaded Sprite Editor Data Provider target is not a value type of Texture Importer. It is a type of: {TargetObject.GetType()}");
				return string.Empty;
			}
		}

		protected virtual void InitDataProviders()
		{
			// TODO: When changing textures reinitialize the UI ValueChanged fields for the new Sprite Data

			if(SelectedObject == null) return;

			SpriteDataProvider = SFSpriteEditorUtilities.CreateInitSpriteEditorDataProvider(SelectedObject);
			TextureDataProvider = GetTextureDataProviderFromObj(SelectedObject);

			if(TextureDataProvider != null)
				Texture = TextureDataProvider.texture;
		}

		// TODO: This could eventually be replaced with the SF Editor Utility class functions. 
		public virtual ITextureDataProvider GetTextureDataProviderFromObj(UnityObject obj)
		{
			if(obj == null)
				return null;

			var factory = new SpriteDataProviderFactories();
			factory.Init();
			var dataProvider = factory.GetSpriteEditorDataProviderFromObject(obj);

			// Get the ItextureDataProvider.
			return dataProvider.GetDataProvider<ITextureDataProvider>();
		}
	}
}

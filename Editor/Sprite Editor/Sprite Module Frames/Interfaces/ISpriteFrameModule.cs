using UnityEngine.UIElements;
using SFEditor.SpritesData.UIElements;

namespace SFEditor.SpritesData
{
	/// <summary>
	/// This is the base class needed to be implemented to make new Sprite Frame Modules to implement new features into the Sprite Editor.
	/// </summary>
	public abstract class SpriteFrameModule : ISpriteFrameModule
	{
		protected string MenuName;
		protected SpriteDataCache _spriteDataCache;
		protected SFSpriteEditor _spriteEditor;

		protected VisualElement _moduleContainer;
		protected SpriteEditorToolbar _mainToolbar;

		public abstract void CreateGUI();
		public abstract void DoMainGui();
		public abstract void LoadModule();
		public abstract void UnloadModule();
	}

	/// <summary>
	/// A sub frame for the Sprite Editor that contains it's own logic, UI, and features to help edit Sprite Data.
	/// Implment this to add new features to the Sprite Editor.
	/// </summary>
	public interface ISpriteFrameModule
    {
		/// <summary>
		/// Creates the needed visual elements for the Sprite Inspectors. Do not do any IMGUI, GL draws, or rendering here. Do that in DoMainGUI.
		/// </summary>
		void CreateGUI();
		/// <summary>
		/// Do any IMGUI, GL draws commands, or rendering passes for things like Sprite rects, handles, or any other thing that is not a visual element. Do not do any Visual Element stuff here unless it is to reacting to a click or editor event.
		/// Example Having the Sprite Inspector appear when a valid sprite is selected.
		/// </summary>
		void DoMainGui();
		void LoadModule();
		void UnloadModule();
		
    }
}

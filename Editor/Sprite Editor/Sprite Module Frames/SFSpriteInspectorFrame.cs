using System.Collections.Generic;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using SFEditor.SpritesData.Utilities;
using UnityEngine.UIElements;
using UnityEditor.ShortcutManagement;

namespace SFEditor.SpritesData
{
	public class SFSpriteInspectorFrame : SpriteFrameModule
	{

		private ToolbarButton _animationToolbarButton;

		protected SpriteInspector _spriteInspector;

		protected ToolbarButton _sliceToolbarButon;
		protected SpriteSliceInspector _sliceOverlay;
		protected SpriteAnimatorInspector _animatorOverlay;

		private SFSpriteEditorSettings _spriteEditorSettings;

		public SFSpriteInspectorFrame(SFSpriteEditor spriteEditorWindow) 
		{
			_spriteEditor = spriteEditorWindow;
			if(_spriteEditor == null)
				return;

			_spriteDataCache = _spriteEditor.SpriteDataCache;
			_spriteEditorSettings = SFSpriteEditorSettings.GetOrCreateSettings();
			_moduleContainer = _spriteEditor.SpriteModuleContainer;

			_mainToolbar = _spriteEditor.SpriteEditorToolbar;

			CreateGUI();
		}

		/// <summary>
		/// Creates the needed visual elements for the Sprite Inspectors. Do not do any IMGUI, GL draws, or rendering here. Do that in DoMainGUI.
		/// </summary>
		public override void CreateGUI()
		{

			AddToolbarButtons();

			_spriteEditor.OnSelectedSpriteDataChanged += OnSelectedSpriteDataChanged;

			if(_spriteEditorSettings.WasAnimatorInspectorOpen)
				ToggleAnimationOverlay();
		}

		/// <summary>
		/// Do any IMGUI, GL draws commands, or rendering passes for things like Sprite rects, handles, or any other thing that is not a visual element. Do not do any Visual Element stuff here unless it is to reacting to a click or editor event.
		/// Example Having the Sprite Inspector appear when a valid sprite is selected.
		/// </summary>
		public override void DoMainGui()
		{
			
		}

		private void OnSelectedSpriteDataChanged(SpriteData spriteData)
		{
			// TODO: Change the logic to into account unpicking previous sprites and having nothing selected.
			// or just allow the last sprite to not be unselected always letting one sprite to be selected.

			// There are null checks in the function so no need to do it before hand.
			CreateSpriteInspector();

			_spriteInspector.visible = true;
			_spriteInspector.UpdateUIFields(spriteData);
		}

		private void AddToolbarButtons()
		{
			// TODO: I should make a UXML element for a better setup for the Toolbar layout.
			_sliceToolbarButon = new ToolbarButton(ToggleSliceOverlay) { text = "Slice" };
			_mainToolbar.AddToolbarButton(_sliceToolbarButon);

			_mainToolbar.AddToolbarButton(_animationToolbarButton = 
				new ToolbarButton(ToggleAnimationOverlay) { text = "Animation Tools" });
		}

		protected virtual void CreateSpriteInspector()
		{
			// Just in case this is called when a Sprite Inspector is already created.
			if(_spriteInspector != null)
				return;

			_spriteInspector = new SpriteInspector(_spriteDataCache);
			_moduleContainer.Add(_spriteInspector);
		}
		protected virtual void ToggleSpriteInspector()
		{
			/// Only checking null here to make sure we don't
			/// instantly toggle the Sprite inspector to invisble after first creating it.
			if(_spriteInspector == null)
				CreateSpriteInspector();
			else
				_spriteInspector.visible = !_spriteInspector.visible;
		}

		protected virtual void ToggleSliceOverlay()
		{
			if(_sliceOverlay == null)
			{
				_sliceOverlay = new SpriteSliceInspector(_spriteEditor);
				_moduleContainer.Add(_sliceOverlay);
			}
			else
				_sliceOverlay.visible = !_sliceOverlay.visible;
		}

		private void ToggleAnimationOverlay()
		{
			if(_animatorOverlay == null)
			{
				_animatorOverlay = new SpriteAnimatorInspector(_spriteEditor);
				_moduleContainer.Add(_animatorOverlay);
				_spriteEditorSettings.WasAnimatorInspectorOpen = _animatorOverlay.visible;
			}
			else
			{
				_animatorOverlay.visible = !_animatorOverlay.visible;
				_spriteEditorSettings.WasAnimatorInspectorOpen = _animatorOverlay.visible;
			}
		}

		public override void LoadModule()
		{
			if(_spriteDataCache.SelectedSpriteRects.Count < 1)
				return;

			ToggleSpriteInspector();
		}
		public override void UnloadModule()
		{
			_mainToolbar.RemoveToolbarButton(_animationToolbarButton);
			_mainToolbar.RemoveToolbarButton(_sliceToolbarButon);
		}




        #region Shortcuts not organized
        [Shortcut("SF/Sprite Tools/Toggle Sprite Slicer UI", typeof(SFSpriteEditor.SFSpriteEditorShortcutContext), KeyCode.S, ShortcutModifiers.Alt)]
        public static void ToggleSpriteSlicerUIShortcut()
        {
            if(EditorWindow.focusedWindow is SFSpriteEditor && SFSpriteEditor.CurrentSpriteFrameModule is SFSpriteInspectorFrame spriteFrame)
            {
				spriteFrame.ToggleSliceOverlay();
            }
        }

        [Shortcut("SF/Sprite Tools/Toggle Sprite Inspector UI", typeof(SFSpriteEditor.SFSpriteEditorShortcutContext), KeyCode.I, ShortcutModifiers.Alt)]
        public static void ToggleSpriteInspectorUIShortcut()
        {
            if(EditorWindow.focusedWindow is SFSpriteEditor && SFSpriteEditor.CurrentSpriteFrameModule is SFSpriteInspectorFrame spriteFrame)
            {
                spriteFrame.ToggleSpriteInspector();
            }
        }

        [Shortcut("SF/Sprite Tools/Toggle Sprite Animation UI", typeof(SFSpriteEditor.SFSpriteEditorShortcutContext), KeyCode.A, ShortcutModifiers.Shift)]
        public static void ToggleSpriteAnimationUIShortcut()
        {
            if(EditorWindow.focusedWindow is SFSpriteEditor && SFSpriteEditor.CurrentSpriteFrameModule is SFSpriteInspectorFrame spriteFrame)
            {
                spriteFrame.ToggleAnimationOverlay();
            }
        }

        #endregion
    }
}

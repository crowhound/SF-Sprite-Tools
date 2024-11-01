using UnityEditor;
using UnityEditor.U2D.Sprites;

using UnityEngine;
using Event = UnityEngine.Event;
using SFEditor.SpritesData.Utilities;
using SFEditor.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;

namespace SFEditor.SpritesData
{

	public enum SpriteEditorRectMode
	{
		AddSprite,
		Selection,
		SelectionClipping
	}

	/// <summary>
	/// This is the file that contains the Editor logic for the Sprite Editor.
	/// </summary>
	/// For the UI implementation <see cref="SFSpriteEditor.CreateGUI"/> inside of the partial SFSpriteEditor class called SFSpriteEditorView
	/// For the ISpriteDataProvider implementation.
	[RequireSpriteDataProvider(typeof(ITextureDataProvider))]
	public partial class SFSpriteEditor : EditorWindow
	{

        /// <summary>
        /// Creates the Short Context to register to Unity's shortcut manager.
        /// 
        /// This can be made a nested class inside types of EditorWindow to gain access to the focusedWindow variable to make sure the shortcut only activates if the desired type of EditorWindow is opened.
        /// </summary>
        public class SFSpriteEditorShortcutContext : IShortcutContext
        {
            public bool active
            {
                get
                {
					return focusedWindow is SFSpriteEditor view;
				}
            }
        }

		protected SFSpriteEditorShortcutContext _shortcutContext = new SFSpriteEditorShortcutContext();
        public bool EditingDisabled;
        public bool IsDebugging = false;

        public SpriteDataCache SpriteDataCache;
		protected SpriteData _selectedSpriteDataRect;
		public Action<SpriteData> OnSelectedSpriteDataChanged;

		public bool HasSelected { get { return SpriteDataCache.LastSelectedSprite != null; } }

		[MenuItem("Tools/SF/Sprite Editor")]
        [Shortcut("SF/Sprite Tools/Open Sprite Editor",null, KeyCode.O, ShortcutModifiers.Alt)]
        public static void ShowWindow()
		{
			SFSpriteEditor wnd = GetWindow<SFSpriteEditor>();
			wnd.titleContent = new GUIContent("SF Sprite Editor");
			wnd.minSize = new Vector2(400,250);
			wnd.ShowTab();
		}

		/* TODO Clean Up Task List:
		 * 1. Clean Up Scripts and properly organize the calls.
		 * 2. Create properly named functions for the iniitializtion.
		 * 3. Take some of the initailization stuff out of the current functions into the newly created functions. The goal is to not accidentally reinit something causing a lost of the current state of selection and tool data. Example right now the Player Bow sprite is being selected when right clicking no sprites do to trying to redo an initialization on null selected. When repainting right now if the lastSelectedSprite value is null during the early processing of GUI it tries to auto select the first sprite in the texture sheet. This should only be done in initialization of the Sprite Editor or when changing texture sheets not during every GUI repaint.
		 * 4. Put anything related to Sprite Inspector inside the proper class for the SpriteFrame. Samething for toolbar buttons.
		 * 5. Clean up stuff with helpers functions. 
		 * 6. Have the Sprites be deselected properly when doing ctrl click on a non sprite rect.
		 * 7. Add an Undo operation for selecting multiple sprites.
		 */

		/* TODO: Important Feature Implementation List
		 * 1. Bind the last selected Sprite Data to the Sprite Inspector UI.
		 * 2. Implment save, apply, revert with a button and a shortcut key option that is customizable.
		 * 3. Implment renaming a single sprite.
		 * 4. Implement a UI Input Field when multiple Sprites are selected for sequencial sprite renaming. 
		 * 5. 
		 * Rename single sprite with assetdatabase refresh and Undo Operations.
		 * Rename all selected sprites with asset database refresh and Undo Operations.
		 * Create a shortcut key for creating animation from selected sprites.
		*/

		/* TODO: Grid Slicing Feature
		 * 1. Subgrid selection slicing: Allow for choosing an area and being able to just slice sprites inside it. 
		 * 2. Slice Grid Area: The area where slicing is being done on. This allows for having slicing in only an selected area instead of the entire sprite sheet for more controlled slicing.
		 */


		/* TODO: Asset Library Feature Overview.
		 * Asset Library view feature. A side panel pop out that has a list of the Tiles, Sprites, textures, sprite atlases, and so forth.
		 * 1. Filter by asset types.
		 * 2. Filter by asset names.
		 * 3. Filter by multiple properties.
		 * 4. Filter by properties in objects.
		 * 5. Filter by asset tags/labels.
		 */

		private void OnEnable()
		{
			InitDataProviders();
			InitSpriteDataCache();

			RegisterCallbacks();
			ShortcutManager.RegisterContext(_shortcutContext);
		}

        private void OnDisable()
        {
            ShortcutManager.UnregisterContext(_shortcutContext);
        }

        protected virtual void InitSpriteDataCache()
		{
			if(SpriteDataCache != null)
				DestroyImmediate(SpriteDataCache);

			SpriteDataCache = CreateInstance<SpriteDataCache>();
			SpriteDataCache.hideFlags = HideFlags.HideAndDontSave;

			if(SpriteDataProvider != null)
			{
				var spriteList = SpriteDataProvider.GetSpriteRects().ToSpriteData();

				if(_nameFileDataProvider == null)
					_nameFileDataProvider = new DefaultSpriteNameFileIdProvider(spriteList);

				var nameFileIDPairs = _nameFileDataProvider.GetNameFileIdPairs();

				SpriteDataCache.SetSpriteDataRects(spriteList);
				SpriteDataCache.SetFileNameIdPairs(nameFileIDPairs);
			}
		}

		protected virtual void RegisterCallbacks()
		{
			//_rectDragManipulator.OnMouseDragHandler += OnMouseDraggedInTextureRectView;
		}

		[Shortcut("SF/Sprite Tools/Apply Sprite Data Changes",typeof(SFSpriteEditorShortcutContext),KeyCode.A ,ShortcutModifiers.Alt)]
        public static void ApplyShortcut()
        {
            if(focusedWindow is SFSpriteEditor view)
            {
                view.ApplySpriteDataChanges();
            }
        }

        [Shortcut("SF/Sprite Tools/Revert Sprite Data Changes", typeof(SFSpriteEditorShortcutContext), KeyCode.R, ShortcutModifiers.Alt)]
        public static void RevertShortcut()
		{
			if(focusedWindow is SFSpriteEditor view)
			{
				view.RevertSpriteDataChanges();
			}
        }

        public virtual void ApplySpriteDataChanges()
		{
			SpriteDataCache.SaveSpriteDataToAsset(SpriteDataProvider);
		}

		public virtual void RevertSpriteDataChanges()
		{
            Debug.LogWarning("The RevertSpriteDataChanges has not been fully implemented yet. At the moment is does nothing.");

            if(SpriteDataCache.HasDataChanged)
			{
				_mouseDragRect = new Rect(0,0,0,0);
				// We first reset the Sprite Data cache back to the saved data of the sprite sheet.
				InitSpriteDataCache();
                /// TODO: Make sure we are properly declaring the HasDataChanged to be true in the places where we change
                /// Any sprite rect data that is being edited.
                /// 1. Where applying data to save set to false after saving the data to the texture asset of the sprite sheet.
                /// 2. When changing any of the selected Sprite Data set HasDataChanged to true.

                // TODO: Add the logic to revert the editable sprite rects to the cached sprite rects that are stored as the original values if there was a data changed. If no data has been changed than do nothing.

            }
        }

		private bool HandleSpriteSelection()
		{
			bool changed = false;

            var oldSelectedRect = SpriteDataCache.LastSelectedSprite;

			var triedSelectedRect = TrySelectSprite(Event.current.mousePosition);

			if(triedSelectedRect != oldSelectedRect)
			{
				Undo.RegisterCompleteObjectUndo(this, "Sprite Selection");
				SpriteDataCache.LastSelectedSprite = (SpriteData)triedSelectedRect;

				if(SpriteDataCache.LastSelectedSprite != null)
				{
					if(Event.current.control)
					{
						SpriteDataCache.SelectedSpriteRects.Add(SpriteDataCache.LastSelectedSprite);
					}
					else
					{
						SpriteDataCache.SelectedSpriteRects.Clear();
						SpriteDataCache.SelectedSpriteRects.Add(SpriteDataCache.LastSelectedSprite);
					}
				}
				else
				{
					SpriteDataCache.SelectedSpriteRects.Clear();
				}

				changed = true;
			}

			return changed;
		}

		private SpriteRect TrySelectSprite(Vector2 mousePosition)
		{
			float selectionSize = float.MaxValue;
			SpriteRect currentRect = null;

			// We have to multiply the mouse position by the handles matrix being used by the texture view to properly get the right position in texture coordintes.
            mousePosition = _textureHandlesMatrix.inverse.MultiplyPoint(mousePosition);

			// Need to convert the mouse position over to the flipped view port matrix to match the texture sprite rect coordinates
			// mousePosition = Handles.inverseMatrix.MultiplyPoint(mousePosition);

			for (int i = 0; i < SpriteDataCache.SpriteDataRects.Count; i++)
			{
				var spriteRect = SpriteDataCache.SpriteDataRects[i];
				if(spriteRect.rect.Contains(mousePosition))
				{
					// If the current sprite was the one being clicked just return the same sprite.
					if(spriteRect == SpriteDataCache.LastSelectedSprite)
						return SpriteDataCache.LastSelectedSprite;

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

		// TODO: Implement this sucker.
		protected List<SpriteRect> GetBoxSelectionSprites()
		{
			return new List<SpriteRect>();
		}

	
		/// <summary>
		/// Handle any extra logic that needs to run after a new sprite has been selected here even if it is a null value
		/// </summary>
		/// <remarks>This runs after the last selected sprite has been updated fully to the recently clicked one. We allow null values to be used in it so we know when to clear the UI of sprite values.</remarks>
		private void OnHandleSpriteSelection()
		{
			OnSelectedSpriteDataChanged?.Invoke(SpriteDataCache.LastSelectedSprite);
		}


		// TODO: Make the OnSelectionChange read sprite clicks to select the correct sprite.

		/// <summary>
		/// This is called when the asset selected in the Unity Editor is changed. 
		/// OnSelectionChange is an EditorWindow auto called event. No need to register a manually callback to it.
		/// </summary>
		protected virtual void OnSelectionChange()
		{
			if(AssetDatabaseEditorUtilities.TryGetSelectedObjectOfType(out Texture2D selectedTexture))
			{

				// If the newly selected texture is the same as the current one don't reload anything.
				if(Texture == selectedTexture) return;

				SelectedObject = selectedTexture;
				InitDataProviders(); // Texture is set to the newly selected Texture in here.

				if(Texture != null)
				{
					// TODO: Implement a UpdateUIOnSelection method
					// UpdateUIOnSelectionChange();
					Repaint();
					InitSpriteDataCache();
				}
			}
		}

		protected virtual void AddSpriteRectFromDrag(Rect spriteRect)
		{
            AddSprite(spriteRect, (int)SpriteAlignment.Center, new Vector2(0.5f, 0.5f), "Sprite " + GUID.Generate(), Vector4.zero);
		}

		protected virtual int AddSprite(Rect rect, int alignment, Vector2 pivot, string name, Vector4 border)
		{
			if(SpriteDataCache.IsNameUsed(name))
				return -1;

			SpriteRect spriteRect = new SpriteRect();
			spriteRect.rect = rect;
			spriteRect.alignment = (SpriteAlignment)alignment;
			spriteRect.pivot = pivot;
			spriteRect.name = name;
			spriteRect.border = border;

			spriteRect.spriteID = GUID.Generate();

            if(!SpriteDataCache.AddSpriteData(new SpriteData(spriteRect)))
                return -1;

			SpriteDataCache.HasDataChanged = true;

			return 1;
		}
    }
}

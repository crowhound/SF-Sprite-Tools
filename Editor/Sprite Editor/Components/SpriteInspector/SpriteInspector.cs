using UnityEditor;

using UnityEngine.UIElements;

using SF;
using UnityEngine;
using UnityEditor.UIElements;
namespace SFEditor.SpritesData
{
	[UxmlElement]
    public partial class SpriteInspector : VisualElement
    {
		/// TODO: Currently clicking on the inspector calls click events under it so clicking in a input field 
		/// actualls tries to change the currently selected sprite which can cause errors in focus out events.

		private SpriteDataCache _spriteDataCache;
		private VisualTreeAsset _spriteInspectorUXML;

		private Label _label;
		private TextField _nameField;
		private Vector4 _positionField;
		private BorderField _borderField;
		private EnumField _pivotAlignmentField;
		/// <summary>
		/// This value is used only to tells the editor if it needs to display the Pivot in a normalized value between 0 and 1 or the value in the pixels x and y of the pivot on the texture.
		/// </summary>
		private EnumField _pivotUnitModeField;
		private Vector2Field _pivotField;

		private StyleSheet _spriteStyles;
		public SpriteInspector()
		{
			Initialize();
		}

		public SpriteInspector(SpriteDataCache spriteDataCache) 
		{
			Initialize();
			_spriteDataCache = spriteDataCache;
		}

		private void Initialize()
		{
			_spriteInspectorUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.shatter-fantasy.sf-sprite-tools/Editor/Sprite Editor/Components/SpriteInspector/SpriteInspector.uxml");
			_spriteStyles = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.shatter-fantasy.sf-sprite-tools/Editor/UI Toolkit/Style Sheets/SFSpriteEditor.uss");

			_spriteInspectorUXML.CloneTree(this);
			AddToClassList("inspector-frame");
			styleSheets.Add(_spriteStyles);

			this.AddManipulator(new DragAndDropManipulator(this));

			_label = this.Q<Label>();
			_nameField = this.Q<TextField>("name-field");
			_borderField = this.Q<BorderField>("border-field");
			_pivotAlignmentField = this.Q<EnumField>("pivot-alignment-field");
			_pivotUnitModeField = this.Q<EnumField>("pivot-unit-field");
			_pivotField = this.Q<Vector2Field>("pivot-field");

			RegisterUICallbacks();
		}
		
		private void RegisterUICallbacks()
		{
			_nameField.RegisterValueChangedCallback((evt) =>
			{

				if(_spriteDataCache.LastSelectedSprite == null)
					return;

				if(_spriteDataCache.SelectedSpriteRects.Count > 1)
				{
					int index = 0;
					foreach(SpriteData data in _spriteDataCache.SelectedSpriteRects)
					{
						data.name = $"{evt.newValue} {index}";
						index++;
					}
				}
				else
					_spriteDataCache.LastSelectedSprite.name = evt.newValue;

			});
			_nameField.RegisterCallback<FocusOutEvent>((focus) =>
			{
				if(_spriteDataCache.LastSelectedSprite != null)
					_nameField.SetValueWithoutNotify(_spriteDataCache.LastSelectedSprite.name);
			});

			_pivotAlignmentField.RegisterValueChangedCallback((evt) =>
			{
				if(_spriteDataCache.LastSelectedSprite == null)
					return;

				if(_spriteDataCache.SelectedSpriteRects.Count > 1)
				{
					for(int index = 0; index < _spriteDataCache.SelectedSpriteRects.Count; index++)
					{
						_spriteDataCache.SelectedSpriteRects[index].alignment = (SpriteAlignment)evt.newValue;
					}
				}
				else
					_spriteDataCache.LastSelectedSprite.alignment = (SpriteAlignment)evt.newValue;
			});

			_pivotAlignmentField.RegisterCallback<FocusOutEvent>((focus) =>
			{
				if(_spriteDataCache.LastSelectedSprite != null)
					_pivotAlignmentField.SetValueWithoutNotify(_spriteDataCache.LastSelectedSprite.alignment);
			});


			_pivotField.RegisterValueChangedCallback((evt) =>
			{
				if(_spriteDataCache.LastSelectedSprite == null)
					return;

				if(_spriteDataCache.SelectedSpriteRects.Count > 1)
				{
					for(int index = 0; index < _spriteDataCache.SelectedSpriteRects.Count; index++)
					{
						_spriteDataCache.SelectedSpriteRects[index].pivot = evt.newValue;
						Debug.Log("Sprite Rect Pivot is: " + _spriteDataCache.SelectedSpriteRects[index].pivot);
					}
				}
				else
					_spriteDataCache.LastSelectedSprite.pivot = evt.newValue;
			});

			_pivotField.RegisterCallback<FocusOutEvent>((focus) =>
			{
				if(_spriteDataCache.LastSelectedSprite != null)
					_pivotField.SetValueWithoutNotify(_spriteDataCache.LastSelectedSprite.pivot);
			});
		}
		// TODO: Do a binding to a serialized object for better asset save/load support when changing values.
		// This might be avoidable with the SpriteEditorDataProvider functions that are built in.
		public void UpdateUIFields(SpriteData spriteData)
		{
			_label.text = $"Sprites Selected: {_spriteDataCache.SelectedSpriteRects.Count}";

			if(spriteData == null)
			{
				ClearUIFields();
				return;
			}

			_nameField.SetValueWithoutNotify(spriteData.name);
			this.Q<RectField>("position-field").value = spriteData.rect;
			this.Q<BorderField>("border-field").value = spriteData.border;
			_pivotAlignmentField.value = spriteData.alignment;

			// TODO: do if/else to see if this value needs normalized or kept in pixel xy values.
			_pivotField.value = spriteData.pivot;
		}

		public void ClearUIFields()
		{
			_nameField.SetValueWithoutNotify("");
			this.Q<RectField>("position-field").value = Rect.zero;
		}
    }
}

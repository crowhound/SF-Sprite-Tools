using System;
using System.Collections.Generic;
using System.Text;

using SF;

using SFEditor.SpritesData;
using SFEditor.SpritesData.Utilities;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace SFEditor
{
	[UxmlElement]
    public partial class SpriteAnimatorInspector : VisualElement
    {
		private SpriteDataCache _spriteDataCache;

		private VisualTreeAsset _spriteAnimationInspectorUXML;
		private const string _spriteAnimationInspectorAssetPath = "Packages/com.shatter-fantasy.sf-sprite-tools/Editor/Sprite Editor/Components/Sprite Animator Inspector/SpriteAnimatorInspector.uxml";
		private StyleSheet _spriteStyles;


		private Label _label;
		private TextField _animationClipNameField;
		private Button _animationClipButton;
		private UnsignedIntegerField _animationSPSField;
		private SFSpriteEditor _spriteEditor;

		private SFSpriteEditorSettings _spriteEditorSettings;
		public SpriteAnimatorInspector()
		{
			Initialize();
		}
		public SpriteAnimatorInspector(SFSpriteEditor spriteEditor) 
		{
			_spriteEditor = spriteEditor;
			_spriteDataCache = spriteEditor.SpriteDataCache;
			_spriteEditorSettings = SFSpriteEditorSettings.GetOrCreateSettings();

			Initialize();
		}

		private void Initialize()
		{
			_spriteAnimationInspectorUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_spriteAnimationInspectorAssetPath);

			if(_spriteAnimationInspectorUXML == null)
				return;

			_spriteAnimationInspectorUXML.CloneTree(this);

			_spriteStyles = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.shatter-fantasy.sf-sprite-tools/Editor/UI Toolkit/Style Sheets/SFSpriteEditor.uss");

			AddToClassList("animation-inspector-frame");
			AddToClassList("inspector-frame");

			styleSheets.Add(_spriteStyles);

			this.AddManipulator(new DragAndDropManipulator(this));

			InitializeUI();
		}

		private void InitializeUI()
		{
			_label = this.Q<Label>();
			_animationClipNameField = this.Q<TextField>("animation-clip-name--field");
			_animationClipButton = this.Q<Button>("animation-clip--button");
			_animationClipButton.clicked += CreateSpriteAnimationClip;
			_animationSPSField = this.Q<UnsignedIntegerField>("animation-sps--field");
            // TODO: The _spriteEditorSettings can be null if we haven't set opened up the project yet. We should fix that.
			if(_spriteEditorSettings != null)
					_animationSPSField.value = _spriteEditorSettings.SamplesPerSecond;
		}

		private void UpdateUI()
		{

		}

		public void CreateSpriteAnimationClip()
		{
			/* Creating procedural animation clip assets logic flow.
			 * 1. Create blank and give it a name.
			 * 2. Create a set of ObjectReferenceKeyframe via an array.
			 * 3. Inject the sprite or Object into the ObjectreferenceKeyFrames.
			 * 4. Create an EditorCurveBinding that takes ObjectReferenceKeyframe data/Object property types.
			 * 5. Bind them together using the AnimationUtility.SetObjectReferenceCurve();
			 * 6. Generate the Asset object and save to the AssetDatabase.
			*/

			var clip = new AnimationClip() { name = _animationClipNameField.value };

			if(string.IsNullOrEmpty(_animationClipNameField.value))
				clip.name = _spriteDataCache.LastSelectedSprite.name;
			
			List<Sprite> selectedSprites = new();
			selectedSprites = SFSpriteEditorUtilities.GetAllSpriteAssetsFromRects(_spriteEditor.SpriteDataProvider, _spriteDataCache.SelectedSpriteRects);

			if(!(selectedSprites.Count > 0)) return;

			ObjectReferenceKeyframe[] keys = new ObjectReferenceKeyframe[selectedSprites.Count];

			for(int i = 0; i < selectedSprites.Count; i++)
			{
				keys[i].time = (float)(i * (1f / _spriteEditorSettings.SamplesPerSecond));
				keys[i].value = selectedSprites[i];
			}

			EditorCurveBinding spriteBinding = new EditorCurveBinding();
			spriteBinding.type = typeof(SpriteRenderer);
			spriteBinding.propertyName = "m_Sprite";
			spriteBinding.path = "";
			clip.frameRate = _spriteEditorSettings.SamplesPerSecond;

			AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keys);
			StringBuilder sb = new StringBuilder();
			sb.Append(_spriteEditor.TextureAssetPath);
			sb.Replace($"{ _spriteEditor.TextureDataProvider.texture.name}.png", "");
			sb.Append($"{ clip.name}.anim");

			AssetDatabase.CreateAsset(clip, sb.ToString());
		}
	}
}
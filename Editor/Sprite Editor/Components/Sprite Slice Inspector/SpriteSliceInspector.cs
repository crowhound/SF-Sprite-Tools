using SF;

using SFEditor.SpritesData;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace SFEditor
{
	[UxmlElement]
    public partial class SpriteSliceInspector : VisualElement
    {
		private VisualTreeAsset _spliceInspectorUXML;
		private StyleSheet _spriteStyles;
		private SpriteSlicer _spriteSlicer;
		private SFSpriteEditor _spriteEditor;

		public SpriteSliceInspector(){ }
		public SpriteSliceInspector(SFSpriteEditor spriteEditor) 
		{
			_spriteEditor = spriteEditor;
			_spliceInspectorUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.shatter-fantasy.sf-sprite-tools/Editor/Sprite Editor/Components/Sprite Slice Inspector/SpriteSliceInspector.uxml");

			_spriteStyles = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.shatter-fantasy.sf-sprite-tools/Editor/UI Toolkit/Style Sheets/SFSpriteEditor.uss");

			if(_spliceInspectorUXML == null)
				return;

			_spliceInspectorUXML.CloneTree(this);
			AddToClassList("inspector-frame");
			styleSheets.Add(_spriteStyles);
			this.AddManipulator(new DragAndDropManipulator(this));
			Add(new Button(OnSliceSpritesClicked) { text = "Slice Sprites" });
		}

		private void OnSliceSpritesClicked()
		{
            if(_spriteSlicer == null)
                _spriteSlicer = new SpriteSlicer(_spriteEditor.Texture);


            _spriteSlicer.DoGridSlicing();
			foreach(Rect rect in _spriteSlicer.SlicedRects) 
			{
				// This is currently off by one for the x axis sometimes.
				Debug.Log(rect);
			}
        }
    }
}

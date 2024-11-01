using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SFEditor
{
	[UxmlElement]
    public partial class BorderField : VisualElement
    {
		public Vector4 value; // kept it lowercase to match Unity's default value naming convention.

		private VisualTreeAsset _borderFieldUXML; 
		public BorderField() 
		{
			_borderFieldUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.shatter-fantasy.sf-sprite-tools/Editor/Sprite Editor/Controls/Border Field/BorderField.uxml");

			_borderFieldUXML.CloneTree(this);
		}
    }
}

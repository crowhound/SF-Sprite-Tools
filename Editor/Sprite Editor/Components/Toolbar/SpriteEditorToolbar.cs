using System;

using SF;

using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace SFEditor.SpritesData.UIElements
{
	[UxmlElement]
    public partial class SpriteEditorToolbar : Toolbar
    {
		public SFSpriteEditor SpriteEditorWindow;

		private VisualTreeAsset _spriteToolbarUXML;
		private ToolbarMenu _spriteToolbarMenu;
		private VisualElement _toolButtonRow;

		protected ToolbarButton _applyToolbarButton;
		protected ToolbarButton _revertToolbarButton;
		protected ToolbarButton _debugToolbarButton;
		public VisualElement DebugInspector;
		private Toggle _canPanToggle;
		private Toggle _isPanningToggle;

		public SpriteEditorToolbar()
		{
			_spriteToolbarUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.shatter-fantasy.sf-sprite-tools/Editor/Sprite Editor/Components/Toolbar/SpriteEditorToolbar.uxml");

			if(_spriteToolbarUXML == null)
				return;

			_spriteToolbarUXML.CloneTree(this);
			_toolButtonRow = this.Q<VisualElement>("tool-button__row");
			_applyToolbarButton = this.Q<ToolbarButton>("apply-button");
			_applyToolbarButton.clicked += OnApplyButtonClicked;
			_revertToolbarButton = this.Q<ToolbarButton>("revert-button");
			_revertToolbarButton.clicked += OnRevertButtonClicked;
			_debugToolbarButton = this.Q<ToolbarButton>("debug-button");
			_debugToolbarButton.clicked += OnDebugClicked;
			InitToolbarMenu();
		}


        public SpriteEditorToolbar(SFSpriteEditor spriteEditorWindow) : base()
		{
			SpriteEditorWindow = spriteEditorWindow;
		}

		public void AddToolbarButton<T>(T toolbarButton) where T : ToolbarButton
		{
			_toolButtonRow.Add(toolbarButton);
		}
		public void RemoveToolbarButton<T>(T toolbarButton) where T : ToolbarButton
		{
			_toolButtonRow.Remove(toolbarButton);
		}


        private void InitDebugInspector()
        {
			DebugInspector = new VisualElement();
			DebugInspector.style.minWidth = 150;
			DebugInspector.style.minHeight = 150;

			_canPanToggle = new Toggle("Can Pan View") { name = "can-pan" };
			_isPanningToggle = new Toggle("Currently Pan View") { name = "is-panning" };
			DebugInspector.AddToClassList("inspector-frame");
			DebugInspector.AddManipulator(new DragAndDropManipulator(DebugInspector));
            DebugInspector.Add(_canPanToggle);
			DebugInspector.Add(_isPanningToggle);

        }

        private void OnDebugClicked()
		{
			SpriteEditorWindow.IsDebugging = !SpriteEditorWindow.IsDebugging;

            if(DebugInspector == null)
            {
				InitDebugInspector();
				SpriteEditorWindow.SpriteModuleContainer.Add(DebugInspector);
            }

            DebugInspector.visible = SpriteEditorWindow.IsDebugging;
		}

		protected virtual void InitToolbarMenu()
		{
			_spriteToolbarMenu = this.Q<ToolbarMenu>("Sprite Module Menu");
			if(_spriteToolbarMenu == null) return;

			_spriteToolbarMenu.menu.AppendAction("Sprite Inspector", (a) => { OpenSpriteFrameModule(new SFSpriteInspectorFrame(SpriteEditorWindow)); });
			_spriteToolbarMenu.menu.AppendAction("Sprite Outline", (a) => { OpenSpriteFrameModule(new SFSpriteOutlineFrame()); });
			_spriteToolbarMenu.menu.AppendAction("Sprite Physics Shape", (a) => { Debug.Log("Opening Sprite Physics Shape"); });
			_spriteToolbarMenu.menu.AppendAction("Sprite Secondary Texture", (a) => { Debug.Log("Opening Sprite Texture"); });
			_spriteToolbarMenu.menu.AppendAction("Sprite Skinning Editor", (a) => { Debug.Log("Opening Skinning Editor"); });
		}

		protected void OnApplyButtonClicked()
		{
			SpriteEditorWindow.ApplySpriteDataChanges();
		}

        private void OnRevertButtonClicked()
        {
            SpriteEditorWindow.RevertSpriteDataChanges();
        }

        protected void OpenSpriteFrameModule(ISpriteFrameModule spriteFrameModule)
		{
			if(SpriteEditorWindow == null)
				return;

			SpriteEditorWindow.OpenSpriteFrameModule.Invoke(spriteFrameModule);
		}
		protected void OpenSpriteOutlineFrameModule(ISpriteFrameModule spriteFrameModule)
		{
			if(SpriteEditorWindow == null)
				return;

			SpriteEditorWindow.OpenSpriteFrameModule.Invoke(spriteFrameModule);
		}
    }


    public class SFSpriteOutlineFrame : SpriteFrameModule
	{

		public override void CreateGUI()
		{
			
		}

		public override void DoMainGui()
		{
			
		}

		public override void LoadModule()
		{
			Debug.Log("The Sprite Out Line Frame has been opened");
		}

		public override void UnloadModule()
		{
			Debug.Log("The Sprite Out Line Frame has been closed");
		}
	}

}

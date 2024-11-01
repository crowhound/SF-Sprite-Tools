using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UIElements;
using Event = UnityEngine.Event;
using UnityEngine.InputSystem;

using SF.Utilities;
using SFEditor.Sprites;
using SFEditor.SpritesData.Utilities;
using SFEditor.UIElements;
using SFEditor.SpritesData.UIElements;
using SFEditor.UIElements.Utilities;

using SF.UIElements;
using UnityEditor.U2D.Sprites;
using SF.UIElements.Utilities;


namespace SFEditor.SpritesData
{
	/* This is the old class for the SF Sprite Editor View.
	*  The end of the file is where the name is different. Old one ends with View and the new one 
	*  ends with UIView. This file exists for reference only.
	*/

	// This is the SFSpriteEditorView file that contains the UI side of the SFSpriteEditor.
	//public class SFSpriteEditorView : EditorWindow
	//{
	//	public bool IsDebugging = false;
	//	protected bool _hasEditedValues = false;

	//	protected class Styles
	//	{
	//		public readonly GUIStyle preBackground = "preBackground"; // Darker background to help people's eyes using better color contrast.
	//	}
	//	protected Styles _styles;

	//	[SerializeField] protected VisualTreeAsset _spriteWindowUXML = default;
	//	protected const string SpriteWindowUXMLFilePath = "Packages/com.shatter-fantasy.sf-sprite-tools/Editor/Sprite Editor/Core Editor/SFSpriteEditorWindowBase.uxml";
	//	[SerializeField] protected StyleSheet _spriteWindowStyleSheet = default;
	//	protected const string SpriteWindowStyleSheetPath = "Packages/com.shatter-fantasy.sf-sprite-tools/Editor/UI Toolkit/Style Sheets/SFSpriteEditor.uss";
	//	[SerializeField] protected StyleSheet _commonStyleSheet = default;
	//	protected const string CommonStyleSheetPath = "Packages/shatter-fantasy.sf-ui-elements/Runtime/Styles/CommonUSS.uss";

	//	protected VisualElement _rootContainer;
	//	public VisualElement MainContainer;
	//	public VisualElement ScrollView;
	//	protected IMGUIContainer _IMGUIContainer;
	//	/// <summary>
	//	/// This is the UI container for features added onto the core SF Sprite Editor.
	//	/// </summary>
	//	/// <remarks> This allows adding UI views for custom features.</remarks>
	//	public VisualElement SpriteModuleContainer;
	//	public VisualElement OverlayContainer;

	//	public SpriteEditorToolbar SpriteEditorToolbar;
 //       private MouseRectDragManipulator _dragManipulator = new MouseRectDragManipulator(MouseButton.LeftMouse, EventModifiers.None);

 //       /// <summary>
 //       /// This is mainly used for debugging at the current momemtn.
 //       /// </summary>
 //       public Rect MouseClickDebugRect = new(100, 100, 20, 20);
	//	/// <summary>
	//	/// The unmodified View Rect for the container holding the texture preview rect.
	//	/// </summary>
	//	protected Rect _textureViewRect;
	//	public Rect TextureViewRect
	//	{
	//		get => _textureViewRect;
	//		set
	//		{
	//			_textureViewRect = value;
	//			/* Doing the below forces a recalculation of the zoom for the new value
	//			   because in ZoomLevel we call the GetMinZoom() function based on the new value. */
	//			ZoomLevel = _zoomLevel;
	//		}
	//	}
	//	/// <summary>
	//	/// The modified View Rect for the actual Sprite Sheet Texture inside of the texture preview frame.
	//	/// </summary>
	//	protected Rect _texturePreviewRect;

	//	/// <summary>
	//	/// The max sized rect for the texture preview rect space
	//	/// </summary>
	//	protected Rect MaxRectTextureSpace
	//	{
	//		get
	//		{
	//			float marginW = _textureViewRect.width * .5f / GetMinZoom();
	//			float marginH = _textureViewRect.height * .5f / GetMinZoom();
	//			float left = -marginW;
	//			float top = -marginH;
	//			float width = Texture.width + marginW * 2f;
	//			float height = Texture.height + marginH * 2f;
	//			return new Rect(left, top, width, height);
	//		}
	//	}

	//	/// TODO: Replace the current GUI Scroll bars with a UI Toolkit scroll maniuplator class.
	//	/// TODO: Fix the horizontal scroll bar from being buried under the bottom part of the texture view rect or it might be just being clipped.
	//	#region Scroll Bar Values
	//	/// <summary>
	//	///  The size of the scroll bar to calculate the view rect offset by.
	//	/// </summary>
	//	protected const float ScrollBarMargin = 16;
	//	protected Vector2 ScrollPosition;
	//	protected Vector2 ScrollPosition
	//	{
	//		get {return ScrollPosition; }
	//		set 
	//		{
	//			if(_zoomLevel < 0)
	//				_zoomLevel = GetMinZoom();

	//			ScrollPosition.x = Mathf.Clamp(value.x, MaxScrollRect.xMin, MaxScrollRect.xMax);
	//			ScrollPosition.y = Mathf.Clamp(value.y, MaxScrollRect.yMin, MaxScrollRect.yMax);
	//		}
	//	}

	//	protected Rect MaxScrollRect
	//	{
	//		get // Notes: Sprite Utility Window has a great example for how this is used and calculated.
	//		{
	//			float halfWidth = Texture.width * .5f * _zoomLevel;
	//			float halfHeight = Texture.height * .5f * _zoomLevel;
	//			return new Rect(-halfWidth, -halfHeight,
	//				_textureViewRect.width + halfWidth * 2f,
	//				_textureViewRect.height + halfHeight * 2f);
	//		}
	//	}
	//	#endregion

	//	// TODO: Clean these up and remove them entirely. We can now use UnityEvents properly.
	//	#region Input Booleans
	//	public bool WasMouseRightReleased => Mouse.current.rightButton.wasPressedThisFrame;
	//	public bool IsMouseRightPressed => Mouse.current.rightButton.isPressed;
	//	public bool IsMouseLeftPressed => Mouse.current.leftButton.isPressed;
	//	public bool IsLeftAltPressed => Keyboard.current.leftAltKey.isPressed;
	//	public bool WasAnyKeyReleased => Keyboard.current.anyKey.wasReleasedThisFrame;
	//	public bool WasAnyKeyPressed => Keyboard.current.anyKey.wasPressedThisFrame;
	//	#endregion

	//	// TODO: It might be a good idea to shove all of the Zoom value stuff into the Zoom manipulator.

	//	#region Zoom Values
	//	protected ViewRectZoomManipulator _zoomManipulator = new(MouseButton.RightMouse, EventModifiers.Alt);
	//	[SerializeField] protected float _zoomLevel = -1; // Unity m_Zoom
	//	protected float ZoomLevel // Unity zoomLevel
	//	{
	//		get { return _zoomLevel; }
	//		set { _zoomLevel = Mathf.Clamp(value, GetMinZoom(), MaxZoomLevel); }
	//	}
	//	protected const float MinZoomPercentage = 0.9f;
	//	protected const float MaxZoomLevel = 50f;
	//	protected const float WheelZoomSpeed = 0.03f;
	//	protected const float MouseZoomSpeed = 0.005f;

	//	/// <summary>
	//	/// Calculated value for the smallest possible zoom allowed in the current size window and texture loaded in.
	//	/// </summary>
	//	/// <returns></returns>
	//	protected float GetMinZoom()
	//	{
	//		if(Texture == null)
	//			return 1.0f;

	//		// Add k_MaxZoom size to min check to ensure that min zoom is smaller than max zoom
	//		return Mathf.Min(
	//			(_textureViewRect.width / Texture.width),
	//			(_textureViewRect.height / Texture.height),
	//			MaxZoomLevel) * MinZoomPercentage;
	//	}
	//	#endregion
	//	protected void CreateGUI()
	//	{
	//		_spriteWindowUXML = _spriteWindowUXML != null
	//			? _spriteWindowUXML
	//			: AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(SpriteWindowUXMLFilePath);

	//		if(_spriteWindowUXML == null)
	//		{
	//			Debug.LogError("There was no UXML file assigned to the SpriteWindowUXML variable in the SF Sprite Editor");
	//			return;
	//		}

	//		_spriteWindowStyleSheet = _spriteWindowStyleSheet != null
	//			? _spriteWindowStyleSheet
	//			: AssetDatabase.LoadAssetAtPath<StyleSheet>(SpriteWindowStyleSheetPath);

	//		if(_spriteWindowStyleSheet == null)
	//		{
	//			Debug.LogError("There was no Style Sheet file assigned to the Sprite Window StyleSheet variable in the SF Sprite Editor");
	//			return;
	//		}

	//		_commonStyleSheet = _commonStyleSheet != null
	//			? _commonStyleSheet
	//			: AssetDatabase.LoadAssetAtPath<StyleSheet>(CommonStyleSheetPath);

	//		_rootContainer = rootVisualElement;
	//		_spriteWindowUXML.CloneTree(_rootContainer);

	//		InitWindowView();
	//		InitToolbar();
	//		InitSpriteModuleViews();
	//		InitIMGUIContainer();
	//		RegisterUICallbacks();
	//		InitOverlay();

	//		_debugRect = new RectField("Drag Rect");
	//		SpriteEditorToolbar.Add(_debugRect);
	//		_debugRect.AddClass("row").AddClass("split");
 //           _IMGUIContainer.AddManipulator(_dragManipulator);

	//		OpenSpriteFrameModule?.Invoke(new SFSpriteInspectorFrame(this));
	//	}

	//	private RectField _debugRect;
 //       private Rect _mouseDragRect;

 //       private void InitIMGUIContainer()
	//	{
	//		_IMGUIContainer = new(OnIMGUIDraw);
	//		_IMGUIContainer.style.position = Position.Absolute;
	//		_IMGUIContainer.StretchToParentSize();
	//		_IMGUIContainer.RegisterCallback<MouseDownEvent>(OnMouseDownUI, TrickleDown.TrickleDown);
	//		_IMGUIContainer.AddManipulator(_zoomManipulator);
	//		_IMGUIContainer.cullingEnabled = true;
	//		_IMGUIContainer.style.flexGrow = 1;
	//		ScrollView.Add(_IMGUIContainer);
	//		ScrollView.style.overflow = Overflow.Hidden;
	//	}

	//	private void OnIMGUIDraw()
	//	{
	//		/* Order of Operations for viewport rect calculations
	//		 * 
	//		 * 1. SpriteEditorWindow has it's DoTextureAndModulesGUI from the IMGUIContainer control during DoGUI calls.
	//		 * 2. The UpdateAssetSelectionChange is called where the SpriteRect cache is rebuilt based on current selection.
	//		 * 3. Unity calculates for the first time the TextureViewRect using the IMGUIContainer layout for calculation values.
	//		 * 4. If a TextureDataProvider is actived and the current sprite frame module is not null Unity does the first of the Repaint checks to draw a box around the TextureViewRect.
	//		 * 5. Unity saves the old Matrix as a back up for viewport calculation restoring.
	//		 * 6. DoTextureGUI is called
	//		 */

	//		// This can happen when the OnGUI gets called the very first frame before a full CreateGUI is finished drawing.
	//		if(ScrollView == null)
	//			return;

	//		InitViewRect();
	//	}
	//	/// <summary>
	//	/// Initializes the view rect that is used to properly calculate the Gl draw command positions and orientations.
	//	/// </summary>
	//	/// <remarks>The file SpriteUtilityWindow has a method called DoTextureGUI. This has a lot of the calculations for stuff like calculating the proper sizing of the TextureViewRect.Also look into the method SetupHandlesMatrix in there for setting up the Matrix for proper position calculations. </remarks>
	//	private void InitViewRect()
	//	{
	//		TextureViewRect = new Rect(0f, 0f, _IMGUIContainer.layout.width - ScrollBarMargin, _IMGUIContainer.layout.height - ScrollBarMargin);

	//		// We keep track of the Handle Matrix before doing the Matrix TRS calculations that way we can restore it later.
	//		Matrix4x4 oldHandleMatrix = Handles.matrix;
	//		DoTextureGUI();
	//		// Restore the Handles Matrix now that we finish needing the Matrix TRS calculations
	//		Handles.matrix = oldHandleMatrix;
	//	}

	//	/// <summary>
	//	/// Use this to use the calculated Texture View Rect with proper scroll position and zoom calculations for any type of GUI needed to be drawn on top of the TexturePreviewRect viewport.
	//	/// </summary>
	//	/// <remarks>This is called near the end of the DoTextureGUI calls before handling zoom and panning changes.</remarks>
	//	private void DoTextureGUIExtras()
	//	{
	//		if(HandleSpriteSelection())
	//			OnHandleSpriteSelection();

	//		if(_spriteEditorDataProvider != null)
	//		{
	//			// TODO: Handle Frame Selected here.
	//			DrawSpriteRects();
	//		}

	//		if(CurrentSpriteFrameModule != null)
	//			CurrentSpriteFrameModule.DoMainGui();


	//		// Below is for Debugging the position with scroll, panning, and zoom taken into consideration.
	//		if(IsDebugging)
	//			DebugDrawIMGUI();
	//	}

	//	protected void DebugDrawIMGUI()
	//	{
	//		MouseClickDebugRect.position = Handles.inverseMatrix.MultiplyPoint(Event.current.mousePosition);
	//		GLUtilities.StartDrawing(Handles.matrix, Color.red, GL.LINES);
	//		GLUtilities.DrawBox(MouseClickDebugRect);
	//		GLUtilities.EndDrawing();
	//	}
		
	//	protected void DrawScreenspaceBackground()
	//	{
	//		_styles = new();
	//		if(Event.current.type == EventType.Repaint)
	//			_styles.preBackground.Draw(_textureViewRect, false, false, false, false);
	//	}

	//	#region Texture View Rect calculations
	//	private void DoTextureGUI()
	//	{
	//		if(Texture == null)
	//			return;

	//		if(_zoomLevel < 0f)
	//			_zoomLevel = GetMinZoom();

	//		// Texture rect in view space0
	//		// First divide the size by 2 to have the coordinates go from 0 to 1 to a -1 to 1 view calculation.
	//		// Than calculate the zoom to properly grab the new zoom levels.
	//		// Line 1 = X position; Line 2 = Y position; Line 3 = Width; Line 4 = Height
	//		_texturePreviewRect = new Rect(
	//			_textureViewRect.width / 2f - (Texture.width * _zoomLevel / 2f),
	//			_textureViewRect.height / 2f - (Texture.height * _zoomLevel / 2f),
	//			(Texture.width * _zoomLevel),
	//			(Texture.height * _zoomLevel)
	//			);


	//		// Set up the scroll bars first so we know how much to offset the texture view rect matrix.
	//		HandleScrollbars(); 
			
	//		// Calculate the texture view rect than normalizes it for handling drawing the sprite rects onto it. 
	//		// This also flips the texture screen space coordinates on the y to properly match the view coordinates.
	//		SetupHandlesMatrix();
			
	//		// No jokes this is just used in IMGUI Containers to draw a darker than default IMGUI background.
	//		DrawScreenspaceBackground();

	//		// Create a clip view for the sprite texture view inside the main editor when zooming and panning.
	//		GUI.BeginClip(_textureViewRect, -ScrollPosition,Vector2.zero,false);

	//		if(Event.current.type == EventType.Repaint)
	//		{
	//			// DrawTextureSpaceBackground();
	//			DrawTexture();
	//		}

	//		DoTextureGUIExtras();
	//		GUI.EndClip();

	//		HandleZoom();
	//		HandlePanning();
	//	}

	//	private void DrawTextureSpaceBackground()
	//	{
	//		float size = Mathf.Max(MaxRectTextureSpace.width, MaxRectTextureSpace.height);
	//		Vector2 offset = new Vector2(MaxRectTextureSpace.xMin, MaxRectTextureSpace.yMin);

	//		float halfSize = size * .5f;
	//		float alpha = EditorGUIUtility.isProSkin ? 0.15f : 0.08f;
	//		float gridSize = 8f;

	//		GLUtilities.StartDrawing(Handles.matrix, new Color(0f, 0f, 0f, alpha), GL.LINES);
	//		for(float v = 0; v <= size; v += gridSize)
	//			GLUtilities.DrawLine(new Vector2(-halfSize + v, halfSize + v) + offset, new Vector2(halfSize + v, -halfSize + v) + offset);
	//		GLUtilities.EndDrawing();
	//	}
	//	private void DrawTexture()
	//	{
	//		// This is the FUCKING GOD DAMN function to create a transparent checkerboard box with a texture drawn over it.
	//		// I stumbled across this on accident after giving up trying to find it for several months.
	//		EditorGUI.DrawTextureTransparent(_texturePreviewRect, Texture, ScaleMode.StretchToFill, 0, 0);
	//	}

	//	protected void HandleZoom()
	//	{
	//		//bool zoomMode = _zoomManipulator.IsZooming;
	//		bool isScrolling = Event.current.type == EventType.ScrollWheel;

	//		bool zoomMode = Event.current.alt && Event.current.button == 1;
	//		if(zoomMode)
	//		{
	//			EditorGUIUtility.AddCursorRect(_textureViewRect, MouseCursor.Zoom);
	//		}

	//		if(
	//			((Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown) && zoomMode) ||
	//			((Event.current.type == EventType.KeyUp || Event.current.type == EventType.KeyDown) && Event.current.keyCode == KeyCode.LeftAlt)
	//		)
	//		{
	//			Repaint();
	//		}

	//		if(Event.current.type == EventType.ScrollWheel || (Event.current.type == EventType.MouseDrag && Event.current.alt && Event.current.button == 1))
	//		{
	//			float zoomMultiplier = 1f - Event.current.delta.y * (Event.current.type == EventType.ScrollWheel ? WheelZoomSpeed : -MouseZoomSpeed);

	//			// Clamp zoom
	//			float wantedZoom = _zoomLevel * zoomMultiplier;

	//			float currentZoom = Mathf.Clamp(wantedZoom, GetMinZoom(), MaxZoomLevel);

	//			if(currentZoom != _zoomLevel)
	//			{
	//				_zoomLevel = currentZoom;

	//				// We need to fix zoomMultiplier if we clamped wantedZoom != currentZoom
	//				if(wantedZoom != currentZoom)
	//					zoomMultiplier /= wantedZoom / currentZoom;

	//				Vector3 textureHalfSize = new Vector2(Texture.width, Texture.height) * 0.5f;

	//				Vector3 mousePositionWorld = Handles.inverseMatrix.MultiplyPoint3x4(Event.current.mousePosition + ScrollPosition);

	//				Vector3 delta = (mousePositionWorld - textureHalfSize) * (zoomMultiplier - 1f);

	//				// Adds the current view rect position to scroll position to make scroll bar line up in view.
	//				ScrollPosition += (Vector2)Handles.matrix.MultiplyVector(delta);
	//				// Event.current.Use() prevents other events from controls to happen.
	//				Event.current.Use();
	//			}
	//		}
	//	}
	//	protected void HandlePanning()
	//	{
	//		// You can pan by holding ALT and using left button or NOT holding ALT and using right button. ALT + right is reserved for zooming.
	//		bool panMode = (!Event.current.alt && Event.current.button > 0 || Event.current.alt && Event.current.button <= 0);
	//		if(panMode && GUIUtility.hotControl == 0)
	//		{
	//			EditorGUIUtility.AddCursorRect(_textureViewRect, MouseCursor.Pan);

	//			if(Event.current.type == EventType.MouseDrag)
	//			{
	//				ScrollPosition -= Event.current.delta;
	//				Event.current.Use();
	//			}
	//		}

	//		//We need to repaint when entering or exiting the pan mode, so the mouse cursor gets refreshed.
	//		if(
	//			((Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown) && panMode) ||
	//			(Event.current.type == EventType.KeyUp || Event.current.type == EventType.KeyDown) && Event.current.keyCode == KeyCode.LeftAlt
	//		)
	//		{
	//			Repaint();
	//		}
	//	}
	//	// Sets up the scrollbar positions on the TextureView Rect. This is used to add an offset to the spire rects boxes drawn on the texture preview.
	//	protected void HandleScrollbars()
	//	{

	//		Rect horizontalScrollBarPosition = new Rect(_textureViewRect.xMin, _textureViewRect.yMax, _textureViewRect.width, ScrollBarMargin);
	//		ScrollPosition.x = GUI.HorizontalScrollbar(horizontalScrollBarPosition, ScrollPosition.x, _textureViewRect.width, MaxScrollRect.xMin, MaxScrollRect.xMax);

	//		Rect verticalScrollBarPosition = new Rect(_textureViewRect.xMax, _textureViewRect.yMin, ScrollBarMargin, _textureViewRect.height);
	//		ScrollPosition.y = GUI.VerticalScrollbar(verticalScrollBarPosition, ScrollPosition.y, _textureViewRect.height, MaxScrollRect.yMin, MaxScrollRect.yMax);
	//	}

	//	/// <summary>
	//	/// Sets up the handles of the Matrix for commands needing conversions for view and texture space.
	//	/// </summary>
	//	private void SetupHandlesMatrix()
	//	{
	//		// Offset from top left to center in view space
	//		Vector3 handlesPos = new Vector3(_texturePreviewRect.x, _texturePreviewRect.yMax, 0f);

	//		// We flip Y-scale because Unity texture space is bottom-up
	//		Vector3 handlesScale = new Vector3(ZoomLevel, -ZoomLevel, 1f);

	//		// Handle matrix is for converting between view and texture space coordinates, without taking account the scroll position.
	//		// Scroll position is added separately so we can use it with GUIClip.
	//		Handles.matrix = Matrix4x4.TRS(handlesPos, Quaternion.identity, handlesScale);
	//		//_rectDragManipulator.ViewRectMatrix = Handles.matrix;
	//	}

	//	private void DrawSpriteRects()
	//	{
	//		if(Event.current.type != EventType.Repaint)
	//			return;

	//		GLUtilities.ApplyHandleMaterial();

	//		GLUtilities.StartDrawing(Handles.matrix, SFSpriteEditorUtilities.SpriteFrameColor * 1.5f);
	//		foreach(var spriteData in SpriteDataCache.SpriteDataRects)
	//		{
	//			Handles.DrawWireCube(spriteData.rect.center, spriteData.rect.size);
	//			// GLUtilities.DrawBox(spriteData.rect);
	//		}

	//		if(SpriteDataCache.SelectedSpriteRects.Count > 0)
	//		{
	//			Handles.color = SFSpriteEditorUtilities.MultiSelectedSpriteFrameColor * 1.5f;
	//			// GL.Color(SFSpriteEditorUtilities.MultiSelectedSpriteFrameColor * 1.5f);
	//			foreach(var spriteData in SpriteDataCache.SelectedSpriteRects)
	//			{
	//				Handles.DrawWireCube(spriteData.rect.center, spriteData.rect.size);
	//			}
	//		}

	//		Handles.color = SFSpriteEditorUtilities.SelectedSpriteFrameColor * 1.5f;
	//		//GL.Color(SFSpriteEditorUtilities.SelectedSpriteFrameColor * 1.5f);
	//		if(SpriteDataCache.LastSelectedSprite != null)
	//			Handles.DrawWireCube(SpriteDataCache.LastSelectedSprite.rect.center, SpriteDataCache.LastSelectedSprite.rect.size);

	//		Handles.color = Color.cyan;
 //           _mouseDragRect = _dragManipulator.InversedDragRect;
 //           _debugRect.value = _mouseDragRect;

 //           Handles.DrawWireCube(_mouseDragRect.center, _mouseDragRect.size);

	//		GLUtilities.EndDrawing();
	//	}
	//	#endregion

	//	/// <summary>
	//	/// Initialize the UI Elements and adds them to the main container
	//	/// </summary>
	//	protected virtual void InitWindowView()
	//	{
	//		MainContainer = _rootContainer.Q<VisualElement>("main-container");
	//		ScrollView = _rootContainer.Q<VisualElement>("main-view");
	//		if(_commonStyleSheet != null)
	//			rootVisualElement.styleSheets.Add(_commonStyleSheet);
	//		if(_spriteWindowStyleSheet != null)
	//			rootVisualElement.styleSheets.Add(_spriteWindowStyleSheet);
	//	}

	//	protected virtual void InitToolbar()
	//	{
	//		SpriteEditorToolbar = rootVisualElement.Q<SpriteEditorToolbar>();
	//		SpriteEditorToolbar.SpriteEditorWindow = this;

	//		ToolbarButton resetZoomButton = 
	//			new ToolbarButton(ResetZoom) { text = "Reset Zoom" };

	//		SpriteEditorToolbar.Add(resetZoomButton);
	//	}

	//	protected virtual void InitOverlay()
	//	{
	//		OverlayContainer = _rootContainer.Q<VisualElement>("overlay-container");
	//	}

	//	protected virtual void InitSpriteModuleViews()
	//	{
	//		SpriteModuleContainer = _rootContainer.Q<VisualElement>("sprite-module-container");
	//	}

	//	protected virtual void RegisterUICallbacks()
	//	{
	//		OpenSpriteFrameModule += OnOpenSpriteFrameModule;
	//	}
	//	private void OnMouseDownUI(MouseDownEvent evt)
	//	{
	//		MouseClickDebugRect.position = evt.mousePosition;
	//	}

	//	protected virtual void UpdateUIOnSelectionChange()
	//	{
	//		if(ScrollView == null || Texture == null) return;


	//	}

	//	// TODO: Create a cached set of opened Sprite Modules to not have to reinit them everytime we switched between two different ones.
	//	protected virtual void OnOpenSpriteFrameModule(ISpriteFrameModule spriteFrameModule)
	//	{
	//		if(spriteFrameModule == null)
	//			return;

	//		if(CurrentSpriteFrameModule != null)
	//			CurrentSpriteFrameModule.UnloadModule();

	//		CurrentSpriteFrameModule = spriteFrameModule;
	//		CurrentSpriteFrameModule.LoadModule();
	//	}

	//	protected virtual void ResetZoom()
	//	{
	//		ZoomLevel = 1;
	//		ScrollPosition = Vector2.zero;
	//	}
	//}
}

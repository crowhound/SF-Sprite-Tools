using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

using SFEditor.UIElements.Utilities;
using SFEditor.SpritesData.UIElements;
using SF.Utilities;
using SFEditor.SpritesData.Utilities;
using System;
using UnityEditor.UIElements;
using SF.UIElements.Utilities;

namespace SFEditor.SpritesData
{
    public partial class SFSpriteEditor : EditorWindow
    {
        #region Visual Elements
        /* Main Window Elements */
        protected VisualElement _rootContainer;
        public VisualElement MainContainer;
        public ScrollView ScrollView;
        public VisualElement TextureViewContainer;

        private Painter2D _painter2d;
        private int _gridLineWidth = 3;
        private Vector2 _gridSize = new(16, 16);

        /* Sprite Module Elements*/
        /// <summary>
        /// This is the UI container for features added onto the core SF Sprite Editor.
        /// </summary>
        /// <remarks> This allows adding UI views for custom features.</remarks>
        public VisualElement SpriteModuleContainer;
        public VisualElement OverlayContainer;


        public SpriteEditorToolbar SpriteEditorToolbar;
        #endregion

        #region UI File Paths
        [SerializeField] protected VisualTreeAsset _spriteWindowUXML = default;
        protected const string SpriteWindowUXMLFilePath = "Packages/com.shatter-fantasy.sf-sprite-tools/Editor/Sprite Editor/Core Editor/SFSpriteEditorWindowBase.uxml";
        [SerializeField] protected StyleSheet _spriteWindowStyleSheet = default;
        protected const string SpriteWindowStyleSheetPath = "Packages/com.shatter-fantasy.sf-sprite-tools/Editor/UI Toolkit/Style Sheets/SFSpriteEditor.uss";
        [SerializeField] protected StyleSheet _commonStyleSheet = default;
        protected const string CommonStyleSheetPath = "Packages/shatter-fantasy.sf-ui-elements/Runtime/Styles/CommonUSS.uss";
        #endregion

        public SpriteEditorRectMode SpriteEditorRectMode;
        #region Mouse Drag 
        private Rect _mouseDragRect;

        private MouseRectDragManipulator _dragManipulator = new MouseRectDragManipulator(MouseButton.LeftMouse, 0.5f, EventModifiers.None);
        private MouseRectDragManipulator _panningManipulator = new MouseRectDragManipulator(MouseButton.LeftMouse, 0.5f, EventModifiers.Alt);
        #endregion

        protected Matrix4x4 _textureHandlesMatrix => SpriteDataCache.TextureHandlesMatrix;

        /// <summary>
		/// The unmodified View Rect for the container holding the texture preview rect.
		/// </summary>
        protected Rect _textureViewRect;
        protected Rect _texturePreviewRect;


        #region Scroll stuff to be moved to a scroll manipulator
        private Vector2 _scrollPosition;
        private Vector2 ScrollPosition
        {
            get { return _scrollPosition; }
            set 
            {
                value = new Vector2
                    (
                    Math.Clamp(value.x, 0, MaxScrollPosition.x),
                    Math.Clamp(value.y, 0, MaxScrollPosition.y)
                    );
                ScrollView.scrollOffset = value;
                _scrollPosition = value; 
            }
        }

        private Vector2 MaxScrollPosition
        {
            get 
            {
                Vector2 checkedValue = new Vector2(
                    ScrollView.layout.xMax - TextureViewContainer.layout.width / 2,
                    ScrollView.layout.yMax - TextureViewContainer.layout.height / 2);

                /*  When loading a new texture there is a chance the newly loaded texture size
                *   could make sutracting the TextureViewContainer return a negative value.
                *   This normally only happens if you have one texture already loaded and than are switching to a new
                *   texture with a vastly different width or height compared to the previous texture.
                */
                if(checkedValue.x < 0)
                    checkedValue = new(0, ScrollView.layout.yMax / 2);
                if(checkedValue.y < 0)
                    checkedValue = new(ScrollView.layout.xMax / 2, 0);

                return checkedValue;
            }
        }

        private Vector2 _scrollSpeed = new Vector2(0.5f,0.3f);
        #endregion

        #region Zoom Stuff to be moved to a zoom manipulator class.
        protected float _zoomLevel = 1.00f;
        public float ZoomLevel;
        #endregion

        public void CreateGUI()
        {
            InitializeUIAssets();

            _rootContainer = rootVisualElement;
            _rootContainer.style.overflow = Overflow.Hidden;

            InitWindowView();
            InitToolbar();
            RegisterUICallbacks();

            // TextureViewContainer.generateVisualContent += Draw;
            OpenSpriteFrameModule?.Invoke(new SFSpriteInspectorFrame(this));


        }
        /// <summary>
        /// NOT FULLY IMPLEMENTED YET.
        /// </summary>
        /// <param name="context"></param>
        private void Draw(MeshGenerationContext context)
        {
            _painter2d = context.painter2D;
            _painter2d.lineJoin = LineJoin.Miter;
            _painter2d.lineWidth = _gridLineWidth;
            _painter2d.lineCap = LineCap.Butt;
            _painter2d.strokeColor = Color.white;

            _painter2d.BeginPath();
            DrawFullGrid(context.visualElement, _painter2d);
            _painter2d.Stroke();
        }

        /// <summary>
        /// Draws a grid to fill the entire element up on the passed in Painter2D.  
        /// </summary>
        /// <param name="context"></param>
        private void DrawFullGrid(VisualElement gridCanvas, Painter2D painter2D)
        {
            Vector2 gridTiling = new Vector2
                (
                    Mathf.RoundToInt(gridCanvas.resolvedStyle.width / _gridSize.x),
                    Mathf.RoundToInt(gridCanvas.resolvedStyle.height / _gridSize.y)
                );

            // We do plus on in the for loops to make sure we get the last row and column of lines.
            for(int x = 0; x < gridTiling.x + 1; x++)
            {
                painter2D.MoveTo(new Vector2(x * _gridSize.x, 0));
                painter2D.LineTo(new Vector2(x * _gridSize.x, gridCanvas.resolvedStyle.height));
            }

            for(int y = 0; y < gridTiling.y + 1; y++)
            {
                painter2D.MoveTo(new Vector2(0, y * _gridSize.y));
                painter2D.LineTo(new Vector2(gridCanvas.resolvedStyle.width, y * _gridSize.y));
            }
        }


        protected virtual void InitWindowView()
        {
            MainContainer = _rootContainer.Q<VisualElement>("main-container");
            ScrollView = _rootContainer.Q<ScrollView>("main-view");
            if(_commonStyleSheet != null)
                rootVisualElement.styleSheets.Add(_commonStyleSheet);

            if(_spriteWindowStyleSheet != null)
                rootVisualElement.styleSheets.Add(_spriteWindowStyleSheet);

            SetUpTextureView();
            

            SpriteModuleContainer = _rootContainer.Q<VisualElement>("sprite-module-container");
            OverlayContainer = _rootContainer.Q<VisualElement>("overlay-container");
        }

        protected virtual void InitToolbar()
        {
            SpriteEditorToolbar = rootVisualElement.Q<SpriteEditorToolbar>();
            SpriteEditorToolbar.SpriteEditorWindow = this;

            ToolbarButton resetZoomButton =
                new ToolbarButton(ResetZoom) { text = "Reset Zoom" };

            SpriteEditorToolbar.Add(resetZoomButton);
        }

        protected virtual void RegisterUICallbacks()
        {
            OpenSpriteFrameModule += OnOpenSpriteFrameModule;
            TextureViewContainer.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void InitializeUIAssets()
        {
            _spriteWindowUXML = _spriteWindowUXML != null
                ? _spriteWindowUXML
                : AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(SpriteWindowUXMLFilePath);

            if(_spriteWindowUXML == null)
            {
                Debug.LogError("There was no UXML file assigned to the SpriteWindowUXML variable in the SF Sprite Editor");
                return;
            }

            _spriteWindowStyleSheet = _spriteWindowStyleSheet != null
                ? _spriteWindowStyleSheet
                : AssetDatabase.LoadAssetAtPath<StyleSheet>(SpriteWindowStyleSheetPath);

            if(_spriteWindowStyleSheet == null)
            {
                Debug.LogError("There was no Style Sheet file assigned to the Sprite Window StyleSheet variable in the SF Sprite Editor");
                return;
            }

            _commonStyleSheet = _commonStyleSheet != null
                ? _commonStyleSheet
                : AssetDatabase.LoadAssetAtPath<StyleSheet>(CommonStyleSheetPath);

            _rootContainer = rootVisualElement;
            _spriteWindowUXML.CloneTree(_rootContainer);
        }

        public void OnGUI()
        {
            if(TextureViewContainer == null || Texture == null)
                return;

            if(Event.current.type == EventType.Repaint)
            {
                //TestOnGUI();

                DoTextureGUI();
            }
        }

        /// <summary>
        /// Sets up the handles of the Matrix for commands needing conversions for view and texture space.
        /// </summary>
        private void SetupHandlesMatrix()
        {
            // We make the scroll world rect so we can take into consideration that the IMGUI containe's Unity uses for docks height value effect GUi and Handle matrix calculations if not taken into consideration when setting a custom Matrix using Matrix TRS. 
            Rect scrollWorldRect = new Rect(
                    ScrollView.layout.x, 
                    ScrollView.worldBound.y,
                    ScrollView.layout.width,
                    ScrollView.layout.height
                    );

            _zoomLevel = 1;

            _texturePreviewRect = new Rect(
                    TextureViewContainer.layout.width  - (Texture.width * _zoomLevel) + ScrollPosition.x,
                    TextureViewContainer.layout.height - (Texture.height * _zoomLevel) + scrollWorldRect.y + ScrollPosition.y, // We use world bound y 
                    (Texture.width * _zoomLevel),
                    (Texture.height * _zoomLevel)
                    );

            /*_texturePreviewRect = new Rect(
                    TextureViewContainer.layout.width / 2f - (Texture.width * _zoomLevel / 2f) + ScrollPosition.x,
                    TextureViewContainer.layout.height / 2f - (Texture.height * _zoomLevel / 2f) + ScrollPosition.y,
                    (Texture.width * _zoomLevel),
                    (Texture.height * _zoomLevel)
                    );
            */

            // Offset from top left to center in view space
            Vector3 handlesPos = new Vector3(_texturePreviewRect.x, _texturePreviewRect.yMax, 0f);

            // We flip Y-scale because Unity texture space is bottom-up
            Vector3 handlesScale = new Vector3(_zoomLevel, -_zoomLevel, 1f);

            // Handle matrix is for converting between view and texture space coordinates, without taking account the scroll position.
            // Scroll position is added separately so we can use it with GUIClip.
            Handles.matrix = Matrix4x4.TRS(handlesPos, Quaternion.identity, handlesScale);

            SpriteDataCache.TextureHandlesMatrix = Handles.matrix;
            _dragManipulator.HandlesMatrix = Handles.matrix;
        }

        private void SetUpTextureView()
        {

            TextureViewContainer = new();
            TextureViewContainer.name = "texture-view-container";
            TextureViewContainer.AddToClassList("texture-container");
            _dragManipulator.OnDragMoveHandler += OnDragMove;
            _dragManipulator.OnDragStartHandler += OnDragStart;
            _dragManipulator.OnDragEndHandler += OnDragEnd;
            TextureViewContainer.AddManipulator(_dragManipulator);
            _panningManipulator.OnDragMoveHandler += OnPanningMove;
            ScrollView.AddManipulator(_panningManipulator);

            ScrollView.Add(TextureViewContainer);


            // If there is no Texture already selected don't try and calculate a starting size/position of the textureViewContainer. Inside of the 
            if(Texture == null)
                return;

            SetTextureViewTransform();     
        }

        private void SetTextureViewTransform()
        {
            TextureViewContainer.style.width = new StyleLength(new Length(Texture.width, LengthUnit.Pixel));
            TextureViewContainer.style.maxWidth = new StyleLength(new Length(Texture.width, LengthUnit.Pixel));
            TextureViewContainer.style.height = new StyleLength(new Length(Texture.height, LengthUnit.Pixel));
            TextureViewContainer.style.maxHeight = new StyleLength(new Length(Texture.height, LengthUnit.Pixel));

            //TextureViewContainer.transform.scale = new Vector3(_zoomLevel, _zoomLevel, 1);
            TextureViewContainer.transform.position = new Vector3(0,0,0);
        }

        private void DoTextureGUI()
        {

            Matrix4x4 oldHandlesMatrix = Handles.matrix;

            // Calculate the texture view rect than normalizes it for handling drawing the sprite rects onto it. 
            // This also flips the texture screen space coordinates on the y to properly match the view coordinates.
            SetupHandlesMatrix();

            // Create a clip view for the sprite texture view inside the main editor when zooming and panning.
            Rect clipRect = new Rect(new Vector2(ScrollView.layout.x, -(ScrollView.layout.y)), ScrollView.layout.size);
            GUI.BeginClip(clipRect, Vector2.zero,Vector2.zero,false);
            // Creates the checkerboard background for the sprite editor
            EditorGUI.DrawTextureTransparent(_texturePreviewRect, Texture, ScaleMode.StretchToFill, 0, 0);
            DoTextureGUIExtras();
            GUI.EndClip();

            Handles.matrix = oldHandlesMatrix;
            
        }

        /// <summary>
        /// Use this to use the calculated Texture View Rect with proper scroll position and zoom calculations for any type of GUI needed to be drawn on top of the TexturePreviewRect viewport.
        /// </summary>
        /// <remarks>This is called near the end of the DoTextureGUI calls before handling zoom and panning changes.</remarks>
        private void DoTextureGUIExtras()
        {
            if(_spriteEditorDataProvider != null)
            {
                DrawSpriteRects();
            }

            if(CurrentSpriteFrameModule != null)
                CurrentSpriteFrameModule.DoMainGui();
        }
        #region Grid slicing functions I found inside of Unity's SpriteFrameModule class file
        /*public IEnumerable<Rect> GetGridRects(Vector2 size, Vector2 offset, Vector2 padding, bool keepEmptyRects)
        {
            var textureToUse = GetTextureToSlice();
            return InternalSpriteUtility.GenerateGridSpriteRectangles((UnityTexture2D)textureToUse, offset, size, padding, keepEmptyRects);
        }
        Texture2D GetTextureToSlice()
        {
            int width, height;
            GetTextureActualWidthAndHeight(out width, out height);
            var readableTexture = GetReadableTexture2D();
            if(readableTexture == null || (readableTexture.width == width && readableTexture.height == height))
                return readableTexture;

            if(m_TextureToSlice == null)
            {
                // we want to slice based on the original texture slice. Upscale the imported texture
                m_TextureToSlice = UnityEditor.SpriteUtility.CreateTemporaryDuplicate(readableTexture, width, height);
                m_TextureToSlice.hideFlags = HideFlags.HideAndDontSave;
            }

            return m_TextureToSlice;
        }
        */
        #endregion

        private void DrawSpriteRects()
        {
            if(Event.current.type != EventType.Repaint)
                return;

            GLUtilities.ApplyHandleMaterial();

            GLUtilities.StartDrawing(Handles.matrix, SFSpriteEditorUtilities.SpriteFrameColor * 1.5f);
            foreach(var spriteData in SpriteDataCache.SpriteDataRects)
            {
                Handles.DrawWireCube(spriteData.rect.center, spriteData.rect.size);
                // GLUtilities.DrawBox(spriteData.rect);
            }

            Handles.DrawWireCube(_mouseDragRect.center, _mouseDragRect.size);

            if(SpriteDataCache.SelectedSpriteRects.Count > 0)
            {
                Handles.color = SFSpriteEditorUtilities.MultiSelectedSpriteFrameColor * 1.5f;
                // GL.Color(SFSpriteEditorUtilities.MultiSelectedSpriteFrameColor * 1.5f);
                foreach(var spriteData in SpriteDataCache.SelectedSpriteRects)
                {
                    Handles.DrawWireCube(spriteData.rect.center, spriteData.rect.size);
                }
            }

            Handles.color = SFSpriteEditorUtilities.SelectedSpriteFrameColor * 1.5f;
            //GL.Color(SFSpriteEditorUtilities.SelectedSpriteFrameColor * 1.5f);
            if(SpriteDataCache.LastSelectedSprite != null)
                Handles.DrawWireCube(SpriteDataCache.LastSelectedSprite.rect.center, SpriteDataCache.LastSelectedSprite.rect.size);

            GLUtilities.EndDrawing();
        }
        
        /// <summary>
        /// If you need to do anything after all GUI steps are completed do it here.
        /// </summary>
        protected virtual void PostGUI()
        {

        }

        protected virtual void ResetZoom()
        {
            ZoomLevel = 1;
            ScrollPosition = new Vector2((ScrollView.layout.size.x / 2 - Texture.width / 2) ,0);
            ScrollView.scrollOffset = ScrollPosition;
            TextureViewContainer.transform.position = ScrollPosition;
        }

        /// <summary>
        /// Updates the UI elements used for views like TextureViewContainer when selected assets get changed 
        /// </summary>
        private void OnTextureChanged()
        {
            // This null happens when first loading up the SFEditor and the ISpriteEditorDataProviders are created before OnCreateGUI.
            if(TextureViewContainer == null)
                return;

            _zoomLevel = 1;
            ScrollPosition = Vector2.zero;
            ScrollView.scrollOffset = ScrollPosition;
            SetTextureViewTransform();
        }

        private void OnDragMove(Rect dragRect)
        {
            // This prevents drag a new rect over older rects. Might be useful for preventing over selection sometimes.
            //if(TrySelectSprite(_dragManipulator.DragPosition) != null)
            //  return;

            _mouseDragRect = _dragManipulator.InversedDragRect;

            Vector2 max = Vector2.zero;
            Vector2 min = Vector2.zero;

            max.x = _texturePreviewRect.width - _mouseDragRect.position.x;
            max.y = _texturePreviewRect.height - _mouseDragRect.position.y;
            min.x = -_mouseDragRect.position.x;
            min.y = -_mouseDragRect.position.y;
            _mouseDragRect.width = Math.Clamp(_mouseDragRect.width,min.x, max.x);
            _mouseDragRect.height = Math.Clamp(_mouseDragRect.height,min.y, max.y);
        }

        private void OnDragStart(Rect rect)
        {
           Vector2 mousePosition = _dragManipulator.StartingPosition;

           if(SpriteDataCache.TrySelectSprite(mousePosition) != null)
               _dragManipulator.StopDragging();
        }
        private void OnDragEnd(Rect rect)
        {
            // Don't let a sprite be made under a certain size to prevent mis click dragging creating sprites.
            if(rect.GetArea() < 6)
                return;

            _mouseDragRect = _dragManipulator.InversedDragRect;

            Vector2 max = Vector2.zero;
            Vector2 min = Vector2.zero;

            max.x = _texturePreviewRect.width - _mouseDragRect.position.x;
            max.y = _texturePreviewRect.height - _mouseDragRect.position.y;
            min.x = -_mouseDragRect.position.x;
            min.y = -_mouseDragRect.position.y;
            _mouseDragRect.width = Math.Clamp(_mouseDragRect.width, min.x, max.x);
            _mouseDragRect.height = Math.Clamp(_mouseDragRect.height, min.y, max.y);


            // Make sure the rect has no negative height or width when dragging down or backwards.
            rect = rect.AbsoluteRect();
            switch(SpriteEditorRectMode)
            {
                case SpriteEditorRectMode.AddSprite:
                    AddSpriteRectFromDrag(_dragManipulator.InversedDragRect);
                    break;
            }
        }
        private void OnPanningMove(Rect rect)
        {

            if(SpriteEditorToolbar.DebugInspector != null)
                SpriteEditorToolbar.DebugInspector.Q<Toggle>("is-panning").value = true;

            ScrollPosition += _panningManipulator.DeltaPosition * _scrollSpeed;
            TextureViewContainer.transform.position = ScrollPosition;
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if(SpriteDataCache.ShouldHandleSpriteSelection())
                OnHandleSpriteSelection();
        }
        protected virtual void OnOpenSpriteFrameModule(ISpriteFrameModule spriteFrameModule)
        {
            if(spriteFrameModule == null)
                return;

            if(CurrentSpriteFrameModule != null)
                CurrentSpriteFrameModule.UnloadModule();

            CurrentSpriteFrameModule = spriteFrameModule;
            CurrentSpriteFrameModule.LoadModule();
        }

    }
}

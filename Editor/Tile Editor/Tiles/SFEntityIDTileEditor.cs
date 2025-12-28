using UnityEditor;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.UIElements;

namespace SFEditor.TileModule
{
    /// <summary>
    /// Editor for SFEntityIDTile
    /// </summary>
    //[CustomEditor(typeof(SFEntityIDTile))]
    [CanEditMultipleObjects]
    public class SFEntityIdTileEditor : TileBaseEditor
    {
        private SerializedProperty _color;
        private SerializedProperty _colliderType;
        private SerializedProperty _sprite;
        private SerializedProperty _instancedGameObject;
        private SerializedProperty _flags;
        private SerializedProperty _transform;

        /*private SFEntityIDTile Tile
         {
         get {return (target as SFEntityIDTile);}
         }*/
        
        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }
        
        private static void DoTilePreview(Sprite sprite, Color color, Matrix4x4 transform)
        {
            
        }

        /// <summary>
        /// Generates a Texture2D showing a preview of the EntityIdTile
        /// </summary>
        /// <param name="assetPath">The asset to operate on.</param>
        /// <param name="subAssets">An array of all Assets at assetPath.</param>
        /// <param name="width">Width of the created texture.</param>
        /// <param name="height">Height of the created texture.</param>
        /// <returns>Texture2D showing a preview of the EntityIdTile</returns>
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }
        
    }
}

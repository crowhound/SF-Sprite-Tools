using UnityEngine;
using UnityEngine.UIElements;

namespace SFUIElements
{
    public static class UIElementsUtility
    {
		public static Vector2 GetTextureDisplaySize(VisualElement ve, Texture2D texture2D)
		{
			Vector2 result = Vector2.zero;
			if(texture2D != null)
			{
				result = new Vector2(texture2D.width, texture2D.height);
			}

			return result;
		}

		public static Vector2 GetTextureDisplaySize(VisualElement ve, Sprite sprite)
		{
			Vector2 result = Vector2.zero;
			if(sprite != null)
			{
				float num = PixelsPerUnitScaleForElement(ve, sprite);
				result = (Vector2)(sprite.bounds.size * sprite.pixelsPerUnit) * num;
			}

			return result;
		}

		public static float PixelsPerUnitScaleForElement(VisualElement ve, Sprite sprite)
		{
			if(ve == null || ve.panel == null || sprite == null)
				return 1f;

			float referenceSpritePixelsPerUnit = ve.panel.scaledPixelsPerPoint;
			float pixelsPerUnit = sprite.pixelsPerUnit;
			pixelsPerUnit = Mathf.Max(0.01f, pixelsPerUnit);
			return referenceSpritePixelsPerUnit / pixelsPerUnit;
		}
    }
}

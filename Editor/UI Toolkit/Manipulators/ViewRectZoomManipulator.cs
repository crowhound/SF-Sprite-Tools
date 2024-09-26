using System;

using UnityEngine;
using UnityEngine.UIElements;

namespace SF.UIElements
{
	public class ViewRectZoomManipulator : MouseManipulator
	{
		public bool IsZooming = false;

		public ViewRectZoomManipulator(MouseButton mouseButton = MouseButton.RightMouse, EventModifiers eventModifiers = EventModifiers.None)
		{
			activators.Add(new ManipulatorActivationFilter
			{
				button = mouseButton,
				modifiers = eventModifiers
			});
		}

		private void OnMouseDown(MouseDownEvent evt)
		{
			if(!CanStartManipulation(evt))
				return;
			IsZooming = true;
		}

		private void OnMouseUp(MouseUpEvent evt)
		{
			if(!CanStartManipulation(evt))
				return;
			IsZooming = false;
		}

		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<MouseDownEvent>(OnMouseDown);
			target.RegisterCallback<MouseUpEvent>(OnMouseUp);
		}


		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
			target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
		}

	}
}

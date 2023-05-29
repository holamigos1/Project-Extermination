using UnityEngine;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core.Data
{
	public struct CharAnimData
	{
		// Input
		public Vector2 deltaAimInput;
		public Vector2 totalAimInput;
		public Vector2 moveInput;
		public int     leanDirection;

		public void AddAimInput(Vector2 aimInput)
		{
			deltaAimInput = aimInput;
			totalAimInput += deltaAimInput;
			totalAimInput.x = Mathf.Clamp(totalAimInput.x, -90f, 90f);
			totalAimInput.y = Mathf.Clamp(totalAimInput.y, -90f, 90f);
		}

		public void SetAimInput(Vector2 aimInput)
		{
			totalAimInput.x = Mathf.Clamp(aimInput.x, -90f, 90f);
			totalAimInput.y = Mathf.Clamp(aimInput.y, -90f, 90f);
		}
	}
}
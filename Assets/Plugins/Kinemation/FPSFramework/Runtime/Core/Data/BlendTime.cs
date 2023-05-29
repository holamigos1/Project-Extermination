using System;
using UnityEngine;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core.Data
{
	[Serializable]
	public struct BlendTime
	{
		[Min(0f)] public float blendStart;
		[Min(0f)] public float blendEnd;

		public BlendTime(float startTime, float endTime)
		{
			blendStart = startTime;
			blendEnd = endTime;
		}

		public void Validate()
		{
			if (blendStart > blendEnd)
				(blendStart, blendEnd) = (blendEnd, blendStart);

			blendStart = blendStart < 0f ?
				0f :
				blendStart;
			blendEnd = blendEnd < 0f ?
				0f :
				blendEnd;
		}
	}
}
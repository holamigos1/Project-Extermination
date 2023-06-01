using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core.Data
{
	public struct MotionPlayer
	{
		private DynamicMotion _motion;

		public void Reset() =>
			_motion.Reset();

		public void Play(DynamicMotion motionToPlay)
		{
			DynamicMotion cache = _motion;
			_motion = motionToPlay;
			_motion.Reset();
			_motion.Play(ref cache);
		}

		public void UpdateMotion() =>
			_motion.UpdateMotion();

		public LocationAndRotation Get() =>
			_motion._currentMotionValues;
	}
}
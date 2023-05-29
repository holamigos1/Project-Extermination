using Plugins.Kinemation.FPSFramework.Runtime.Core.Delegates;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core.Data
{
	public struct AnimState
	{
		public ConditionDelegate CheckCondition;
		public PlayDelegate      OnPlay;
		public StopDelegate      OnStop;
	}
}
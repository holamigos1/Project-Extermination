namespace Plugins.Kinemation.FPSFramework.Runtime.Core.Data
{
	public struct StartRest
	{
		public StartRest(bool x, bool y, bool z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public bool x;
		public bool y;
		public bool z;
	}
}
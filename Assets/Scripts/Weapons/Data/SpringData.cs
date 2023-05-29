using System;

namespace Weapons.Data
{
	[Serializable]
	public struct SpringData
	{
		public                 float stiffness;
		public                 float criticalDamping;
		public                 float speed;
		public                 float mass;
		public                 float maxValue;
		[NonSerialized] public float error;
		[NonSerialized] public float velocity;

		public SpringData(float stiffness, float damping, float speed, float mass)
		{
			this.stiffness = stiffness;
			criticalDamping = damping;
			this.speed = speed;
			this.mass = mass;

			error = 0f;
			velocity = 0f;
			maxValue = 0f;
		}
        
		public SpringData(float stiffness, float damping, float speed)
		{
			this.stiffness = stiffness;
			criticalDamping = damping;
			this.speed = speed;
			mass = 1f;

			error = 0f;
			velocity = 0f;
			maxValue = 0f;
		}
	}
}
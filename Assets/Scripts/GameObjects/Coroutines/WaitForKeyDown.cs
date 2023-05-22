using Misc.Extensions;
using UnityEngine;

namespace GameObjects.Coroutines
{
	public sealed class WaitForKeyDown : ExtendedYieldInstruction
	{
		public KeyCode Key { get; set; }

		protected override bool Update()
		{
			return !Input.GetKeyDown(Key);
		}

		public ExtendedYieldInstruction Execute(KeyCode key)
		{
			Key = key;

			return base.Execute();
		}
	}
}
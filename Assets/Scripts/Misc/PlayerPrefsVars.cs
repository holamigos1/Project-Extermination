using UnityEngine;

namespace Misc
{
	/// <summary> Класс для быстрого внесения изменений в PlayerPrefs переменные. </summary>
	public static class PlayerPrefsVars
	{
		public static float GlobalMusicValue
		{
			get => PlayerPrefs.GetFloat(nameof(GlobalMusicValue));
			set => PlayerPrefs.SetFloat(nameof(GlobalMusicValue), value);
		}
		
		public static float GlobalSoundsValue
		{
			get => PlayerPrefs.GetFloat(nameof(GlobalSoundsValue));
			set => PlayerPrefs.SetFloat(nameof(GlobalSoundsValue), value);
		}
	}
}
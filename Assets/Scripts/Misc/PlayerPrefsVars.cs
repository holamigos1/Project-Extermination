using UnityEngine;

namespace Misc
{
	/// <summary> Класс для быстрого внесения изменений в PlayerPrefs переменные. </summary>
	public static class PlayerPrefsVars
	{
		public static float FPSFoVValue
		{
			get => PlayerPrefs.GetFloat(nameof(FPSFoVValue), 75f);
			set => PlayerPrefs.SetFloat(nameof(FPSFoVValue), value);
		}
		
		public static float UIScaleValue
		{
			get => PlayerPrefs.GetFloat(nameof(UIScaleValue), 1f);
			set => PlayerPrefs.SetFloat(nameof(UIScaleValue), value);
		}
		
		public static float UITransparencyValue
		{
			get => PlayerPrefs.GetFloat(nameof(UITransparencyValue), 1f);
			set => PlayerPrefs.SetFloat(nameof(UITransparencyValue), value);
		}
		
		public static float GlobalMusicValue
		{
			get => PlayerPrefs.GetFloat(nameof(GlobalMusicValue), 1f);
			set => PlayerPrefs.SetFloat(nameof(GlobalMusicValue), value);
		}
		
		public static float GlobalSoundsValue
		{
			get => PlayerPrefs.GetFloat(nameof(GlobalSoundsValue), 1f);
			set => PlayerPrefs.SetFloat(nameof(GlobalSoundsValue), value);
		}
	}
}
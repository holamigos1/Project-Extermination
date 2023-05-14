using System;
using Misc;
using UnityEngine;

namespace UserInterface
{
	public static class UIGod
	{
		static UIGod()
		{
			Debug.Log($"UIGod Application.isPlaying {Application.isPlaying}");
			Debug.Log($"UIGod Application.isFocused {Application.isFocused}");
			
			SheetOfAllGameUIs = FindUISheet();
		}


		
		public static int? UISheetInstanceID => SheetOfAllGameUIs == null ? 
												null : 
												SheetOfAllGameUIs.GetInstanceID();
		
		private const  string  UISheetFolder     = "Assets/Resources/Scriptable Data/UI/"; //TODO UIGod должен знать о том где хранится UISheet?
		private const  string  FullSheetSavePath = UISheetFolder + nameof(UISheet) + ".asset"; //TODO А если команда захочет поменять папки сохранения?
		private static readonly UISheet SheetOfAllGameUIs;

		public static bool TryGetCanvas<TGameCanvas>(out TGameCanvas desiredCanvas) 
			where TGameCanvas : GameCanvasBase
		{
			desiredCanvas = null;
			Debug.Log("TryGetCanvas");
			return false;
			desiredCanvas = SheetOfAllGameUIs.FindCanvas(desiredCanvas) as TGameCanvas;
			return false;
		}

		private static UISheet FindUISheet()
		{
			UISheet[] loadedAssetsArray = AssetDataBaseExtensions.LoadAssets<UISheet>();
			
			UISheet uiSheet = loadedAssetsArray.Length switch 
				{
					 1 => loadedAssetsArray[0],
					 0 => AssetDataBaseExtensions.SaveAsset<UISheet>(FullSheetSavePath.Split('/')),
					> 1 => throw new ArgumentOutOfRangeException(nameof(UISheet),
								$"{nameof(UISheet)} должен существовать в единственном экземпляре в проекте. Удалите лишний {nameof(UISheet)}!"),
				};

			return uiSheet;
		}
	}
}

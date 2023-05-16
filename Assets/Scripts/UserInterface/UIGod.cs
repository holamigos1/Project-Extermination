using System;
using Misc.Extensions;
using UnityEngine;

namespace UserInterface
{
	//решил оставить дохуя док.коментов тк к ней будут дохуя обращений
	public static class UIGod
	{
		static UIGod()
		{
			SheetOfAllGameUIs = FindUISheet();

			foreach (GameCanvasBase gameCanvas in SheetOfAllGameUIs.AllGameCanvases)
				if (gameCanvas == null)
					throw new NullReferenceException("В обозревателе игровых интерфейсов имеется пустой UI!");
		}
		
		public static int? UISheetInstanceID => SheetOfAllGameUIs == null ? 
												null : 
												SheetOfAllGameUIs.GetInstanceID();
		
		internal const  string  UISheetFolder     = "Assets/Resources/Scriptable Data/UI/"; //TODO UIGod должен знать о том где хранится UISheet?
		internal const  string  FullSheetSavePath = UISheetFolder + nameof(UISheet) + ".asset"; //TODO А если команда захочет поменять папки сохранения?
		
		internal static string GameCanvasesFolder = "Assets/Resources/Prefabs/UI";
		
		private static readonly UISheet SheetOfAllGameUIs;

		/// <summary> Делает попытку найти желаемое GameCanvasBase заданного типа.</summary>
		/// <param name="desiredCanvas">Выдаёт TGameCanvas заданного типа если он существует, иначе null.</param>
		/// <typeparam name="TGameCanvas">Объект типа GameCanvasBase.</typeparam>
		/// <seealso cref="GameCanvasBase"/>
		/// <returns>True если таковой GameCanvasBase существует и выводит ссылку на него через desiredCanvas параметр, а если нихрена не существует то вернёт False.</returns>
		public static bool TryGetCanvas<TGameCanvas>(out TGameCanvas desiredCanvas) 
			where TGameCanvas : GameCanvasBase
		{
			desiredCanvas = SheetOfAllGameUIs.FindCanvas(typeof(TGameCanvas)) as TGameCanvas;
			return desiredCanvas != null;
		}
		
		public static bool TryAddCanvas(GameCanvasBase gameCanvasReference) =>
			SheetOfAllGameUIs.FindCanvas(gameCanvasReference.GetType()) != null;

		internal static UISheet FindUISheet()
		{
			UISheet[] loadedAssetsArray = AssetDataBaseExtensions.LoadAssets<UISheet>();
			
			UISheet uiSheet = loadedAssetsArray.Length switch 
				{
					 1 => loadedAssetsArray[Index.Start],
				   <= 0 => AssetDataBaseExtensions.CreateAsset<UISheet>(FullSheetSavePath.Split('/')),
					> 1 => throw new ArgumentOutOfRangeException(nameof(UISheet),
								$"{nameof(UISheet)} должен существовать в единственном экземпляре в проекте. Удалите лишний {nameof(UISheet)}!"),
				};

			return uiSheet;
		}
	}
}

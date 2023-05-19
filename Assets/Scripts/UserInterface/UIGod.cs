using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Misc.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UserInterface
{
	//решил оставить дохуя док.коментов тк к ней будут дохуя обращений
	public static class UIGod
	{
		static UIGod()
		{
			SheetOfAllGameUIs = FindUISheet();
			s_gameCanvasInstancesCache = new SortedList<IComparable<string>, GameCanvasBase>(20);

			SceneManager.activeSceneChanged += (arg0, scene) =>  
				s_gameCanvasInstancesCache = new SortedList<IComparable<string>, GameCanvasBase>(20);
			
			foreach (GameCanvasBase gameCanvas in SheetOfAllGameUIs.AllGameCanvases)
				if (gameCanvas == null)
					throw new NullReferenceException("В обозревателе игровых интерфейсов имеется пустой UI!");
		}
		
		public static int? UISheetInstanceID => SheetOfAllGameUIs == null ? 
												null : 
												SheetOfAllGameUIs.GetInstanceID();
		
		internal const  string  UI_SHEET_FOLDER     = "Assets/Resources/Scriptable Data/UI/"; //TODO UIGod должен знать о том где хранится UISheet?
		internal const  string  FULL_SHEET_SAVE_PATH = UI_SHEET_FOLDER + nameof(UISheet) + ".asset"; //TODO А если команда захочет поменять папки сохранения?

		public static string GameCanvasesFolder = "Assets/Resources/Prefabs/UI";

		private const string UIParentName = "UI";

		private static Transform s_uiParentInstance;
		
		private static readonly UISheet SheetOfAllGameUIs;

		private static SortedList<IComparable<string>, GameCanvasBase> s_gameCanvasInstancesCache;

		// ReSharper disable Unity.PerformanceAnalysis
		/// <summary> Делает попытку найти желаемое GameCanvasBase заданного типа.</summary>
		/// <param name="desiredCanvas">prefab TGameCanvas заданного типа если он существует, иначе null.</param>
		/// <typeparam name="TGameCanvas">Объект типа GameCanvasBase.</typeparam>
		/// <seealso cref="GameCanvasBase"/>
		/// <returns>True если таковой GameCanvasBase существует и выводит ссылку на него через desiredCanvas параметр, а если нихрена не существует то вернёт False.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryGetCanvas<TGameCanvas>(out TGameCanvas desiredCanvas) 
			where TGameCanvas : GameCanvasBase
		{
			desiredCanvas = SheetOfAllGameUIs.FindCanvas(typeof(TGameCanvas)) as TGameCanvas;
			
			bool isFounded = desiredCanvas;

			if(!isFounded)
				Debug.LogWarning($"Canvas {typeof(TGameCanvas).Name} не нашёлся! Добавь его в список UISheet.");
			
			return isFounded;
		}

		/// <summary> Возвращает уже созданный на текущей сцене клон префаба TGameCanvas </summary>
		/// <typeparam name="TGameCanvas">Объект типа GameCanvasBase.</typeparam>
		/// <seealso cref="GameCanvasBase"/>
		/// <returns>Возвращает ссылку на созданный через Instantiate клон префаба типа TGameCanvas.
		/// Если заданный TGameCanvas не существует для UIGod то вернёт null.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TGameCanvas GetCanvasInstance<TGameCanvas>()
			where TGameCanvas : GameCanvasBase
		{
			if (s_gameCanvasInstancesCache.TryGetValue(typeof(TGameCanvas).Name, out GameCanvasBase canvasInstance))
				if (canvasInstance) 
					return canvasInstance as TGameCanvas;

			TGameCanvas desiredCanvasInstance = TryGetCanvas(out TGameCanvas prefabricatedCanvas) ?
				UnityEngine.Object.Instantiate(prefabricatedCanvas, GetUIParentInstance()) :
				null;

			if(desiredCanvasInstance) 
				s_gameCanvasInstancesCache[desiredCanvasInstance.GetType().Name] = desiredCanvasInstance;

			return desiredCanvasInstance;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Transform GetUIParentInstance()
		{
			if (s_uiParentInstance != null)
				return s_uiParentInstance;

			s_uiParentInstance = new GameObject(UIParentName).transform;

			return s_uiParentInstance;
		}

		internal static UISheet FindUISheet()
		{
			UISheet[] loadedAssetsArray = AssetDataBaseExtensions.LoadAssets<UISheet>();
			
			UISheet uiSheet = loadedAssetsArray.Length switch 
				{
					 1 => loadedAssetsArray[Index.Start],
				   <= 0 => AssetDataBaseExtensions.CreateAsset<UISheet>(FULL_SHEET_SAVE_PATH.Split('/')),
					> 1 => throw new ArgumentOutOfRangeException(nameof(UISheet),
								$"{nameof(UISheet)} должен существовать в единственном экземпляре в проекте. Удалите лишний {nameof(UISheet)}!"),
				};

			return uiSheet;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Misc.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UserInterface
{
	/// <summary> Класс хранения и выдачи GameCanvasBase UI префабов. </summary>
	/// <seealso cref="GameCanvasBase"/>
	public static class UIGod
	{
		static UIGod()
		{
			s_SheetOfAllGameUIs = UISheetInstance;

			OnActiveSceneChanged(new Scene(), new Scene());
			SceneManager.activeSceneChanged += OnActiveSceneChanged;
			
			foreach (GameCanvasBase gameCanvas in s_SheetOfAllGameUIs.AllGameCanvases)
				if (gameCanvas == null)
					throw new NullReferenceException("В обозревателе игровых интерфейсов имеется пустой UI!");
		}

		public static Transform UIParentInstance => 
								s_uiParentInstance ? 
								s_uiParentInstance :
								s_uiParentInstance = new GameObject(UI_Layer_Name) 
														{ tag = "GameController", 
														layer = LayerMask.NameToLayer(UI_Layer_Name) }
														.transform;

		
		public static int? UISheetInstanceID => s_SheetOfAllGameUIs ? 
												s_SheetOfAllGameUIs.GetInstanceID() : 
												null;

		internal static UISheet UISheetInstance
		{
			get
			{
				if (s_SheetOfAllGameUIs)
					return s_SheetOfAllGameUIs;
				
				UISheet[] loadedAssetsArray = AssetDataBaseExtensions.LoadAssets<UISheet>();
			
				UISheet uiSheet = loadedAssetsArray.Length switch 
				{
					1 => loadedAssetsArray[Index.Start],
					<= 0 => AssetDataBaseExtensions.CreateAsset<UISheet>(Full_Sheet_Save_Path.Split('/')),
					var _ => throw new ArgumentOutOfRangeException(nameof(UISheet),
																   $"{nameof(UISheet)} должен существовать в единственном экземпляре в проекте. Удалите лишний {nameof(UISheet)}!"),
				};

				return uiSheet;
			}
		}

		//TODO Убрать const в объект конфигурации
		private  const int CanvasInstancesCacheSize   = 20;
		private  const string UI_Layer_Name           = "UI";
		private  const string UI_Sheet_Default_Folder = "Assets/Resources/Scriptable Data/UI/"; //TODO UIGod должен знать о том где хранится UISheet?
		private  const string Full_Sheet_Save_Path    = UI_Sheet_Default_Folder + nameof(UISheet) + ".asset"; //TODO А если команда захочет поменять папки сохранения?
		internal const string GAME_CANVASES_FOLDER    = "Assets/Resources/Prefabs/UI";

		private static Transform s_uiParentInstance;
		
		private static readonly UISheet s_SheetOfAllGameUIs;

		private static SortedList<IComparable<string>, GameCanvasBase> s_gameCanvasInstancesCache;

		/// <summary> Создаёт на текущей сцене клон префаба TGameCanvas если его нету на сцене. </summary>
		/// <typeparam name="TGameCanvas">Объект типа GameCanvasBase.</typeparam>
		/// <seealso cref="GameCanvasBase"/>
		/// <returns>Возвращает ссылку на клон префаба типа TGameCanvas имеющийся на сцене. <br/>
		/// Если заданного TGameCanvas не существует для UIGod то вернёт null.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TGameCanvas GetCanvasInstance<TGameCanvas>()
			where TGameCanvas : GameCanvasBase
		{
			if (s_gameCanvasInstancesCache.TryGetValue(typeof(TGameCanvas).Name, out GameCanvasBase canvasInstance))
				if (canvasInstance) 
					return canvasInstance as TGameCanvas;

			TGameCanvas desiredCanvasInstance = TryGetCanvas(out TGameCanvas prefabricatedCanvas) ?
				UnityEngine.Object.Instantiate(prefabricatedCanvas, UIParentInstance) :
				null;

			if(desiredCanvasInstance) 
				s_gameCanvasInstancesCache[desiredCanvasInstance.GetType().Name] = desiredCanvasInstance;

			return desiredCanvasInstance;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryGetCanvas<TGameCanvas>(out TGameCanvas desiredCanvas) 
			where TGameCanvas : GameCanvasBase
		{
			desiredCanvas = s_SheetOfAllGameUIs.FindCanvas(typeof(TGameCanvas)) as TGameCanvas;
			
			bool isFounded = desiredCanvas;

			if(!isFounded)
				Debug.LogWarning($"Canvas {typeof(TGameCanvas).Name} не нашёлся! Добавь его в список UISheet.");
			
			return isFounded;
		}

		private static void OnActiveSceneChanged(Scene previousScene, Scene loadedScene)
		{
			s_gameCanvasInstancesCache = new SortedList<IComparable<string>, GameCanvasBase>(CanvasInstancesCacheSize);
		}
	}
}

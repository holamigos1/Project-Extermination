using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UserInterface;
using UserInterface.GameUIs;

namespace DefaultNamespace
{
	public class realezator : MonoBehaviour
	{
		[SerializeField] 
		protected List<GameCanvasBase> AllGameCanvases = new List<GameCanvasBase>();
		
		private void Start()
		{
			if (UIGod.TryGetCanvas(out HUDCanvas canvas))
				Instantiate(canvas);
		}
	}
}
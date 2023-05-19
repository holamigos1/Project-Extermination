using System;
using System.Collections.Generic;
using UnityEngine;
using UserInterface;
using UserInterface.GameUIs;

namespace DefaultNamespace
{
	public class realezator : MonoBehaviour
	{
		private void Start()
		{
			HUDCanvas canvas = null;
			HUDCanvas canvasinst = UIGod.GetCanvasInstance<HUDCanvas>();
			
			if(canvas) 
				Debug.Log("canvas");
			
			bool sa = canvasinst;
			
			if(sa) 
				Debug.Log("canvasinst");

			canvasinst = null;
			sa = canvasinst;
			
			if(canvasinst) 
				Debug.Log("canvasinst");

			if(sa)
				Debug.Log("SADAD");
		}
	}
}
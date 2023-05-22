using System;
using System.Collections;
using System.Collections.Generic;
using GameObjects.Coroutines;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UserInterface;
using UserInterface.GameUIs;

namespace DefaultNamespace
{
	public class realezator : MonoBehaviour
	{
		public void Start()
		{
			var waitForKeyDown = new WaitForKeyDown();
			waitForKeyDown.Execute(KeyCode.Mouse0).Done += instruction => Debug.Log("dsad");
		}

		private void Reset()
		{
			
		}
	}
}


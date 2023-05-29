// Designed by Kinemation, 2023

using Plugins.Kinemation.FPSFramework.Runtime.Layers;
using UnityEditor;
using UnityEngine;

namespace Plugins.Kinemation.FPSFramework.Editor.Layers
{
	[CustomEditor(typeof(BlendingLayer), true)]
	public class BlendingLayerEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var layer = (BlendingLayer)target;

			if (GUILayout.Button("To Mesh Space Rot"))
				layer.EvaluateSpineMS();
		}
	}
}
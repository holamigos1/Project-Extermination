// Designed by Kinemation, 2023

using System;
using System.Collections.Generic;
using System.Reflection;
using Plugins.Kinemation.FPSFramework.Runtime.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Plugins.Kinemation.FPSFramework.Editor.Core
{
	[CustomEditor(typeof(CoreAnimComponent), true)]
	public class CoreAnimComponentEditor : UnityEditor.Editor
	{
		// Inspector of the currently selected anim layer
		private UnityEditor.Editor _layerEditor;

		// Interactable Anim Layers
		private ReorderableList _layersReorderable;

		// Collection of all classes derived from Anim Layer
		private List<Type>        _layerTypes;
		private CoreAnimComponent _owner;
		private int               _selectedLayer = -1;

		private void OnEnable()
		{
			_owner = (CoreAnimComponent)target;

			_layersReorderable = new ReorderableList(serializedObject, serializedObject.FindProperty("animLayers"),
													true, true, true, true);

			_layersReorderable.drawHeaderCallback += DrawHeader;
			_layersReorderable.drawElementCallback += DrawElement;
			_layersReorderable.onSelectCallback += OnSelectElement;
			_layersReorderable.onRemoveCallback += OnRemoveCallback;
			_layersReorderable.onAddCallback += OnAddCallback;

			// Used to hide layers which reside in the Core component
			HideRegisteredLayers();
			EditorUtility.SetDirty(target);
			Repaint();
		}

		// Draws a header of the reordererable list
		private void DrawHeader(Rect rect)
		{
			GUI.Label(rect, "Layers");

			if (Event.current.type == EventType.MouseDown
				&& Event.current.button == 1
				&& rect.Contains(Event.current.mousePosition))
			{
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("Copy layers"), false, CopyList);
				menu.AddItem(new GUIContent("Paste layers"), false, PasteList);
				menu.ShowAsContext();
				Event.current.Use();
			}
		}

		// Draws an element of the re-order-able list
		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty element = _layersReorderable.serializedProperty.GetArrayElementAtIndex(index);
			if (element.objectReferenceValue == null)
			{
				GUI.Label(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
						  "Invalid layer");
				return;
			}

			Type type = element.objectReferenceValue.GetType();
			rect.y += 2;
			GUI.Label(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), type.Name);

			if (index == _selectedLayer
				&& Event.current.type == EventType.MouseUp
				&& Event.current.button == 1
				&& rect.Contains(Event.current.mousePosition))
			{
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("Copy"), false, CopyLayerValues);
				menu.AddItem(new GUIContent("Paste"), false, PasteLayerValues);
				menu.ShowAsContext();
				Event.current.Use();
			}
		}

		// Called when layer is selected
		private void OnSelectElement(ReorderableList list)
		{
			//Debug.Log("Selected element: " + list.index);
			_selectedLayer = list.index;

			AnimLayer displayedComponent = _owner.GetLayer(_selectedLayer);
			if (displayedComponent == null)
				return;

			_layerEditor = CreateEditor(displayedComponent);
		}

		private void OnAddCallback(ReorderableList list)
		{
			_layerTypes = GetSubClasses<AnimLayer>();

			var menuOptions = new GUIContent[_layerTypes.Count];

			for (var i = 0; i < _layerTypes.Count; i++)
				menuOptions[i] = new GUIContent(_layerTypes[i].Name);

			EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.zero), menuOptions,
											-1, OnMenuOptionSelected, null);
		}

		private void OnRemoveCallback(ReorderableList list)
		{
			//Debug.Log("Removed element from list");
			_layerEditor = null;
			_owner.RemoveLayer(list.index);
		}

		private void OnMenuOptionSelected(object userData, string[] options, int selected)
		{
			//Debug.Log("Selected menu option: " + options[selected]);

			Type selectedType = _layerTypes[selected];

			if (!_owner.IsLayerUnique(selectedType))
				return;

			// Add item class based on the selected index
			Component newLayer = _owner.transform.gameObject.AddComponent(selectedType);

			// Hide newly created item in the inspector
			newLayer.hideFlags = HideFlags.HideInInspector;
			_owner.AddLayer((AnimLayer)newLayer);

			EditorUtility.SetDirty(target);
			Repaint();
		}

		private void RenderAnimButtons()
		{
			if (GUILayout.Button(new GUIContent("Setup rig", "Will find and assign bones")))
				_owner.SetupBones();

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Preview animation"))
				_owner.EnableEditorPreview();

			if (GUILayout.Button("Reset pose"))
				_owner.DisableEditorPreview();

			GUILayout.EndHorizontal();
		}

		// Renders the inspector of currently selected Layer
		private void RenderItem()
		{
			if (_layerEditor == null)
				return;

			EditorGUILayout.LabelField("Animation Layer", EditorStyles.boldLabel);
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			Color oldColor = GUI.backgroundColor;
			GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f);

			// Display the Inspector for a component that is a member of the component being edited
			_layerEditor.OnInspectorGUI();

			EditorGUILayout.EndVertical();

			// Reset the background color
			GUI.backgroundColor = oldColor;
		}

		// Hides layers which reside in CoreAnimComponent
		private void HideRegisteredLayers()
		{
			var foundLayers = _owner.gameObject.GetComponentsInChildren<AnimLayer>();

			foreach (AnimLayer layer in foundLayers)
				if (_owner.HasA(layer))
					layer.hideFlags = HideFlags.HideInInspector;
			EditorUtility.SetDirty(target);
			Repaint();
		}

		private static List<Type> GetSubClasses<T>()
		{
			var subClasses = new List<Type>();
			var assembly = Assembly.GetAssembly(typeof(T));
			foreach (Type type in assembly.GetTypes())
				if (type.IsSubclassOf(typeof(T)))
					subClasses.Add(type);
			return subClasses;
		}

		private void RenderLayerHelpers()
		{
			GUISkin skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
			// Get a reference to the "MiniButton" style from the skin
			GUIStyle miniButtonStyle = skin.FindStyle("MiniButton");

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var register = new GUIContent(" Register layers", "Will find and hide layers. "
															  + "Unregistered layers will be added to the Core Component");

			if (GUILayout.Button(register, miniButtonStyle, GUILayout.ExpandWidth(false)))
			{
				var foundLayers = _owner.gameObject.GetComponentsInChildren<AnimLayer>();

				foreach (AnimLayer layer in foundLayers)
					if (_owner.IsLayerUnique(layer.GetType()))
					{
						// Add new ones
						layer.hideFlags = HideFlags.HideInInspector;
						_owner.AddLayer(layer);
					}
					else
					{
						if (!_owner.HasA(layer))
							// Destroy other "clone" components
							DestroyImmediate(layer);
						else
							// Hide already exisiting
							layer.hideFlags = HideFlags.HideInInspector;
					}

				EditorUtility.SetDirty(target);
				Repaint();
			}

			var collapse = new GUIContent(" Collapse", "Will deselect the layer");
			if (GUILayout.Button(collapse, miniButtonStyle, GUILayout.ExpandWidth(false)))
			{
				_layerEditor = null;
				_layersReorderable.ClearSelection();
			}

			GUILayout.EndHorizontal();
		}

		private void PasteLayerValues()
		{
			// Check if buffer is valid
			string serializedData = EditorGUIUtility.systemCopyBuffer;
			if (string.IsNullOrEmpty(serializedData))
				return;

			AnimLayer selectedItem = _owner.GetLayer(_selectedLayer);
			EditorJsonUtility.FromJsonOverwrite(serializedData, selectedItem);
		}

		private void CopyLayerValues() =>
			EditorGUIUtility.systemCopyBuffer = EditorJsonUtility.ToJson(_owner.GetLayer(_selectedLayer));

		private void CopyList()
		{
			var jsonString = "";
			for (var i = 0; i < _layersReorderable.count; i++)
			{
				AnimLayer layerToEncode = _owner.GetLayer(i);
				// Serialize each layer component
				jsonString += layerToEncode.GetType().AssemblyQualifiedName
							  + "$"
							  + EditorJsonUtility.ToJson(layerToEncode)
							  + "\n";
			}

			jsonString = string.IsNullOrEmpty(jsonString) ?
				jsonString :
				jsonString.Remove(jsonString.Length - 1);
			EditorGUIUtility.systemCopyBuffer = jsonString;
		}

		private void PasteList()
		{
			string serializedLayers = EditorGUIUtility.systemCopyBuffer;
			if (string.IsNullOrEmpty(serializedLayers))
				return;

			// Clear all the attached anim layers
			int count = _layersReorderable.count;
			for (var i = 0; i < count; i++)
				_owner.RemoveLayer(0);

			string[] jsonList = serializedLayers.Split("\n");
			foreach (string json in jsonList)
			{
				string[] typeAndData = json.Split("$");
				var layerType = Type.GetType(typeAndData[0]);

				if (layerType == null)
				{
					Debug.Log("Invalid layer type: " + typeAndData[0]);
					continue;
				}

				Component layer = _owner.transform.gameObject.AddComponent(layerType);

				EditorJsonUtility.FromJsonOverwrite(typeAndData[1], layer);

				layer.hideFlags = HideFlags.HideInInspector;
				_owner.AddLayer((AnimLayer)layer);
			}
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			serializedObject.Update();
			_layersReorderable.DoLayoutList();

			RenderItem();
			RenderLayerHelpers();
			RenderAnimButtons();

			serializedObject.ApplyModifiedProperties();
		}
	}
}
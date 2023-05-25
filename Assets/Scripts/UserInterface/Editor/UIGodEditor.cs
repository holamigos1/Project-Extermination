using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UserInterface.Editor
{
    public class UIGodEditor : EditorWindow
    {
        [SerializeField] 
        private List<GameCanvasBase> _gameCanvases;
        
        [SerializeField] 
        private string _uiSheetFolder = "Assets/Resources/Scriptable Data/UI/";
        private string _fullSheetSavePath => _uiSheetFolder + nameof(UISheet) + ".asset"; 
		
        [SerializeField] 
        private string _gameCanvasesFolder = "Assets/Resources/Prefabs/UI";

        private StyleSheet      _styleSheet;
        private VisualTreeAsset _visualTreeUxml;
        private VisualElement   _root;
    
        [MenuItem("Tools/UI/Обозреватель игровых интерфейсов")]
        public static void ShowExample()
        {
            UIGodEditor window = GetWindow<UIGodEditor>();
            window.titleContent = new GUIContent("Обозреватель игровых интерфейсов");
        }

        private void OnEnable()
        {
            Debug.Log("UIGodEditor OnEnable");
            
            //TODO Если пути к UIGodEditor.uss и UIGodEditor.uxml файлам поменяются то будет больно
            _styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/UserInterface/Editor/UIGodEditor.uss");
            _visualTreeUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/UserInterface/Editor/UIGodEditor.uxml");
            _root = rootVisualElement;
            _gameCanvases = UIGod.UISheetInstance.AllGameCanvases;
        }

        private void OnDisable()
        {
            Debug.Log("UIGodEditor OnDisable");
            UIGod.UISheetInstance.AllGameCanvases = _gameCanvases;
        }

        public void CreateGUI()
        {
            _root.Add(_visualTreeUxml.Instantiate());
        
            //TODO Процедурный код разбросай на методы
            SerializedObject serializedObject = new (this);
            
            //TODO Про DRY что нибудь слышал? Убери повторения пж.
            
            SerializedProperty property = serializedObject.FindProperty(nameof(_uiSheetFolder));
            PropertyField field = new (property);
            field.label = "Путь к UISheet         ";
            field.Bind(serializedObject);
            _root.Add(field);
            
            property = serializedObject.FindProperty(nameof(_gameCanvasesFolder));
            field = new (property);
            field.label = "Путь к игровым полотнам";
            field.Bind(serializedObject);
            _root.Add(field);
            
            property = serializedObject.FindProperty(nameof(_gameCanvases));
            field = new (property);
            field.label = "Список игровых полотен ";
            field.Bind(serializedObject);
            _root.Add(field);
            
        }
    }
}
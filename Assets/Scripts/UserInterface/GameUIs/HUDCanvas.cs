using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.GameUIs
{
	public class HUDCanvas : GameCanvasBase
	{
		[SerializeField] 
		private Slider _healthSlider; //TODO Прописать отдельный класс для полоски хп
		
		[SerializeField] 
		private TMP_Text _ammoText; //TODO Прописать отдельный класс для панельки текущих патронов 
	}
}
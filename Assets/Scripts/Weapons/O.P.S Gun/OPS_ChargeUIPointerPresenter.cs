using UnityEngine;
using UnityEngine.UI;

namespace Weapons.O.P.S_Gun
{
    public class OPS_ChargeUIPointerPresenter : MonoBehaviour
    {
        [SerializeField] private Image _popOutPointerImage;
        private Canvas _fpsCanvas;
        private Camera _mainCamera;

        private void Start()
        {
            _fpsCanvas = GameObject.FindWithTag(Data.Tags.GameTags.FPS_CANVAS_TAG).GetComponent<Canvas>();
            _mainCamera = GameObject.FindWithTag(Data.Tags.GameTags.MAIN_CAMERA_TAG).GetComponent<Camera>();
            _popOutPointerImage.gameObject.SetActive(false);
        }

        public void DrawPointer(Vector3 chargePosition, bool isDraw)
        {
            //Debug.Log(MainCamera.name);
            if (isDraw)
            {
                _popOutPointerImage.gameObject.SetActive(true);
                Vector2 viewportPosition = _mainCamera.WorldToViewportPoint(chargePosition);
                _popOutPointerImage.rectTransform.anchorMin = viewportPosition;
                _popOutPointerImage.rectTransform.anchorMax = viewportPosition;
            }
            else
            {
                _popOutPointerImage.gameObject.SetActive(false);
            }
        }
    }
}
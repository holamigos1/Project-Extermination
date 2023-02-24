using Scripts.TagHolders;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OPS_ChargePointerUI : MonoBehaviour
{
    [FormerlySerializedAs("popOutPointerImage")] [SerializeField] private Image PopOutPointerImage;
    private Canvas _fpsCanvas;
    private Camera _mainCamera;

    private void Start()
    {
        _fpsCanvas = GameObject.FindWithTag(UnityTags.FPS_CANVAS_TAG).GetComponent<Canvas>();
        _mainCamera = GameObject.FindWithTag(UnityTags.MAIN_CAMERA_TAG).GetComponent<Camera>();
        PopOutPointerImage.gameObject.SetActive(false);
    }

    public void DrawPointer(Vector3 chargePosition, bool isDraw)
    {
        //Debug.Log(MainCamera.name);
        if (isDraw)
        {
            PopOutPointerImage.gameObject.SetActive(true);
            Vector2 viewportPosition = _mainCamera.WorldToViewportPoint(chargePosition);
            PopOutPointerImage.rectTransform.anchorMin = viewportPosition;
            PopOutPointerImage.rectTransform.anchorMax = viewportPosition;
        }
        else
        {
            PopOutPointerImage.gameObject.SetActive(false);
        }
    }
}
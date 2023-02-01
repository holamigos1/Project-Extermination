using Scripts.Controllers;
using UnityEngine;

namespace Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        private InputHandlersController inputControllerOnScene;

        public InputHandlersController InputControllerOnScene
        {
            get
            {
                if (inputControllerOnScene != null) return inputControllerOnScene;
                Debug.LogWarning("There's no InputHandlersController in scene!");
                return null;
            }
        }

        private void Awake()
        {
            inputControllerOnScene = (InputHandlersController)FindObjectOfType(typeof(InputHandlersController));
        }
    }
}
using Scripts.GameEnums;
using TMPro;
using UnityEngine;

namespace Scripts.GameEnums
{
    public enum OPS_Charge
    {
        Horizontal,
        Gravitation,
        Antigravity
    }
}

namespace Scripts.Weapons.OPS
{
    public class OPS_Display : MonoBehaviour
    {
        [SerializeField] private TMP_Text StatusTextField;
        [SerializeField] private TMP_Text PickedCargeText;

        public void SetCharge(GameEnums.OPS_Charge opsCharge)
        {
            switch (opsCharge)
            {
                case GameEnums.OPS_Charge.Gravitation:
                    PickedCargeText.text = "Gravitation";
                    PickedCargeText.color = Color.green;
                    break;
                case GameEnums.OPS_Charge.Antigravity:
                    PickedCargeText.text = "Anti-gravity";
                    PickedCargeText.color = Color.magenta;
                    break;
                case GameEnums.OPS_Charge.Horizontal:
                    PickedCargeText.text = "Horizontal";
                    PickedCargeText.color = Color.red;
                    break;
            }
        }

        public void SetStatus(string status)
        {
            StatusTextField.text = status;
        }
    }
}
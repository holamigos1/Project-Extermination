using TMPro;
using UnityEngine;

namespace Weapons.Range.O.P.S_Gun
{
    public class OPS_Display : MonoBehaviour
    {
        [SerializeField] private TMP_Text StatusTextField;
        [SerializeField] private TMP_Text PickedCargeText;

        public void SetCharge(OPS_ChargeType opsCharge)
        {
            switch (opsCharge)
            {
                case OPS_ChargeType.Gravitation:
                    PickedCargeText.text = "Gravitation";
                    PickedCargeText.color = Color.green;
                    break;
                
                case OPS_ChargeType.Antigravity:
                    PickedCargeText.text = "Anti-gravity";
                    PickedCargeText.color = Color.magenta;
                    break;
                
                case OPS_ChargeType.Horizontal:
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
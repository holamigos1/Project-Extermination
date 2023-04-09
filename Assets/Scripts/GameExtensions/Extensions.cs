using System.Collections.Generic;
using UnityEngine;

namespace GameExtensions
{
    public static partial class Extensions
    {
        //TODO ????? 
        public static Color GetEnableToggleColor(bool isEnabled)
        {
            return isEnabled ? 
                new Color(0.32f, 0.75f, 0.5f) : 
                new Color(0.8f, 0.17f, 0.129f);
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UserInterface
{
    public class UISheet : ScriptableObject
    {
        [SerializeField] 
        protected List<GameCanvasBase> AllGameCanvases;

        public GameCanvasBase FindCanvas(GameCanvasBase genericCanvas)
        {
            Type genericCanvasType = genericCanvas.GetType();
            GameCanvasBase desiredCanvas = null;
            
            Debug.Log(genericCanvas.GetType());
            
            foreach (GameCanvasBase canvas in AllGameCanvases)
                if (genericCanvasType == canvas.GetType())
                    desiredCanvas = canvas;
            
            return desiredCanvas;
        }
    }
}

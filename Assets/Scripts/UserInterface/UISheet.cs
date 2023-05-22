using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace UserInterface
{
    /// <summary> ScriptableObject контейнер массива всех GameCanvasBase в проекте. </summary>
    /// <seealso cref="GameCanvasBase"/>
    /// <seealso cref="ScriptableObject"/>
    public class UISheet : ScriptableObject
    {
        [SerializeField] 
        internal List<GameCanvasBase> AllGameCanvases = new ();//TODO 
        
        [CanBeNull] 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal GameCanvasBase FindCanvas(Type desiredCanvasType)
        {
            if (AllGameCanvases.Count == 0)
                throw new ArgumentNullException(nameof(UIGod), $"Ни одного {nameof(GameCanvasBase)} не указано в списке ссылок.");
            
            foreach (GameCanvasBase canvas in AllGameCanvases)
                if (desiredCanvasType == canvas.GetType())
                    return canvas;
            
            return null;
        }
    }
}

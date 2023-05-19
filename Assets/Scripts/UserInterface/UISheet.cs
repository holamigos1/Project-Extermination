using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace UserInterface
{
    public class UISheet : ScriptableObject
    {
        [SerializeField] 
        internal List<GameCanvasBase> AllGameCanvases = new ();

        /// <summary>Пытается найти в себе GameCanvasBase заданного типа.</summary>
        /// <typeparam name="TGameCanvasBase">Тип искомого наследника от GameCanvasBase.</typeparam>
        /// <seealso cref="GameCanvasBase"/>
        /// <returns>Возвращает TGameCanvasBase искомого типа если он есть, иначе вернёт null.</returns>
        [CanBeNull] [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

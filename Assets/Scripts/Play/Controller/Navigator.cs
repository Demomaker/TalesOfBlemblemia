using System.Collections.Generic;
using System.Linq;
using Harmony;
using UnityEngine;

namespace Game
{
    //Author: Antoine Lessard
    [Findable("Navigator")]
    public class Navigator : MonoBehaviour
    {
        private Stack<Canvas> menusStack;

        private void Awake()
        {
            menusStack = new Stack<Canvas>();
        }
        
        public void Enter(Canvas canvas)
        {
            var menu = menusStack.Any() ? menusStack.Peek() : null;
            if (menu != null) menu.enabled = false;
            canvas.enabled = true;
            menusStack.Push(canvas);
        }
        
        public void Leave(int nbMenusToLeave = 1)
        {
            for (uint i = 0; i < nbMenusToLeave; ++i)
            {
                var menu = menusStack.Any() ? menusStack.Pop() : null;
                if (menu != null) menu.enabled = false;
                menu = menusStack.Any() ? menusStack.Peek() : null;
                if (menu != null) menu.enabled = true;
            }
        }
    }
}
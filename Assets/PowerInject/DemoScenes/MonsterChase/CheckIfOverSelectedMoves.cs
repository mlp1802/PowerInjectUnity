using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PowerInject;
using UnityEngine;
namespace Uhyre1
{
    [Power]
    public class CheckIfOverSelectedMoves : MonoBehaviour, ISelectedMoves
    {
        ISelectedMoves selected;
        [Inject]

        GameState gameState;
        [Produce]

        protected ISelectedMoves produceThis(ISelectedMoves selected)
        {
            this.selected = selected;
            return this;
        }
        public List<string> getControl()
        {
            if (!gameState.PlayerIsDead)
            {
                return selected.getControl();
            }
            else
            {
                return new List<string>();
            }
        }
    }
}

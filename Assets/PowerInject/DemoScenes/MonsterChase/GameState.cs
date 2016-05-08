using UnityEngine;
using System.Collections;
using PowerInject;
namespace Uhyre1
{
    public class GameState
    {
        [Inject]
        Labyrinth labyrinth;

        public bool PlayerIsDead { get; set; }
        public bool PlayerWon
        {
            get
            {
                return labyrinth.NumberOfDots == 0;
            }
        }

    }
}
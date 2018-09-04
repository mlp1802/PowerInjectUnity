using UnityEngine;
using System.Collections;
using PowerInject;

namespace Uhyre1
{
    [Power]
    public class GameInit : MonoBehaviour
    {
        [Produce]
        protected PointsKeeper getPointsKeeper()
        {
            return new PointsKeeper();
        }

        [Produce]
        public GameState getGameState()
        {
            return new GameState();
        }
    }
}

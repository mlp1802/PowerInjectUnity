using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PowerInject;
namespace Uhyre1
{
    [Insert(Typed = typeof(ISelectedMoves))]
    public class SelectedMovesFromKeyboard : MonoBehaviour, ISelectedMoves
    {
        public List<string> getControl()
        {
            var result = new List<string>();
            if (Input.GetKey(KeyCode.UpArrow))
            {
                result.Add("forward");
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                result.Add("back");
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                result.Add("left");
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                result.Add("right");
            }

            return result;
        }
    }
}


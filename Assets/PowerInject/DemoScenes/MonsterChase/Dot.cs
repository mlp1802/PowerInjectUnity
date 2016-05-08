using UnityEngine;
using System.Collections;
using PowerInject;
namespace Uhyre1
{

    public class Dot : MonoBehaviour
    {
        void Start()
        {
            var collider = gameObject.GetComponent<Collider>();
            collider.isTrigger = true;
            var r = GetComponent<Renderer>();
            r.material.color = Color.yellow;
        }
    }
}
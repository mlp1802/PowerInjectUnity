using UnityEngine;
using System.Collections;
using PowerInject;
namespace Uhyre1
{
    [Insert]
    public class Monster : MonoBehaviour
    {
        float speed = 8f;
        
        [Inject]
        Player player;

        [Inject]
        GameState gameState;
        Rigidbody actor;
        
        void Start() 
        {
            actor = GetComponent<Rigidbody>();
        }
        
        void move()
        {
            var pos = transform.position;
            var force = new Vector3();
            var playerPos = player.transform.position;
            if (playerPos.z < pos.z)
            {
                force.z = -1;
            }
            else
            {
                force.z = 1;
            }
            if (playerPos.x < pos.x)
            {
                force.x = -1;
            }
            else
            {
                force.x = 1;
            }
            actor.AddForce(force * speed);
        }

        bool canMove()
        {
            return !gameState.PlayerIsDead & !gameState.PlayerWon;
        }
        void FixedUpdate()
        {
            if (canMove())
            {
                move();
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<Player>())
            {
                gameState.PlayerIsDead = true;
            }
        }

    }
}

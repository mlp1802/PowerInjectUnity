using UnityEngine;
using System.Collections;
using PowerInject;
namespace Uhyre1
{
    [Power]
    [Insert]
    public class Player : MonoBehaviour
    {
        float moveStep = 0.1f;
        [Inject]
        ISelectedMoves controls;
        [Inject]
        Labyrinth labyrinth;
        [Inject]
        PointsKeeper pointsKeeper;
        Rigidbody actor;
        int moveForce = 100;
        void move(float x, float z)
        {
            var pos = transform.position;
            var force = new Vector3(x, 0, z) * moveForce;
            actor.AddForce(force);

        }

        void moveForward()
        {
            move(moveStep, 0);
        }

        void moveBack()
        {
            move(-moveStep, 0);
        }

        void moveLeft()
        {
            move(0, moveStep);
        }

        void moveRight()
        {
            move(0, -moveStep);
        }

        public void FixedUpdate()
        {
            var move = controls.getControl();

            if (move.Contains("forward"))
            {
                moveForward();
            }
            if (move.Contains("back"))
            {
                moveBack();
            }
            if (move.Contains("left"))
            {
                moveLeft();
            }
            if (move.Contains("right"))
            {
                moveRight();
            }

        }

        [OnInjected]
        public void InitPlayer()
        {
            var x = labyrinth.transform.position.x + 1;
            var y = labyrinth.transform.position.y;
            var z = labyrinth.transform.position.z + 3;
            this.transform.position = new Vector3(x, y, z);
            actor = GetComponent<Rigidbody>();
        }

        void OnTriggerEnter(Collider collision)
        {

            if (collision.gameObject.GetComponent<Dot>())
            {

                collision.gameObject.active = false;
                pointsKeeper.addPoints(5);
                labyrinth.NumberOfDots -= 1;
            }
        }

    }
}

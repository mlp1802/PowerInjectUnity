using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PowerInject;
namespace Uhyre1
{
    [Insert]
    public class Labyrinth : MonoBehaviour
    {
        public int width = 101;
        public float brickSize = 2.0f;
        public int spaceBetweenBricks = 3;
        public int NumberOfDots { get; set; }

        [Inject]
        Monster monster;
        
        void addWall(float x, float y)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(brickSize, 2.0f, brickSize);
            var pos = new Vector3(x * brickSize * spaceBetweenBricks, 0, y * brickSize * spaceBetweenBricks);
            cube.transform.parent = this.transform;
            cube.transform.position = pos;
        }

        void addDot(float x, float y)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var dot = sphere.AddComponent<Dot>();
            var scale = 0.3f * brickSize;
            sphere.transform.localScale = new Vector3(scale, scale, scale);
            var pos = new Vector3(x * brickSize * spaceBetweenBricks, 0, y * brickSize * spaceBetweenBricks);
            sphere.transform.parent = this.transform;
            sphere.transform.position = pos;
            NumberOfDots += 1;
        }




        void createLabyrinth()
        {
            for (int x = 2; x < width - 1; x += 2)
            {
                for (int y = 1; y < width; y += 1)
                {
                    if (Random.Range(0, 100f) < 60)
                    {
                        addWall(x, y);
                    }
                    else
                    {
                        addDot(x, y);
                    }
                }
            }

            for (int x = 0; x <= width; x++)
            {
                addWall(x, 0);
                addWall(x, width);
                addWall(width, x);
                addWall(0, x);
            }

            for (int x = 1; x < width; x += 2)
            {
                for (int y = 1; y < width; y += 1)
                {

                    addDot(x, y);
                }
            }
        }

        [OnInjected]
        public void Init()
        {
            createLabyrinth();
            var monsterX = (width - 1) * brickSize;
            var monsterZ = (width - 1) * brickSize;
            monster.transform.position = new Vector3(monsterX, 0, monsterZ);

        }
    }
}
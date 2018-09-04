using UnityEngine;
using System.Collections;
using PowerInject;
namespace Uhyre1
{
    [Insert]
    public class CameraUpdater : MonoBehaviour
    {
        [Inject]
        Player player;
        [Inject]
        CameraProvider cameraProvider;

        [OnInjected]
        public void InitCameraUpdater()
        {

            cameraProvider.transform.rotation = Quaternion.identity * Quaternion.AngleAxis(90, Vector3.right) * Quaternion.AngleAxis(90, Vector3.back);
        }

        public void FixedUpdate()
        {
            cameraProvider.transform.position = new Vector3(player.transform.position.x, 40, player.transform.position.z);
        }
    }
}

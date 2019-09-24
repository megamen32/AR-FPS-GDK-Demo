using System;
using UnityEngine;
using Object = System.Object;

namespace Fps
{
    [RequireComponent(typeof(Camera))]
    public class SkyboxCamera : MonoBehaviour
    {
        private Camera myCamera;
        [SerializeField] Boolean synthRotation;

        private void Awake()
        {
            myCamera = GetComponent<Camera>();
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            var playerCamera = Camera.main;
            if (playerCamera == null)
            {
                return;
            }

            if (synthRotation)
            {
                myCamera.transform.rotation = playerCamera.transform.rotation;
            }

            myCamera.fieldOfView = playerCamera.fieldOfView;
        }
    }
}

using OWML.ModHelper;
using OWML.Common;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ThirdPersonCamera
{
    class Utility
    {
        public static GameObject[] FindGameObjectsWithLayer(int layer)
        {
            List<GameObject> objects = new List<GameObject>();
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.layer == layer) objects.Add(go);
            }
            if (objects.Count == 0) return null;
            return objects.ToArray();
        }

        public static string GetPath(Transform current)
        {
            if (current.parent == null) return "/" + current.name;
            return GetPath(current.parent) + "/" + current.name;
        }

        // Same result as PlayerCameraController.CenterCameraOverSeconds but without the camera gliding back on its own.
        // Pass centerPitch false to only undo the free look yaw and keep wherever the player was looking up or down.
        public static void CenterCameraInstantly(PlayerCameraController cameraController, bool centerPitch = true)
        {
            if (cameraController == null) return;

            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            Type type = typeof(PlayerCameraController);

            // Stop any centering the game already started, else it would keep moving the camera
            type.GetField("_isSnapping", flags)?.SetValue(cameraController, false);
            type.GetField("_degreesX", flags)?.SetValue(cameraController, 0f);
            if (centerPitch) type.GetField("_degreesY", flags)?.SetValue(cameraController, 0f);

            // The rotation is rebuilt from the degrees each frame, this just avoids a frame of delay
            if (type.GetField("_playerCamera", flags)?.GetValue(cameraController) is OWCamera playerCamera)
            {
                float degreesY = centerPitch ? 0f : (float)(type.GetField("_degreesY", flags)?.GetValue(cameraController) ?? 0f);
                playerCamera.transform.localRotation = Quaternion.AngleAxis(degreesY, -Vector3.right);
            }
        }

        public static void ChangeLayersRecursively(Transform transform, int layer)
        {
            transform.gameObject.layer = layer;
            foreach(Transform child in transform)
            {
                ChangeLayersRecursively(child, layer);
            }
        }
    }
}

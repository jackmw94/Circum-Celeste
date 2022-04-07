using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapLogger : MonoBehaviour
{
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Camera camera = Camera.main;

            if (camera == null)
            {
                Debug.LogError("Cannot detect touches while there is no main camera");
            }
            else
            {
                for (int index = 0; index < Input.touches.Length; index++)
                {
                    Touch touch = Input.touches[index];
                    Ray screenPointToRay = camera.ScreenPointToRay(touch.position);

                    Debug.Log(Physics.Raycast(screenPointToRay, out RaycastHit hit)
                        ? $"Touch {index} ({touch.phase}, {touch.fingerId}): hit '{hit.transform.gameObject}'"
                        : $"Touch {index} ({touch.phase}, {touch.fingerId}): hit nothing");
                }
            }
        }
    }
}

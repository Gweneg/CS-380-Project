using System;
using System.Collections;
using System.Collections.Generic;
using Environment;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	private Camera _mainCamera;
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
	    _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + new Vector3(0, 1, -5);
        
        // RENDER MAP
        // Compute the camera's new position.
        float cameraHalfWidth = _mainCamera.orthographicSize * _mainCamera.aspect;
        Vector3 newPosition = _mainCamera.transform.position;
        Vector3 cameraCornerOffset = new Vector3(cameraHalfWidth, _mainCamera.orthographicSize);
        Vector3 newPositionBLCorner = newPosition - cameraCornerOffset;

				
        // Compute the bounds of the map.
        Vector2 bottomleftCorner = Map.GetWorldCoordinates((0, 0));
        Vector2 toprightCorner = Map.GetWorldCoordinates((Map.MapWidth - 8, Map.MapHeight - 1)) - new Vector2(cameraHalfWidth, 0);
				
        // Clamp the new position to within the bounds of the map.
        Vector3 clampedPosition = new Vector3(x: Math.Clamp(newPositionBLCorner.x, bottomleftCorner.x, toprightCorner.x), 
                                              y: Math.Clamp(newPositionBLCorner.y, bottomleftCorner.y, toprightCorner.y), 
                                              z: newPositionBLCorner.z);
        _mainCamera.transform.position = clampedPosition + cameraCornerOffset;
        // Update the corner.
        Map.Instance.SetRenderArea(Map.GetTileCoordinates(clampedPosition));
    }
}

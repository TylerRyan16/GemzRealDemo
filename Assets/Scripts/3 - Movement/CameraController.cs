using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float moveSpeed = 10.0f;
    public float maxMoveSpeed = 50.0f;
    public float rotationSpeed = 5.0f;
    public float zoomSpeed = 10.0f;
    public float sprintMultiplier = 2.0f;
    private PrefabManager prefabManager;

    public Terrain terrain;


    private void Start()
    {
        prefabManager = FindObjectOfType<PrefabManager>();  
        CenterCameraOnTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        MoveIfClicking();
        HandleMovement();
        HandleScrolling();
        KeepCameraAboveTerrain();
        KeepCameraBelowCeiling();
        ResetMoveSpeedIfNeeded();
    }

    private void MoveIfClicking()
    {
        // if right click, set speed/rotation
        if (Input.GetMouseButton(1))
        {
            float h = rotationSpeed * Input.GetAxis("Mouse X");
            float v = rotationSpeed * Input.GetAxis("Mouse Y");

            transform.Rotate(Vector3.up, h, Space.World);
            transform.Rotate(Vector3.right, -v);
        }

        // move
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        transform.Translate(moveX, 0, moveZ);
    }

    private void HandleMovement()
    {
        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= sprintMultiplier;
        }

        // if holding w
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);
        }

        // if holding s
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * currentSpeed * Time.deltaTime, Space.Self);
        }

        // if holding d
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * currentSpeed * Time.deltaTime, Space.Self);
        }

        // if holding a
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * currentSpeed * Time.deltaTime, Space.Self);
        }

        // if holding spacebar
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(Vector3.up * currentSpeed * Time.deltaTime, Space.Self);
        }

        // if holding shift
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.Translate(Vector3.down * currentSpeed * Time.deltaTime, Space.Self);
        }

    }

    private void HandleScrolling()
    {
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");

        if (Input.GetMouseButton(1) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            // Adjust speed when holding right-click and shift
            moveSpeed += scroll * 5f;
            moveSpeed = Mathf.Clamp(moveSpeed, 5, maxMoveSpeed);
        }
        else if (scroll != 0)
        {
            // Zoom in and out
            Vector3 zoomDirection = transform.forward * scroll * zoomSpeed;
            transform.position += zoomDirection;
        }
    }


    private void KeepCameraAboveTerrain()
    {
        // Ensure Camera does not go below terrain
        Vector3 position = transform.position;
        position.y = Mathf.Max(position.y, 3);
        transform.position = position;
    }

    private void KeepCameraBelowCeiling()
    {
        Vector3 position = transform.position;
        position.y = Mathf.Min(position.y, 50);
        transform.position = position;
    }

    private void CenterCameraOnTerrain()
    {
        if (terrain != null)
        {
            float centerX = terrain.terrainData.size.x / 2;
            float centerZ = terrain.terrainData.size.z / 2;

            transform.position = new Vector3(centerX, 25, centerZ);
            transform.rotation = Quaternion.Euler(30f, 0f, 0f);

        } else
        {
            Debug.Log("Terrain not assigned");
        }
    }

    private void ResetMoveSpeedIfNeeded()
    {
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Resetting speed");
            moveSpeed = 10.0f;
        }
    }
}

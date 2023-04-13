using UnityEngine;

namespace PCG
{
    public class FlyingCamera : MonoBehaviour
    {
        [SerializeField] private Vector2 sensitivity;
        [SerializeField] private float moveSpeed;

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            Vector2 cameraRotate = new Vector2(Input.GetAxis("Mouse X") * sensitivity.x, Input.GetAxis("Mouse Y") * sensitivity.y);
            
            transform.Rotate (-cameraRotate.y * Vector3.right, Space.Self);
            transform.Rotate (cameraRotate.x * Vector3.up, Space.World);

            Vector3 moveInput = new Vector3(
                Input.GetAxisRaw("Horizontal"),
                Input.GetKey(KeyCode.Space) ? 1f : Input.GetKey(KeyCode.LeftShift) ? -1f : 0f,
                Input.GetAxisRaw("Vertical")).normalized * (moveSpeed * Time.deltaTime);
            transform.position += transform.rotation * moveInput;
        }
    }
}

using UnityEngine;

namespace PCG
{
    public class MoveForwardScript : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;

        private void Update()
        {
            transform.position += transform.forward * (moveSpeed * Time.deltaTime);
        }
    }
}

namespace Sandbox
{
    using UnityEngine;

    public class SimpleMove : MonoBehaviour
    {
        [SerializeField] private float radius = 4;
        [SerializeField] private float timeScale = 0.5f;
        [SerializeField] private float speedX = 3;
        [SerializeField] private float speedY = 4;

        void Update()
        {
            float time = Time.time * timeScale;
            float x = Mathf.Cos(speedX * time) * radius;
            float y = Mathf.Sin(speedY * time) * radius;
            transform.position = new Vector3(x, y, 0);
        }
    }

}
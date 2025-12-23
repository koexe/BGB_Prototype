using System.Collections;
using UnityEngine;

namespace CrowNyknck
{
    public class Crow : MonoBehaviour
    {
        public Vector2 targetPosition;
        public float moveTime = 3f;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        void Start()
        {
            StartCoroutine(MoveToTarget(targetPosition));
        }

        IEnumerator MoveToTarget(Vector2 targetPosition)
        {
            float elapsedTime = 0f;
            Vector2 startingPosition = transform.position;

            while (elapsedTime < moveTime)
            {
                transform.position = Vector2.Lerp(startingPosition, targetPosition, elapsedTime / moveTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            animator.SetTrigger("grounded");
        }
    }
}

// GhostTrailController.cs
using UnityEngine;

namespace Units.Projectiles
{
    public class GhostTrailController : MonoBehaviour
    {
        public GameObject ghostPrefab;
        public float spawnRate = 0.05f;
        private float timer;

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= spawnRate)
            {
                timer = 0;
                CreateGhost();
            }
        }

        void CreateGhost()
        {
            GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
            SpriteRenderer ghostSr = ghost.GetComponent<SpriteRenderer>();
            SpriteRenderer currentSr = GetComponent<SpriteRenderer>();

            if (ghostSr != null && currentSr != null)
            {
                ghostSr.sprite = currentSr.sprite;
                ghostSr.flipX = currentSr.flipX;
                ghostSr.color = currentSr.color;
                ghostSr.sortingLayerID = currentSr.sortingLayerID;
                ghostSr.sortingOrder = currentSr.sortingOrder - 1;
            }

            ghost.transform.localScale = transform.localScale;
        }
    }
}

using Mirror;
using System.Collections;
using UnityEngine;

namespace Beast
{
    public class DeathCollider : MonoBehaviour
    {
        [SerializeField] private float delayToRespawn = 3f;

        private LayerMask layerPlayer = 1 << 7;

        void OnTriggerEnter(Collider other)
        {
            if (layerPlayer == (1 << other.gameObject.layer))
            {
                if (other.gameObject.TryGetComponent(out ThirdPersonPlayer thirdPersonPlayer))
                {
                    StartCoroutine(DeathCoroutine(thirdPersonPlayer));
                }
            }
        }

        private IEnumerator DeathCoroutine(ThirdPersonPlayer thirdPersonPlayer)
        {
            thirdPersonPlayer.gameObject.SetActive(false);

            yield return new WaitForSeconds(delayToRespawn);

            thirdPersonPlayer.transform.position = NetworkManager.startPositions[Random.Range(0, NetworkManager.startPositions.Count)].position;
            thirdPersonPlayer.gameObject.SetActive(true);
        }
    }
}

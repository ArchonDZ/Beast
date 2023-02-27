using System.Collections;
using UnityEngine;

namespace Beast
{
    public class ModelPerson : MonoBehaviour
    {
        [SerializeField] private float struckTime = 3f;
        [SerializeField] private ThirdPersonPlayer thirdPersonPlayer;
        [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
        [SerializeField] private Material materialNormal;
        [SerializeField] private Material materialStruck;

        public void GetDamage()
        {
            StartCoroutine(StruckCoroutine());
        }

        private IEnumerator StruckCoroutine()
        {
            thirdPersonPlayer.IsStruck = true;
            skinnedMeshRenderer.material = materialStruck;

            yield return new WaitForSeconds(struckTime);

            skinnedMeshRenderer.material = materialNormal;
            thirdPersonPlayer.IsStruck = false;
        }
    }
}

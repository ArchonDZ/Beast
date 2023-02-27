using Mirror;
using System.Collections;
using UnityEngine;

namespace Beast
{
    public class ControllerPerson : MonoBehaviour
    {
        [SerializeField] private float speed = 4f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float turnSmoothTime = 0.035f;
        [SerializeField] private float distanceAttack = 4f;
        [Tooltip("SpeedAttack is best set on DistanceAttack * 2f")]
        [SerializeField] private float speedAttack = 8f;
        [SerializeField] private LayerMask layerEnviroment = 1 << 6;
        [SerializeField] private ThirdPersonPlayer thirdPersonPlayer;
        [SerializeField] private CharacterController controller;
        [SerializeField] private NetworkAnimator networkAnimator;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject cinemachineFreeLook;

        private Vector3 pathShift;
        private Vector3 startPosition;
        private Transform cameraTransform;

        public void StartController()
        {
            cameraTransform = Camera.main.transform;
            cinemachineFreeLook.SetActive(true);
        }

        public void UpdateController()
        {
            if (!thirdPersonPlayer.IsAttackActivated)
            {
                Movement();

                if (!thirdPersonPlayer.IsStruck)
                    Attack();
            }
        }

        private void Movement()
        {
            Vector3 directionVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
            Vector3 fallDirection = controller.isGrounded ? Vector3.zero : Vector3.up * gravity * Time.deltaTime;
            Vector3 moveDirection = Vector3.zero;

            if (directionVector.sqrMagnitude >= 0.01f)
            {
                float turnSmoothVelocity = 0f;
                float targetAngle = Mathf.Atan2(directionVector.x, directionVector.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                animator.SetBool("Idle", false);
            }
            else
            {
                animator.SetBool("Idle", true);
            }

            controller.Move(moveDirection * speed * Time.deltaTime + fallDirection);
        }

        private void Attack()
        {
            if (Input.GetMouseButtonDown(0))
            {
                thirdPersonPlayer.IsAttackActivated = true;
                networkAnimator.SetTrigger("Attack");

                startPosition = transform.position;
                pathShift = transform.position + transform.forward * distanceAttack;
                Vector3 deltaUp = Vector3.up * 0.5f;
                Vector3 startRay = startPosition + deltaUp;
                Vector3 endRay = pathShift + deltaUp;
                Vector3 directionRay = endRay - startRay;

                if (Physics.Raycast(startRay, directionRay, out RaycastHit hitInfo, distanceAttack, layerEnviroment))
                {
                    Vector3 hit = hitInfo.point;
                    hit.y = transform.position.y;
                    pathShift = hit + ((startPosition - hit).normalized * (controller.radius * 2f));
                }

                StartCoroutine(AttackCoroutine());
            }
        }

        private IEnumerator AttackCoroutine()
        {
            float startTime = Time.time;
            float durationTime = distanceAttack / speedAttack;
            float age;

            do
            {
                age = (Time.time - startTime) / durationTime;
                transform.LookAt(pathShift);
                transform.position = Vector3.Lerp(startPosition, pathShift, age);
                yield return null;
            } while (age < 1f);

            thirdPersonPlayer.IsAttackActivated = false;
        }
    }
}

using Cinemachine;
using Mirror;
using UnityEngine;

namespace Beast
{
    public class ThirdPersonPlayer : NetworkBehaviour
    {
        [SerializeField] private ControllerPerson controllerPerson;
        [SerializeField] private ModelPerson modelPerson;
        [SerializeField] private LayerMask layerPlayer = 1 << 8;

        [SyncVar] private int index;
        [SyncVar(hook = nameof(OnChangedScore))] private int score;
        [SyncVar] private bool isAttackActivated;
        [SyncVar] private bool isStruck;

        private NetRoomManager roomManager;

        public bool IsAttackActivated { get => isAttackActivated; set => isAttackActivated = value; }
        public bool IsStruck { get => isStruck; set => isStruck = value; }

        void Start()
        {
            if (!isLocalPlayer)
                return;

            roomManager = NetRoomManager.singleton;
            Camera.main.transform.gameObject.AddComponent(typeof(CinemachineBrain));
            controllerPerson.StartController();
        }

        void Update()
        {
            if (!isLocalPlayer)
                return;

            if (roomManager.EndGame)
                return;

            controllerPerson.UpdateController();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!isLocalPlayer)
                return;

            if (isStruck)
                return;

            if (layerPlayer == (1 << other.gameObject.layer))
            {
                if (other.transform.parent.TryGetComponent(out ThirdPersonPlayer thirdPersonPlayer))
                {
                    if (!isAttackActivated)
                    {
                        if (thirdPersonPlayer.isAttackActivated)
                            CmdGetDamage();
                    }
                    else
                    {
                        if (!thirdPersonPlayer.isAttackActivated && !thirdPersonPlayer.isStruck)
                            score++;
                    }
                }
            }
        }

        public void InitIndex(int value)
        {
            index = value;
        }

        public void ResetControls()
        {
            score = 0;
        }

        private void OnChangedScore(int _Old, int _New)
        {
            if (_New == 3)
                NetRoomManager.singleton.FinishGame(index);
        }

        [Command]
        private void CmdGetDamage()
        {
            RpcGetDamage();
        }

        [ClientRpc]
        private void RpcGetDamage()
        {
            modelPerson.GetDamage();
        }

        void OnGUI()
        {
            GUI.Box(new Rect(10f + (index * 110), 10f, 100f, 25f), $"P{index}: {score:0}");
        }
    }
}

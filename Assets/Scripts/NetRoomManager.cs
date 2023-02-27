using Mirror;
using System.Collections;
using UnityEngine;

namespace Beast
{
    public class NetRoomManager : NetworkRoomManager
    {
        public static new NetRoomManager singleton { get; private set; }

        [SerializeField] private float restartTime = 5f;

        private bool showStartButton;
        private bool endGame;
        private int indexPlayer;

        public bool EndGame => endGame;

        public override void Awake()
        {
            base.Awake();
            singleton = this;
        }

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            ThirdPersonPlayer thirdPersonPlayer = gamePlayer.GetComponent<ThirdPersonPlayer>();
            thirdPersonPlayer.InitIndex(roomPlayer.GetComponent<NetworkRoomPlayer>().index);
            return true;
        }

        public override void OnRoomServerPlayersReady()
        {
#if UNITY_SERVER
            base.OnRoomServerPlayersReady();
#else
            showStartButton = true;
#endif
        }

        public void FinishGame(int index)
        {
            endGame = true;
            indexPlayer = index;
            StartCoroutine(Restart());
        }

        private IEnumerator Restart()
        {
            yield return new WaitForSeconds(restartTime);

            foreach (NetworkConnectionToClient connection in NetworkServer.connections.Values)
            {
                Transform startPos = GetStartPosition();
                GameObject player = connection.identity.gameObject;
                player.transform.SetPositionAndRotation(startPos.position, startPos.rotation);
                ThirdPersonPlayer thirdPersonPlayer = player.GetComponent<ThirdPersonPlayer>();
                thirdPersonPlayer.ResetControls();
            }
            endGame = false;
        }

        public override void OnGUI()
        {
            base.OnGUI();

            if (allPlayersReady && showStartButton && GUI.Button(new Rect(150, 300, 120, 20), "START GAME"))
            {
                showStartButton = false;
                ServerChangeScene(GameplayScene);
            }

            if (endGame)
            {
                GUI.Box(new Rect(810f, 515f, 300f, 50f), $"Win Player {indexPlayer}");
            }
        }
    }
}

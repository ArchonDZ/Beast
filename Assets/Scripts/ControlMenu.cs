using UnityEngine;

namespace Beast
{
    public class ControlMenu : MonoBehaviour
    {
        private bool isCursorLocked;

        void Start()
        {
            isCursorLocked = true;
            Cursor.lockState = CursorLockMode.Locked;
            NetRoomManager.singleton.showRoomGUI = !isCursorLocked;
        }

        void Update()
        {
            Control();
        }

        private void Control()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isCursorLocked = !isCursorLocked;
                Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.Confined;
                NetRoomManager.singleton.showRoomGUI = !isCursorLocked;
            }
        }
    }
}

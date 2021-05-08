namespace ReGaSLZR.Dare.Misc
{

    using UnityEngine;

    public class MouseController : MonoBehaviour
    {
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

}
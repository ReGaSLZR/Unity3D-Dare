namespace ReGaSLZR.Dare.View
{
    using NaughtyAttributes;
    using UnityEngine;

    public class CanvasManager : MonoBehaviour
    {

        [SerializeField]
        [Required]
        private Canvas canvas;

        #region Unity Callbacks

        private void Awake()
        {
            canvas.gameObject.SetActive(true);
        }

        #endregion

    }

}
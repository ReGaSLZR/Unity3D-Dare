namespace ReGaSLZR.Dare.Model
{

    using NaughtyAttributes;
    using UnityEngine;

    [RequireComponent(typeof(SphereCollider))]
    public class NoiseMaker : MonoBehaviour
    {

        public const string NOISE_TAG = "Noise";

        #region Inspector Variables

        [SerializeField]
        [Tag]
        private string tagCustom = NOISE_TAG;

        #endregion

        private SphereCollider sphereCollider;

        private void Awake()
        {
            sphereCollider = GetComponent<SphereCollider>();
            this.tag = tagCustom;
        }

        public void SetNoise(int value)
        {
            sphereCollider.radius = value;
        }
        
    }

}
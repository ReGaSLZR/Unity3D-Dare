namespace ReGaSLZR.Dare.Model
{

    using NaughtyAttributes;
    using UnityEngine;

    [RequireComponent(typeof(Animation))]
    [RequireComponent(typeof(SphereCollider))]
    public class NoiseMaker : MonoBehaviour
    {

        public const string NOISE_TAG = "Noise";

        #region Inspector Variables

        [SerializeField]
        [Tag]
        private string tagCustom = NOISE_TAG;

        #endregion

        private new Animation animation;
        private SphereCollider sphereCollider;

        private void Awake()
        {
            animation = GetComponent<Animation>();
            sphereCollider = GetComponent<SphereCollider>();
            this.tag = tagCustom;
        }

        public void SetNoise(int value)
        {
            animation.Play();
            sphereCollider.radius = value;
        }
        
    }

}
namespace ReGaSLZR.Dare.Model
{

    using UnityEngine;

    [RequireComponent(typeof(SphereCollider))]
    public class NoiseMaker : MonoBehaviour
    {

        public const string NOISE_TAG = "Noise";

        private SphereCollider sphereCollider;

        private void Awake()
        {
            sphereCollider = GetComponent<SphereCollider>();
            tag = NOISE_TAG;
        }

        public void SetNoise(int value)
        {
            sphereCollider.radius = value;
        }
        
    }

}
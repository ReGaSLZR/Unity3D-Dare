namespace ReGaSLZR.Dare.Detector
{

    using NaughtyAttributes;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;

    [RequireComponent(typeof(Collider))]
    public class CollisionDetector : MonoBehaviour
    {

        [SerializeField]
        [Tag]
        private string trackedTag;

        private CompositeDisposable disposable = new CompositeDisposable();
        private ReactiveProperty<bool> hasCollision = new ReactiveProperty<bool>();

        public Vector3 CollisionContactPoint { get; private set; } = Vector3.zero;
        public Vector3 TrackedPosition { get; private set; } = Vector3.zero;

        public IReactiveProperty<bool> HasCollision()
        {
            return hasCollision;
        }

        private void OnEnable()
        {
            this.OnTriggerStayAsObservable()
                .Where(collider => (collider.tag.Equals(trackedTag)))
                .Subscribe(collider =>
                {
                    TrackedPosition = collider.gameObject.transform.position;
                    hasCollision.Value = true;
                })
                .AddTo(disposable);

            this.OnTriggerExitAsObservable()
                .Where(collider => (collider.tag.Equals(trackedTag)))
                .Subscribe(_ => hasCollision.Value = false)
                .AddTo(disposable);

            this.OnCollisionStayAsObservable()
               .Where(collision => (collision.gameObject.tag.Equals(trackedTag)))
               .Subscribe(collision =>
               {
                   CollisionContactPoint = collision.contacts
                        [collision.contacts.Length - 1].point;
                   TrackedPosition = collision.gameObject.transform.position;
                   hasCollision.Value = true;
               })
               .AddTo(disposable);

            this.OnCollisionExitAsObservable()
               .Where(collision => (collision.gameObject.tag.Equals(trackedTag)))
               .Subscribe(_ =>
               {
                   CollisionContactPoint = Vector3.zero;
                   hasCollision.Value = false;
               })
               .AddTo(disposable);
        }

        private void OnDisable()
        {
            disposable.Clear();
        }

    }

}
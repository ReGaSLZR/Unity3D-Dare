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

        public Vector3 ContactPoint { get; private set; } = Vector3.zero;

        public IReactiveProperty<bool> HasCollision()
        {
            return hasCollision;
        }

        private void OnEnable()
        {
            this.OnTriggerStayAsObservable()
                .Where(collider => (collider.tag.Equals(trackedTag)))
                .Subscribe(_ => hasCollision.Value = true)
                .AddTo(disposable);

            this.OnTriggerExitAsObservable()
                .Where(collider => (collider.tag.Equals(trackedTag)))
                .Subscribe(_ => hasCollision.Value = false)
                .AddTo(disposable);

            this.OnCollisionStayAsObservable()
               .Where(collision => (collision.gameObject.tag.Equals(trackedTag)))
               .Subscribe(collision =>
               {
                   ContactPoint = collision.contacts
                        [collision.contacts.Length - 1].point;
                   hasCollision.Value = true;
               })
               .AddTo(disposable);

            this.OnCollisionExitAsObservable()
               .Where(collision => (collision.gameObject.tag.Equals(trackedTag)))
               .Subscribe(_ =>
               {
                   ContactPoint = Vector3.zero;
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
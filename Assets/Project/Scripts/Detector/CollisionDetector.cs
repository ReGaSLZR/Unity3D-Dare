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
        public GameObject CollidedObject { get; private set; }
        public Vector3 CollidedObjectPosition 
            { get {
                if (CollidedObject == null)
                {
                    return Vector3.zero;
                }

                return CollidedObject.transform.position;
            } }

        public IReadOnlyReactiveProperty<bool> HasCollision()
        {
            return hasCollision;
        }

        private void OnEnable()
        {
            this.OnTriggerStayAsObservable()
                .Where(collider => (collider.tag.Equals(trackedTag)))
                .Subscribe(collider =>
                {
                    CollidedObject = collider.gameObject;
                    hasCollision.Value = true;
                })
                .AddTo(disposable);

            this.OnTriggerExitAsObservable()
                .Where(collider => (collider.tag.Equals(trackedTag)))
                .Subscribe(_ => RemoveCollisionData())
                .AddTo(disposable);

            this.OnCollisionStayAsObservable()
               .Where(collision => (collision.gameObject.tag.Equals(trackedTag)))
               .Subscribe(collision =>
               {
                   CollidedObject = collision.gameObject;
                   CollisionContactPoint = collision.contacts
                        [collision.contacts.Length - 1].point;
                   hasCollision.Value = true;
               })
               .AddTo(disposable);

            this.OnCollisionExitAsObservable()
               .Where(collision => (collision.gameObject.tag.Equals(trackedTag)))
               .Subscribe(_ => RemoveCollisionData())
               .AddTo(disposable);

            //If the CollidedObject is disabled while hasCollision == true, revert to false
            Observable.Interval(System.TimeSpan.FromSeconds(5))
                .Where(_ => hasCollision.Value)
                .Where(_ => (CollidedObject == null) || 
                    !CollidedObject.activeInHierarchy)
                .Subscribe(_ => RemoveCollisionData())
                .AddTo(disposable);
        }

        private void OnDisable()
        {
            disposable.Clear();
        }

        private void RemoveCollisionData()
        {
            CollidedObject = null;
            CollisionContactPoint = Vector3.zero;
            hasCollision.Value = false;
        }

    }

}
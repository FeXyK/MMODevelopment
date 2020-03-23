using UnityEngine;

namespace Assets.AreaServer.SkillSystem
{
    class SkillProjectile : MonoBehaviour
    {
        public Transform source;
        public Entity.Entity target;
        public float speed;
        void Update()
        {
            this.transform.LookAt(target.transform);
            this.transform.Translate(Vector3.forward * Time.deltaTime * speed);
            if (Vector3.Distance(this.transform.position, target.transform.position) < 0.2f)
            {
                target.ApplyDamage(40);
                Destroy(this.gameObject);
            }
        }
        public void Set(Transform source, Entity.Entity target)
        {
            this.source = source;
            this.target = target;
            this.transform.position = source.position;
        }
    }
}

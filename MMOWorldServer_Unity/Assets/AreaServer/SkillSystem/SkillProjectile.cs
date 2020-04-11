using UnityEngine;

namespace Assets.AreaServer.SkillSystem
{
    class SkillProjectile : MonoBehaviour
    {
        public Transform source;
        public Entity.Entity target;
        public float speed;
        SkillItem skill;
        void Update()
        {
            this.transform.LookAt(target.transform);
            this.transform.Translate(Vector3.forward * Time.deltaTime * speed);
            if (Vector3.Distance(this.transform.position, target.transform.position) < 0.2f)
            {
                target.ApplyEffects(skill);
                Destroy(this.gameObject);
            }
        }
        public void Set(Transform source, Entity.Entity target, SkillItem skill)
        {
            this.source = source;
            this.target = target;
            this.transform.position = source.position;
            this.skill = skill;
        }
    }
}

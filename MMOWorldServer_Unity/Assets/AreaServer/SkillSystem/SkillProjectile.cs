using UnityEngine;

namespace Assets.AreaServer.SkillSystem
{
    class SkillProjectile : MonoBehaviour
    {
        public Entity.Entity source;
        public Entity.Entity target;
        public float speed;
        SkillItem skill;
        void Update()
        {
            this.transform.LookAt(target.transform);
            this.transform.Translate(Vector3.forward * Time.deltaTime * speed);
            if (Vector3.Distance(this.transform.position, target.transform.position) < 0.2f)
            {
                target.ApplyEffects(skill, source);
                Destroy(this.gameObject);
            }
        }
        public void Set(Entity.Entity source, Entity.Entity target, SkillItem skill)
        {
            this.source = source;
            this.target = target;
            this.transform.position = source.transform.position;
            this.skill = skill;
        }
    }
}

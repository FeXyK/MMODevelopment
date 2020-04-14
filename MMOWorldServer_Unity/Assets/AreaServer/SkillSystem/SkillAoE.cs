using UnityEngine;
namespace Assets.AreaServer.SkillSystem
{
    class SkillAoE : MonoBehaviour
    {
        public Entity.Entity source;

        public Entity.Entity target;
        public float time;
        public float damageDelay = 0.3f;
        public float range;
        SkillItem skill;
        void Update()
        {
            time += Time.deltaTime;
            if (time > damageDelay)
            {
                foreach (var entity in Physics.OverlapSphere(target.transform.position, range))
                {
                    if (entity.GetComponent<Entity.Entity>() != null)
                    {
                        entity.GetComponent<Entity.Entity>().ApplyEffects(skill, source);
                    }
                }
                Destroy(this.gameObject);
            }
        }
        public void Set(Entity.Entity source, Entity.Entity target, SkillItem skill, float range, float damageDelay)
        {
            this.source = source;
            this.target = target;
            this.transform.position = source.position;
            this.skill = skill;
            this.range = range;
            this.damageDelay = damageDelay;
        }
    }
}

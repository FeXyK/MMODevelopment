using UnityEngine;
namespace Assets.AreaServer.SkillSystem
{
    class SkillAoE : MonoBehaviour
    {
        public Transform source;
        
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
                        entity.GetComponent<Entity.Entity>().ApplyEffects(skill);
                    }
                }
                Destroy(this.gameObject);
            }
        }
        public void Set(Transform source, Entity.Entity target, SkillItem skill, float range, float damageDelay)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AreaServer.SkillSystem
{
    class SkillBuff : SkillBase
    {
        float time;
        public void ApplyEffect()
        {
            time -= Time.deltaTime;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Player", menuName = "PlaneSO", order = 1)]
    public class PlaneSO : ScriptableObject
    {
        
        public float XAngleConstraint = 15;

        public float ZAngleConstraint = 15;

        public float AngleVelocity = 1;
    }
}

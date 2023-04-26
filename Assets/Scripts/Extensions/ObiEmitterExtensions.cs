using Obi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Extensions
{
    public static class ObiEmitterExtensions
    {
        public static int GetMaxPoints(this ObiEmitter emitter)
        {
            var shape = emitter.GetComponentInChildren<ObiEmitterShape>();
            return shape.distribution.Count;
        }
    }
}

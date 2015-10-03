using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicCohesion : DynamicArrive
    {
        public override string Name
        {
            get { return "Cohesion"; }
        }

        public float radius;

        public float FanAngle;

        public override MovementOutput GetMovement()
        {

            Vector3 massCenter = new Vector3();
            float closeBoids = 0.0f;
            foreach(GameObject boid in Flock)
            {
              //  if(this.Character != boid.GetComponent)
            }


            var output = new MovementOutput();

            output.linear = this.Target.position - this.Character.position;

            if (output.linear.sqrMagnitude > 0)
            {
                output.linear.Normalize();
                output.linear *= this.MaxAcceleration;
            }

            return output;
        }
    }
}

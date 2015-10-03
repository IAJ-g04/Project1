using Assets.Scripts.IAJ.Unity.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicFlockVelocityMatch : DynamicVelocityMatch
    {

        public DynamicFlockVelocityMatch()
        {
            this.Target = new KinematicData();
        }


        public override string Name
        {
            get { return "FlockVelocityMatch"; }
        }

        public float radius;

        public float FanAngle;

        public override MovementOutput GetMovement()
        {

            Vector3 averageVelocity = new Vector3();
            float closeBoids = 0.0f;
            foreach(DynamicCharacter boid in Flock)
            {
                if(this.Character != boid.KinematicData)
                {
                    Vector3 direction = boid.KinematicData.position - this.Character.position;
                    if(direction.sqrMagnitude <= radius)
                    {
                        float angle = MathHelper.ConvertVectorToOrientation(direction);
                        float angleDiff = MathHelper.ShortestAngleDifference(this.Character.orientation, angle);
                        if( Math.Abs(angleDiff) <= FanAngle)
                        {
                            averageVelocity += boid.KinematicData.velocity;
                            closeBoids++;
                        }
                    }
                }
            }

            if(closeBoids == 0)
            {
                return new MovementOutput();
            }

            averageVelocity /= closeBoids;
            this.Target.velocity = averageVelocity;

            return base.GetMovement();
        }
    }
}

using Assets.Scripts.IAJ.Unity.Util;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicSeparation : DynamicMovement
    {
        public override string Name
        {
            get { return "Separation"; }
        }

        public override KinematicData Target { get; set; }
        public float SeparationFactor { get; set; }
        public float Radius { get; set; }

        public override MovementOutput GetMovement()
        {
            var output = new MovementOutput();

            foreach (DynamicCharacter boid in Flock)
            {
                if (boid.KinematicData != this.Character)
                {
                    Vector3 direction = this.Character.position - boid.KinematicData.position;
                    float distance = direction.sqrMagnitude;
                    if (distance < Radius)
                    {
                        float separationStrength = Math.Min(SeparationFactor / (distance * distance), MaxAcceleration);
                        direction.Normalize();
                        output.linear += direction * separationStrength;
                    }
                }
            }

            if (output.linear.sqrMagnitude > MaxAcceleration)
                {
                    output.linear.Normalize();
                    output.linear *= MaxAcceleration;
                }        
          
            return output;
        }
    }
}

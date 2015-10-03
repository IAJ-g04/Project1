using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicArrive : DynamicVelocityMatch
    {


        public DynamicArrive()
        {
            this.maxSpeed = 20.0f;
            this.slowRadius = 15.0f;
            this.stopRadius = 3.0f;
        }
        public override string Name
        {
            get { return "Arrive"; }
        }
        public float maxSpeed { get; private set; }

        public float stopRadius { get; private set; }
        public float slowRadius { get; private set; }




        public override MovementOutput GetMovement()
        {
            Vector3 direction = Target.position - Character.position;
            float distance = Vector3.Magnitude(direction);
            float targetSpeed;

            if (distance < stopRadius)
            {
                var output = new MovementOutput();
                return output;

            }
            if (distance > slowRadius)
                targetSpeed = maxSpeed;
            else
                targetSpeed = maxSpeed * (distance / slowRadius);

            this.MovingTarget = new KinematicData();
            this.MovingTarget.velocity = direction.normalized * targetSpeed;


            return base.GetMovement();
        }
    }
}
using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicArrive : DynamicVelocityMatch
    {


        public DynamicArrive()
        {
            Arrived = false;
        }

        public override string Name
        {
            get { return "Arrive"; }
        }
        public float MaxSpeed { get; set; }

        public float StopRadius { get;  set; }
        public float SlowRadius { get; set; }

        public bool Arrived { get; set; }


        public override MovementOutput GetMovement()
        {
            Vector3 direction = Target.position - Character.position;
            float distance = Vector3.Magnitude(direction);
            float targetSpeed;

            if (distance < StopRadius)
            {
                Arrived = true;
                var output = new MovementOutput();
                return output;
            }

            if (distance > SlowRadius)
                targetSpeed = MaxSpeed;
            else
                targetSpeed = MaxSpeed * (distance / SlowRadius);

            this.MovingTarget = new KinematicData();
            this.MovingTarget.velocity = direction.normalized * targetSpeed;


            return base.GetMovement();
        }
    }
}
using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicAvoidObstacle : DynamicSeek
    {
		public DynamicAvoidObstacle()
        {
            this.Target = new KinematicData();
        }

        public override string Name
        {
            get { return "Avoid Obstacle"; }
        }
		
		public float AvoidDistance { get; set; }
		public float LookAhead { get; set; }

		public GameObject Obstacle { get; set; }

        public override MovementOutput GetMovement()
        {


			Ray RayVector = new Ray(this.Character.position, Character.velocity.normalized);

            Ray WhiskerA = new Ray(this.Character.position, MathHelper.Rotate2D(this.Character.velocity, 45.0f));
            Ray WhiskerB = new Ray(this.Character.position, MathHelper.Rotate2D(this.Character.velocity, -45.0f));

            RaycastHit hit;
            RaycastHit hitA;
            RaycastHit hitB;


            Debug.DrawRay(this.Character.position, this.Character.velocity.normalized * LookAhead, new Color(255,0,0));
            Debug.DrawRay(this.Character.position, MathHelper.Rotate2D(this.Character.velocity, 45.0f) * LookAhead * 0.1f, new Color(0, 255, 0));
            Debug.DrawRay(this.Character.position, MathHelper.Rotate2D(this.Character.velocity, -45.0f) * LookAhead * 0.1f, new Color(0, 0, 255));


            if (Obstacle.GetComponent<Collider>().Raycast(RayVector, out hit, LookAhead))
                this.Target.position = hit.point + hit.normal * AvoidDistance;

            else if (Obstacle.GetComponent<Collider>().Raycast(WhiskerA, out hitA, LookAhead * 0.1f))
                this.Target.position = hitA.point + hitA.normal * AvoidDistance;

            else if (Obstacle.GetComponent<Collider>().Raycast(WhiskerB, out hitB, LookAhead * 0.1f))
                this.Target.position = hitB.point + hitB.normal * AvoidDistance;
            else
                return new MovementOutput();

            return base.GetMovement();

        }
    }
}

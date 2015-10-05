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

			Ray RayVector = new Ray(this.Character.position,this.Character.velocity.normalized);

            Ray WhiskerA = new Ray(this.Character.position, MathHelper.Rotate2D(this.Character.velocity, 45.0f));
            Ray WhiskerB = new Ray(this.Character.position, MathHelper.Rotate2D(this.Character.velocity, -45.0f));

            RaycastHit hit = new RaycastHit();
            RaycastHit hitA = new RaycastHit();
            RaycastHit hitB = new RaycastHit();

            bool Collision = Obstacle.GetComponent<Collider>().Raycast(RayVector, out hit, LookAhead);
            bool CollisionA = Obstacle.GetComponent<Collider>().Raycast(WhiskerA, out hit, LookAhead*0.1f);
            bool CollisionB = Obstacle.GetComponent<Collider>().Raycast(WhiskerB, out hit, LookAhead*0.1f);

            Debug.DrawRay(this.Character.position, this.Character.velocity.normalized * LookAhead, new Color(255,0,0));
            Debug.DrawRay(this.Character.position, MathHelper.Rotate2D(this.Character.velocity, 45.0f) * LookAhead * 0.1f, new Color(0, 255, 0));
            Debug.DrawRay(this.Character.position, MathHelper.Rotate2D(this.Character.velocity, -45.0f) * LookAhead * 0.1f, new Color(0, 0, 255));

            if (!Collision && !CollisionA && !CollisionB) return new MovementOutput();

            if(Collision)
             this.Target.position = hit.point + hit.normal * AvoidDistance;

            else if (CollisionA)
                this.Target.position = hitA.point + hitA.normal * AvoidDistance;

            else if (CollisionB)
                this.Target.position = hitB.point + hitB.normal * AvoidDistance;

			return base.GetMovement();

        }
    }
}

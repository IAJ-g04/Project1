using Assets.Scripts.IAJ.Unity.Util;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicWander : DynamicSeek
    {
        public DynamicWander()
        {
            this.TurnAngle = 1.0f;
            this.WanderOffset = 5.0f;
            this.WanderRadius = 3.5f;
        }
        public override string Name
        {
            get { return "Wander"; }
        }
        public float TurnAngle { get; private set; }

        public float WanderOffset { get; private set; }
        public float WanderRadius { get; private set; }

        protected float WanderOrientation { get; set; }
        public System.Random r = new System.Random();

        public override MovementOutput GetMovement()
        {
            var output = new MovementOutput();

               
                
                this.WanderOrientation += TurnAngle * RandomHelper.RandomBinomial();
                this.Target.orientation = WanderOrientation + this.Character.orientation;
                var circleCenter = this.Character.position + WanderOffset * this.Character.GetOrientationAsVector();
                this.Target = new KinematicData(circleCenter + WanderRadius * this.Target.GetOrientationAsVector(), Target.velocity, this.Target.orientation, TurnAngle);
            

            return base.GetMovement();
        }
    }
}

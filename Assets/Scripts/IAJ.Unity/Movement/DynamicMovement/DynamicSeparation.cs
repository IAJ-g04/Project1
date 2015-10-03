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

          
            return output;
        }
    }
}

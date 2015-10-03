using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicAvoidCharacters : DynamicVelocityMatch
    {
        private List<DynamicCharacter> characters;


        public DynamicAvoidCharacters(List<DynamicCharacter> characters)
        {
            this.obstacles = characters;
            tvel = new Vector3(0.0f, 0.0f, 0.0f); ;
        }

        public override string Name
        {
            get { return "AvoidCharacters"; }
        }

        public float MaxLookAhead { get; internal set; }
        public float AvoidMargin { get; internal set; }
        public List<DynamicCharacter> obstacles { get; internal set; }
        public Vector3 tvel { get; internal set; }

        public override MovementOutput GetMovement()
        {
            var output = new MovementOutput();


            float shortestTime = 99999999f;

            float closestMinSeparation = 999990f;
            float closestDistance = 9999999f;
            Vector3 closestDeltaPos = new Vector3();
            Vector3 closestDeltaVel = new Vector3();
            KinematicData closestTarget = new KinematicData();
            Vector3 avoidanceDirection = new Vector3();

            foreach (DynamicCharacter obstacle in obstacles)
            {
                if (obstacle.KinematicData != this.Character)
                {
                    Vector3 tpos = obstacle.KinematicData.position;
                    Vector3 deltapos = tpos - Character.position;
                    Vector3 deltavel = tpos - Character.velocity;
                    float deltaSpeed = deltavel.sqrMagnitude;

                    if (deltaSpeed == 0)
                    {
                        continue;
                    }

                    float timetoclosest = -Vector3.Dot(deltapos, deltavel) / (deltaSpeed * deltaSpeed);

                    if (timetoclosest > MaxLookAhead)
                    {
                        continue;
                    }

                    float distance = deltapos.sqrMagnitude;
                    float minSeparation = distance - deltaSpeed * timetoclosest;



                    if (minSeparation > 2 * AvoidMargin)
                    {
                        continue;
                    }

                    if (timetoclosest > 0 && timetoclosest < shortestTime)
                    {
                        shortestTime = timetoclosest;
                        closestTarget = obstacle.KinematicData;
                        closestMinSeparation = minSeparation;
                        closestDistance = distance;
                        closestDeltaPos = deltapos;
                        closestDeltaVel = deltavel;
                    }
                }
            }







            if (shortestTime == 99999999f)
            {
                return new MovementOutput();
            }

            if ((closestMinSeparation <= 0f) || closestDistance < 2f * AvoidMargin)
            {
                avoidanceDirection = Character.position - closestTarget.position;
            }
            else
            {
                avoidanceDirection = (closestDeltaPos + closestDeltaVel * shortestTime) * -1;
            }

            output = new MovementOutput();
            output.linear = avoidanceDirection.normalized * MaxAcceleration;
            return output;


        }
    }
}
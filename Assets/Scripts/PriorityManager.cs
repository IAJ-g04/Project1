﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PriorityManager : MonoBehaviour
{
    //Constants
    public const float X_WORLD_SIZE = 55;
    public const float Z_WORLD_SIZE = 32.5f;
    public const float AVOID_MARGIN = 8.0f;
    public const float MAX_SPEED = 20.0f;
    public const float MAX_LOOK_AHEAD = 10.0f;
    public const float MAX_ACCELERATION = 40.0f;
    public const float DRAG = 0.1f;
    public const float FLOCK_SEPARATION_FACTOR = 5.0f;
    public const float FLOCK_RADIUS = 10.0f;
    public const float FLOCK_FAN_ANGLE = 10.0f;

    private Text RedMovementText { get; set; }

    //Characters
    private DynamicCharacter RedCharacter { get; set; }
    private List<DynamicCharacter> Flock { get; set; }

	// Use this for initialization
	void Start () 
	{
		var textObj = GameObject.Find ("InstructionsText");
		if (textObj != null) 
		{
			textObj.GetComponent<Text>().text = 
				"Instructions\n\n" +
                "Q - stop"; 
		}

	    this.RedMovementText = GameObject.Find("RedMovement").GetComponent<Text>();
		var redObj = GameObject.Find ("Red");

	    this.RedCharacter = new DynamicCharacter(redObj)
	    {
	        Drag = DRAG,
	        MaxSpeed = MAX_SPEED
	    };
        
	    var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

	    this.Flock = this.CloneSecondaryCharacters(redObj, 10, obstacles);
	    this.Flock.Add(this.RedCharacter);

        this.InitializeMainCharacter(obstacles);

        //initialize all but the last character (because it was already initialized as the main character)
	    foreach (var character in this.Flock.Take(this.Flock.Count-1))
	    {
	        this.InitializeSecondaryCharacter(character, obstacles);
	    }

	}

    private void InitializeMainCharacter(GameObject[] obstacles)
    {
        BlendedMovement Blended;

       Blended = new BlendedMovement
        {
            Character = this.RedCharacter.KinematicData
        };

        //Obstacles : TO FINISH
        foreach(var obstacle in obstacles){

            //TODO: add your AvoidObstacle movement here
            
            var avoidObstacleMovement = new DynamicAvoidObstacle()
            {
                MaxAcceleration = MAX_ACCELERATION,
                AvoidDistance = AVOID_MARGIN,
                LookAhead = MAX_LOOK_AHEAD,
                Character = this.RedCharacter.KinematicData,
                Obstacle = obstacle,
                MovementDebugColor = Color.magenta
            };
        /*     this.Blended.Movements.Add(new MovementWithWeight(avoidObstacleMovement, 5.0f));
           this.Priority.Movements.Add(avoidObstacleMovement);*/

           }

                //TODO: add your AvoidCharacter movement here
                var avoidCharacter = new DynamicAvoidCharacters(Flock)
                {
                    MaxLookAhead = MAX_LOOK_AHEAD,
                    Character = this.RedCharacter.KinematicData,
                    MaxAcceleration = MAX_ACCELERATION,
                    AvoidMargin = AVOID_MARGIN,
                    MovementDebugColor = Color.cyan 
                };

        /* this.Priority.Movements.Add(avoidCharacter);
   this.Blended.Movements.Add(new MovementWithWeight(avoidCharacter, obstacles.Length + this.Characters.Count));*/

        var DynamicWanderMovement = new DynamicWander
        {
            Character = this.RedCharacter.KinematicData,
            Target = this.RedCharacter.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.yellow
        };

        Blended.Movements.Add(new MovementWithWeight(DynamicWanderMovement, obstacles.Length + this.Flock.Count));

        var DynamicSeparationMovement = new DynamicSeparation()
        {
            Character = this.RedCharacter.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.blue,
            Flock = this.Flock,
            SeparationFactor = FLOCK_SEPARATION_FACTOR,
            Radius = FLOCK_RADIUS
        };

        Blended.Movements.Add(new MovementWithWeight(DynamicSeparationMovement, obstacles.Length + this.Flock.Count));

        var DynamicCohesionMovement = new DynamicCohesion()
        {
            Character = this.RedCharacter.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.blue,
            Flock = this.Flock,
            FanAngle = FLOCK_FAN_ANGLE,
            radius = FLOCK_RADIUS
        };

        Blended.Movements.Add(new MovementWithWeight(DynamicCohesionMovement, obstacles.Length + this.Flock.Count));

        var DynamicFlockVelocityMatchMovement = new DynamicFlockVelocityMatch()
        {
            Character = this.RedCharacter.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.blue,
            Flock = this.Flock,
            FanAngle = FLOCK_FAN_ANGLE,
            radius = FLOCK_RADIUS
        };

        Blended.Movements.Add(new MovementWithWeight(DynamicFlockVelocityMatchMovement, obstacles.Length + this.Flock.Count));

        this.RedCharacter.Movement = Blended;

    }

    private void InitializeSecondaryCharacter(DynamicCharacter character, GameObject[] obstacles)
    {
        BlendedMovement Blended;

        Blended = new BlendedMovement
        {
            Character = character.KinematicData
        };

        //Obstacles : TO FINISH
        foreach (var obstacle in obstacles)
        {
            var avoidObstacleMovement = new DynamicAvoidObstacle()
            {
                MaxAcceleration = MAX_ACCELERATION,
                AvoidDistance = AVOID_MARGIN,
                LookAhead = MAX_LOOK_AHEAD,
                Character = character.KinematicData,
                Obstacle = obstacle,
                MovementDebugColor = Color.magenta
            };

          /*  priority.Movements.Add(avoidObstacleMovement);
            this.Blended.Movements.Add(new MovementWithWeight(avoidObstacleMovement, obstacles.Length + this.Characters.Count));*/

         }

        //TODO: add your avoidCharacter movement here
        var avoidCharacter = new DynamicAvoidCharacters(Flock)
                    {
                        Character = character.KinematicData,
                        MaxAcceleration = MAX_ACCELERATION,
                        MaxLookAhead = MAX_LOOK_AHEAD,
                        AvoidMargin = AVOID_MARGIN,
                        MovementDebugColor = Color.cyan
                    };

        /*priority.Movements.Add(avoidCharacter);
        this.Blended.Movements.Add(new MovementWithWeight(avoidCharacter, obstacles.Length + this.Characters.Count));*/

        var DynamicWanderMovement = new DynamicWander
        {
            Character = character.KinematicData,
            Target = character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.yellow
        };

        Blended.Movements.Add(new MovementWithWeight(DynamicWanderMovement, obstacles.Length + this.Flock.Count));

        var DynamicSeparationMovement = new DynamicSeparation()
        {
            Character = character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.blue,
            Flock = this.Flock,
            SeparationFactor = FLOCK_SEPARATION_FACTOR,
            Radius = FLOCK_RADIUS
        };

        Blended.Movements.Add(new MovementWithWeight(DynamicSeparationMovement, obstacles.Length + this.Flock.Count));

        var DynamicCohesionMovement = new DynamicCohesion()
        {
            Character = character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.blue,
            Flock = this.Flock,
            FanAngle = FLOCK_FAN_ANGLE,
            radius = FLOCK_RADIUS
        };

        Blended.Movements.Add(new MovementWithWeight(DynamicCohesionMovement, obstacles.Length + this.Flock.Count));

        var DynamicFlockVelocityMatchMovement = new DynamicFlockVelocityMatch()
        {
            Character = character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.blue,
            Flock = this.Flock,
            FanAngle = FLOCK_FAN_ANGLE,
            radius = FLOCK_RADIUS
        };

        Blended.Movements.Add(new MovementWithWeight(DynamicFlockVelocityMatchMovement, obstacles.Length + this.Flock.Count));

        character.Movement = Blended;
    }

    private List<DynamicCharacter> CloneSecondaryCharacters(GameObject objectToClone,int numberOfCharacters, GameObject[] obstacles)
    {
        var characters = new List<DynamicCharacter>();
        for (int i = 0; i < numberOfCharacters; i++)
        {
            var clone = GameObject.Instantiate(objectToClone);
            //clone.transform.position = new Vector3(30,0,i*20);
            clone.transform.position = this.GenerateRandomClearPosition(obstacles);
            var character = new DynamicCharacter(clone)
            {
                MaxSpeed = MAX_SPEED,
                Drag = DRAG
            };
            //character.KinematicData.orientation = (float)Math.PI*i;
            characters.Add(character);
        }

        return characters;
    }


    private Vector3 GenerateRandomClearPosition(GameObject[] obstacles)
    {
        Vector3 position = new Vector3();
        var ok = false;
        while (!ok)
        {
            ok = true;

            position = new Vector3(Random.Range(-X_WORLD_SIZE,X_WORLD_SIZE), 0, Random.Range(-Z_WORLD_SIZE,Z_WORLD_SIZE));

            foreach (var obstacle in obstacles)
            {
                var distance = (position - obstacle.transform.position).magnitude;

                //assuming obstacle is a sphere just to simplify the point selection
                if (distance < obstacle.transform.localScale.x + AVOID_MARGIN)
                {
                    ok = false;
                    break;
                }
            }
        }

        return position;
    }

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Q)) 
		{
			this.RedCharacter.Movement = null;
		} 

	    foreach (var character in this.Flock)
	    {
	        this.UpdateMovingGameObject(character);
	    }

        this.UpdateMovementText();
	}

    private void UpdateMovingGameObject(DynamicCharacter movingCharacter)
    {
        if (movingCharacter.Movement != null)
        {
            movingCharacter.Update();
            movingCharacter.KinematicData.ApplyWorldLimit(X_WORLD_SIZE,Z_WORLD_SIZE);
            movingCharacter.GameObject.transform.position = movingCharacter.Movement.Character.position;
        }
    }

    private void UpdateMovementText()
    {
        if (this.RedCharacter.Movement == null)
        {
            this.RedMovementText.text = "Red Movement: Stationary";
        }
        else
        {
            this.RedMovementText.text = "Red Movement: " + this.RedCharacter.Movement.Name;
        }
    }
}

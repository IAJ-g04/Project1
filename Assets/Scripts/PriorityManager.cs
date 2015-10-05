using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Assets.Scripts.IAJ.Unity.Movement;

public class PriorityManager : MonoBehaviour
{
    //Constants
    public const float CAMERA_Y = 55.8f;
    public const float X_WORLD_SIZE = 55;
    public const float Z_WORLD_SIZE = 32.5f;
    public const float AVOID_MARGIN =12.0f;
    public const float MAX_SPEED = 40.0f;
    public const float MAX_LOOK_AHEAD = 10.0f;
    public const float DRAG = 0.1f;
    public const float MAXSPEED = 30.0f;
    public const float SLOWRADIUS = 15.0f;
    public const float STOPRADIUS = 3.0f;
    public const float FLOCK_PERCENTAGE = 0.25f;

    //Cohesion
    public const float FLOCK_COHESION_RADIUS = 10.0f;
    public const float FLOCK_COHESION_FAN_ANGLE = 25.0f;

    //Separation
    public const float FLOCK_SEPARATION_FACTOR = 300.0f;
    public const float FLOCK_SEPARATION_RADIUS = 48.0f;
    public const float MAX_ACCELERATION = 60.0f;

    //VelocityMatching
    public const float FLOCK_VELOCITYMATCHING_RADIUS = 5.0f;
    public const float FLOCK_VELOCITYMATCHING_FAN_ANGLE = 35.0f;

    //Blended
    float COHESIONB = 7f;
    float SEPARATIONB = 4f;
    float ALIGNB = 6f;
    float ARRIVEB = 4f;
    float AVOIDOB = 9f;

    private Text RedMovementText { get; set; }

    private GameObject[] obstacles;

    //Characters
    private DynamicCharacter RedCharacter { get; set; }
    private List<DynamicCharacter> Flock { get; set; }

    // Use this for initialization
    void Start()
    {
        var textObj = GameObject.Find("InstructionsText");
        if (textObj != null)
        {
            textObj.GetComponent<Text>().text =
                "Instructions\n\n" +
                "Q - stop";
        }

        this.RedMovementText = GameObject.Find("RedMovement").GetComponent<Text>();
        var redObj = GameObject.Find("Red");



        this.RedCharacter = new DynamicCharacter(redObj)
        {
            Drag = DRAG,
            MaxSpeed = MAX_SPEED

        };
        
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        this.Flock = this.CloneSecondaryCharacters(redObj, 50, obstacles);
        this.Flock.Add(this.RedCharacter);
        
        
        foreach (var character in this.Flock.Take(this.Flock.Count))
        {
            this.InitializeCharacter(character, obstacles);
        }


    }

    private void InitializeCharacter(DynamicCharacter character, GameObject[] obstacles)
    {
        BlendedMovement Blended;

        Blended = new BlendedMovement
        {
            Character = character.KinematicData
        };
        
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
            
              Blended.Movements.Add(new MovementWithWeight(avoidObstacleMovement, (obstacles.Length + this.Flock.Count) * AVOIDOB));

        }
     
        var DynamicSeparationMovement = new DynamicSeparation()
        {
            Character = character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.blue,
            Flock = this.Flock,
            SeparationFactor = FLOCK_SEPARATION_FACTOR,
            Radius = FLOCK_SEPARATION_RADIUS
        };

        Blended.Movements.Add(new MovementWithWeight(DynamicSeparationMovement, (obstacles.Length + this.Flock.Count) * SEPARATIONB));

        var DynamicCohesionMovement = new DynamicCohesion()
        {
            Character = character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.blue,
            Flock = this.Flock,
            FanAngle = FLOCK_COHESION_FAN_ANGLE,
            radius = FLOCK_COHESION_RADIUS
        };

        Blended.Movements.Add(new MovementWithWeight(DynamicCohesionMovement, (obstacles.Length + this.Flock.Count) * COHESIONB));

        var DynamicFlockVelocityMatchMovement = new DynamicFlockVelocityMatch()
        {
            Character = character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.blue,
            Flock = this.Flock,
            FanAngle = FLOCK_VELOCITYMATCHING_FAN_ANGLE,
            radius = FLOCK_VELOCITYMATCHING_RADIUS
        };

        Blended.Movements.Add(new MovementWithWeight(DynamicFlockVelocityMatchMovement, (obstacles.Length + this.Flock.Count) * ALIGNB));

        character.Movement = Blended;
    }

    private List<DynamicCharacter> CloneSecondaryCharacters(GameObject objectToClone, int numberOfCharacters, GameObject[] obstacles)
    {
        var characters = new List<DynamicCharacter>();
        for (int i = 0; i < numberOfCharacters; i++)
        {
            var clone = GameObject.Instantiate(objectToClone);
            //clone.transform.position = new Vector3(30,0,i*20);
            clone.transform.position = this.GenerateRandomClearPosition(obstacles);
            var character = new DynamicCharacter(clone)
            {
                MaxSpeed = Random.Range(10.0F, MAX_SPEED),
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

            position = new Vector3(Random.Range(-X_WORLD_SIZE, X_WORLD_SIZE), 0, Random.Range(-Z_WORLD_SIZE, Z_WORLD_SIZE));

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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            this.RedCharacter.Movement = null;
        }

        Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Vector3 PointInWorld = new Vector3();

        bool buttonPressed = false;

        if (Input.GetMouseButton(0))
        {
            var mousePos = Input.mousePosition;
            mousePos.z = CAMERA_Y;
            PointInWorld = camera.ScreenToWorldPoint(mousePos);
            buttonPressed = true;
        }


        int FlockArriveCount = 0;

        foreach (var character in this.Flock)
        {
            BlendedMovement movement = (BlendedMovement)character.Movement;
            MovementWithWeight arriveSearch = movement.Movements.Find(x => x.Movement.Name == "Arrive");

            if (arriveSearch != null)
            {
                DynamicArrive DynamicArriveMovement = (DynamicArrive)arriveSearch.Movement;
                if (DynamicArriveMovement.Arrived)
                {
                    FlockArriveCount++;
                }

            }
        }


        foreach (var character in this.Flock)
        {
            BlendedMovement movement = (BlendedMovement)character.Movement;
            MovementWithWeight arriveSearch = movement.Movements.Find(x => x.Movement.Name == "Arrive");

            if (buttonPressed)
            {
                MovementWithWeight wanderSearch = movement.Movements.Find(x => x.Movement.Name == "Wander");
                if (wanderSearch != null)
                {
                    movement.Movements.Remove(wanderSearch);
                }

                var DynamicArriveMovement = new DynamicArrive()
                {
                    Character = character.KinematicData,
                    MaxAcceleration = MAX_ACCELERATION,
                    MovementDebugColor = Color.blue,
                    Flock = this.Flock,
                    Target = new KinematicData(),
                    MaxSpeed = MAXSPEED,
                    SlowRadius = 0,
                    StopRadius = (STOPRADIUS*Flock.Count)/20
                };

                if (arriveSearch != null)
                {
                    movement.Movements.Remove(arriveSearch);
                }

                DynamicArriveMovement.Target.position = PointInWorld;
                DynamicArriveMovement.Target.position.y = character.KinematicData.position.y;
                movement.Movements.Add(new MovementWithWeight(DynamicArriveMovement, (obstacles.Length + this.Flock.Count) * ARRIVEB));

                character.Movement = movement;
            }
            
                        if (arriveSearch != null)
                        {
                            DynamicArrive DynamicArriveMovement = (DynamicArrive)arriveSearch.Movement;
                            if (FlockArriveCount >= this.Flock.Count)
                            {
                                movement.Movements.Remove(arriveSearch);
                    /*

                                var DynamicWanderMovement = new DynamicWander
                                {
                                    Character = character.KinematicData,
                                    Target = character.KinematicData,
                                    MaxAcceleration = MAX_ACCELERATION,
                                    MovementDebugColor = Color.yellow
                                };
                    
                                movement.Movements.Add(new MovementWithWeight(DynamicWanderMovement, obstacles.Length + this.Flock.Count));
                    */
                                character.Movement = movement;
                            }

                        }
                           

            this.UpdateMovingGameObject(character);
        }

        this.UpdateMovementText();
    }

    private void UpdateMovingGameObject(DynamicCharacter movingCharacter)
    {
        if (movingCharacter.Movement != null)
        {
            movingCharacter.Update();
            movingCharacter.KinematicData.ApplyWorldLimit(X_WORLD_SIZE, Z_WORLD_SIZE);
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

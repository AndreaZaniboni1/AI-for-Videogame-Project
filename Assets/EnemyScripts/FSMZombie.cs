using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class FSMZombie : MonoBehaviour
{
    private string myScene;

    public float roamingSpeed = 3.5f;
    public float chasingSpeed = 5.5f;
    public float wanderRadius = 20f;
    public float wanderTimer = 0.08f;
    private float timer;

    public bool isTaunted = false;
    public bool keyTaken = false;
    public Vector3 chalicePosition;

    public float reactionTime = 0.2f;
    private FSM fsm;
    private FSM fsmBattery;

    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    private FieldOfView myFOV;
    [Range(0, 360)]
    public float fovAngle = 159;
    public float fovRadius = 10;
    public string targetTag = "Player";

    public float batteryLifeMax = 1f;
    public float batteryLife;
    public float batteryLifeMin = 0.01f;
    public float drainRate = 500f;
    public float chargeRate = 5000f;
    private Renderer myRenderer;
    public string enemyID;
    private GameObject otherEnemy;
    private float distanceFromOtherEnemy;


    public bool chaseTarget = false;
    public float stoppingD = 3.5f;
    public float delayBeweenAttacks = 2f;
    public float attackCooldown;
    private float distanceFromTarget;
    private Vector3 playerLast;

    private Renderer[] eyes;
    public int damage;
    private PlayerHealt playerHealth;
    private Coroutine LookCoroutine;
    private float laserCooldown;
    private float delayBetweenLasers = 0.05f;
    private float rotatingSpeed = 1.5f;
    private LineRenderer laserLine;
    private Color myColor;

    private float wallCheckRadius = 2.5f;
    private float attackWallCheckRadius = 5f;
    private float shootingStoppingD = 12.5f;
    private float slowedSpeed = 3.5f;
    private float fasterSpeed = 6f;


    public bool seesPlayer;

    void Start()
    {
        timer = wanderTimer;

        myScene = SceneManager.GetActiveScene().name;
        if (myScene=="GodModeScene")
        {
            damage = 0;
        }
        else
        {
            damage = 10;
        }

        batteryLife = batteryLifeMax;
        myRenderer = gameObject.GetComponent<Renderer>();
        enemyID = this.name;
        if (enemyID == "Enemy1")
        {
            myColor = new Color(1, 0, 0, 1);
            otherEnemy = GameObject.Find("Enemy2");
            chasingSpeed = 5.5f;
            fovRadius = 14;


        }
        else
        {
            myColor = new Color(0, 1, 0, 1);
            otherEnemy = GameObject.Find("Enemy1");
            chasingSpeed = 4f;
            fovRadius = 17;
        }

        myRenderer.material.color = myColor;

        agent = GetComponent<NavMeshAgent>();
        myFOV = GetComponent<FieldOfView>();
        agent.stoppingDistance = stoppingD;
        agent.stoppingDistance = stoppingD;
        myFOV.angle = fovAngle;
        myFOV.radius = fovRadius;
        attackCooldown = Time.time;
        laserCooldown = Time.time;


        whatIsGround = LayerMask.GetMask("Ground");
        whatIsPlayer = LayerMask.GetMask("Player");
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealt>();
        eyes = gameObject.GetComponentsInChildren<Renderer>();
        laserLine = this.gameObject.AddComponent<LineRenderer>();




        FSMState roaming = new FSMState();
        roaming.stayActions.Add(Roaming);


        FSMState chasing = new FSMState();
        chasing.stayActions.Add(Chasing);
        chasing.exitActions.Add(Around);


        FSMState checking = new FSMState();
        checking.stayActions.Add(Checking);

        FSMTransition t1 = new FSMTransition(SeesPlayer);
        FSMTransition t2 = new FSMTransition(LosePlayer);
        FSMTransition t3 = new FSMTransition(HearNoise);

        roaming.AddTransition(t1, chasing);
        roaming.AddTransition(t3, checking);

        chasing.AddTransition(t2, roaming);

        checking.AddTransition(t1, chasing);
        checking.AddTransition(t2, roaming);

        fsm = new FSM(roaming);



        FSMState drainingState = new FSMState();
        drainingState.stayActions.Add(fsm.Update);

        FSMState drainedState = new FSMState();
        drainedState.stayActions.Add(GoCharge);

        FSMTransition t11 = new FSMTransition(Drained);
        FSMTransition t22 = new FSMTransition(FullBattery);

        drainingState.AddTransition(t11, drainedState);
        drainedState.AddTransition(t22, drainingState);

        fsmBattery = new FSM(drainingState);

        StartCoroutine(Patrol());

    }

    public IEnumerator Patrol()
    {
        while (true)
        {
            fsmBattery.Update();
            Draining();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    // CONDITIONS

    public bool SeesPlayer() 
    {
        {
            if (myFOV.seesPlayer == true || myFOV.seesPlayerClose == true)
            {
                return true;
            }
            else return false;

        }
    }

    private bool LosePlayer()
    {
        if (isTaunted == false && SeesPlayer() == false)
        {
            return !SeesPlayer();
        }
        else return false;

    }
    private bool HearNoise()
    {
        return isTaunted;
    }

    private bool Drained()
    {
        if (batteryLife <= batteryLifeMin)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
    private bool FullBattery()
    {
        if (batteryLife >= batteryLifeMax)
        {
            return true;
        }
        else
        {
            return false;
        }

    }


    // ACTIONS


    public void Draining()
    {

        if (batteryLife > batteryLifeMin)
        {
            batteryLife -= Time.deltaTime * (drainRate / 1000);

            if (enemyID == "Enemy1")
            {
                myColor = new Color(batteryLife, 0, 0, 1);
            }
            else
            {
                myColor = new Color(0, batteryLife, 0, 1);
            }
            myRenderer.material.color = myColor;
        }
        else
        {
            batteryLife = batteryLifeMin;
        }


    }

    public void GoCharge()
    {
        agent.speed = chasingSpeed;
        distanceFromOtherEnemy =  Vector3.Distance(this.transform.position, otherEnemy.transform.position);
        agent.SetDestination(otherEnemy.transform.position);
        foreach (Renderer eye in eyes)
        {

            if (eye.gameObject.transform.parent != null)
            {
                eye.material.color = Color.white;
            }

        }
        if (distanceFromOtherEnemy <= 4)
        {
            agent.speed = 0;
            if (batteryLife < batteryLifeMax)
            {
                batteryLife += Time.deltaTime * (chargeRate / 1000);

                if (enemyID == "Enemy1")
                {
                    myColor = new Color(batteryLife, 0, 0, 1);
                }
                else
                {
                    myColor = new Color(0, batteryLife, 0, 1);
                }
                myRenderer.material.color = myColor;
            }
            else
            {
                batteryLife = batteryLifeMax;
            }
        }
    }

    public void Chasing()
    {
        
        Debug.Log("Chasing");
        

        chaseTarget = true;

        
        foreach (Renderer eye in eyes)
        {
            
            if (eye.gameObject.transform.parent != null)
            {
                eye.material.color = Color.red;
            }

        }
        Collider[] wallsAroundCheck = Physics.OverlapSphere(transform.position, wallCheckRadius, whatIsGround);
        if (enemyID == "Enemy1")
        {

            if (wallsAroundCheck.Length > 4)
            {
                agent.speed = slowedSpeed;
            }
            else
            {
                agent.speed = chasingSpeed;
            }
        }
        else
        {
            if (Time.time > laserCooldown)
            {
                laserLine.enabled = false;
            }

            if (wallsAroundCheck.Length > 4)
            {
                agent.speed = fasterSpeed;
            }
            else
            {
                
                agent.speed = chasingSpeed;
                wallsAroundCheck = Physics.OverlapSphere(transform.position, attackWallCheckRadius, whatIsGround);
                if (wallsAroundCheck.Length == 0)
                {
                    agent.stoppingDistance = shootingStoppingD;
                }
                else
                {
                    agent.stoppingDistance = stoppingD;
                }
            }

        }


        if (chaseTarget == true)
        {
            agent.SetDestination(player.position);

        }
        distanceFromTarget = agent.remainingDistance;
        if (distanceFromTarget <= agent.stoppingDistance)
        {
            chaseTarget = false;
            Attack();
        }
        if (distanceFromTarget > stoppingD)
        {
            chaseTarget = true;
        }

        myFOV.angle = 360;

        playerLast = player.position;


    }


    public void Attack()
    {
        if (Time.time > laserCooldown)
        {
            laserLine.enabled = false;
        }
        StartRotating();
        if (Time.time > attackCooldown)
        {
            foreach (Renderer eye in eyes)
            {

                if (eye.gameObject.transform.parent != null)
                {
                    eye.material.color = Color.yellow;
                }

            }
            if (enemyID == "Enemy2")
            {
                laserLine.enabled = true;
                laserLine.startWidth = 0.02f;
                laserLine.startColor = Color.yellow;
                laserLine.SetPosition(0, transform.position);
                laserLine.SetPosition(1, player.transform.position);

            }



            playerHealth.TakeDamage(damage);
            attackCooldown = Time.time + delayBeweenAttacks;
            laserCooldown = Time.time + delayBetweenLasers;


            Debug.Log("Attacking");

        }
    }


    public void StartRotating()
    {
        if (LookCoroutine != null)
        {
            StopCoroutine(LookCoroutine);
        }

        LookCoroutine = StartCoroutine(LookAt());
    }
    private IEnumerator LookAt()
    {

        float time = 0;

        Quaternion initialRotation = transform.rotation;
        while (time < 1)
        {
            Vector3 lookPos = player.transform.position - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, time);

            time += Time.deltaTime * rotatingSpeed;

            yield return null;
        }
    }
    private void Around()
    {
        foreach (Renderer eye in eyes)
        {

            if (eye.gameObject.transform.parent != null)
            {
                eye.material.color = Color.white;
            }

        }
        Debug.Log("Looking around");
        chaseTarget = false;
        agent.SetDestination(playerLast);

    }

    private void Roaming()
    {
        agent.speed = roamingSpeed;
        agent.stoppingDistance = stoppingD;
        Debug.Log("Roaming");
        myFOV.angle = fovAngle;


        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            if (keyTaken == true)
            {
                Vector3 newPos = RandomNavSphere(player.position, wanderRadius+3, -1);
                agent.SetDestination(newPos);
                timer = 0;
            }
            else
            {

                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;

            }
        }
    }

    private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
    private void Checking()
    {
        
        myFOV.angle = fovAngle;
        Debug.Log("Checking");

        agent.SetDestination(chalicePosition);


    }


}

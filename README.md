# Spider tutorial EP 1

Video: https://youtu.be/EdjAYrssxDM

Hope you enjoyed the video!

if you have any questions please don't hesitate to join the discord. You are all welcome!

Discord: https://discord.gg/pVQsS79FQh

OrangeSpider Asset: https://assetstore.unity.com/packages/3d/characters/robots/spider-orange-181154

Hope you enjoy this series!

code used will be below if you dont want to dig into the files!




















Body Controller code:
----------------------


public class bodyController : MonoBehaviour
{

    Vector3 velocity;
    Vector3 lastVelocity = Vector3.one;
    Vector3 lastSpiderPosition;
    Vector3[] legPositions;
    Vector3[] legOriginalPositions;
    List<int> nextIndexToMove = new List<int>();
    List<int> IndexMoving = new List<int>();
    Vector3 lastBodyUp;
    List<int> oppositeLeg = new List<int>();
    bool currentLeg = true;
    float resetTimer = 0.5f;

    [Space(10)]
    [Header("GameObject Assignment")]
    [Space(10)]

    public GameObject spider;
    public GameObject[] legTargets;
    public GameObject[] legCubes;

    [Space(10)]
    [Header("Rotation of Body and Movment of leg")]
    [Space(10)]

    public bool enableBodyRotation = false;
    public bool enableMovementRotation = false;
    public bool rigidBodyController;

    [Space(10)]
    [Header("Values for leg Movement")]
    [Space(10)]

    public float moveDistance = 0.7f;
    public float stepHeight = .15f;
    public float spiderJitterCutOff = 0f;
    public int waitTimeBetweenEveryStep = 0;
    public float LegSmoothness = 8;
    public float BodySmoothness = 8;
    public float OverStepMultiplier = 4;


    void Start()
    {
        lastBodyUp = transform.up;

        legPositions = new Vector3[legTargets.Length];
        legOriginalPositions = new Vector3[legTargets.Length];

        for (int i = 0; i < legTargets.Length; i++)
        {
            legPositions[i] = legTargets[i].transform.position;
            legOriginalPositions[i] = legTargets[i].transform.position;

            if (currentLeg) { oppositeLeg.Add(i + 1); currentLeg = false; }
            else if (!currentLeg) { oppositeLeg.Add(i - 1); currentLeg = true; }
        }

        lastSpiderPosition = spider.transform.position;

        rotateBody();
    }


    void FixedUpdate()
    {
        velocity = spider.transform.position - lastSpiderPosition;
        velocity = (velocity + BodySmoothness * lastVelocity) / (BodySmoothness + 1f);

        moveLegs();
        rotateBody();


        lastSpiderPosition = spider.transform.position;
        lastVelocity = velocity;
    }

    void moveLegs()
    {
        if (!enableMovementRotation) return;
        for (int i = 0; i < legTargets.Length; i++)
        {
            if (Vector3.Distance(legTargets[i].transform.position, legCubes[i].transform.position) >= moveDistance)
            {
                if (!nextIndexToMove.Contains(i) && !IndexMoving.Contains(i)) nextIndexToMove.Add(i);
            }
            else if (!IndexMoving.Contains(i))
            {
                legTargets[i].transform.position = legOriginalPositions[i];
            }

        }

        if (nextIndexToMove.Count == 0 || IndexMoving.Count != 0) return;
        Vector3 targetPosition = legCubes[nextIndexToMove[0]].transform.position + Mathf.Clamp(velocity.magnitude * OverStepMultiplier, 0.0f, 1.5f) * (legCubes[nextIndexToMove[0]].transform.position - legTargets[nextIndexToMove[0]].transform.position) + velocity * OverStepMultiplier;
        StartCoroutine(step(nextIndexToMove[0], targetPosition, false));
    }

    IEnumerator step(int index, Vector3 moveTo, bool isOpposite)
    {
        if (!isOpposite) moveOppisteLeg(oppositeLeg[index]);

        if (nextIndexToMove.Contains(index)) nextIndexToMove.Remove(index);

        if (!IndexMoving.Contains(index)) IndexMoving.Add(index);

        Vector3 startPos = legOriginalPositions[index];

        for (int i = 1; i <= LegSmoothness; ++i)
        {
            legTargets[index].transform.position = Vector3.Lerp(startPos, moveTo + new Vector3(0, Mathf.Sin(i / (float)(LegSmoothness + spiderJitterCutOff) * Mathf.PI) * stepHeight, 0), (i / LegSmoothness + spiderJitterCutOff));
            yield return new WaitForFixedUpdate();
        }


        legOriginalPositions[index] = moveTo;


        for (int i = 1; i <= waitTimeBetweenEveryStep; ++i) yield return new WaitForFixedUpdate();

        if (IndexMoving.Contains(index)) IndexMoving.Remove(index);

    }

    void moveOppisteLeg(int index)
    {
        Vector3 targetPosition = legCubes[index].transform.position + Mathf.Clamp(velocity.magnitude * OverStepMultiplier, 0.0f, 1.5f) * (legCubes[index].transform.position - legTargets[index].transform.position) + velocity * OverStepMultiplier;
        StartCoroutine(step(index, targetPosition, true));
    }

    void rotateBody()
    {
        if (!enableBodyRotation) return;

        Vector3 v1 = legTargets[0].transform.position - legTargets[1].transform.position;
        Vector3 v2 = legTargets[2].transform.position - legTargets[3].transform.position;
        Vector3 normal = Vector3.Cross(v1, v2).normalized;
        Vector3 up = Vector3.Lerp(lastBodyUp, normal, 1f / (float)(BodySmoothness));
        transform.up = up;
        if (!rigidBodyController) transform.rotation = Quaternion.LookRotation(transform.parent.forward, up);
        lastBodyUp = transform.up;
    }
}
---------------
  
  
  
Leg aim grounding code:
-------------------------

public class LegAimGrounding : MonoBehaviour
{

    GameObject raycastOrigin;
    int layerMask;
    void Start()
    {
        layerMask = LayerMask.GetMask("Ground");
        raycastOrigin = transform.parent.gameObject;
    }

    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(raycastOrigin.transform.position, -transform.up, out hit, Mathf.Infinity, layerMask))
        {
            transform.position = hit.point;
        }
    }
}
-----------------------------------
  
  
  
  
  
  
basic spider controller:
-------------------------
public class simpleController : MonoBehaviour
{

    public Rigidbody cntroller;
    public Transform cam;

    public float speed = 6;
    Vector3 velocity;

    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    void Update()
    {

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f && cntroller.velocity.magnitude < 4)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            cntroller.AddForce(moveDir.normalized * speed * Time.deltaTime);
        }
    }
}
      
------------------------------------------------------------------
  

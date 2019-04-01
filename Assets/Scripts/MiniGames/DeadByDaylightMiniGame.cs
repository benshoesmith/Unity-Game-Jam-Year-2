using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadByDaylightMiniGame : MiniGame {

    [Header("Meshes")]
    [SerializeField]
    private MeshFilter fullArea_ = null;
    [SerializeField]
    private MeshFilter normalArea_ = null;
    [SerializeField]
    private MeshFilter critArea_ = null;
    [SerializeField]
    private MeshFilter playerArea_ = null;

    private Mesh fullAreaMesh_ = null;
    private Mesh normalAreaMesh_ = null;
    private Mesh critAreaMesh_ = null;
    private Mesh playerAreaMesh_ = null;

    [Range(0.1f, 100.0f)]
    [SerializeField]
    private float radius_ = 50.0f;
    [Range(0.1f, 100.0f)]
    [SerializeField]
    private float thickness_ = 20.0f;

    [Range(0.01f, 1.0f)]
    [SerializeField]
    private float difficultyRatio = 0.1f;

    [Range(0.01f, 1.0f)]
    [SerializeField]
    private float critRatio_ = 0.1f;

    [Range(0.0001f, 1.0f)]
    [SerializeField]
    private float playerRatio_ = 0.1f;

    [Range(0.0f, 1000.0f)]
    [SerializeField]
    private float playerAngularSpeed_ = 1.0f;

    private float startOffset_ = 0.0f;

    private void Awake()
    {
        UpdateMeshes();
        startOffset_ =  (difficultyRatio * Mathf.PI * 2);

        playerArea_.transform.rotation = Quaternion.AngleAxis(startOffset_*Mathf.Rad2Deg, -Vector3.forward);

    }

    private float GetCurrentPlayerAngle()
    {
        float playerAngle = 0.0f;
        Vector3 axis;
        playerArea_.transform.rotation.ToAngleAxis(out playerAngle, out axis);

        playerAngle *= Mathf.Deg2Rad;

        if (axis == Vector3.forward)
            playerAngle = Mathf.PI * 2 - playerAngle;

        return Mathf.Repeat(playerAngle, Mathf.PI * 2);
    }

    private void UpdateMeshes()
    {
        
        fullAreaMesh_ = CreateCircleSegment(radius_, 1.0f, thickness_, 1);
        normalAreaMesh_ = CreateCircleSegment(radius_, difficultyRatio, thickness_, 1);
        critAreaMesh_ = CreateCircleSegment(radius_, difficultyRatio * critRatio_, thickness_, 1);
        playerAreaMesh_ = CreateCircleSegment(radius_, playerRatio_, thickness_, 1);



        if (fullArea_)
            fullArea_.mesh = fullAreaMesh_;
        if (normalArea_)
            normalArea_.mesh = normalAreaMesh_;
        if (critArea_)
            critArea_.mesh = critAreaMesh_;
        if (playerArea_)
            playerArea_.mesh = playerAreaMesh_;
    }

    private void FixedUpdate()
    {
        // UpdateMeshes();

        
        if (!Started || Finished)
            return;

        //if player area has moved more than a full cycle.
        if(playerAngularSpeed_*(Time.time -  TimeOfStart) * Mathf.Deg2Rad  > Mathf.PI*2 )
            Fail();

        playerArea_.transform.rotation *= Quaternion.AngleAxis(playerAngularSpeed_*Time.fixedDeltaTime, -Vector3.forward); 
    }

    public Vector3 GetPointOnCircleWithAngle(float angle, float radius)
    {
        return new Vector3(radius * Mathf.Sin(angle), -radius * (1 - Mathf.Cos(angle)), 0) + new Vector3(0, radius,0);

    }

    Mesh CreateCircleSegment(float radius, float segmentRatio, float thickness, float resolutionDistance = 0.1f)
    {
        if (thickness < 0)
            thickness = -thickness;

        thickness = Mathf.Clamp(thickness, 0.01f, 100.0f);
        radius = Mathf.Max(radius, thickness);
        Mesh mesh = new Mesh();

        segmentRatio = Mathf.Clamp(segmentRatio, 0, 1.0f);
        //get angle from 0 to segment end.
        float segmentAngle = segmentRatio * 2 * Mathf.PI;

        float segmentArcLength = (2 * Mathf.PI * radius * segmentRatio);
        //make sure there are atleast 2 points.
        resolutionDistance = Mathf.Clamp(resolutionDistance, 0.01f, segmentArcLength / 2);

        //calculate how many quads the line is split up into.
        int amountOfPointsOnCircle = (int)(segmentArcLength / resolutionDistance);

        //list of mesh points.
        List<Vector3> points = new List<Vector3>();

        //previous points for creating the triangles in the line.
        Vector3 prevOuterPoint = GetPointOnCircleWithAngle(0, radius);


        Vector3 prevInnerPoint = prevOuterPoint + (-prevOuterPoint.normalized * thickness);

        for (int i = 1; i < amountOfPointsOnCircle; i++)
        {
            float angle = segmentAngle * ((float)(i)/amountOfPointsOnCircle);

            Vector3 outerPoint = GetPointOnCircleWithAngle(angle, radius);
            Vector3 innerPoint = outerPoint + (-outerPoint.normalized * thickness);
            
            //triangle 1
            points.Add(prevOuterPoint);
            points.Add(outerPoint);
            points.Add(prevInnerPoint);

            //triangle 2
            points.Add(prevInnerPoint);
            points.Add(outerPoint);
            points.Add(innerPoint);

            prevOuterPoint = outerPoint;
            prevInnerPoint = innerPoint;
        }

        //join if meant to be full circle.
        if(segmentRatio == 1.0f)
        {   
            //triangle 1
            points.Add(prevOuterPoint);
            points.Add(points[0]);
            points.Add(prevInnerPoint);

            //triangle 2
            points.Add(prevInnerPoint);
            points.Add(points[0]);
            points.Add(points[2]);
        }

        mesh.vertices = points.ToArray();

        List<int> triangleIndices = new List<int>();
        for(int i = 0; i < points.Count; i++)
        {
            triangleIndices.Add(i);
        }

        mesh.triangles = triangleIndices.ToArray();

        return mesh;
    }

    private void Update()
    {
        float playerAngle = 0.0f;
        Vector3 axis;
        playerArea_.transform.rotation.ToAngleAxis(out playerAngle, out axis);
        playerAngle *= Mathf.Deg2Rad;
  

        if (Input.GetKeyDown("space") && Started && !Finished)
            MiniGameInteract();
    }

    public override void MiniGameInteract() 
    {

        base.MiniGameInteract();

        float playerEndRatio = GetCurrentPlayerAngle()/(Mathf.PI*2);

       // Debug.Log("Player Angle:" + playerAngle);

        //default value for trying
        score_ = 0.9f;
        //if inside area
        Outcome = GameOutcome.FailWithAttempt;
        if(playerEndRatio < difficultyRatio)
        {
            Outcome = GameOutcome.NormalWin;
            //value if inside area but not crit.
            score_ = 1.2f;
            //if inside the crit area
            if (playerEndRatio < critRatio_ * difficultyRatio)
            {
                score_ = 1.5f;
                Outcome = GameOutcome.BonusWin;
            }
        }

        Debug.Log("Player Ratio:" + playerEndRatio + " Score:" + score_);

        EndGame();
    }

    private void Fail()
    {
        score_ = 0.8f;
        Outcome = GameOutcome.Fail;
        EndGame();
    }
}

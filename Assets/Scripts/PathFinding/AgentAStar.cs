using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(CharacterSpriteController))]
public class AgentAStar : MonoBehaviour {

    [Header("Agent settings")]
    public Rect randomPatrolArea;
    public float turnDistance = 1f;
    public float turnSpeed = 0.5f;
    [Range(0.0f, 1000.0f)]
    public float waitTimeBetweenRandomPos = 3.5f;
    [Header("If the unit does not move for the seconds given a new path will be generated")]
    public const float StuckTime = 5.0f;
    public bool showDebugPath = true;

    private WorldPath m_path;
    private Character m_characterScript;
    private Vector2 m_targetPos;
    private bool m_randomGeneratePos = true;
    private float m_startWaitingTime = 0;

    private void Awake()
    {
        m_characterScript = GetComponent<Character>();
    }

    private void Update()
    {
        if (m_path == null && m_randomGeneratePos && Time.time >= m_startWaitingTime + waitTimeBetweenRandomPos)
        {
            m_targetPos = new Vector2(Random.Range(randomPatrolArea.xMin, randomPatrolArea.xMax), Random.Range(randomPatrolArea.yMin, randomPatrolArea.yMax));
            if (!Physics.Raycast(new Vector3(m_targetPos.x, m_targetPos.y, 1), Vector3.forward)) 
            {
                PathFinderRequestManager.RequestPath(new PathRequest(transform.position, m_targetPos, PathFound));
            }
        }
    }

    public void SetDestinationPos(Vector2 pos)
    {
        m_targetPos = pos;
        m_randomGeneratePos = false;
        StopCoroutine("FollowFoundPath");
        m_path = null;
        PathFinderRequestManager.RequestPath(new PathRequest(transform.position, m_targetPos, PathFound));
    }

    public void SetFollowRandomPos()
    {
        if (m_randomGeneratePos)
            return;
        m_randomGeneratePos = true;
        StopCoroutine("FollowFoundPath");
        m_path = null;
    }

    private void PathFound(Vector3[] waypoints, bool found)
    {
        if (found && gameObject.activeSelf)
        {
            m_path = new WorldPath(waypoints, transform.position, turnDistance);
            StopCoroutine("FollowFoundPath");
            StartCoroutine("FollowFoundPath");
        }
    }

    private IEnumerator FollowFoundPath()
    {
        int currentIndex = 0;
        bool exitPathFollow = false;
        float timeAtSamePos = 0.0f;
        Vector2 lasPost = transform.position;
        while (currentIndex < m_path.PathSize)
        {
            Vector2 currentAgentPos = transform.position;
            while(m_path.HasCrossedNode(currentIndex, currentAgentPos))
            {
                currentIndex++;
                if (currentIndex == m_path.PathSize)
                {
                    exitPathFollow = true;
                    break;
                }
            }
            if (exitPathFollow || timeAtSamePos >= StuckTime)
            {
                break;
            }

            if (lasPost == (Vector2)transform.position)
            {
                timeAtSamePos += Time.deltaTime;
            } else
            {
                timeAtSamePos = 0.0f;
            }
            lasPost = transform.position;

            Vector2 targetDir = m_path.GetNodePos(currentIndex) - m_path.GetNodePos(currentIndex-1);
            Vector2 currentPlayerDir = m_path.GetDirToNodeFrom(currentIndex, currentAgentPos);
            Vector2 dir = Vector2.Lerp(currentPlayerDir, targetDir, Time.deltaTime * turnSpeed).normalized;
            if (dir.sqrMagnitude == 0)
            {
                m_characterScript.SetMovementStatus(Character.MovementStatus.None);
            } else
            {
                m_characterScript.SetMovementStatus(Character.MovementStatus.Walk);
            }
            m_characterScript.SetDirection(dir);

            //Wait until next frame
            yield return null;
        }
        currentIndex = 0;
        m_characterScript.SetDirection(new Vector2(0,0));
        m_characterScript.SetMovementStatus(Character.MovementStatus.None);
        m_path = null;
        m_startWaitingTime = Time.time;
    }

    private void OnDrawGizmos()
    {
        if (!showDebugPath)
            return;
        Gizmos.color = Color.red;
        Vector2 pos = randomPatrolArea.position;
        pos += new Vector2(randomPatrolArea.size.x / 2, randomPatrolArea.size.y / 2);
        Gizmos.DrawWireCube(pos, randomPatrolArea.size);
        if (m_targetPos != null && m_path != null) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(m_targetPos, new Vector3(1, 1, 1));
            m_path.DebugDraw();
        }
    }
}

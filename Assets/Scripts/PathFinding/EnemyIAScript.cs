using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(AgentAStar))]
public class EnemyIAScript : MonoBehaviour {

    public float chasingDistance;
    public Transform playerTransform;

    private Character m_CharacterScript;
    private AgentAStar m_agentScript;

    private void Awake()
    {
        m_CharacterScript = GetComponent<Character>();
        m_agentScript = GetComponent<AgentAStar>();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) < chasingDistance)
        {
            m_agentScript.SetDestinationPos((Vector2)playerTransform.position);
        } else
        {
            m_agentScript.SetFollowRandomPos();
        }
    }
}

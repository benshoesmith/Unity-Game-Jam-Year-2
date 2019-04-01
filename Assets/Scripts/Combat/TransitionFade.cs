using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eTranitionStart
{
    START, END
}

public class TransitionFade : MonoBehaviour {
        
    public Material TransitionMaterial;
    private bool m_fade = false;
    private float m_speed = 0;
    private float m_timeToCompleat = 0;
    private float m_currentThreshold = 0;

    private void Start()
    {
        TransitionMaterial.SetFloat("_Threshold", 0);
    }

    private void Update()
    {
        if (m_fade)
        {
            m_currentThreshold += m_speed * Time.deltaTime;
            m_currentThreshold = Mathf.Clamp01(m_currentThreshold);
            TransitionMaterial.SetFloat("_Threshold", m_currentThreshold);
            if ((Mathf.Sign(m_speed) == -1 && m_currentThreshold == 0) || (Mathf.Sign(m_speed) == 1 && m_currentThreshold == 1) || Mathf.Sign(m_speed) == 0)
            {
                m_fade = false;
            }
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {   
        Graphics.Blit(source, destination, TransitionMaterial);
    }

    public float StartFade(float TimeToCompleat, eTranitionStart whereToStart = eTranitionStart.START)
    {
        m_fade = true;
        m_timeToCompleat = TimeToCompleat;
        switch (whereToStart)
        {
            case eTranitionStart.START:
                m_speed = 1.0f/TimeToCompleat;
                m_currentThreshold = 0;
                TransitionMaterial.SetFloat("_Threshold", m_currentThreshold);
                break;
            case eTranitionStart.END:
                m_speed = -1.0f/TimeToCompleat;
                m_currentThreshold = 1;
                TransitionMaterial.SetFloat("_Threshold", m_currentThreshold);
                break;
        }
        return TimeToCompleat;
    }

    public float GetTimeRemaining()
    {
        float remaining_time = m_currentThreshold * m_timeToCompleat;
        if (Mathf.Sign(m_speed) == 1)
        {
            remaining_time = m_timeToCompleat - remaining_time;
        }
        return remaining_time;
    }

    public void StopFade(eTranitionStart where_to_stop)
    {
        m_fade = false;
        TransitionMaterial.SetFloat("_Threshold", where_to_stop == eTranitionStart.START ? 0.0f : 1.0f);
    }
}

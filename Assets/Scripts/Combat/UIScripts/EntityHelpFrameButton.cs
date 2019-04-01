using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EntityHelpFrameButton : MonoBehaviour, ISelectHandler, IDeselectHandler {

    public GameObject SelectedPointerImg;
    public Image HpSlider;
    public Image MpSlider;
    public Text Name;
    public Text Level;
    [HideInInspector]
    public Character ThisEntity;
    [HideInInspector]
    public CombatUIHandler combatUIHandler;
    private Vector2 m_HPMaxSize;
    private Vector2 m_MPMaxSize;


    private void Start()
    {
        Name.text = ThisEntity.name;
        Level.text = "Level " + ThisEntity.Level;
        m_HPMaxSize = HpSlider.GetComponent<RectTransform>().sizeDelta;
        m_MPMaxSize = MpSlider.GetComponent<RectTransform>().sizeDelta;
    }

    private void Update()
    {
        float HPPorcent = (float)ThisEntity.Health / (float)ThisEntity.MaxHealth;
        HpSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(m_HPMaxSize.x * HPPorcent, m_HPMaxSize.y);
        float MPPorcent = (float)ThisEntity.Mana / (float)ThisEntity.MaxMana;
        MpSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(m_MPMaxSize.x * MPPorcent, m_MPMaxSize.y);
    }

    public void ButtonClick()
    {
        combatUIHandler.EntityClicked(ThisEntity);
        SelectedPointerImg.GetComponent<Animator>().SetBool("IsActive", false);
        SelectedPointerImg.SetActive(false);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SelectedPointerImg.GetComponent<Animator>().SetBool("IsActive", false);
        SelectedPointerImg.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        SelectedPointerImg.SetActive(true);
        SelectedPointerImg.GetComponent<Animator>().SetBool("IsActive", true);
    }
}

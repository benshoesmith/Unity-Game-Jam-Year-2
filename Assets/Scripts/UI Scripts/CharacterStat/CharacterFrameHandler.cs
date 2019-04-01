using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterFrameHandler : MonoBehaviour {
    public Text PlayerNameText;
    public Text LevelText;
    public RectTransform HPFrame;
    public RectTransform MPFrame;
    public RectTransform XPFrame;
    public Text IntellectText;
    public Text DexterityText;
    public Text StrenghtText;
    public Text LightText;
    public Text CritChance;
    public Text CritDamage;

    [SerializeField]
    private Character m_character = null;
    private float m_HPWidth;
    private float m_MPWidth;
    private float m_XPWidth;

    private void Awake()
    {
        if(!m_character)
            m_character = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
    }

    public void Start()
    {
        m_HPWidth = HPFrame.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
        m_MPWidth = MPFrame.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
        m_XPWidth = XPFrame.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
    }

    public void Update()
    {
        //Do not update if it is disabled
        if (!gameObject.activeSelf)
            return;
        //Update level
        LevelText.text = "Level " + m_character.Level;

        //Update the status bar fill element
        UpdateFillAmount(HPFrame, m_HPWidth, m_character.Health, m_character.MaxHealth);
        UpdateFillAmount(MPFrame, m_MPWidth, m_character.Mana, m_character.MaxMana);
        UpdateFillAmount(XPFrame, m_XPWidth, m_character.Xp, m_character.NextLevelXP);


        PlayerNameText.text = m_character.Name;

        //Update the stats
        IntellectText.text = m_character.Intellect.ToString();
        DexterityText.text = m_character.Dexterity.ToString();
        StrenghtText.text = m_character.Strength.ToString();
        LightText.text = m_character.Light.ToString();
        CritChance.text = m_character.CritCance.ToString();
        CritDamage.text = (m_character.CritDamageMult + 1.0f) + "*DPS";
    }

    private void UpdateFillAmount(RectTransform MainText, float maxWidth, float currentAmount, float maxAmount)
    {
        float porcent01 = currentAmount / maxAmount;
        int porcent = Mathf.RoundToInt(porcent01 * 100.0f);

        MainText.transform.GetChild(0).GetComponent<Text>().text = porcent + "%";
        RectTransform colourBar = MainText.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();
        colourBar.sizeDelta = new Vector2(maxWidth * porcent01, colourBar.sizeDelta.y);
    }
}

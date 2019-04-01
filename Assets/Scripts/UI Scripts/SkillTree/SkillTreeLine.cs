using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeLine : MonoBehaviour {

    public Vector2 targetLocation;
    public Color disabledLineColor;
    public Color enabledLineColor;
    public Color unlockedImageColor;

    public enum eLineStatus
    {
        Disabled, Enabled, Unlocked
    }

    private Image img_line;

    private void Awake()
    {
        this.img_line = GetComponent<Image>();
        if (null == this.img_line)
        {
            gameObject.AddComponent<Image>();
        }
    }

    void Start()
    {
        RectTransform thisTrasnform = GetComponent<RectTransform>();
        Vector2 thisPos = new Vector2(thisTrasnform.localPosition.x, thisTrasnform.localPosition.y);
        Vector2 diffVector = this.targetLocation - thisPos;
        thisTrasnform.sizeDelta = new Vector2(diffVector.magnitude, 5);
        float angle = Mathf.Atan2(diffVector.y, diffVector.x) * Mathf.Rad2Deg;
        thisTrasnform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetLineStatus(eLineStatus status)
    {
        switch (status)
        {
            case eLineStatus.Disabled:
                this.img_line.color = this.disabledLineColor;
                break;
            case eLineStatus.Enabled:
                this.img_line.color = this.enabledLineColor;
                break;
            case eLineStatus.Unlocked:
                this.img_line.color = this.unlockedImageColor;
                break;
        }
    }

}

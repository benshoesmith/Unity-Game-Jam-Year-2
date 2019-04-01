using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour {

    [SerializeField]
    private Text text_ = null;
    [SerializeField]
    private Image image = null;
    [SerializeField]
    private Image background_ = null;

    [SerializeField]
    private float lengthUntilRemoved_ = 4.0f;
    private float timeOfCreation_ = 0.0f;
    [SerializeField]
    private float timeToDisappear_ = 1.0f;

    private void Awake()
    {
        timeOfCreation_ = Time.time;
    }

    private void Update()
    {
        float timeSinceNotification = Time.time - timeOfCreation_;

        if (timeSinceNotification > lengthUntilRemoved_)
            Destroy(gameObject);
        else if(timeSinceNotification > lengthUntilRemoved_ - timeToDisappear_)
        {
            float interp = (lengthUntilRemoved_ - timeSinceNotification) / timeToDisappear_;

            image.color = new Color(image.color.r, image.color.g, image.color.b, interp);
            background_.color = new Color(background_.color.r, background_.color.g, background_.color.b, interp);
            text_.color = new Color(text_.color.r, text_.color.g, text_.color.b, interp);
        }
    }

    public Text NotificationText
    {
        get { return text_; }
    }

    public Image NotificationImage
    {
        get { return image; }
        set { image = value; }
    }
}

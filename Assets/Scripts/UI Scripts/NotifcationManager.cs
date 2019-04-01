using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifcationManager : MonoBehaviour {

    private static NotifcationManager singleton_ = null;

    [SerializeField]
    private GameObject notificationPrefab_;

    [SerializeField]
    private GameObject parentNotificationsContent_;

    // Use this for initialization
    void Start ()
    {
        if (!singleton_ || singleton_ == this)
        {
            singleton_ = this;
        }
        else
        {
            Destroy(this);
            Debug.LogError("More than one Notification Manager was created. This is not supported. Remove duplicate from scene.");
        }
    }

    public void AddNotification(string text)
    {
        GameObject notificationGO = Instantiate(notificationPrefab_, parentNotificationsContent_.transform);
        Notification notification = notificationGO.GetComponent<Notification>();

        if(notification)
        {
            notification.NotificationText.text = text;
        }
    }

    public static NotifcationManager Instance
    {
        get { return singleton_; }
    }

}

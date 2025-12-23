using System;
using UnityEngine;

public class MapDoor : MonoBehaviour
{
    [SerializeField] float stayTime;
    [SerializeField] Action stayEvent;
    [SerializeField] float currentStayTime;
    [SerializeField] bool isOnStay;
    public void Initialization(Action _stayEvent)
    {
        this.stayEvent = null;
        this.stayEvent += _stayEvent;
    }
    private void FixedUpdate()
    {
        if (!this.isOnStay)
        {
            this.currentStayTime = Mathf.MoveTowards(this.currentStayTime, 0, Time.fixedDeltaTime);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerScript>(out var t_script))
        {
            this.currentStayTime = Mathf.MoveTowards(this.currentStayTime, this.stayTime, Time.fixedDeltaTime);
            this.isOnStay = true;
            if (this.currentStayTime == this.stayTime)
            {
                this.stayEvent?.Invoke();
                this.currentStayTime = 0;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerScript>(out var t_script))
        {
            this.isOnStay = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Image healthbar;
    public Image healthbarDelta;
    Entity attachedEntity;
    public float deltaTime = 1f;
    [Range(0.01f, 1f)]
    public float deltaSpeed;
    float t;
    public void Attach(Entity e) {
        if(e != null) {

            attachedEntity = e;
            healthbar.fillAmount = ((float)(float)e.currentHealth / (float)e.maxHealth);
        }
    }   

    // Update is called once per frame

    public void Refresh() {
        t = deltaTime;
    }
    void Update()
    {
        t -= Time.deltaTime;

        transform.LookAt(Camera.main.transform.position);
        if (attachedEntity) {
            healthbar.fillAmount = ((float)(float)attachedEntity.currentHealth / (float)attachedEntity.maxHealth);
            if(healthbarDelta.fillAmount > healthbar.fillAmount) {

                healthbarDelta.enabled = true;
                if(t <= 0)
                healthbarDelta.fillAmount = Mathf.Lerp(healthbarDelta.fillAmount, healthbar.fillAmount, deltaSpeed);
            }
            else {
                healthbarDelta.enabled = false;
            }
            
            transform.position = attachedEntity.transform.position + attachedEntity.healthbarOffset;
        }
        else {
            Destroy(this.gameObject);
        }
           

    }
}

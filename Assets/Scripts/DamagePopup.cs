using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagePopup : MonoBehaviour
{
    public float moveSpeed;
    Text text;
    // Start is called before the first frame update


    private void Update() {
        transform.LookAt(Camera.main.transform.position);
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        Color temp = text.color;
        temp.a -= Time.deltaTime;
        text.color = temp;
    }

    public void SetDamageText(Damage dmg) {
        text = transform.GetChild(0).GetComponent<Text>();

        text.text = "-" + dmg.DamageAmount;
    }
}

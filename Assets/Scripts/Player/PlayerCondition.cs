using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public UICondition uICondition;

    Condition health { get { return uICondition.health; } }
    Condition hunger { get { return uICondition.hunger; } }
    Condition stamina { get { return uICondition.stamina; } }

    public float noHungerHealthDecay;

    // Update is called once per frame
    void Update()
    {
        hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if(hunger.curValue <= 0f)
        {
            health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

        if(health.curValue <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }


    public void Die()
    {
        Debug.Log("�ֱ�");
    }

    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }
        stamina.Subtract(amount);
        return true;
    }
}

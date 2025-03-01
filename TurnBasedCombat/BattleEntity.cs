using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEntity : MonoBehaviour
{
    [TextArea(4,4)]
    public string decription = "Sorry i forgot to add one :C";
    public int maxHp = 75, dmg = 5, def = 10, spd = 1;

    public int currentHp, speedCount;

    public Attack[] attacks;
    public List<StatusEffect> appliedEffects;

    public void TakeDamage(int dmg)
    {
        currentHp -= Mathf.CeilToInt(dmg / ((def + 100) / 100));
    }

    public void UpdateSpeed()
    {
        speedCount += spd;
    }

    public void ResetSpeed()
    {
        speedCount = 0;
    }

    public void AddHP(int hp)
    {
        currentHp = Mathf.Clamp(currentHp + hp, 0, maxHp);
    }

    public void AddEffect(StatusEffect newEffect)
    {
        appliedEffects.Add(newEffect);
    }

    public Attack GetAttack(int i)
    {
        return attacks[i];
    }

    public void Die()
    {

    }
}

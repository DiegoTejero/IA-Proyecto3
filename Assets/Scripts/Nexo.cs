using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nexo : MonoBehaviour {

    private int life = 5;

    public void SetLife(int _life)
    {
        life = _life;
    }

    public void DamageNexo(int damage)
    {
        life = life - damage;
    }

    public int GetLife()
    {
        return life;
    }
}

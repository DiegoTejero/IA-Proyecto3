using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParentCromosomes {

    private float aptitude;
    private float[] damages;
    private float[] resistances;

    public EnemyParentCromosomes(float _aptitude, float[] _damages, float[] _resistances)
    {
        setAptitude(_aptitude);
        setDamages(_damages);
        setResistances(_resistances);
    }

    public float getAptitude()
    {
        return aptitude;
    }
    public void setAptitude(float _aptitude)
    {
        aptitude = _aptitude;
    }
    public float getDamages(int i)
    {
        return damages[i];
    }
    public void setDamages(float[] _damages)
    {
        damages= _damages;
    }
    public float getResistances(int i)
    {
        return resistances[i];
    }
    public void setResistances (float[] _resistances)
    {
        resistances = _resistances;
    }

}

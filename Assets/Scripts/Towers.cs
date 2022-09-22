using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Towers : MonoBehaviour {

    public float life;
    public float range;
    public float[] damage = new float[4]; // 0= Fire, 1= water, 2= Electric, 3= Normal
    public float[] resistances = new float[4]; // 0= Fire, 1= water, 2= Electric, 3= Normal
    public float atkSpeed;
    public int type;

    public GameObject lifeText;
    GameObject myLifeText;
    public List<GameObject> enemyInRange;
    private bool isShooting;

    public GameObject projectile;


    // Use this for initialization
    void Start ()
    {

        damage[type] = 50;
        resistances[type] += 10;

        this.GetComponent<CircleCollider2D>().radius = range;

        myLifeText = (GameObject)Instantiate(lifeText, this.transform);
        myLifeText.transform.SetParent(this.transform);

        switch (type)
        {
            case 0:
                myLifeText.GetComponent<TextMesh>().color = Color.red;
                break;
            case 1:
                myLifeText.GetComponent<TextMesh>().color = Color.blue;
                break;
            case 2:
                myLifeText.GetComponent<TextMesh>().color = Color.yellow;
                break;
            case 3:
                myLifeText.GetComponent<TextMesh>().color = Color.white;
                break;
        }


    }

    void Update()
    {
        RemoveEnemyFromList();
        if (isShooting == false)
        {
            StartCoroutine(ShotDelay());
        }
        if (life <= 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            myLifeText.GetComponent<TextMesh>().text = life.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D enemy)
    {
        if (enemy.gameObject.tag == "Projectile")
        {
            if (enemy.gameObject.GetComponent<Projectile>().instigator != "Tower")
            {
            ReceiveDamage(enemy.gameObject.GetComponent<Projectile>().damage);
            Destroy(enemy.gameObject);
            }
            
        }
    }

    private void OnTriggerEnter2D(Collider2D enemy)
    {
        if (enemy.tag == "Enemy" && !enemy.isTrigger)
        {
            enemyInRange.Add(enemy.gameObject);
           
        }
    }

    private void OnTriggerExit2D(Collider2D enemy)
    {
        if (enemy.tag == "Enemy")
        {
            enemyInRange.Remove(enemy.gameObject);
        }
    }

    public void Shoot()
    {
        Transform enemyPosition;
        enemyPosition = enemyInRange[0].transform;
        //Quaternion rotationToEnemy = Quaternion.FromToRotation(this.transform.position, enemyInRange[0].transform.position);
        GameObject newShot = (GameObject)Instantiate(projectile, this.transform.position, Quaternion.identity);
        newShot.GetComponent<Projectile>().instigator = this.gameObject.tag;
        newShot.GetComponent<Projectile>().target = enemyPosition;
        newShot.GetComponent<Projectile>().damage = this.damage;

        switch (type)
        {
            case 0:
                newShot.GetComponent<Projectile>().color = Color.red;
                break;
            case 1:
                newShot.GetComponent<Projectile>().color = Color.blue;
                break;
            case 2:
                newShot.GetComponent<Projectile>().color = Color.yellow;
                break;
            case 3:
                newShot.GetComponent<Projectile>().color = Color.grey;
                break;
        }           
    }

    public void RemoveEnemyFromList()
    {
        for (int i = 0; i < enemyInRange.Count; i++)
        {
            if (enemyInRange[i] == null)
            {
                enemyInRange.Remove(enemyInRange[i]);
            }
        }
    }

    public void ReceiveDamage(float[] damage)
    {
        float trueDamage = 0;
        for (int i = 0; i < damage.Length; i++)
        {
            trueDamage += damage[i] * 1/resistances[i];
        }

        life = life - trueDamage;
    }

    public IEnumerator ShotDelay()
    {

        isShooting = true;
        if (enemyInRange.Count != 0)
        {
            Shoot();
        }

        yield return new WaitForSeconds(atkSpeed);
        isShooting = false;
    }

}

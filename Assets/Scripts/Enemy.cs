using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Enemy : MonoBehaviour {

    private GameController gc;
    private DateTime spawnTimer;
    private DateTime deathTimer;

    public float life;
    public float range;
    public float speed;
    public float[] damage = new float[4]; // 0= Fire, 1= water, 2= Electric, 3= Normal
    public float[] resistances = new float[4]; // 0= Fire, 1= water, 2= Electric, 3= Normal
    public float atkSpeed;

    public float aptitude;
    public float timeAlive;
    public float[] dealtDamage = new float[4];
    public float[] resistedDamage = new float[4]; 
    public bool nexusHit;



    private bool isShooting = false;
    private Rigidbody2D physics2D;
    public int type;
    public List<GameObject> towerInRange;
    public GameObject projectile;

    private void Awake()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        physics2D = this.GetComponent<Rigidbody2D>();
        towerInRange.Clear();

        dealtDamage = new float[] {0,0,0,0};
        resistedDamage = new float[] { 0, 0, 0, 0 };
        nexusHit = false;
        timeAlive = 0;
        spawnTimer = DateTime.Now;
        
    }

    // Use this for initialization
    void Start ()
    {
        this.physics2D.velocity = Vector2.left * speed;
        this.GetComponent<CircleCollider2D>().radius = range;
    }

    // Update is called once per frame
    void Update()
    {
        if (isShooting == false)
        {
            StartCoroutine(ShotDelay());
        }

        if (life <= 0)
        {
            //Guardar las estadisticas en gamecontroller antes de morir
            
            deathTimer = DateTime.Now;
            timeAlive = (float) (deathTimer - spawnTimer).TotalSeconds;
            CalculateAptitude();
            gc.AddParent(aptitude, damage, resistances);
            Destroy(this.gameObject);
        }

    }

    private void OnCollisionEnter2D(Collision2D enemy)
    {
        if (enemy.gameObject.tag == "Nexo")
        {
            
            enemy.gameObject.GetComponent<Nexo>().DamageNexo(1);
            Debug.Log("NEXUS HEALTH " + enemy.gameObject.GetComponent<Nexo>().GetLife());
            this.nexusHit = true;
            this.life = 0;
        }
        if (enemy.gameObject.tag == "Projectile")
        {
            if (enemy.gameObject.GetComponent<Projectile>().instigator != "Enemy")
            {
            
                ReceiveDamage(enemy.gameObject.GetComponent<Projectile>().damage);
                Destroy(enemy.gameObject.GetComponent<Projectile>().gameObject);
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D enemy)
    {
        if (enemy.tag == "Tower" && !enemy.isTrigger)
        {
            towerInRange.Add(enemy.gameObject);
        }

    }

    private void OnTriggerExit2D(Collider2D enemy)
    {
        if (enemy.tag == "Tower")
        {
            towerInRange.Remove(enemy.gameObject);
        }
    }

    public void ReceiveDamage(float[] damage)
    {
        float trueDamage = 0;
        for (int i = 0; i < damage.Length; i++)
        {
            trueDamage += damage[i] * 1 / resistances[i];
            this.resistedDamage[i] = damage[i] - ( damage[i] * 1 / resistances[i]); 
        }

        life = life - trueDamage;
    }

    public void Shoot()
    {
        Transform towerPosition;
        towerPosition = towerInRange[0].transform;

        GameObject newShot = (GameObject)Instantiate(projectile, this.transform.position, Quaternion.identity );
        newShot.GetComponent<Projectile>().target = towerPosition;
        newShot.GetComponent<Projectile>().instigator = this.gameObject.tag;
        newShot.GetComponent<Projectile>().damage = this.damage;

        for (int i = 0; i < damage.Length; i++)
        {
            this.dealtDamage[i] = (damage[i] * 1 / towerInRange[0].GetComponent<Towers>().resistances[i]);
        }

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

    public IEnumerator ShotDelay()
    {
        isShooting = true;
        if (towerInRange.Count != 0)
        {
            Shoot();
        }

        yield return new WaitForSeconds(atkSpeed);
        isShooting = false;
    }

    public void CalculateAptitude()
    {
        aptitude = 0;
        aptitude += timeAlive;
        for (int i = 0; i < damage.Length; i++)
        {
            aptitude += dealtDamage[i] + resistedDamage[i];
        }


        if (nexusHit)
            aptitude +=  aptitude/20;

    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Projectile : MonoBehaviour {

    private float speed;
    public float[] damage;
    public Color color;
    private Rigidbody2D physics2D;
    public Transform target;
    public string instigator;

    private void Awake()
    {
        speed = 11;
        physics2D = this.GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start ()
    {
        this.GetComponent<SpriteRenderer>().color = color;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (target)
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        else
            Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D enemy)
    {
        if (enemy.gameObject.tag != "Tower" && (enemy.gameObject.tag != "Enemy"))
        {
            Destroy(this.gameObject);
        }
    }
}

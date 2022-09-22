using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameController : MonoBehaviour {

    public List<EnemyParentCromosomes> possibleParents;
    public List<EnemyParentCromosomes> parents;
    public float maxDamage, maxResistance;
    public float minDamage, minResistance;
    public float mutationRate;

    public List<GameObject> prefabEnemylist;
    public List<GameObject> enemyList;
    public List<GameObject> towers;
    public GameObject[] spawnPoints;
    public GameObject[] towersPrefab;
    private Nexo nexus;
    private Ray ray;
    private RaycastHit2D hit;
    private bool waveOn = false;
    public bool firstWave = true;
    private bool isSpawningEnemy = false;
    private DateTime timer;
    private float enemySpawnTimer = 2;
    private float waveDuration = 30;
    public Text gameOver;
    public Button nextWave;
    public Button startWave;
    public Text timerText;

    // Use this for initialization
    void Awake ()
    {
        nexus = GameObject.Find("Nexo").GetComponent<Nexo>();
        possibleParents = new List<EnemyParentCromosomes>();
        parents = new List<EnemyParentCromosomes>();
        maxDamage = 100;
        minDamage = 0;
        maxResistance = 50;
        minDamage = 1;
        mutationRate = 0.05f;
    /*gameOver = GameObject.Find("GameOver").GetComponent<Text>();
    nextWave = GameObject.Find("NextWave").GetComponent<Button>();
    gameOver.gameObject.SetActive(false);*/
}

    void Update()
    {
        if (waveOn)
        {
            if (nexus.GetLife() <= 0)
            {
                StopAllCoroutines();
                GameOver(true);
                
            }

            timerText.text = "" + (30 - (DateTime.Now - timer).TotalSeconds);

            RemoveEnemyFromList();

            if (!isSpawningEnemy)
            {
                StartCoroutine(SpawnEnemyDelay());
            }
           
        }

        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            onHover(ray, hit);
        }
    }

    public void HideSelector(bool activator)
    {
        for (int i = 0; i <= 2; i++)
        {
            spawnPoints[i].SetActive(activator);
        }
    }

    public void SelectTower(GameObject spawn)
    {
        
        Transform[] allChildren = new Transform[spawn.transform.childCount];
        for (int i = 0; i < spawn.transform.childCount; i++)
        {
             allChildren[i] = spawn.transform.GetChild(i);
        }

        foreach (Transform child in allChildren)
        {
            if (child.gameObject.active == true)
            {
                child.gameObject.SetActive(false);
            }
            else
                child.gameObject.SetActive(true);

        }
    }

    public void RemoveEnemyFromList()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i] == null)
            {
                enemyList.Remove(enemyList[i]);
            }
        }
    }

    public void PaintTower(int tower, GameObject parent)
    {
        switch (tower)
        {
            case 0:
                parent.GetComponent<SpriteRenderer>().color = Color.red;
                SelectTower(parent);
                break;
            case 1:
                parent.GetComponent<SpriteRenderer>().color = Color.blue;
                SelectTower(parent);
                break;
            case 2:
                parent.GetComponent<SpriteRenderer>().color = Color.yellow;
                SelectTower(parent);
                break;
            case 3:
                parent.GetComponent<SpriteRenderer>().color = Color.grey;
                SelectTower(parent);
                break;
        }
    }


    public void PlaceTower(GameObject parent)
    {
        GameObject newTower;
        if (parent.GetComponent<SpriteRenderer>().color == Color.red)
        {
             newTower = (GameObject)Instantiate(towersPrefab[0], parent.transform.position, Quaternion.identity);
            towers.Add(newTower);
        }
        if (parent.GetComponent<SpriteRenderer>().color == Color.blue)
        {
            newTower = (GameObject)Instantiate(towersPrefab[1], parent.transform.position, Quaternion.identity);
            towers.Add(newTower);
        }
        if (parent.GetComponent<SpriteRenderer>().color == Color.yellow)
        {
            newTower = (GameObject)Instantiate(towersPrefab[2], parent.transform.position, Quaternion.identity);
            towers.Add(newTower);
        }
        if (parent.GetComponent<SpriteRenderer>().color == Color.grey)
        {
            newTower = (GameObject)Instantiate(towersPrefab[3], parent.transform.position, Quaternion.identity);
            towers.Add(newTower);
        }
        

    }


    public void StartWave()
    {
        
        for (int i = 1; i <= 3; i++)
        {
            PlaceTower(GameObject.Find(i+"Spawn"));
        }

        timer = DateTime.Now;
        HideSelector(false);
        waveOn = true;
        isSpawningEnemy = false;
        StartCoroutine(WaveTimer());
        startWave.gameObject.SetActive(false);
    }

    public void SpawnEnemy()
    {
        int randPos = UnityEngine.Random.Range(1, 3);
        if (firstWave == true)
        {
            int rand = UnityEngine.Random.Range(0, prefabEnemylist.Count);
            GameObject enemy = (GameObject)Instantiate(prefabEnemylist[rand].gameObject, GameObject.Find("EnemySpawn" + randPos).transform.localPosition, Quaternion.identity);
            if (!enemyList.Contains(enemy))
            {
                enemyList.Add(enemy);
            }
        }
        else
        {
            int rand = UnityEngine.Random.Range(0, prefabEnemylist.Count);
            GameObject enemy = (GameObject)Instantiate(prefabEnemylist[rand].gameObject, GameObject.Find("EnemySpawn" + randPos).transform.localPosition, Quaternion.identity);
            GenerateChildren(enemy);
            MutateChildren(enemy);
            if (!enemyList.Contains(enemy))
            {
                enemyList.Add(enemy);
            }
        }
    }

    public void DestroyEnemys()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].GetComponent<Enemy>().life = 0; 
        }
    }

    public void DestroyTowers()
    {
        for (int i = 0; i < towers.Count; i++)
        {

            Destroy(towers[i].gameObject);
        }
    }

    public void GameOver(bool state)
    {
        //true GameOver
        if (state)
        {
            gameOver.gameObject.SetActive(true);
            nexus.gameObject.SetActive(false);
        }
        //End of wave
        if (!state)
            gameOver.gameObject.SetActive(false);

        nextWave.gameObject.SetActive(true);

        waveOn = false;
        firstWave = false;
        nexus.SetLife(5);
        DestroyEnemys();
        timerText.text = "";
    }

    public void NextWave()
    {
        gameOver.gameObject.SetActive(false);
        nextWave.gameObject.SetActive(false);
        nexus.gameObject.SetActive(true);
        DestroyTowers();


        SelectParents(possibleParents);
        possibleParents.RemoveRange(0, possibleParents.Count);

        enemyList.RemoveRange(0, enemyList.Count);
        towers.RemoveRange(0, towers.Count);
        HideSelector(true);
        startWave.gameObject.SetActive(true);
        
    }

    public IEnumerator SpawnEnemyDelay()
    {
            isSpawningEnemy = true;
            SpawnEnemy();

        yield return new WaitForSeconds(enemySpawnTimer);
        isSpawningEnemy = false;
    }

    public IEnumerator WaveTimer()
    {
        yield return new WaitForSecondsRealtime(waveDuration);
        if (waveOn == true) {
            GameOver(false);
        }
    }

    public void onHover(Ray ray, RaycastHit2D hit)
    {
        hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit.collider != null)
        {
            if (hit.collider.name == "1Spawn")
            {
                SelectTower(hit.collider.gameObject);
            }
            else if (hit.collider.name == "2Spawn")
            {
                SelectTower(hit.collider.gameObject);
            }
            else if (hit.collider.name == "3Spawn")
            {
                SelectTower(hit.collider.gameObject);
            }
            else if (hit.collider.name == "Fire")
            {

                PaintTower(0, hit.transform.parent.gameObject);
            }
            else if (hit.collider.name == "Water")
            {
                PaintTower(1, hit.transform.parent.gameObject);
            }
            else if (hit.collider.name == "Elect")
            {
                PaintTower(2, hit.transform.parent.gameObject);
            }
            else if (hit.collider.name == "Normal")
            {
                PaintTower(3, hit.transform.parent.gameObject);
            }
        }
    }

    public void AddParent(float aptitude, float[]damages, float[] resistances)
    {

        EnemyParentCromosomes parent = new EnemyParentCromosomes(aptitude, damages, resistances);
        possibleParents.Add(parent);

        
    }

    public void SelectParents(List<EnemyParentCromosomes> _possibleParents)
    {

        _possibleParents.Sort(delegate (EnemyParentCromosomes x, EnemyParentCromosomes y )
        {
            return y.getAptitude().CompareTo(x.getAptitude());
        }
        );

        parents.RemoveRange(0, parents.Count);
        parents = _possibleParents.GetRange(0, 5);

    }

    public void MutateChildren(GameObject enemy)
    {
        float rand;

        for (int i = 0; i < enemy.GetComponent<Enemy>().damage.Length; i++)
        {
            rand = UnityEngine.Random.Range(0.00f, 1.00f);

            if (rand < mutationRate)
            {
                enemy.GetComponent<Enemy>().damage[i] = UnityEngine.Random.Range(minDamage, maxDamage);
            }
        }

        for (int i = 0; i < enemy.GetComponent<Enemy>().resistances.Length; i++)
        {
            rand = UnityEngine.Random.Range(0.00f, 1.00f);

            if (rand < mutationRate)
            {
                enemy.GetComponent<Enemy>().resistances[i] = UnityEngine.Random.Range(minResistance, maxResistance);
            }
        }

    }

    public void GenerateChildren(GameObject enemy)
    {
        int rnd = UnityEngine.Random.Range(0, 5);
        int rnd2 = UnityEngine.Random.Range(0, 5);
        while (rnd == rnd2)
        {
            rnd2 = UnityEngine.Random.Range(0, 5);
        }

        EnemyParentCromosomes parent1 = parents[rnd];
        EnemyParentCromosomes parent2 = parents[rnd2];

        
        for (int i = 0; i < enemy.GetComponent<Enemy>().damage.Length; i++)
        {
            int finalRand = UnityEngine.Random.Range(0, 2);

            if (finalRand == 0)
            {
                enemy.GetComponent<Enemy>().damage[i] = parent1.getDamages(i);
            }
            else
                enemy.GetComponent<Enemy>().damage[i] = parent2.getDamages(i);

        }


        for (int i = 0; i < enemy.GetComponent<Enemy>().resistances.Length; i++)
        {
            int finalRand = UnityEngine.Random.Range(0, 2);

            if (finalRand == 0)
            {
                enemy.GetComponent<Enemy>().resistances[i] = parent1.getResistances(i);
            }
            else
                enemy.GetComponent<Enemy>().resistances[i] = parent2.getResistances(i);

        }
    }

}

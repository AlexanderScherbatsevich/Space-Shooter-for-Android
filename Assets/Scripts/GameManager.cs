using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    static public GameManager S;  
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    //public GameObject scoreGO;
    public GameObject prefabExplosion;
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyDefaultPadding = 1.5f;  //отступ для позиционирования
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[] { WeaponType.blaster, WeaponType.blaster, 
        WeaponType.spread, WeaponType.shield};

    [Header("Set in Canvas")]
    public Text scoreCounter;
    public Text levelCounter;
    public Image timeScale;
    public float timeMax = 60f;
    public float timePassed;

    private BoundsCheck bndCheck;
    private string _scene;
    [SerializeField] private GameObject victoryText; 
    public void AddScore(Enemy e)
    {
        int score = int.Parse(scoreCounter.text);
        score += e.score;
        scoreCounter.text = score.ToString();
        //PlayerPrefs.SetString("Score", scoreCounter.text);
        if (score > HighScore.score)
        {
            HighScore.score = score;
        }
    }

    public void ShipDestroyed(Enemy e)
    {
        if (e.name == "Enemy_Boss(Clone)")
        {
            GameObject droneAnchor = GameObject.Find("_DroneAnchor");
            GameObject[] drones = GetAllDrones(droneAnchor);
            foreach(GameObject drone in drones)
            {
                Destroy(drone);
                Explode(drone);
            }
            victoryText.SetActive(true);
            DelayedRestart(8f);
        }
        //сгенерировать бонус с заданной вероятностью
        if (Random.value <= e.powerUpDropChance)
        {
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            //установить соответствующий тип PowerUp
            pu.SetType(puType);
            pu.transform.position = e.transform.position;
        }
    }
    private void Awake()
    {
        S = this;

        Scene = SceneManager.GetActiveScene().name;
        bndCheck = GetComponent<BoundsCheck>();

        Invoke("SpawnEnemy", 3f / enemySpawnPerSecond);

        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    private void Start()
    {
        Cursor.visible = false;

        if (PlayerPrefs.HasKey("Score") && Scene != "Level_1")        
            scoreCounter.text = PlayerPrefs.GetString("Score");       
        else       
            scoreCounter.text = "0";
        
        timePassed = 0f;
        timeScale.GetComponent<Image>();

        if (SceneManager.GetActiveScene().buildIndex == 4)
            levelCounter.text = "Last";
        else
            levelCounter.text = SceneManager.GetActiveScene().buildIndex.ToString();
    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            return;
        }
        else
        {
            timePassed += Time.fixedDeltaTime;
            timeScale.fillAmount = timePassed / timeMax;
            if (timeScale.fillAmount >= 1)
            {
                PlayerPrefs.SetString("Score", scoreCounter.text);
                LevelLoader.S.LoadNextLevel(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

    public void SpawnEnemy()
    {
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);
        //разместить врага над экраном в случайной позиции х
        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        //установить начальные координаты созданного врага
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        if ((SceneManager.GetActiveScene().buildIndex != 4))
        {
            Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
        }
    }

    public void DelayedRestart(float delay)
    {
        Invoke("Restart", delay);
    }
    public void Restart()
    {
        SceneManager.LoadScene("Menu");
        Debug.Log("Menu");
    }

    //статическа функция, возвращающая WeaponDefinition из статического защищенного поля WEAP_DICT класса Main
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        //проверить наличие ключа
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        //если не найдено оружие вернуть .none тип
        return (new WeaponDefinition());
    }

    public void Explode(GameObject go)
    {
        GameObject explosion = Instantiate(prefabExplosion);
        explosion.transform.position = go.transform.position;
        Destroy(explosion, 4f);
    }

    public string Scene
    {
        get { return _scene; }
        set { _scene = value; }
    }

    public void AddLevelText()
    {
        int lnl = int.Parse(levelCounter.text);
        lnl++;
        levelCounter.text = lnl.ToString();
    }

     public GameObject[] GetAllDrones(GameObject go)
    {
        Transform[] trans = go.GetComponentsInChildren<Transform>();
        //Transform droneTrans = trans.Find("Enemy_Drone(Clone)");
        List<GameObject> drones = new List<GameObject>();
        foreach (Transform tr in trans)
        {
            drones.Add(tr.gameObject);
        }
        return (drones.ToArray());
    }

}

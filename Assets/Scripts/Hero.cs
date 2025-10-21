using UnityEngine;


public class Hero : MonoBehaviour
{
    static public Hero S;

    [Header("Set in Inspector")]
    public FloatingJoystick joystick;
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float rotate = -30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;
    public Shield shield;

    [Header("Sounds")]
    public AudioSource setWeaponSound;
    public AudioSource addShieldSound;
    public AudioSource removeShieldSound;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 1;

    //последний столкнувшийся объект
    private GameObject lastTriggerGo = null;

    //объявление нового делегата типа WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    //создаем поле типа WeaponFireDelegate
    public WeaponFireDelegate fireDelegate;


    private void Start()
    {
        if (S == null)
        {
            S = this; 
        }
        else
        {
            Destroy(this);
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
        //GameObject.DontDestroyOnLoad(this.gameObject);
        
    }

    void Update()
    {
        float xAxis = joystick.Horizontal;
        float yAxis = joystick.Vertical;

        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        if (yAxis >= 0)
            transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, xAxis * rotate);
        else
            transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, xAxis * rotate);

        //if (Input.touchCount > 0 && fireDelegate != null
        if (fireDelegate != null && joystick.IsPointerDown)
        {
            fireDelegate();
        }
    }

    //void TempFire()
    //{
    //    GameObject projGO = Instantiate<GameObject>(projectilePrefab);
    //    projGO.transform.position = transform.position;
    //    Rigidbody rigidB = projGO.GetComponent<Rigidbody>();

    //    Projectile proj = projGO.GetComponent<Projectile>();
    //    proj.type = WeaponType.blaster;
    //    float tSpeed = Main.GetWeaponDefinition(proj.type).velocity;
    //    rigidB.velocity = Vector3.up * tSpeed;
    //}

    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //print("Triggered: " + go.name);

        //гарантировать невозможность повторного столкновения с тем же объектом (для кораблей Enemy)
        if (go == lastTriggerGo && other.tag != "ProjectileEnemy") return;
        lastTriggerGo = go;

        if (go.tag == "Enemy")
        {
            shieldLevel--;
            removeShieldSound.Play();
            if (shield.gameObject.activeInHierarchy == false)
            {
                Destroy(this.gameObject);
                GameManager.S.Explode(this.gameObject);
                GameManager.S.DelayedRestart(gameRestartDelay);
            }
            else if (go.name == "Enemy_Boss(Clone)")
            {
                Destroy(this.gameObject);
                GameManager.S.Explode(this.gameObject);
                GameManager.S.DelayedRestart(gameRestartDelay);
            }
            Destroy(go);
            GameManager.S.Explode(go);
            //Debug.Log(go.name);
        }
        else if (other.tag == "ProjectileEnemy")
        {
            removeShieldSound.Play();
            shieldLevel--;
        if (shield.gameObject.activeInHierarchy == false)
            {
                PlayerPrefs.SetString("Score", "0");
                Destroy(this.gameObject);
                GameManager.S.Explode(this.gameObject);
                GameManager.S.DelayedRestart(gameRestartDelay);
            }    
            Destroy(other.gameObject);
        }
        else if (go.tag == "PowerUp")
        {
            AbsorbPowerUp(go);
        }
        else
        {
            print("Triggered non-Enemy: " + go.name);
        }
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:
                if (shieldLevel < 4)
                {
                    shield.gameObject.SetActive(true);
                    shieldLevel++;
                    addShieldSound.Play();
                }
                else return;
                break;
            default:
                if (pu.type == weapons[0].type)  //Если оружие того же типа
                {
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null) 
                    {
                        w.SetType(pu.type);
                        setWeaponSound.Play();
                    }
                    else return;
                }
                else
                {
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                    setWeaponSound.Play();
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel
    {
        get { return (_shieldLevel); }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value > -1) shield.gameObject.SetActive(true);
            else shield.gameObject.SetActive(false);
        }
    }

    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none) return (weapons[i]);
        }
        return (null);
    }
    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }
}

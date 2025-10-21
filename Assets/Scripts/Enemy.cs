using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;
    public float fireRate = 3f; //секунд между выстрелами
    public float health = 10;
    public int score = 100;
    public float showDamageDuration = 0.1f;
    public float powerUpDropChance = 1;
    public Weapon[] weapons;
    

    [Header("Set Dynamically: Enemy")]
    public Color[] oridinalColors;
    public Material[] materials; //все материалы игрового объекта и его потомков
    public bool showingDamage = false;
    public float damageDoneTime;
    public bool notifiedOfDestruction = false;
    float lastShotTimeEnemy = 0;

    protected BoundsCheck bndCheck;

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        //получить все материалы игрового объекта и его потомков
        materials = Utils.GetAllMaterials(gameObject);
        oridinalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            oridinalColors[i] = materials[i].color;
        }
    }

    private void Start()
    {
        SetWeapon();
    }

    public Vector3 pos
    {
        get { return (this.transform.position); }
        set { this.transform.position = value; }
    }

    void Update()
    {
        Move();
        if (showingDamage && Time.time > damageDoneTime) UnShowDamage();
        if (bndCheck != null && bndCheck.offDown) Destroy(gameObject);

        int[] rotRaycast = { -30, -20,-10, 0, 10, 20, 30 };
        RaycastHit[] hits = new RaycastHit[7];
        for (int i = 0; i < hits.Length; i++)
        {
            //Debug.DrawRay(transform.position, -transform.up * 100 - Vector3.right * rotRaycast[i], Color.red);
            if (Physics.Raycast(transform.position,
                -transform.up * 100 - Vector3.right * rotRaycast[i], out hits[i]))
            {
                if (hits[i].collider.tag == "Hero")
                {
                    if (Time.time - lastShotTimeEnemy >= fireRate)
                    {
                        FireEnemy();
                    }
                    //Debug.Log("Hit:");
                }
            }
        }
    }
    public virtual void SetWeapon()
    {
        foreach (Weapon wp in weapons)
        {
            wp.SetType(WeaponType.blaster);
        }
    }
    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    private void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();
                //если вражеский корабль за границами экрана, не наносить ему повреждений
                if (!bndCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }
                //поразить вражеский корабль, значение урона из WEAP_DICT в классе Main
                ShowDamage();
                health -= GameManager.GetWeaponDefinition(p.type).damageOnHit;

                if (health <= 0)
                {
                    //сообщить объекту-одиночке Main об уничтожении
                    if (!notifiedOfDestruction)
                    {
                        GameManager.S.ShipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    Destroy(this.gameObject);
                    GameManager.S.Explode(this.gameObject);
                    GameManager.S.AddScore(this);
                }
                Destroy(otherGO);
                break;
            default:
                print("Enemy hit by non-ProjectileHero: " + otherGO.name);
                break;
        }
    }

    void ShowDamage()
    {
        foreach (Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }
    void UnShowDamage()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = oridinalColors[i];
        }
        showingDamage = false;
    }
    public virtual void FireEnemy()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Fire();
        }
        lastShotTimeEnemy = Time.time;
        //Invoke("FireEnemy", fireRate);
    }
}



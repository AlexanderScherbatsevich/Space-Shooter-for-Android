using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BoundsCheck bndCheck;
    private Renderer rend;

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    [SerializeField]
    private WeaponType _type;

    public WeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (bndCheck.offUp || bndCheck.offDown) Destroy(gameObject);
    }

    //изменяет скрытое поле _type и устанавливает цвет снаряда определенного в WeaponDefinition
    public void SetType(WeaponType eType)
    {
        _type = eType;
        WeaponDefinition def = GameManager.GetWeaponDefinition(_type);
        //rend.material.color = def.projectileColor;
        rend.material.SetColor("_Color", def.projectileColor);
        rend.material.SetColor("_EmissionColor", def.projectileColor);
    }
}

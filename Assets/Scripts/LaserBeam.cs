using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    LineRenderer lr;
    public RaycastHit hit;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {

        lr.SetPosition(0, transform.position);
        if (Physics.Raycast(transform.position, transform.up, out hit))
        {
            if (hit.collider.tag == "Enemy")
            {
                float enemyPosY = hit.collider.transform.position.y;
                lr.SetPosition(1, new Vector3(0, enemyPosY, 0));
            }
            else
            {
                lr.SetPosition(1, new Vector3(0, 200, 0));
            }

        }
    }
}

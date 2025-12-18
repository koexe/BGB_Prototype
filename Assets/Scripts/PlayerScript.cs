using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;

    [SerializeField] Vector2 currentDir;
    [SerializeField] float speed;
    [SerializeField] List<Mag> bulletSetting;
    [SerializeField] List<Mag> bullets;
    [SerializeField] int currentBulletIndex;

    [SerializeField] bool isInAttack;
    [SerializeField] float attackCooltime;
    [SerializeField] float currentAttackCooltime;

    [SerializeField] float rollCooltime;
    [SerializeField] float currentRollCooltime;

    [SerializeField] bool isInRoll;
    [SerializeField] float rollTime;
    [SerializeField] float currentRolltime;

    private void Start()
    {
        var t_list = new List<Mag>();
        foreach (Mag mag in this.bulletSetting)
        {
            var t_mag = Instantiate(mag);
            t_mag.buffSystem = new BuffSystem();
            t_list.Add(t_mag);
        }
        this.bullets = t_list;

    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (this.inventoryUI.gameObject.activeSelf)
            {
                GameManager.instance.gameState = GameState.Running;
                this.inventoryUI.gameObject.SetActive(false);
            }
            else
            {
                GameManager.instance.gameState = GameState.Paused;
                this.inventoryUI.Initialization(this.bullets);
                this.inventoryUI.gameObject.SetActive(true);
            }
        }

        if (GameManager.instance.gameState == GameState.Paused) return;

        if (!this.isInRoll)
        {
            this.currentDir = Vector2.zero;
            if (Input.GetKey(KeyCode.W))
            {
                this.currentDir.y += 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                this.currentDir.x += -1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                this.currentDir.y += -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                this.currentDir.x += 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            this.currentBulletIndex += 1;
            if (this.currentBulletIndex >= this.bullets.Count)
                currentBulletIndex = 0;
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (this.currentAttackCooltime == 0 && !this.isInAttack)
            {
                this.isInAttack = true;
                StartCoroutine(CreateBullet());
            }
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (this.currentRollCooltime == 0 && !this.isInRoll)
            {
                this.isInRoll = true;
                StartCoroutine(Roll());
            }
        }

    }
    private void LateUpdate()
    {
        if (GameManager.instance.gameState == GameState.Paused) return;
        Camera.main.transform.position = this.transform.position + new Vector3(0, 0, -3);
    }
    private void FixedUpdate()
    {
        if (GameManager.instance.gameState == GameState.Paused) return;
        this.transform.position += (Vector3)(this.currentDir * this.speed * Time.fixedDeltaTime);

        if (this.currentAttackCooltime != 0)
            this.currentAttackCooltime = Mathf.MoveTowards(this.currentAttackCooltime, 0f, Time.fixedDeltaTime);
        if (this.currentRollCooltime != 0)
            this.currentRollCooltime = Mathf.MoveTowards(this.currentRollCooltime, 0f, Time.fixedDeltaTime);
    }
    IEnumerator CreateBullet()
    {
        Mag t_mag = this.bullets[currentBulletIndex];
        for (int i = 0; i < t_mag.GetAttackAmount(); i++)
        {
            if (t_mag.GetCurrentAmount() <= 0) continue;

            var t_bullet = Instantiate(t_mag.GetBulletPrefab()).GetComponent<Bullet>();
            Vector3 t_mouseScreenPos = Input.mousePosition;
            Vector3 t_mouseWorldPos = Camera.main.ScreenToWorldPoint(t_mouseScreenPos);
            t_mouseWorldPos.z = 0f;

            Vector3 t_dir = (t_mouseWorldPos - transform.position).normalized;
            t_bullet.transform.position = this.transform.position;
            t_bullet.Initialization(t_dir, 10f, t_mag.GetBuffSystem().GetStat(BuffType.BulletScale), 5f, t_mag.GetBuffSystem().GetStat(BuffType.Judo) != -1);

            t_mag.UseBullet();
            yield return new WaitForSeconds(0.1f);
        }
        this.currentAttackCooltime = this.attackCooltime;
        this.isInAttack = false;
    }

    IEnumerator Roll()
    {
        float t_time = 0;

        Vector3 t_mouseScreenPos = Input.mousePosition;
        Vector3 t_mouseWorldPos = Camera.main.ScreenToWorldPoint(t_mouseScreenPos);
        t_mouseWorldPos.z = 0f;

        Vector3 t_dir = (t_mouseWorldPos - transform.position).normalized;
        this.currentDir = Vector2.zero;

        while (t_time < this.rollTime)
        {
            t_time += Time.fixedDeltaTime;

            this.transform.position += t_dir * 20 * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        this.isInRoll = false;
        this.currentRollCooltime = this.rollCooltime;
    }
    public void FullMag()
    {
        this.bullets[currentBulletIndex].FullCurrentAmount();

    }

    public List<Mag> GetAllMag() => this.bullets;
}

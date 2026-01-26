using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] PhysicsModule physicsModule;

    [Header("스탯")]
    StatSystem statSystem;
    [SerializeField] PlayerWeapon weapon;
    [SerializeField] int maxHp = 3;
    [SerializeField] int currentHp;
    [SerializeField] int maxRoll = 2;
    [SerializeField] int currentRoll;
    [SerializeField] int maxMag;
    [SerializeField] int currentMag;

    [SerializeField] float rollRegain = 2.0f;
    [SerializeField] float currentRollRegain;


    [Header("이동")]
    [SerializeField] float speed;
    [SerializeField] Vector2 currentInput;
    [SerializeField] float rollCooltime;
    [SerializeField] float currentRollCooltime;
    [SerializeField] bool isInRoll;
    [SerializeField] bool isAttackCooltime;
    [SerializeField] Force rollForce;

    [Header("Temp_Must be remove")]
    [SerializeField] Bullet temp_bullet;

    public void Initialization()
    {
        InitializeInput();
        InitializeStat();
        this.currentHp = this.maxHp;
        this.currentRoll = this.maxRoll;
        this.currentMag = (int)this.statSystem.GetStat(StatType.Mag);

        UIManager.instance.AddOrUpdateUI<IngameStatUI>
            (new IngameStatUIData()
            {
                identifier = IngameStatUI.identifier,
                hp = this.maxHp,
                rollPoint = this.maxRoll,
                magAmount = (int)this.statSystem.GetStat(StatType.Mag),
            });

    }
    private void FixedUpdate()
    {
        if (IngameManager.instance.gameState != GameState.Running) return;
        this.physicsModule.SetInputForce(this.currentInput.normalized * this.speed);
        this.currentInput = Vector2.zero;
        RollRegain();
    }
    #region 스탯 관련
    void InitializeStat()
    {
        this.statSystem = new StatSystem(DataLibrary.instance.GetWeaponInfo(IngameManager.instance.currentWeapon));
        this.weapon.Initialization(this.statSystem);
    }

    void RollRegain()
    {
        if (this.currentRoll == this.maxRoll) return;

        if (this.currentRollRegain != 0)
        {
            this.currentRollRegain = Mathf.MoveTowards(this.currentRollRegain, 0, Time.fixedDeltaTime);
        }
        else
        {
            this.currentRollRegain = this.rollRegain;
            this.currentRoll++;
            UIManager.instance.GetUI<IngameStatUI>(IngameStatUI.identifier).GainRoll();
        }
    }

    #endregion
    #region 인풋 관련
    void InitializeInput()
    {
        InputManager.instance.AddKeyBind(KeyCode.A, LeftInput);
        InputManager.instance.AddKeyBind(KeyCode.S, DownInput);
        InputManager.instance.AddKeyBind(KeyCode.D, RightInput);
        InputManager.instance.AddKeyBind(KeyCode.W, UpInput);


        InputManager.instance.AddKeyDownBind(KeyCode.Space, StartRoll);

        InputManager.instance.AddMouseDownBind(1, StartRoll);
        InputManager.instance.AddMouseBind(0, StartAttack);

    }
    void UpInput()
    {
        if (IngameManager.instance.gameState != GameState.Running || this.isInRoll) return;
        this.currentInput.y = 1;
    }
    void DownInput()
    {
        if (IngameManager.instance.gameState != GameState.Running || this.isInRoll) return;
        this.currentInput.y = -1;
    }

    void RightInput()
    {
        if (IngameManager.instance.gameState != GameState.Running || this.isInRoll) return;
        this.currentInput.x = 1;
    }
    void LeftInput()
    {
        if (IngameManager.instance.gameState != GameState.Running || this.isInRoll) return;
        this.currentInput.x = -1;
    }

    async void StartRoll()
    {
        if (IngameManager.instance.gameState != GameState.Running || this.isInRoll) return;

        if (this.currentRoll == 0) return;

        this.isInRoll = true;
        this.currentRoll--;
        Vector3 t_mouseScreenPos = Input.mousePosition;
        Vector3 t_mouseWorldPos = Camera.main.ScreenToWorldPoint(t_mouseScreenPos);
        t_mouseWorldPos.z = 0f;

        Vector3 t_dir = (t_mouseWorldPos - transform.position).normalized;

        this.physicsModule.AddForce(this.rollForce.DuplacateForce(t_dir));
        await UniTask.WaitForSeconds(this.rollForce.duration);

        this.isInRoll = false;

        UIManager.instance.GetUI<IngameStatUI>(IngameStatUI.identifier).UseRoll();

    }

    void StartAttack()
    {
        if (IngameManager.instance.gameState != GameState.Running) return;
        this.weapon.Attack(this.temp_bullet.gameObject);

    }
    #endregion

    void HitDamage()
    {
        this.currentHp--;
        UIManager.instance.GetUI<IngameStatUI>(IngameStatUI.identifier).ToggleHp(false);
    }

    private void OnTriggerEnter2D(Collider2D _col)
    {
        if (_col.transform.TryGetComponent<EnemyBase>(out var t_enemy))
        {
            HitDamage();
        }
        else
        {
            return;
        }
    }
}

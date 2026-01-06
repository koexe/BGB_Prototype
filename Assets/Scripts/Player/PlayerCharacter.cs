using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] PhysicsModule physicsModule;
    [SerializeField] float speed;

    [SerializeField] Vector2 currentInput;


    [SerializeField] float rollCooltime;
    [SerializeField] float currentRollCooltime;

    [SerializeField] bool isInRoll;

    [SerializeField] Force rollForce;

    private void Start()
    {
        Initialization();
    }
    public void Initialization()
    {
        InputManager.instance.AddKeyBind(KeyCode.A, LeftInput);
        InputManager.instance.AddKeyBind(KeyCode.S, DownInput);
        InputManager.instance.AddKeyBind (KeyCode.D, RightInput);
        InputManager.instance.AddKeyBind(KeyCode.W, UpInput);

        InputManager.instance.AddMouseDownBind(1, StartRoll);
    }
    private void FixedUpdate()
    {
        this.physicsModule.SetInputForce(this.currentInput.normalized * this.speed);
        this.currentInput = Vector2.zero;
    }

    void UpInput()
    {
        this.currentInput.y = 1;
    }
    void DownInput()
    {
        this.currentInput.y = -1;
    }

    void RightInput()
    {
        this.currentInput.x = 1;
    }
    void LeftInput()
    {
        this.currentInput.x = -1;
    }

    async void StartRoll()
    {
        if (IngameManager.instance.gameState != GameState.Running || this.isInRoll) return;

        this.isInRoll = true;
        Vector3 t_mouseScreenPos = Input.mousePosition;
        Vector3 t_mouseWorldPos = Camera.main.ScreenToWorldPoint(t_mouseScreenPos);
        t_mouseWorldPos.z = 0f;

        Vector3 t_dir = (t_mouseWorldPos - transform.position).normalized;

        this.physicsModule.AddForce(this.rollForce.DuplacateForce(t_dir));
        await UniTask.WaitForSeconds(this.rollForce.duration);

        this.isInRoll = false;
    }


}

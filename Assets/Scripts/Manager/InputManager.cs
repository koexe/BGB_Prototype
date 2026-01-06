using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    Dictionary<KeyCode, Action> keyDownActions;
    Dictionary<KeyCode, Action> keyActions;
    Dictionary<KeyCode, Action> keyUpActions;

    Dictionary<int, Action> mouseDownActions;
    Dictionary<int, Action> mouseActions;
    Dictionary<int, Action> mouseUpActions;

    private void Awake()
    {
        instance = this;

        this.keyDownActions = new Dictionary<KeyCode, Action>();
        this.keyActions = new Dictionary<KeyCode, Action>();
        this.keyUpActions = new Dictionary<KeyCode, Action>();

        this.mouseDownActions = new Dictionary<int, Action>();
        this.mouseActions = new Dictionary<int, Action>();
        this.mouseUpActions = new Dictionary<int, Action>();
    }

    private void Update()
    {
        // ---------- Keyboard ----------

        foreach (var t_action in this.keyDownActions)
        {
            if (Input.GetKeyDown(t_action.Key))
                t_action.Value?.Invoke();
        }

        foreach (var t_action in this.keyActions)
        {
            if (Input.GetKey(t_action.Key))
                t_action.Value?.Invoke();
        }

        foreach (var t_action in this.keyUpActions)
        {
            if (Input.GetKeyUp(t_action.Key))
                t_action.Value?.Invoke();
        }

        // ---------- Mouse ----------

        foreach (var t_action in this.mouseDownActions)
        {
            if (Input.GetMouseButtonDown(t_action.Key))
                t_action.Value?.Invoke();
        }

        foreach (var t_action in this.mouseActions)
        {
            if (Input.GetMouseButton(t_action.Key))
                t_action.Value?.Invoke();
        }

        foreach (var t_action in this.mouseUpActions)
        {
            if (Input.GetMouseButtonUp(t_action.Key))
                t_action.Value?.Invoke();
        }
    }

    // ==================== Keyboard ====================

    public void AddKeyDownBind(KeyCode _key, Action _action)
        => AddBind(this.keyDownActions, _key, _action);

    public void AddKeyBind(KeyCode _key, Action _action)
        => AddBind(this.keyActions, _key, _action);

    public void AddKeyUpBind(KeyCode _key, Action _action)
        => AddBind(this.keyUpActions, _key, _action);

    public void RemoveKeyDownBind(KeyCode _key, Action _action)
        => RemoveBind(this.keyDownActions, _key, _action);

    public void RemoveKeyBind(KeyCode _key, Action _action)
        => RemoveBind(this.keyActions, _key, _action);

    public void RemoveKeyUpBind(KeyCode _key, Action _action)
        => RemoveBind(this.keyUpActions, _key, _action);

    // ==================== Mouse ====================

    public void AddMouseDownBind(int _button, Action _action)
        => AddBind(this.mouseDownActions, _button, _action);

    public void AddMouseBind(int _button, Action _action)
        => AddBind(this.mouseActions, _button, _action);

    public void AddMouseUpBind(int _button, Action _action)
        => AddBind(this.mouseUpActions, _button, _action);

    public void RemoveMouseDownBind(int _button, Action _action)
        => RemoveBind(this.mouseDownActions, _button, _action);

    public void RemoveMouseBind(int _button, Action _action)
        => RemoveBind(this.mouseActions, _button, _action);

    public void RemoveMouseUpBind(int _button, Action _action)
        => RemoveBind(this.mouseUpActions, _button, _action);

    // ==================== Internal ====================

    void AddBind<T>(Dictionary<T, Action> _dic, T _key, Action _action)
    {
        if (_dic.ContainsKey(_key))
            _dic[_key] += _action;
        else
            _dic.Add(_key, _action);
    }

    void RemoveBind<T>(Dictionary<T, Action> _dic, T _key, Action _action)
    {
        if (_dic.ContainsKey(_key))
        {
            _dic[_key] -= _action;
            if (_dic[_key] == null)
                _dic.Remove(_key);
        }
        else
        {
            LogUtil.LogWarning($"not Exist Bind {_key}");
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Cysharp.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] Canvas canvas;
    [SerializeField] Transform uiRoot; // UI들이 생성될 부모 Transform

    private Dictionary<string, UIBase> activeUIs = new Dictionary<string, UIBase>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (canvas == null)
                canvas = GetComponent<Canvas>();
            if (uiRoot == null)
                uiRoot = canvas.transform;

            return;
        }
        Destroy(gameObject);
    }

    public void DestroyAllUI()
    {
        foreach (var ui in activeUIs)
        {
            Destroy(ui.Value.gameObject);
        }
        this.activeUIs.Clear();

    }
    public T GetUI<T>(string _identifier) where T : UIBase
    {
        if (activeUIs.TryGetValue(_identifier, out var t_ui))
        {
            return t_ui as T;
        }
        else
        {
            LogUtil.Log($"No Such UI {_identifier}");
            return null;
        }
    }
    public T HideUI<T>(string _identifier) where T : UIBase
    {
        if (activeUIs.TryGetValue(_identifier, out var t_ui))
        {
            t_ui.Hide();
            return t_ui as T;
        }
        else
        {
            LogUtil.Log($"No Such UI {_identifier}");
            return null;
        }
    }
    public T ShowUI<T>(string _identifier) where T : UIBase
    {
        if (activeUIs.TryGetValue(_identifier, out var t_ui))
        {
            t_ui.Show();
            return t_ui as T;
        }
        else
        {
            LogUtil.Log($"No Such UI {_identifier}");
            return null;
        }
    }
    /// <summary>
    /// Toggle UI
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_identifier"></param>
    /// <returns></returns>
    public T ToggleUI<T>(string _identifier) where T : UIBase
    {
        // 이미 존재하는 UI 체크
        if (this.activeUIs.TryGetValue(_identifier, out var t_existingUI))
        {
            if (t_existingUI.isShow)
            {
                t_existingUI.Hide();
            }
            else
            {
                t_existingUI.Show();
            }
            return t_existingUI as T;
        }
        else
        {
            LogUtil.Log($"No Such UI {_identifier}");
            return null;
        }
    }
    /// <summary>
    /// Add Or Update UI
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_data"></param>
    /// <returns></returns>
    public T AddOrUpdateUI<T>(UIData _data = null) where T : UIBase
    {
        // Check Exist Ui
        if (this.activeUIs.TryGetValue(_data.identifier, out var existingUI))
        {
            if (!_data.isAllowMultifle)
            {
                existingUI.transform.SetAsLastSibling();
                existingUI.Initialization(_data);
                existingUI.Show();

                return existingUI as T;
            }
        }

        // Get UI Prefab From AddressbleAsset
        GameObject uiPrefab = DataLibrary.instance.GetUI(_data.identifier);
        if (uiPrefab == null)
        {
            Debug.LogError($"UI Prefab Not Exist: {typeof(T).Name}");
            return null;
        }

        // Create Instance
        T uiInstance = Instantiate(uiPrefab, uiRoot).GetComponent<T>();
        if (uiInstance == null)
        {
            Debug.LogError($"UI Component Not Exist: {typeof(T).Name}");
            return null;
        }


        // UI Init and Show
        this.activeUIs[_data.identifier] = uiInstance;
        uiInstance.transform.SetAsLastSibling();
        uiInstance.Initialization(_data);
        uiInstance.Show();

        return uiInstance;
    }

    public void CleanupInactiveUIs()
    {
        var t_inactiveUIs = new List<string>();
        foreach (var t_kvp in activeUIs)
        {
            if (!t_kvp.Value.isShow)
            {
                t_inactiveUIs.Add(t_kvp.Key);
            }
        }

        foreach (var key in t_inactiveUIs)
        {
            if (activeUIs.TryGetValue(key, out var ui))
            {
                Destroy(ui.gameObject);
                activeUIs.Remove(key);
            }
        }
    }
}


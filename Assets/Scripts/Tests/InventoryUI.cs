using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] List<MagUI> magUIs;
    public void Initialization(List<Mag> _mags, bool _isInteractable = false, Action<Mag> _buttonAction = null)
    {
        for (int i = 0; i < _mags.Count; i++)
        {
            this.magUIs[i].initialization(_mags[i], _isInteractable, _buttonAction);
        }
    }
}

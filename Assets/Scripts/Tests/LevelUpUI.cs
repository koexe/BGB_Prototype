using UnityEngine;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;

    public void ButtonAction(int _type)
    {
        switch (_type)
        {
            case 0:
                this.inventoryUI.Initialization(GameManagerr.instance.playerScript.GetAllMag(), true, (Mag t_mag) =>
                {
                    t_mag.buffSystem.AddBuff(BuffType.BulletScale, 5f);
                    this.gameObject.SetActive(false);
                    this.inventoryUI.gameObject.SetActive(false);
                    GameManagerr.instance.gameState = GameState.Running;
                });
                break;
            case 1:
                this.inventoryUI.Initialization(GameManagerr.instance.playerScript.GetAllMag(), true, (Mag t_mag) =>
                {
                    t_mag.buffSystem.AddBuff(BuffType.BulletAmount, 10f); this.gameObject.SetActive(false);
                    this.gameObject.SetActive(false);
                    this.inventoryUI.gameObject.SetActive(false);
                    GameManagerr.instance.gameState = GameState.Running;
                });
                break;
            case 2:
                this.inventoryUI.Initialization(GameManagerr.instance.playerScript.GetAllMag(), true, (Mag t_mag) =>
                {
                    t_mag.buffSystem.AddBuff(BuffType.Judo, 5f); this.gameObject.SetActive(false);
                    this.gameObject.SetActive(false);
                    this.inventoryUI.gameObject.SetActive(false);
                    GameManagerr.instance.gameState = GameState.Running;
                });
                break;
        }
        this.inventoryUI.gameObject.SetActive(true);
    }
}

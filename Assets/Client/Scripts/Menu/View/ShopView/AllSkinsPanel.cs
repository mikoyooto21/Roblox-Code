using UnityEngine;

public class AllSkinsPanel : MonoBehaviour
{
    [SerializeField] private SelectSkinBtn[] _selectSkinBtns;
    [SerializeField] private Transform _selectSkinParent;

    private void OnEnable()
    {
        SkinsConfig skinConfig = MenuDataManager.Instance.SkinsConfig;

        for (int i = 0; i < skinConfig.Skins.Count; i++)
        {
            if (_selectSkinBtns.Length > i)
            {
                _selectSkinBtns[i].SetSkin(skinConfig.Skins[i], _selectSkinParent);
            }
        }
    }
}

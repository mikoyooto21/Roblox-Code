using UnityEngine;
using UnityEngine.UI;

public class SelectSkinBtn : MonoBehaviour
{
    [SerializeField] private Image _skinImg;

    private SkinData _skinData;
    private Transform _selectSkinParent;

    public void SetSkin(SkinData skinData, Transform selectSkinParent)
    {
        _skinData = skinData;
        _skinImg.sprite = skinData._skinImage;

        _selectSkinParent = selectSkinParent;
    }

    public void SelectSkin()
    {
        if (_skinData == null)
            return;

        PlayerPrefs.SetInt("PlayerSkinId", _skinData.SkinId);

        if (_selectSkinParent.childCount > 0)
            Destroy(_selectSkinParent.GetChild(0).gameObject);

        Instantiate(_skinData.SkinModelPrefab, _selectSkinParent);

        Debug.Log("Skin selected");
    }
}

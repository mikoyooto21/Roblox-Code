using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinsConfig", menuName = "New Skins Config")]
public class SkinsConfig : ScriptableObject
{
    [SerializeField] private List<SkinData> _skins = new List<SkinData>();

    public List<SkinData> Skins => _skins;
}

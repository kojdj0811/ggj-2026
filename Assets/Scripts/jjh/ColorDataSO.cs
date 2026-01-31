
using UnityEngine;

[CreateAssetMenu(fileName = "ColorData", menuName = "Scriptable Objects/ColorData")]
[System.Serializable]
public class ColorData
{
	public int Index;
	public string ColorCode;
	public Sprite CharacterSprite;
}

public class ColorDataSO : ScriptableObject
{
    [SerializeField]
    public ColorData[] colorDatas;
    
    	// index로 colorCode 반환
	public string GetColorCodeByIndex(int idx)
	{
		foreach (var data in colorDatas)
		{
			if (data.Index == idx)
				return data.ColorCode;
		}
		return null;
	}

	// index로 characterSprite 반환
	public Sprite GetCharacterSpriteByIndex(int idx)
	{
		foreach (var data in colorDatas)
		{
			if (data.Index == idx)
				return data.CharacterSprite;
		}
		return null;
	}
}

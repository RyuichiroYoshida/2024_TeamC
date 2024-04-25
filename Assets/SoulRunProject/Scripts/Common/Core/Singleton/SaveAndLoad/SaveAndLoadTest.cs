using SoulRunProject.Common;
using UnityEngine;

namespace SoulRunProject.SoulRunProject.Scripts.Common.Core.Singleton
{
    public class SaveAndLoadTest : MonoBehaviour
    {
        SaveAndLoadManager _saveAndLoadManager;
        public void Save()
        {
            
        }
        public void Load()
        {
        }
        
    }
}


/*
 
DataStorage{
	PlayerData
	MasterData
}
 

MasterData{
	SoulCardDataList
	SoulCardDataCombinations
	EnemyDataList
	ItemDataList
	PlayerDataList
}

MasterData
	Version v0.1
		SoulCardDataList
		SoulCardDataCombinations
		EnemyDataList
		ItemDataList
		PlayerDataList
		
PlayerData
	PlayerDataID (固有識別番号16進数の10桁)
	PlayerName
	Stage1
		MaxScore
	Stage2
		MaxScore
	CurrentMoney
	CurrentSoulCardDataList



SoulCardData
	CardID(固有番号)
	IndividualIdentificationNumber(固有識別番号16進数の10桁)
	Image
	SoulName
	SoulLevel
	SoulAbility
	Status
	TraitList

Combinations
	Ingredient1
	Ingredient2
	Result

EnemyData
	EnemyID
	EnemyImage
	EnemyName
	EnemyLevel
	EnemyAbility
	EnemyStatus
	
ItemDataList
	ItemID
	ItemImage
    ItemName
*/
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
   // game state
   public bool isGameInProgess;
   public bool hasChosenSkill;
   //scene
   public string currentSceneName;

   // player position
   public bool hasPlayerPosition;
   public float playerPositionX;
   public float playerPositionY;
   public float playerPositionZ;

   //player health
   public float playerCurrentHP = -1;
   
   //skill
   public int chosenSkillType;
   public int skillDecay;
   public int numOfUse;
   public float remainSkillColldown;
   public List<string> unlockedSkillUpgrades = new List<string>();
   public List<string> unlockedBetterUpgrades = new List<string>();

   //attribute upgrades
   public List<AttributeUpgradeSaveData> attributeUpgrades = new List<AttributeUpgradeSaveData>();

   //chest
   public List<string> openedChestID = new List<string>();
}

[Serializable]
public class AttributeUpgradeSaveData
{
   public int upgradeType;
   public int count;
}

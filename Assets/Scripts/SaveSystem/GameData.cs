using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
   // boss level
   public int numOfLevel;
   
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
   public float playerTemporaryShield = 0;
   public float playerPermanentShield = 0;
   public float playerTemporaryShieldRemainingDuration = 0;
   
   //skill
   public int chosenSkillType;
   public int skillDecay;
   public int numOfUse;
   public float remainSkillColldown;
   public List<string> unlockedSkillUpgrades = new List<string>();
   public List<string> unlockedBetterUpgrades = new List<string>();
   public List<string> unlockedSkillPhaseUpgrades = new List<string>();
   public int bloodGrowthMaxHPStacks;
   public int bloodGrowthAdvancedStacks;

   //attribute upgrades
   public List<AttributeUpgradeSaveData> attributeUpgrades = new List<AttributeUpgradeSaveData>();

   //chest
   public List<string> openedChestID = new List<string>();

   //level clear
   public List<string> clearedSceneNames = new List<string>();
}

[Serializable]
public class AttributeUpgradeSaveData
{
   public int upgradeType;
   public int count;
}

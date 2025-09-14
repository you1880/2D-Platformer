using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.User
{
    [System.Serializable]
    public class Userdata
    {
        public int playerType;
        public int maxActivityPoint;
        public int activityPoint;
        public int gold;
        public int level;
        public int exp;
        public int maxHp;
        public int hp;
        public int attack;
        public string lastSavedTime;
        public List<ClearStage> clearStages;

        public Userdata(int playerType, int gold = 0, int level = 1, int exp = 0)
        {
            this.playerType = playerType;
            this.gold = gold;
            this.level = level;
            this.exp = exp;
            attack = 1;
            maxHp = 3;
            hp = maxHp;
            lastSavedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            clearStages = new List<ClearStage>();
        }
    }

    [System.Serializable]
    public class ClearStage
    {
        public Define.Stage stage;
        public int clearMin;
        public int clearSec;

        public ClearStage(Define.Stage stage, int clearMin, int clearSec)
        {
            this.stage = stage;
            this.clearMin = clearMin;
            this.clearSec = clearSec;
        }
    }
}

namespace Data.Game
{
    [System.Serializable]
    public class GameData
    {
        public List<StageDependency> stageDependencies;
        public List<RequireExp> requireExp;
        public List<StageData> stageData;
        public List<Item> itemData;
        public Shop shopData;
        public List<Enemy> enemyData;

        public GameData()
        {
            stageDependencies = new List<StageDependency>();
            requireExp = new List<RequireExp>();
            stageData = new List<StageData>();
            itemData = new List<Item>();
            shopData = new Shop();
        }
    }

    [System.Serializable]
    public class StageDependency
    {
        public Define.Stage stage;
        public List<Define.Stage> requireStages;

        public StageDependency()
        {
            requireStages = new List<Define.Stage>();
        }
    }

    [System.Serializable]
    public class StageData
    {
        public Define.Stage stage;
        public int requireActivityPoint;
        public int clearGold;
        public int clearExp;
        public int minSkipGold;
        public int maxSkipGold;
    }

    [System.Serializable]
    public class RequireExp
    {
        public int level;
        public int exp;
    }

    [System.Serializable]
    public class Item
    {
        public int itemCode;
        public Define.ItemType itemType;
        public string itemName;
        public string itemLore;
        public int itemPrice;
        public ItemEffectSO itemEffect;
    }

    [System.Serializable]
    public class Shop
    {
        public List<int> consumableItemList;
        public List<int> equipmentItemList;

        public Shop()
        {
            consumableItemList = new List<int>();
            equipmentItemList = new List<int>();
        }
    }

    [System.Serializable]
    public class Enemy
    {
        public Define.EnemyType enemyType;
        public int maxHp;
        public int attack;
        public float moveSpeed;
        public float attackRange;
        public float attackDelay;
    }
}
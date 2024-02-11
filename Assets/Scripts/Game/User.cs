using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    [Serializable]
    public class CardV3
    {
        public int id;
        public int level;
        public int p;

        public CardV3(int id, int level, int p)
        {
            this.id = id;
            this.level = level;
            this.p = p;
        }

        public bool present()
        { return id != -1; }

        public int country()
        {
            if (id < 6) return 2;
            if (id < 12) return 1;
            return 0;
        }
    }

    [Serializable]
    public class DeckV2
    {
        [SerializeField] public List<CardV3> cards = new List<CardV3>();
        [SerializeField] public List<CardV3> reserve = new List<CardV3>();

        public DeckV2() {}
    }

    [Serializable]
    public class Task
    {
        [SerializeField] public int type;
        [SerializeField] public string description;
        [SerializeField] public int value;
        [SerializeField] public int need;
        [SerializeField] public int diff;
        [SerializeField] public int timeout;

        public bool complete()
        { return value >= need; }
    }

    [Serializable]
    public class TaskGeneralProcess
    {
        public const int TASK_GENERAL_STATE_NOT_COMPLETE = 0;
        public const int TASK_GENERAL_STATE_HAVE_REWARD = 1;
        public const int TASK_GENERAL_STATE_COMPLETE = 2;
        [SerializeField] public int id = 0;
        [SerializeField] public int value = 0;
        [SerializeField] public int goal = 1;
        [SerializeField] public string description;
        [SerializeField] public int coins = 10;
        [SerializeField] public int state = TASK_GENERAL_STATE_NOT_COMPLETE;
        [SerializeField] public int level = 0;
    };

    [Serializable]
    public class User
    {
        public string login = "";
        public string pass = "";
        public string name = "";
        public string auth = "";
        public int gold;
        public int backpacks;
        public int backpacks2;
        public int backpacks3;
        public int exp = 0;
        public int needExp = 1000;
        public int boosters = 0;
        public int level;
        public int backpackTime;
        public int remainBattles;
        public int remainBackpacks;
        public int lang = -1;
        public int proTimeout;
        public int wins;
        public int fails;
        public int totalTime;
        public int winsWithoutGold = 0;
        public int winsWithoutCard = 0;
        public int openedBackpacks = 0;
        public int openedBackpacks2 = 0;
        public int openedBackpacks3 = 0;
        public int todayBattles = 0;
        public int rank = 0;
        public int activeDeck = 0;

        [SerializeField] public List<DeckV2> decks = new List<DeckV2>();
        [SerializeField] public List<List<List<int>>> campaigns;
        [SerializeField] public List<Task> tasks;

        public int taskStartDate;
        public bool tasksRewardGetted;

        [SerializeField] public List<string> usedCoupons;
        [SerializeField] public int todayCoupons;

        [SerializeField] public bool arenaRewardPresent;
        [SerializeField] public ArenaResult arenaResult;

        public int lastBattleMoment = 0;
        public int lastChatMessage = 0;

        public int chatBanTimeout = 0;
        public int regTime = 0;
        public int adsDisableTimeout = 0;

        public int langV = -1;

        public bool tutorualGiftTaken = false;

        public int userID = -1;

        public User(){}

        public void applyResult(BackpackResult backpackResult)
        {
            backpacks       = backpackResult.backpacks;
            backpacks2      = backpackResult.backpacks2;
            backpacks3      = backpackResult.backpacks3;
            gold            = backpackResult.gold;
            exp             = backpackResult.exp;
            level           = backpackResult.level;
            boosters        = backpackResult.boosters;
            level           = backpackResult.level;
            proTimeout      = backpackResult.proTimeout;
            remainBackpacks = backpackResult.remainBackpacks;
            needExp         = backpackResult.needExp;
            remainBackpacks = backpackResult.remainBackpacks;

            foreach (CardMini card in backpackResult.cards)
            { decks[0].reserve.Add(new CardV3(card.id, card.level, card.p)); }
        }

        public void applyBattleResult(Battle battle)
        {
            remainBattles = battle.remainBattles = 0;
            wins = battle.total_wins;
            fails = battle.total_fails;
            applyResult(battle.profileResult);
            lastBattleMoment = battle.lastBattleMoment;
            todayBattles = battle.todayBattles;
        }

        public bool allTasksComplete()
        {
            foreach (Task t in tasks)
            { if (!t.complete()) return false; }

            return true;
        }

        public bool canGetTasksReward()
        { return allTasksComplete() && !tasksRewardGetted; }
    }
}
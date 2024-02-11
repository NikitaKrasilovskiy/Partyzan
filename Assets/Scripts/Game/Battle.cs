using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    [Serializable]
    public class BattleCard
    {
        public int id = -1;
        public int level = 0;
        public int attack = 0;
        public int hp = 0;
        public int p = 0;

        public bool present()
        { return id != -1; }

        public int country()
        {
            if (id < 6) return 2;
            if (id < 12) return 1;
            return 0;
        }

        internal bool isGeneral()
        { return ((id == 0) || (id == 6) || (id == 12)); }
    }

    [Serializable]
    public class EffectDef
    {
        public int deck = 0;
        public int id = 0;
        public int attack = 0;
        public int hp = 0;
        public int icon = -1;
    };

    [Serializable]
    public class ActionDef
    {
        public int action = 0;
        public int actorDeck = 0;
        public int actorId = 0;

        [SerializeField]
        public List<EffectDef> effects;
    };

    [Serializable]
    public class Battle
    {
        [SerializeField] public List<List<BattleCard>> disposition;
        [SerializeField] public List<ActionDef> actions;

        public int remainBattles = 0;
        public int stars = 0;
        public int total_wins = 0;
        public int total_fails = 0;
        public int arenaDropMoment = 0;
        public int lastBattleMoment = 0;
        public int todayBattles = 0;

        [SerializeField] public BackpackResult profileResult;
    }

    [Serializable]
    public class CardMini
    {
        public int id;
        public int level;
        public int p;
    }


    [Serializable]
    public class BackpackResult
    {
        public int result;
        public int backpacks;
        public int backpacks2;
        public int backpacks3;
        public int gold;
        public int exp;
        public int boosters;
        public int level;
        public List<Level> reward;
        public int proTimeout = 0;
        public int remainBackpacks;
        public List<CardMini> cards;
        public int deltaGold = 0;
        public int deltaExp = 0;
        public int deltaPremium = 0;
        public int needExp = 1000;
    }

    [Serializable]
    public class ServerState
    {
        public int servertime;
        public int arenaAwardMoment;
        public int arenaDropMoment;
        public bool chatBlocked;
        public bool noAds;
    }

    [Serializable]
    class FreeBackpack
    {
        public int backpackTime = 0;
        public int backpacks = 0;
    }
}
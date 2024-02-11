using ServerSide;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerSide
{
    public class RatingServerSide
    {
        /*
     *  Установка /setArenaTimer/<int>
        Получение награды /getArenaReward
     */
        private const string GET_ARENA_TIMER = "getArenaTimer";
        private const string GET_ARENA_REWARD = "getArenaReward/";

        //Test
        DateTime dt = DateTime.Now+new TimeSpan(0,3,0);

        public void LoadEndTime(TimeCallback timeCallback)
        { ServerData.Instance.StartCoroutine(IELoadEndTime(timeCallback)); }

        IEnumerator IELoadEndTime(TimeCallback timeCallback)
        {
            yield return ServerData.Instance.loadDataFromServer(GET_ARENA_TIMER, (string response) =>
             {
                 int lastTime = 0;
                 int.TryParse(response, out lastTime);
                 DateTime dateTime = new DateTime(1970, 1, 1) + new TimeSpan(0, 0, lastTime);
#if UNITY_EDITOR
                 timeCallback.Invoke(dt);
#else
                timeCallback.Invoke(dateTime);
#endif
                 return 0;
             });
        }

        public void SetLastTime(TimeCallback timeCallback)
        { ServerData.Instance.StartCoroutine(IESetLastTime(timeCallback)); }

        IEnumerator IESetLastTime(TimeCallback timeCallback)
        {
            yield return ServerData.Instance.TimeRequest();
            DateTime curentRewardDT = ServerData.Instance.lastRequestTime + new TimeSpan(12, 0, 0);
            TimeSpan timeSpan = curentRewardDT - new DateTime(1970, 1, 1);
            Debug.LogError(timeSpan.TotalSeconds);
#if UNITY_EDITOR
            dt = DateTime.Now + new TimeSpan(0, 3, 0);
            timeCallback.Invoke(dt);
#else
                timeCallback.Invoke(curentRewardDT);
#endif
          //  timeCallback.Invoke(curentRewardDT);
            yield return ServerData.Instance.loadDataFromServer("setArenaTimer", (string response) => { return 0; }, body: timeSpan.TotalSeconds.ToString());
        }

        public void GetReward(TimeCallback timeCallback)
        { ServerData.Instance.StartCoroutine(IEGetReward(timeCallback)); }

        IEnumerator IEGetReward(TimeCallback timeCallback)
        {
            yield return ServerData.Instance.loadDataFromServer(GET_ARENA_REWARD, (string response) => { return 0; });
            yield return ServerData.Instance.TimeRequest();

            DateTime curentRewardDT = ServerData.Instance.lastRequestTime + new TimeSpan(12, 0, 0);
            TimeSpan timeSpan = curentRewardDT - new DateTime(1970, 1, 1);
#if UNITY_EDITOR
            dt = DateTime.Now + new TimeSpan(0, 3, 0);
            timeCallback.Invoke(dt);
#else
                timeCallback.Invoke(curentRewardDT);
#endif
            yield return ServerData.Instance.loadDataFromServer("setArenaTimer", (string response) => { return 0; }, body: timeSpan.TotalSeconds.ToString());
        }
    }
}
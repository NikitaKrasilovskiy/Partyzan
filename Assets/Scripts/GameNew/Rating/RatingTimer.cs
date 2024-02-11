using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerSide;
using System;
using UnityEngine.Events;


namespace Rating
{
    public class RatingTimer : MonoBehaviour
    {
        DateTime endTime;
        float lengthInSecconds;

        RatingServerSide ratingServerSide;

        void Awake()
        {
            ratingServerSide = new RatingServerSide();
            LoadEndTime();
        }

        public TimeSpan GetTimeToReward()
        {
            TimeSpan ts = new TimeSpan(0, 0, (int)lengthInSecconds);
            return ts;
        }

        public void OnApplicationFocus(bool focus)
        {
            if (focus)
            { UpdateTimer(); }
        }

        void LoadEndTime()
        { ratingServerSide.LoadEndTime(LoadEndTime); }

        void LoadEndTime(DateTime dateTime)
        {
            //  Debug.LogError("Load End Time " + dateTime);
            if (dateTime.Year < 2018)
            {
                Debug.Log("Set");
                SaveEndTime();
            }
            else
            { endTime = dateTime; }

            UpdateTimer();
        }

        void SaveEndTime()
        { ratingServerSide.SetLastTime(LoadEndTime); }

        public void GetReward()
        { ServerData.Instance.GetTime(GetRewardCallBack); }

        IEnumerator IEGetReward()
        { yield return true; }

        void GetRewardCallBack(DateTime time)
        {
            Debug.Log(time+" "+endTime+ " "+ (time - endTime).TotalSeconds+" "+ (endTime < time));

            if (endTime<time)
            { ratingServerSide.GetReward(LoadEndTime); }
            else
            { UpdateTime(time); }
        }

        private void OnEnable()
        { UpdateTimer(); }

        public int GetTimeToEnd()
        { return (int)lengthInSecconds; }

        public void UpdateTimer()
        { ServerData.Instance.GetTime(UpdateTime); }

        private void Update()
        { lengthInSecconds -= Time.deltaTime; }

        void UpdateTime(DateTime dateTime)
        {
            Debug.Log("Update Time" + lengthInSecconds);
            lengthInSecconds = (float)(endTime - dateTime).TotalSeconds;
        }
    }
}
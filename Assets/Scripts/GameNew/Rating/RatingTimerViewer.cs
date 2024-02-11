using CCGKit;
using Rating;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rating {
    public class RatingTimerViewer : MonoBehaviour
    {
        [SerializeField]
        RatingTimer ratingTimer;

        [SerializeField]
        GameObject timeViewer;

        [SerializeField]
        GameObject rewardButton;

        [SerializeField]
        GameObject helpButton;

        [SerializeField]
        TextMeshProUGUI textMesh;

        const string EMPTY_STRING = "--:--:--";

        private void OnEnable()
        {
            Model model = GameManager.Instance.model;

            StartCoroutine(IEUpdate());
        }

        IEnumerator IEUpdate()
        {
            while (enabled)
            {
                yield return new WaitForSeconds(0.3f);
                UpdateView(ratingTimer.GetTimeToReward());
            }
        }

        void UpdateView(TimeSpan timeSpan)
        {
            Model model = GameManager.Instance.model;
            int i = model.ranks.rank;

            if (i <= 100)
            {
                helpButton.SetActive(false);

                if (timeSpan.TotalSeconds == 0)
                {
                    textMesh.text = EMPTY_STRING;
                    timeViewer.SetActive(true);
                }
                else
                {
                    if (timeSpan.TotalSeconds > 0)
                    {
                        timeViewer.SetActive(true);
                        rewardButton.SetActive(false);
                        textMesh.text = timeSpan.ToString();
                    }
                    else
                    {
                        timeViewer.SetActive(false);
                        rewardButton.SetActive(true);
                        textMesh.text = "";
                    }
                }
            }
            else
            {
                textMesh.text = "";
                timeViewer.SetActive(false);
                rewardButton.SetActive(false);
                helpButton.SetActive(true);
            }
        }
    }
}
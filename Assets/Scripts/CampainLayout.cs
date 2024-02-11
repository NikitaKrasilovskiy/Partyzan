using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampainLayout : MonoBehaviour
{
    [SerializeField]
    List<GameObject> points;

    [SerializeField]
    Transform content;

    [SerializeField]
    float er = 0.05f;

    [SerializeField]
    float speed = 1;

    private IEnumerator Start()
    {
        Model model = GameManager.Instance.model;
        yield return true;
        int index = 0;

        for (int i = 0; i < points.Count; i++)
        {
            CityButtonV2 cb = points[i].GetComponent<CityButtonV2>();
          int stars = model.user.campaigns[cb.campaignID][cb.cityID][cb.areaID];

            if (stars > 0)
                index = i;
            else
                break;
        }
        Transform t = points[index].transform;

        if(index>1)
        while (t.position.x > er)
        {
            Vector3 v = content.position;
            v.x -= Time.deltaTime * speed;
            content.position = v;

                if (Input.GetMouseButton(0) || Input.touchCount > 0)
                    break;
            yield return true;
        }
    }
}
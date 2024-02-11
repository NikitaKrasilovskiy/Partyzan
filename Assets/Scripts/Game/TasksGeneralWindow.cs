using CCGKit;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Hub;

// ReSharper disable All

public class TasksGeneralWindow: MonoBehaviour
{
	public TaskGeneralScript prefab;
	public ScrollRect scrollRect;
	public PreloaderAmination preloader;

	private List<TaskGeneralScript> tgss;

	public void OpenWindow()
	{
		gameObject.SetActive(true);
		UpdateData();
	}

	public void CloseWindow()
	{ gameObject.SetActive(false); }

	void UpdateData()
	{ StartCoroutine(UpdateDataProcess()); }

	private float y = -220;
	private Model model;

	void ShowTasksByState(int state)
	{
		foreach(TaskGeneralProcess taskGeneralProcess in model.tasksGeneral)
		{
			if(taskGeneralProcess.state != state) continue;
			TaskGeneralScript tgs = Instantiate(prefab);
			tgs.gameObject.transform.SetParent(scrollRect.content);
			tgs.Show(taskGeneralProcess);
			tgs.transform.localPosition = new Vector3(8 + 562.65f, y, 0);
			tgs.transform.localScale = Vector3.one;
			tgs.preloader = preloader;
			tgss.Add(tgs);
			y -= 180;
		}
	}

	IEnumerator UpdateDataProcess()
	{
		if(tgss != null)
		{
			foreach(TaskGeneralScript tgs in tgss)
			{ Destroy(tgs.gameObject); }
		}

		tgss = new List<TaskGeneralScript>();
		model = GameManager.Instance.model;
		prefab.gameObject.SetActive(false);
		preloader.Activate();
		yield return model.getTasksGeneral();
		y = -220;
		prefab.gameObject.SetActive(true);
		ShowTasksByState(TaskGeneralProcess.TASK_GENERAL_STATE_HAVE_REWARD);
		ShowTasksByState(TaskGeneralProcess.TASK_GENERAL_STATE_NOT_COMPLETE);
		ShowTasksByState(TaskGeneralProcess.TASK_GENERAL_STATE_COMPLETE);

		//   log("======================");
		//   
		//   // Write to disk
		//   StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/sp.txt", true);
		//
		//   string data = "";
		//   
		//
		//   foreach (TaskGeneralProcess taskGeneralProcess in model.tasksGeneral)
		//   {
		//    
		//    
		//    string str = taskGeneralProcess.description;
		//    var id = model.getTextId(str);
		//    if(!model.haveText(id))
		//    {
		//     
		// str = str.Replace("%d", "" + taskGeneralProcess.goal);
		//
		// id = model.getTextId(str);
		//
		// if(!model.haveText(id))
		// {
		//
		// 	str = str.Replace("-", " ");
		// 	
		// 	id = model.getTextId(str);
		//
		// 	if(!model.haveText(id))
		// 	{
		// 		str = str.Replace("3 место", "3е место");
		// 		str = str.Replace("2 место", "2е место");
		// 		str = str.Replace("1 место", "1е место");
		// 		
		// 		str = str.Replace("Открыть ячейку для карт 3", "Открыть ячейку для карт %d");
		// 		str = str.Replace("Открыть ячейку для карт 4", "Открыть ячейку для карт %d");
		// 		str = str.Replace("Открыть ячейку для карт 5", "Открыть ячейку для карт %d");
		// 	
		// 		id = model.getTextId(str);
		// 		
		// 	}
		// }
		//    }
		//
		//    if(id == -1)
		//    {
		//     log(str);
		//    }
		//    
		//    var txt = model.getText(id);
		//
		//    txt = txt.Replace("%d", "" + taskGeneralProcess.goal);
		//
		//    // data += "Erreichung " + (taskGeneralProcess.id + 1) + "\n";
		//    data += "Logro " + (taskGeneralProcess.id + 1) + "\n";
		//
		//    data += txt + "\n";
		//   }
		//   
		//   writer.Write(data);
		//   writer.Close();
		//   
		prefab.gameObject.SetActive(false);
		preloader.Deactivate();
	}

	public void ClickNext()
	{ scrollRect.verticalNormalizedPosition = scrollRect.verticalNormalizedPosition - 0.0476f; }
}
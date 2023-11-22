using RPGCore.Base;
using RPGCore.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.UI
{
	//使用这个UIManager来控制Panel的显隐
	public class UIManager : Singleton<UIManager>
	{
		public int PanelCount => panelDic.Count;
		//用来存储Panel 当一个UIManger第一次Show一个Panel时 会创建这个Panel的实例加入到这个Dictionary中
		private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

		private Transform canvasTrans;

		public UIManager()
		{
			canvasTrans = GameObject.Find("Canvas").transform;
			GameObject.DontDestroyOnLoad(canvasTrans.gameObject);
		}

		/// <summary>
		/// 显示一个面板
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T ShowPanel<T>() where T : BasePanel
		{
			//通过反射拿到名称 再根据名称判断当前面板是否已经有了
			string panelName = typeof(T).Name;
			if (panelDic.ContainsKey(panelName))
			{
				panelDic[panelName].gameObject.SetActive(true);
				panelDic[panelName].Show();
				return panelDic[panelName] as T;
			}
			//当前面板不存在就创建一个 并且加入dictionary
			GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName));
			panelObj.transform.SetParent(canvasTrans, false);
			T panel = panelObj.GetComponent<T>();
			panelDic[panelName] = panel;

			//调用面板自身的显示方法
			panel.Show();

			return panel;
		}

		/// <summary>
		/// 隐藏面板
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="isFade"></param>
		public void HidePanel<T>(bool isFade = true) where T : BasePanel
		{
			string panelName = typeof(T).Name;
			if (panelDic.ContainsKey(panelName))
			{
				if (isFade)
				{
					panelDic[panelName].Hide(() =>
					{
						panelDic[panelName].gameObject.SetActive(false);
					});
				}
				else
				{
					GameObject.Destroy(panelDic[panelName].gameObject);
					panelDic.Remove(panelName);
				}
			}
		}

		public T GetPanel<T>() where T : BasePanel
		{
			string panelName = typeof(T).Name;
			if (panelDic.ContainsKey(panelName))
			{
				return panelDic[panelName] as T;
			}
			else
			{
				return null;
			}
		}
	}
}
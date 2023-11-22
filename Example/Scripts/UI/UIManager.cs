using RPGCore.Base;
using RPGCore.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.UI
{
	//ʹ�����UIManager������Panel������
	public class UIManager : Singleton<UIManager>
	{
		public int PanelCount => panelDic.Count;
		//�����洢Panel ��һ��UIManger��һ��Showһ��Panelʱ �ᴴ�����Panel��ʵ�����뵽���Dictionary��
		private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

		private Transform canvasTrans;

		public UIManager()
		{
			canvasTrans = GameObject.Find("Canvas").transform;
			GameObject.DontDestroyOnLoad(canvasTrans.gameObject);
		}

		/// <summary>
		/// ��ʾһ�����
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T ShowPanel<T>() where T : BasePanel
		{
			//ͨ�������õ����� �ٸ��������жϵ�ǰ����Ƿ��Ѿ�����
			string panelName = typeof(T).Name;
			if (panelDic.ContainsKey(panelName))
			{
				panelDic[panelName].gameObject.SetActive(true);
				panelDic[panelName].Show();
				return panelDic[panelName] as T;
			}
			//��ǰ��岻���ھʹ���һ�� ���Ҽ���dictionary
			GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName));
			panelObj.transform.SetParent(canvasTrans, false);
			T panel = panelObj.GetComponent<T>();
			panelDic[panelName] = panel;

			//��������������ʾ����
			panel.Show();

			return panel;
		}

		/// <summary>
		/// �������
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
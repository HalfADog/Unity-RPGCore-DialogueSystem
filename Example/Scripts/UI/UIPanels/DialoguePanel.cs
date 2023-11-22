using RPGCore.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : BasePanel
{
	public TMP_Text sentenceContent;
	public TMP_Text speakerContent;
	public GameObject choicesContainer;
	public GameObject choiceItem;
	public Action<int> OnChoiceSelected;
	public Action OnMoveNext;
	private int choiceCount=0;

	public override void Init()
	{
		OnMoveNext?.Invoke();
	}

	public void ShowChoices()
	{
		choicesContainer.SetActive(true);
	}

	public void HideChoices() 
	{
		choicesContainer.SetActive(false);
		for(int i = 0;i<choicesContainer.transform.childCount;i++) 
		{
			Destroy(choicesContainer.transform.GetChild(i).gameObject);
		}
		choiceCount=0;
	}

	public void AddChoiceItem(string content)
	{
		GameObject ci = GameObject.Instantiate(choiceItem);
		ci.GetComponent<Button>().onClick.AddListener(()=> { OnChoiceSelected?.Invoke(ci.GetComponent<ChoiceItem>().Id); });
		ci.GetComponent<TMP_Text>().text = content;
		ci.GetComponent<ChoiceItem>().Id = choiceCount;
		ci.transform.SetParent(choicesContainer.transform);
		choiceCount++;
	}

	public void SetSentenceContent(string content) 
	{
		sentenceContent.text = content;
	}

	protected override void Update()
	{
		base.Update();	
		if (Input.GetKeyDown(KeyCode.Space))
		{
			OnMoveNext?.Invoke();
		}
	}
}

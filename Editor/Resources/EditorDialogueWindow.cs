using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGCore.Dialogue.Editor
{
    public enum EditorDialogueType 
    {
        Message,
        CreateAsset
    }
    public class EditorDialogueWindow
    {
        public VisualTreeAsset DialogueVisualTreeAsset { get; private set; }
        public VisualElement DialogueContainer { get; private set; }

        private TemplateContainer template;

        //�Ի������õĲ�ͬ�����
        private VisualElement messagePanel;
        private VisualElement createAssetPanel;
        //��ǰ�Ի���򿪵����
        private VisualElement currentOpendPanel;
        //��������ϵĿؼ�
        private Label dialogueTitleLabel;
        private Label messagePanelmessageLabel;
        private TextField createAssetNameField;
        private TextField createAssetDescriptionField;
        private Button okButton;
        private Button cancelButton;

        //��ť�¼�
        private EventCallback<ClickEvent> okEvent;
		private EventCallback<ClickEvent> cancelEvent;
		public EditorDialogueWindow(VisualElement contanier,string assetPath)
        {
            DialogueContainer = contanier;
			//DialogueVisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);
			DialogueVisualTreeAsset = Resources.Load<VisualTreeAsset>(assetPath);
			if (DialogueVisualTreeAsset == null)
            {
                Debug.LogWarning($"Load Asset {assetPath} failed.");
                return;
            }
            template = DialogueVisualTreeAsset.CloneTree();
			DialogueContainer.Add(template);

            //��ȡ�����пؼ��Ϳؼ��ϵ����
            dialogueTitleLabel = template.Q<Label>("DialogueTitle");
            messagePanel = template.Q<VisualElement>("Message");
            createAssetPanel = template.Q<VisualElement>("CreateAsset");
            //messagePanel
            messagePanelmessageLabel = template.Q<Label>("Message");
            //createGroupPanel
            createAssetNameField = template.Q<TextField>("AssetNameField");
            createAssetDescriptionField = template.Q<TextField>("AssetDescriptionField");
            //button
            okButton = template.Q<Button>("OK");
            cancelButton = template.Q<Button>("Cancel");

			//��ʼ�����
		}

        public void OpenDialogue(string title,EditorDialogueType dialogueType,
            EventCallback<ClickEvent> okEvent,EventCallback<ClickEvent> cancelEvent,string message = "")
        {
            dialogueTitleLabel.text = title;
            this.okEvent = okEvent;
            this.cancelEvent = cancelEvent;
            okButton.RegisterCallback(this.okEvent);
            cancelButton.RegisterCallback(this.cancelEvent);
            switch (dialogueType)
            {
                case EditorDialogueType.Message:
                    messagePanel.style.visibility = Visibility.Visible;
                    messagePanel.style.display = DisplayStyle.Flex;
                    messagePanelmessageLabel.text = message;
                    currentOpendPanel = messagePanel;
					break;
                case EditorDialogueType.CreateAsset:
                    createAssetPanel.style.visibility = Visibility.Visible;
                    createAssetPanel.style.display = DisplayStyle.Flex;
                    currentOpendPanel = createAssetPanel;
					break;
            }
            DialogueContainer.style.visibility = Visibility.Visible;
		}

        public void CloseDialogue()
        {
			currentOpendPanel.style.visibility = Visibility.Hidden;
			currentOpendPanel.style.display = DisplayStyle.None;
            okButton.UnregisterCallback(okEvent);
			cancelButton.UnregisterCallback(cancelEvent);
			DialogueContainer.style.visibility = Visibility.Hidden;
			//WindowContainer.Remove(template);
        }

        public string GetCreateAssetName() 
        {
            return createAssetNameField.text;
        }

        public string GetCreateAssetDescription() 
        {
            return createAssetDescriptionField.text;
        }
    }
}

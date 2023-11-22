namespace RPGCore.Dialogue.Runtime
{
	public interface IDgNode
    {
		string Guid { get; }
		DgNodeType Type { get; }
		//���ݾ��������ȡ��һ���ڵ�
		IDgNode GetNext(object param = null);
        T GetNext<T>(object param = null) where T : IDgNode;
        //ת���ڵ�����
        T Get<T>() where T : IDgNode;
        //���һ���ڵ�
        void AddNext(IDgNode dgNode);
    }
}
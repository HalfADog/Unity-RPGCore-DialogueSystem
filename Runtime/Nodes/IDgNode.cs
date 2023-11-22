namespace RPGCore.Dialogue.Runtime
{
	public interface IDgNode
    {
		string Guid { get; }
		DgNodeType Type { get; }
		//根据具体参数获取下一个节点
		IDgNode GetNext(object param = null);
        T GetNext<T>(object param = null) where T : IDgNode;
        //转化节点类型
        T Get<T>() where T : IDgNode;
        //添加一个节点
        void AddNext(IDgNode dgNode);
    }
}
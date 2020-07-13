using System.Collections;
using System.Collections.Generic;
using Assets._Project.Scripts.treatment;

class TreeNode : IEnumerable<TreeNode>
{
    private readonly Dictionary<ModelData, TreeNode> _children =
        new Dictionary<ModelData, TreeNode>();

    public readonly ModelData ID;
    public TreeNode Parent { get; private set; }

    public TreeNode(ModelData id)
    {
        this.ID = id;
    }

    public TreeNode GetChild(ModelData id)
    {
        return this._children[id];
    }

    public void Add(TreeNode item)
    {
        if (item.Parent != null)
        {
            item.Parent._children.Remove(item.ID);
        }

        item.Parent = this;
        this._children.Add(item.ID, item);
    }

    public IEnumerator<TreeNode> GetEnumerator()
    {
        return this._children.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public int Count
    {
        get { return this._children.Count; }
    }


}





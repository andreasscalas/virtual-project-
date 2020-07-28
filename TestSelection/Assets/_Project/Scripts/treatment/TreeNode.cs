using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets._Project.Scripts.treatment;
using UnityEngine;

public class TreeNode : IEnumerable<TreeNode>
{
    private readonly Dictionary<int, TreeNode> _children =
        new Dictionary<int, TreeNode>();
    public readonly int ID;
    public TreeNode Parent { get; private set; }
    private GameObject go;
    private ModelData data;
    private int Level;
    public bool visited = false;

    public TreeNode(ModelData data)
    {
        this.data = data;
        this.ID = data.id;
        this.Level = 0;
        this.go = new GameObject();
        this.go.name = data.tag;
    }

    public GameObject GetGameobject()
    {
        return go;
    }

    public ModelData GetData()
    {
        return this.data;
    }

    public int GetLevel()
    {
        return Level;
    }

    public void SetLevel(int Level)
    {
        this.Level = Level;
    }

    public List<TreeNode> GetChildren()
    {
        var childrenList = new List<TreeNode>();
        foreach (var child in _children.Values)
        {
            childrenList.Add(child);
        }

        //var childrenList =  _children.Values.SelectMany(x => x).ToList();
        return childrenList;
        
    }


    public TreeNode GetChild(int id)
    {
        return this._children[id];
    }

    public void Add(TreeNode item)
    {
        if (item.Parent != null)
        {
            item.Parent._children.Remove(item.ID);
        }

        this._children.Add(item.ID, item);
        item.Parent = this;
        item.SetLevel(Level + 1);
        item.GetGameobject().transform.parent = item.Parent.GetGameobject().transform;
    }

    public IEnumerator<TreeNode> GetEnumerator()
    {
        return this._children.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public int GetChildrenCount
    {
        get { return this._children.Count; }
    }

    public TreeNode GetDescendent(int id)
    {
        if (this.ID==id)
        {
            return this;
        }

        var children = this.GetChildren();

        for (int i = 0; i < children.Count; i++)
        {
            var result = children[i].GetDescendent(id);
            if (result!=null)
            {
                return result;
            }
        }

        return null;
    }



}





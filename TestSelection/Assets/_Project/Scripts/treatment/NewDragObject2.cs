using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewDragObject2 : MonoBehaviour
{

 
 
    // 创建一个Cube 将该代码拖拽给Cube然后运行，点击Cube然后拖拽Cube，
    //Cube会被鼠标拖拽移动
    // 注意 ： 创建的 Cube 默认带有一个 Box Collider 碰撞盒， 
    //如果拖拽对象上没有 碰撞盒， 则不能被检测到拖拽 
    
    public Transform myTransform;
    Vector3 selfScenePosition;
    //private float mZCoord;
    //private Vector3 mOffset;

    void Start()
    {
        //myTransform = transform;
        Debug.Log("transform "+ transform);
            //将自身坐标转换为屏幕坐标
        selfScenePosition = Camera.main.WorldToScreenPoint(transform.position);
        
        print("scenePosition   :  " + selfScenePosition);
    }

    void OnMouseDrag() //鼠标拖拽时系统自动调用该方法
    {
            //获取拖拽点鼠标坐标
        print("     x  "+Input.mousePosition.x + "     y  " + Input.mousePosition.y + "     z  " + Input.mousePosition.z);
            //新的屏幕点坐标
        Vector3 currentScenePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, selfScenePosition.z);
            //将屏幕坐标转换为世界坐标
        Vector3 crrrentWorldPosition = Camera.main.ScreenToWorldPoint(currentScenePosition);
            //设置对象位置为鼠标的世界位置
        transform.position = crrrentWorldPosition;
    }

    void OnMouseDown()
    {
        Debug.Log("鼠标按下时");
    }

    void OnMouseUp()
    {
        print("鼠标抬起时");
    }

    void OnMouseEnter()
    {
        print("鼠标进入该对象区域时");
    }

    void OnMouseExit()
    {
        print("鼠标离开该模型区域时");
    }
}



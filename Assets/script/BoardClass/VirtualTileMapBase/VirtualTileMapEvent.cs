using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VirtualTileMapEvent
{
    /// <summary>
    ///  타겟팅되는 타일이 존재할 때 호출된다.
    /// </summary>
    public virtual void OnDraggingObjectTo(int x, int y, ResourceObjectInfo ResourceObject, ref List<ResourceObjectInfo> existObjects) { }

    /// <summary>
    /// linkedObject에 있는 0번 버퍼가 움직이고 있는 객체이다.
    /// </summary>
    public virtual void OnDragObject(List<int> FocusTiles, List<ResourceObjectInfo> linkedObject) { } 

    /// <summary>
    ///  합쳐지는 정보
    /// </summary>
    public virtual void OnMergeAction(List<ResourceObjectInfo> linkedObject) { }

    /// <summary>
    ///  리소스 오브잭트가 이동할 때 호출된다.
    /// </summary>
    public virtual void OnMoveResourceObject(int oldX, int oldY, int newX, int newY, ResourceObjectInfo ResourceObject) { }

    /// <summary>
    ///  리소스 오브잭트를 타일에 배치할 때 불려진다.
    /// </summary>
    public virtual void OnDropResourceObject(int x, int y, ResourceObjectInfo ResourceObject) { }

    /// <summary>
    ///  타일 속성이 업데이트 됨
    /// </summary>
    public virtual void OnUpdateTileState(int x, int y) { }

    /// <summary>
    ///  파괴될 오브잭트
    /// </summary>
    public virtual void OnDestroyResourceObject(ResourceObjectInfo obj) {  }

    /// <summary>
    /// 객체 합치기를 요청함.
    /// </summary>
    /// <param name="x">합쳐지는 기준 x</param>
    /// <param name="y">합쳐지는 기준 y</param>
    /// <param name="size">합쳐지는 수</param>
    /// <param name="objectID"> 오브잭트</param>
    public virtual void OnRequestMergeObject(int x,int y,int size,int objectID) { }
}

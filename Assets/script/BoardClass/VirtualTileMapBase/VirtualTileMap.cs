using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class ResourceObjectType
{
    public Vector2Int size;
    public int[] ResourceObjectArray;
}

public class ResourceObjectInfo
{
    public int ObjectID;
    public bool isMerge;
    public ResourceObjectType shape;
    public Vector2Int tilePosition;
}

public enum TileState
{
    NORMAL = 0
}
public class TileObject
{
    public int x, y;
    public int LayerIndex;
    public int LockIndex;
    public TileState State;
    public ResourceObjectInfo Object;
}
public class VirtualTileMap : VirtualTileMapEvent
{
    #region 변수 모음

    /// <summary>
    ///  블럭 형태
    /// </summary>
    #region 블럭 클래스 - TileObject
    #endregion
    protected Vector2Int boardSize;
    protected TileObject[] _Board = null;
    #endregion
    
    // 맵 생성
    public void Create(int maxX,int maxY)
    {
        boardSize = new Vector2Int(maxX, maxY);
        _Board = new TileObject[maxX * maxY];
        // 원래대로면 로딩하는 코드도 여기에 있어야 한다.
    }


    // 타일 정보 얻기
    public TileObject GetTile(int x,int y)
    {
        int address = boardSize.x * y + x;
        if(address >= boardSize.x * boardSize.y)
        {
            return null;
        }
        return _Board[address];
    }

    /// <summary>
    ///  이동이 가능한 객체인가.
    /// </summary>
    public bool IsLockedResourceObject(ResourceObjectInfo resourceObject)
    {
        int x = resourceObject.tilePosition.x;
        int y = resourceObject.tilePosition.y;

        int ROA = 0;
        for (int yy = 0; yy < resourceObject.shape.size.y; yy++)
        {
            for (int xx = 0; xx < resourceObject.shape.size.x; xx++)
            {
                if (resourceObject.shape.ResourceObjectArray[ROA] != 0)
                {
                    var tile = GetTile(x+xx, y+yy);
                    if (tile != null)
                    {
                        if (tile.LayerIndex > 0 || tile.LockIndex > 0 || tile.State != TileState.NORMAL)
                        {
                            return false;
                        }
                    } else
                    {
                        return false;
                    }
                }
                ROA++;
            }
        }
        return true;
    }

    // 타일에 오브젝트 채우거나 비우기
    protected void PaintResourceObjectToTile(int x,int y, ResourceObjectInfo resourceObject, bool isClear)
    {
        int ROA = 0;
        for (int yy = 0; yy < resourceObject.shape.size.y; yy++)
        {
            for (int xx = 0; xx < resourceObject.shape.size.x; xx++)
            {
                if (resourceObject.shape.ResourceObjectArray[ROA] != 0)
                {
                    var tile = GetTile(x + xx, y + yy);
                    if (tile != null)
                    {
                        if(isClear == true)
                        {
                            tile.Object = null;
                            resourceObject.tilePosition.x = -1;
                            resourceObject.tilePosition.y = -1;
                        } else
                        {
                            tile.Object = resourceObject;
                            resourceObject.tilePosition.x = x;
                            resourceObject.tilePosition.y = y;
                        }

                    }
                }
                ROA++;
            }
        }
    }

    // 선택가능한 타일인지 정보 얻기
    public bool IsEnableTile(int x,int y,out ResourceObjectInfo retValue)
    {
        retValue = null;
        var tile = GetTile(x, y);
        if(tile != null)
        {
            if(tile.LayerIndex > 0 || tile.LockIndex > 0 || tile.State != TileState.NORMAL)
            {
                return false;
            }
            else
            {
                // 1개 이상의 타일을 사용하는 오염된 객체가 있을 수 있다.
                if(tile.Object != null)
                {
                    if(tile.Object.shape.size.y * tile.Object.shape.size.x > 1)
                    {
                        if(IsLockedResourceObject(tile.Object) == false)
                        {
                            return false;
                        } else
                        {
                            retValue = tile.Object;
                            return true;
                        }
                    } else
                    {
                        retValue = tile.Object;
                        return true;
                    }
                } else
                {
                    return true;
                }
            }
        }
        return false;
    }

    //IsEnableMoveTo 클라이언트용으로 땅을 차지하고 있는 ResourceObjectInfo들을 얻는 함수가 더 필요하다.
    public bool IsEnableMoveTo(int x,int y, ResourceObjectInfo resourceObject)
    {
        List<ResourceObjectInfo> existObjects = new List<ResourceObjectInfo>();
        return IsEnableMoveTo(x, y, resourceObject, ref existObjects);
    }

    public bool IsEnableMoveTo(int x, int y, ResourceObjectInfo resourceObject,ref List<ResourceObjectInfo> existObjects)
    {
        int ROA = 0;
        bool returnValue = true;
        for (int yy = y; yy < y + resourceObject.shape.size.y; yy++)
        {
            for (int xx = x; xx < x + resourceObject.shape.size.x; xx++)
            {
                if (resourceObject.shape.ResourceObjectArray[ROA] != 0)
                {
                    ResourceObjectInfo retValue = null;

                    if(IsEnableTile(x + xx, y + yy, out retValue) == true)
                    {
                        if(resourceObject.Equals(retValue) == false && retValue.Equals(null) == false)
                        {
                            if(existObjects.Contains(retValue) == false)
                            {
                                existObjects.Add(retValue);
                            }
                        }
                    } else
                    {
                        returnValue = false;
                    }
                }
                ROA++;
            }
        }
        return returnValue;
    }

    /// <summary>
    /// 타일에 객체를 내려 놓는다.
    /// </summary>
    public void ResourceObjectDropToTile(int x,int y, ResourceObjectInfo resourceObject, bool checkMerge)
    {
        // 놓을 수 없는 영역에 위치하는가
        List<ResourceObjectInfo> linkedObject = new List<ResourceObjectInfo>();
        List<ResourceObjectInfo> AlreadyExistTileObject = new List<ResourceObjectInfo>();   // 이미 있는 타일들

        List<int> _checkedSameTiles = new List<int>();

        bool ismove = IsEnableMoveTo(x, y, resourceObject, ref AlreadyExistTileObject);
        if(ismove == false)
        {
            // 빈 자리로 이동하게 한다.
            return;
        }
        // 자리를 차지고 있던 원래 객체들을 밀쳐낸다.( 밀쳐낸 객체를 기록하고 있자. )

        // checkMerge 옵션에 따라서 머지를 하고 결과를 노티한다.
        if (resourceObject.isMerge == true && checkMerge == true)
        {
            int objectID = resourceObject.ObjectID;
            int linkedCount = linkedObject.Count;

            GetSameResourceObject(x, y, resourceObject, ref linkedObject, ref _checkedSameTiles);

            // 나머지 객체는 이동 시킨다.
            List<ResourceObjectInfo> result = AlreadyExistTileObject.Except(linkedObject).ToList();

            for (int i = 0; i < result.Count; i++)
            {
                bool canMove = GetEmptyTile(result[i].tilePosition.x, result[i].tilePosition.y, result[i],out Vector2Int newPos);
                if(canMove == true)
                {
                    PaintResourceObjectToTile(result[i].tilePosition.x, result[i].tilePosition.y, result[i], true);
                    OnMoveResourceObject(result[i].tilePosition.x, result[i].tilePosition.y, newPos.x, newPos.y, result[i]); // 노티
                    PaintResourceObjectToTile(newPos.x, newPos.y, result[i], false);
                }
            }

            if (linkedCount >= 3) /*## 게임 옵션에 따라(5개씩 합쳐지등등) 합쳐지는 모습이 보여지 않아야 하는 경우가 있다. 이부분은 고도화에서 처리 하도록 한다.  ##*/
            {
                foreach(int iaddr in _checkedSameTiles)
                {
                    _Board[iaddr].Object = null;
                    if(_Board[iaddr].State != TileState.NORMAL)
                    {
                        _Board[iaddr].State = TileState.NORMAL;
                        OnUpdateTileState(iaddr % boardSize.x, iaddr / boardSize.x);
                    }
                }

                // 기존 객체는 삭제 하도록 한다.
                for (int i = 0; i < linkedObject.Count; i++)
                {
                    OnDestroyResourceObject(linkedObject[i]);
                }

                // 다음단계를 요청한다.
                OnRequestMergeObject(x, y, linkedCount, objectID);

 
            }
        } else
        {
            // 기존에 자리를 차지하고 있는 오브젝트는 다른 곳으로 보낸다.
            for (int i = 0; i < AlreadyExistTileObject.Count; i++)
            {
                bool canMove = GetEmptyTile(AlreadyExistTileObject[i].tilePosition.x, AlreadyExistTileObject[i].tilePosition.y, AlreadyExistTileObject[i], out Vector2Int newPos);
                if (canMove == true)
                {
                    PaintResourceObjectToTile(AlreadyExistTileObject[i].tilePosition.x, AlreadyExistTileObject[i].tilePosition.y, AlreadyExistTileObject[i], true);
                    OnMoveResourceObject(AlreadyExistTileObject[i].tilePosition.x, AlreadyExistTileObject[i].tilePosition.y, newPos.x, newPos.y, AlreadyExistTileObject[i]);
                    PaintResourceObjectToTile(newPos.x, newPos.y, AlreadyExistTileObject[i], false);
                }
            }
        }
    }

    /// <summary>
    /// ##### [클라이언트용] ##### 
    /// 객체를 보드에 넣어보자
    /// false면 공중에 뜨게 해야 한다.
    /// tryMerge로 합치기를 시도해보자
    /// </summary>
    public bool ResourceObjectDragToTile(int x,int y, ResourceObjectInfo resourceObject)
    {
        List<ResourceObjectInfo> linkedObject = new List<ResourceObjectInfo>();
        List<ResourceObjectInfo> AlreadyExistTileObject = new List<ResourceObjectInfo>();   // 이미 있는 타일들

        List<int> _checkedSameTiles = new List<int>();

        bool ismove = IsEnableMoveTo(x, y, resourceObject, ref AlreadyExistTileObject);

        OnDraggingObjectTo(x, y, resourceObject,ref AlreadyExistTileObject);

        AlreadyExistTileObject.Clear();
        
        if (ismove == true)
        {
            if (resourceObject.isMerge == true)
            {
                GetSameResourceObject(x, y, resourceObject, ref linkedObject, ref _checkedSameTiles);

                if (linkedObject.Count >= 3) /*## 게임 옵션에 따라(5개씩 합쳐지등등) 합쳐지는 모습이 보여지 않아야 하는 경우가 있다. 이부분은 고도화에서 처리 하도록 한다.  ##*/
                {
                    OnDragObject(_checkedSameTiles, linkedObject);
                    return true;
                }
            }
            return true;
        } 
        return false;
    }


    /// <summary>
    /// 비어있는 위치 찾기
    /// GetEmptyTile(int startX,int startY, ResourceObjectInfo resourceObject, out Vector2Int position)
    /// </summary>
    #region
    public bool GetEmptyTile(int startX,int startY, ResourceObjectInfo resourceObject, out Vector2Int position)
    {
        position = new Vector2Int();
        int rangecount = 0;
        while(_FindEmptyTile(startX, startY, rangecount, resourceObject, ref position) == false)
        {
            rangecount++;
            if(startX- rangecount < 0 && startX + rangecount > boardSize.x)
            {
                if (startY - rangecount < 0 && startY + rangecount > boardSize.y)
                {
                    return false;
                }
            }
        }
        return true;
    }

    protected bool _FindEmptyTile(int _x,int _y,int count, ResourceObjectInfo resourceObject, ref Vector2Int findpos)
    {
        int maxy = _y + (count + 1);
        int maxx = _x + (count + 1);

        for (int y = _y-count; y< maxy; y++)
        {
            int xplus =1;
            if (!(y == _y - count || y == maxy - 1))
            {
                xplus = count + count;
            }

            for (int x = _x - count; x < maxx; x+= xplus)
            {
                if(_CheckTileEmptyShape(_x,_y, resourceObject) == true)
                {
                    findpos.x = _x;
                    findpos.y = _y;
                    return true;
                }
            }
        }
        return false;
    }

    protected bool _IsEmptyTile(int address)
    {
        if (address >= boardSize.x * boardSize.y || address < 0)
        {
            return false;
        }

        var board = _Board[address];
        if (board != null)
        {
            if (board.Object.Equals(null) && board.State == TileState.NORMAL)
            {
                return true;
            }
        }

        return false;
    }


    protected bool _CheckTileEmptyShape(int x,int y, ResourceObjectInfo resourceObject)
    {
        int ROA = 0;
        int address = y * boardSize.x + x;
        for (int yy = 0; yy < resourceObject.shape.size.y; yy++)
        {
            for (int xx = 0; xx < resourceObject.shape.size.x; xx++)
            {
                if (resourceObject.shape.ResourceObjectArray[ROA] > 0 && _IsEmptyTile(address+xx) == false)
                {
                    return false;
                }
                ROA++;
            }
            address += boardSize.x;
        }
        return true;
    }
    #endregion


    /// <summary>
    /// 같은 자원이 얼마나 있는지 확인한다. 
    /// </summary>
    protected void GetSameResourceObject(int x, int y, ResourceObjectInfo tryMerge, ref List<ResourceObjectInfo> linkedObject,ref List<int> _sameTiles)
    {
        int ROA = 0;
        List<int>  _checkedBoard = new List<int>();  // 확인한 배열 패스용
        linkedObject.Add(tryMerge);
        
        for (int yy = y; yy < y+ tryMerge.shape.size.y; yy++)
        {
            for (int xx = x; xx < x + tryMerge.shape.size.x; xx++)
            {
                if(tryMerge.shape.ResourceObjectArray[ROA] != 0)
                {
                    _LoopCheckSameObjectID(xx, yy, tryMerge.ObjectID, ref linkedObject, ref _checkedBoard,ref _sameTiles);
                }
                ROA++;
            }
        }
        _checkedBoard.Clear();
    }


    /// <summary>
    /// 같은 object id가 있는지 확인한다.
    /// </summary>
    protected ResourceObjectInfo _isSameObject(int address, int objID)
    {
        if (address >= boardSize.x * boardSize.y || address < 0)
        {
            return null;
        }

        var board = _Board[address];
        if (board != null)
        {
            if (board.Object != null)
            {
                if (board.Object.ObjectID == objID)
                {
                    return board.Object;
                }
            }
        }
        return null;
    }


    /// <summary>
    /// 같은 자원이 얼마나 있는지 재귀호출한다.
    /// </summary>
    private void _LoopCheckSameObjectID(int x,int y,int ObjID, ref List<ResourceObjectInfo> linkedObject, ref List<int> checkedArray, ref List<int> _sameTiles)
    {
        int address = boardSize.x * y + x;
        if(checkedArray.Contains(address) == true)  // 이미 조사했으면 더이상 진행하지 않는다.
        {
            return;
        }
        checkedArray.Add(address);

        ResourceObjectInfo oid = _isSameObject(x, y);
        if (oid != null)
        {
            _sameTiles.Add(address);
            if(linkedObject.Contains(oid) == false)
            {
                linkedObject.Add(oid);
            }
            _LoopCheckSameObjectID(x - 1, y, ObjID, ref linkedObject, ref checkedArray,ref _sameTiles);
            _LoopCheckSameObjectID(x + 1, y, ObjID, ref linkedObject, ref checkedArray, ref _sameTiles);
            _LoopCheckSameObjectID(x , y - 1, ObjID, ref linkedObject, ref checkedArray, ref _sameTiles);
            _LoopCheckSameObjectID(x , y + 1, ObjID, ref linkedObject, ref checkedArray, ref _sameTiles);
        }
    }

}
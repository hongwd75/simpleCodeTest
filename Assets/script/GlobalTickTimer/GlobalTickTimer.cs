using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class _MYOBJECT_
{
    public float abc;
}


public class TickObject
{
    public float _tick = 0.0f;
    public _MYOBJECT_ _object = null;
    public bool TryMerge;   // 합치기를 시도한다.
    public bool tileLogic; // 타일 로직을 직접제어하는건 어떨까
    public TickObject(float tick, _MYOBJECT_ obj,bool merge,bool tilelogic) { _tick = tick; _object = obj; TryMerge = merge; tileLogic = tilelogic; }
 
}


public class GlobalTickTimer
{
    protected float _TimeCounter = 0.0f;
    protected List<TickObject> _TickList = new List<TickObject>();
    protected List<TickObject> _RunList = null;

    public float GetTick() { return _TimeCounter; }

    public void Add(float delayTime, _MYOBJECT_ obj,bool merge, bool tileLogic)
    {
        var findObject = _TickList.Find(item => item._object == obj);
        if(findObject != null)
        {
            if(findObject._tick > delayTime+ _TimeCounter)
            {
                findObject._tick = delayTime + _TimeCounter;
            }
        }
        else
        {
            _TickList.Add(new TickObject(delayTime + _TimeCounter, obj, merge, tileLogic));
        }
    }

    // 시간이 지난 리소스에 대한 처리
    public void UpdateList()
    {
        _TimeCounter += Time.deltaTime;

        if (_TickList.Count > 0)
        {
            _RunList = _TickList.Where(item => item._tick <= _TimeCounter).ToList();
            _TickList = _TickList.Except(_RunList).ToList();

            if (_RunList != null && _RunList.Count > 0)
            {
                foreach(var changeTile in _RunList)
                {
                    // 타일 속성을 예약_이동->예약_검사로 바꾼다.
                }
                while (_RunList.Count > 0)
                {

                    RemoveAllList(_RunList[0]._object);
                }

                _RunList.Clear();
                _RunList = null;
            }
        }
    }

    public void RemoveExecList(_MYOBJECT_ obj)
    {
        RemoveList(_RunList,obj);
    }
    public void RemoveWaitList(_MYOBJECT_ obj)
    {
        RemoveList(_TickList,obj);
    }
    public void RemoveAllList(_MYOBJECT_ obj)
    {
        RemoveList(_TickList,obj);
        RemoveList(_RunList,obj);
    }

    protected bool RemoveList(List<TickObject> list, _MYOBJECT_ obj)
    {
        if(list != null)
        {
            var itemObj = list.Find(item => item._object.Equals(obj));
            if (itemObj != null)
            {
                list.Remove(itemObj);
                return true;
            }
        }
        return false;
    }


    public void OnDestroy()
    {
        _TickList.Clear();
        if(_RunList != null)
        {
            _RunList.Clear();
        }
    }

}

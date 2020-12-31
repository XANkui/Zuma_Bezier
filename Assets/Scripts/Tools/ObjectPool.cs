using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T>
{
    /// <summary>
    /// 计数
    /// </summary>
    public int Counter;

    private List<T> mPool = new List<T>();
    private Func<T> mFunc;

    public ObjectPool(Func<T> func, int count) {
        mFunc = func;
        InstanceObject(count);
    }

    /// <summary>
    /// 从对象池中取东西
    /// </summary>
    /// <returns></returns>
    public T GetObject() {
        int i = mPool.Count;

        while (i-- > 0)
        {
            T t = mPool[i];
            mPool.RemoveAt(i);
            return t;
        }

        InstanceObject(3);
        return GetObject();
    }

    public void AddObject(T t)
    {
        mPool.Add(t);
    }

    /// <summary>
    /// 实例化对象
    /// </summary>
    /// <param name="count"></param>
    private void InstanceObject(int count) {
        for (int i = 0; i < count; i++)
        {
            mPool.Add(mFunc());
        }
    }
}

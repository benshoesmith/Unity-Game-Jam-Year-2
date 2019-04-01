using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcurrentQueue<T>{
    private readonly object syncLock = new object();
    private Queue<T> queue;

    public int Count
    {
        get
        {
            lock (syncLock)
            {
                return queue.Count;
            }
        }
    }

    public ConcurrentQueue()
    {
        this.queue = new Queue<T>();
    }

    public T Peek()
    {
        lock (syncLock)
        {
            return queue.Peek();
        }
    }

    public void Enqueue(T obj)
    {
        lock (syncLock)
        {
            queue.Enqueue(obj);
        }
    }

    public T Dequeue()
    {
        lock (syncLock)
        {
            return queue.Dequeue();
        }
    }

    public void Clear()
    {
        lock (syncLock)
        {
            queue.Clear();
        }
    }
}

﻿using System;
using System.Collections.Generic;

namespace Logging.SmartStandards.Transport {

  public partial class TraceBusFeed {

    /// <remarks>
    ///   Taken from https://stackoverflow.com/a/5924776
    /// </remarks>
    private class MyCircularBuffer<T> : IEnumerable<T> { // v 1.0.0

      readonly int _Size;

      readonly object _Locker;

      int _Count;
      int _Head;
      int _Rear;
      T[] _Values;

      public MyCircularBuffer(int max) {
        _Size = max;
        _Locker = new object();
        _Count = 0;
        _Head = 0;
        _Rear = 0;
        _Values = new T[_Size];
      }

      static int Incr(int index, int size) {
        return (index + 1) % size;
      }

      public object SyncRoot { get { return _Locker; } }

      public void SafeEnqueue(T obj) {
        lock (_Locker) { this.UnsafeEnqueue(obj); }
      }

      public void UnsafeEnqueue(T obj) {

        _Values[_Rear] = obj;

        if (_Count == _Size) _Head = Incr(_Head, _Size);

        _Rear = Incr(_Rear, _Size);
        _Count = Math.Min(_Count + 1, _Size);
      }

      public IEnumerator<T> GetEnumerator() {
        int index = _Head;

        for (int i = 0; i < _Count; i++) {
          yield return _Values[index];
          index = Incr(index, _Size);
        }

      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
      }

    }

  }

}

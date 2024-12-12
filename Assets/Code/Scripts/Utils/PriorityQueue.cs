using System;
using System.Collections.Generic;
using System.Linq;

namespace FrostfallSaga.Utils
{
    public class PriorityQueue<T>
    {
        private SortedList<float, Queue<T>> _elements = new SortedList<float, Queue<T>>();
        private int _count = 0;

        public int Count => _count;

        public void Enqueue(T item, float priority)
        {
            if (!_elements.ContainsKey(priority))
            {
                _elements[priority] = new Queue<T>();
            }
            _elements[priority].Enqueue(item);
            _count++;
        }

        public T Dequeue()
        {
            if (_elements.Count == 0)
                throw new InvalidOperationException("The priority queue is empty.");

            var firstPair = _elements.First();
            var item = firstPair.Value.Dequeue();
            if (firstPair.Value.Count == 0)
            {
                _elements.Remove(firstPair.Key);
            }
            _count--;
            return item;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace FrostfallSaga.Utils
{
    public class PriorityQueue<T>
    {
        private readonly SortedList<float, Queue<T>> _elements = new();

        public int Count { get; private set; }

        public void Enqueue(T item, float priority)
        {
            if (!_elements.ContainsKey(priority)) _elements[priority] = new Queue<T>();
            _elements[priority].Enqueue(item);
            Count++;
        }

        public T Dequeue()
        {
            if (_elements.Count == 0)
                throw new InvalidOperationException("The priority queue is empty.");

            KeyValuePair<float, Queue<T>> firstPair = _elements.First();
            T item = firstPair.Value.Dequeue();
            if (firstPair.Value.Count == 0) _elements.Remove(firstPair.Key);
            Count--;
            return item;
        }
    }
}
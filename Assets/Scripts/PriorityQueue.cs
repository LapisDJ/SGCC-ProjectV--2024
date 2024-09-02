using System;
using System.Collections.Generic;

namespace Utilities
{
    public class PriorityQueue<T>
    {
        private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

        public int Count => elements.Count;

        public void Enqueue(T item, float priority)
        {
            elements.Add(new KeyValuePair<T, float>(item, priority));
            HeapifyUp(elements.Count - 1);
        }

        public T Dequeue()
        {
            if (elements.Count == 0)
                throw new InvalidOperationException("The queue is empty.");

            T bestItem = elements[0].Key;
            elements[0] = elements[elements.Count - 1];
            elements.RemoveAt(elements.Count - 1);
            HeapifyDown(0);

            return bestItem;
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (elements[index].Value >= elements[parentIndex].Value) break;

                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        private void HeapifyDown(int index)
        {
            while (index * 2 + 1 < elements.Count)
            {
                int leftChildIndex = index * 2 + 1;
                int rightChildIndex = index * 2 + 2;
                int smallestChildIndex = leftChildIndex;

                if (rightChildIndex < elements.Count && elements[rightChildIndex].Value < elements[leftChildIndex].Value)
                {
                    smallestChildIndex = rightChildIndex;
                }

                if (elements[index].Value <= elements[smallestChildIndex].Value) break;

                Swap(index, smallestChildIndex);
                index = smallestChildIndex;
            }
        }

        private void Swap(int indexA, int indexB)
        {
            var temp = elements[indexA];
            elements[indexA] = elements[indexB];
            elements[indexB] = temp;
        }
    }
}

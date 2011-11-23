using System;
using System.Collections.Generic;

namespace SCICT.Utility
{
    /// <summary>
    /// RoundQueue used in N-Gram text reading
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RoundQueue<T>
    {
        private readonly int m_preItemsCount;
        private readonly int m_postItemsCount;
        private readonly List<T> m_list = new List<T>();
        private bool m_isEntryBlocked = false;
        private int m_curItemIndicator = -1;

        public RoundQueue(int preItemsCount, int postItemsCount)
        {
            m_preItemsCount = preItemsCount;
            m_postItemsCount = postItemsCount;
            m_curItemIndicator = -1;
        }

        public void Clear()
        {
            m_list.Clear();
            m_curItemIndicator = -1;
            m_isEntryBlocked = false;
        }

        public void AddItem(T item)
        {
            if (IsFull())
            {
                if (m_curItemIndicator == 0)
                    throw new Exception("Cannot delete an unread item!");

                m_list.RemoveAt(0);
                m_curItemIndicator--;
            }
            m_list.Add(item);
            if (m_curItemIndicator < 0)
                m_curItemIndicator = 0;
        }

        public T this[int i]
        {
            get { return m_list[i]; }
        }

        public int Count
        {
            get { return m_list.Count; }
        }

        public int CurMainItemIndex
        {
            get { return m_curItemIndicator; }
        }

        public T LastItem
        {
            get
            {
                if (m_list.Count <= 0)
                    throw new Exception("No items to read!");
                //return default(T);

                return m_list[m_list.Count - 1];
            }
        }

        public void BlockEntry()
        {
            m_isEntryBlocked = true;
        }

        public bool IsFull()
        {
            return m_list.Count >= m_preItemsCount + m_postItemsCount + 1;
        }

        private bool CanRead()
        {
            if (m_curItemIndicator >= m_list.Count || m_curItemIndicator < 0)
                return false;

            if (m_isEntryBlocked)
                return m_list.Count > 0;

            return m_list.Count > 0 && m_list.Count >= m_postItemsCount + 1;
        }

        public bool ReadNextWordLists(out T[] items, out int mainItemIndex)
        {
            if (!CanRead())
            {
                if (m_isEntryBlocked)
                    Clear();

                items = null;
                mainItemIndex = -1;

                return false;
            }

            //if (m_isEntryBlocked && m_curItemIndicator >= m_preItemsCount)
            if (m_isEntryBlocked && m_curItemIndicator > m_preItemsCount)
            {
                m_list.RemoveAt(0);
                m_curItemIndicator--;
            }

            items = m_list.ToArray();
            mainItemIndex = m_curItemIndicator;

            // Get ready for the next read
            m_curItemIndicator++;

            return true;
        }
    }
}

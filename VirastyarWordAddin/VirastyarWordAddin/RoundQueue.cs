// Virastyar
// http://www.virastyar.ir
// Copyright (C) 2011 Supreme Council for Information and Communication Technology (SCICT) of Iran
// 
// This file is part of Virastyar.
// 
// Virastyar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Virastyar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Virastyar.  If not, see <http://www.gnu.org/licenses/>.
// 
// Additional permission under GNU GPL version 3 section 7
// The sole exception to the license's terms and requierments might be the
// integration of Virastyar with Microsoft Word (any version) as an add-in.

using System;
using System.Collections.Generic;

namespace VirastyarWordAddin
{
    internal class RoundQueue<T>
    {
        private int preItemsCount;
        private bool isEntryBlocked = false;
        private List<T> list = new List<T>();
        private int curItemIndicator = -1;

        public RoundQueue(int preItemsCount)
        {
            this.preItemsCount = preItemsCount;
            curItemIndicator = -1;
        }

        public void Clear()
        {
            list.Clear();
            curItemIndicator = -1;
            isEntryBlocked = false;
        }

        public void AddItem(T item)
        {
            if (IsFull())
            {
                if (curItemIndicator == 0)
                    throw new Exception("Cannot delete an unread item!");

                list.RemoveAt(0);
                curItemIndicator--;
            }
            list.Add(item);
            if (curItemIndicator < 0)
                curItemIndicator = 0;
        }

        public T GetLastItem()
        {
            if (list.Count <= 0)
                return default(T);

            return list[list.Count - 1];
        }

        public void BlockEntry()
        {
            isEntryBlocked = true;
        }

        public bool IsFull()
        {
            return list.Count >= 2 * preItemsCount + 1;
        }

        private bool CanRead()
        {
            if (curItemIndicator >= list.Count || curItemIndicator < 0)
                return false;

            if (isEntryBlocked)
                return list.Count > 0;
            else
                return list.Count > 0 && list.Count >= preItemsCount + 1;
        }

        public bool ReadNextWordLists(out T[] items, out int mainItemIndex)
        {
            items = null;
            mainItemIndex = -1;

            if (!CanRead())
            {
                if (isEntryBlocked)
                    Clear();

                return false;
            }

            if (isEntryBlocked && curItemIndicator > preItemsCount)
            {
                list.RemoveAt(0);
                curItemIndicator--;
            }

            items = list.ToArray();
            mainItemIndex = curItemIndicator;

            // Get ready for the next read
            curItemIndicator++;

            return true;
        }
    }
}

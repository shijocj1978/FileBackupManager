using System;
using System.Collections.Generic;
using System.Text;

namespace Logging.Base
{
    public class ExtendedList<T>: List<T>
    {
        public ExtendedList(int expectedIndex)
        {
            ExpectedIndex = expectedIndex;
        }
        private int _ExpectedIndex;

        private int ExpectedIndex
        {
            get { return _ExpectedIndex; }
            set { _ExpectedIndex = value; }
        }

        /// <summary>
        /// Fires when an item is added.
        /// </summary>
        public event EventHandler<EventArgs> ItemAdded;

        /// <summary>
        /// Fires when an item is removed.
        /// </summary>
        public event EventHandler<EventArgs> ItemRemoved;

        /// <summary>
        /// Fires when collection reached to maximum limit.
        /// </summary>
        public event EventHandler<EventArgs> ExpectedIndexReached;

        

        public new void  Add(T item)
        {
            base.Add(item);
            if (ItemAdded != null)
                ItemAdded(this, new EventArgs());
            if(base.Count >= ExpectedIndex)
            {
                if (ExpectedIndexReached != null)
                    ExpectedIndexReached(this, new EventArgs());
            }
        }

        public new void Remove(T item)
        {
            base.Remove(item);
            if (ItemRemoved != null)
                ItemRemoved(this, new EventArgs());
        }

        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            if (ItemRemoved != null)
                ItemRemoved(this, new EventArgs());
        }
    }
}

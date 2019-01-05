using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Linq
{
    public static class IEnumerableExt
    {

        /// <summary>
        /// Custom Compare for the linq.
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <example>use like list.UniqueBy(row => row[0])</example>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TElement> Unique<TElement, TKey>(this IEnumerable<TElement> source, Func<TElement, TKey> criteria)
        {
            var results = new LinkedList<TElement>();
            // Seen a key 0 times, it won't be in here.
            // Seen it once, it will be in as a node.
            // Seen it more than once, it will be in as null.
            var nodeMap = new Dictionary<TKey, LinkedListNode<TElement>>();

            foreach (TElement element in source)
            {
                TKey key = criteria(element);
                LinkedListNode<TElement> currentNode;

                if (nodeMap.TryGetValue(key, out currentNode))
                {
                    // Seen it before. Remove if non-null
                    if (currentNode != null)
                    {
                        results.Remove(currentNode);
                        nodeMap[key] = null;
                    }
                    // Otherwise no action needed
                }
                else
                {
                    LinkedListNode<TElement> node = results.AddLast(element);
                    nodeMap[key] = node;
                }
            }
            foreach (TElement element in results)
            {
                yield return element;
            }
        }

        /// <summary>
        /// Custom Compare for the linq.
        /// </summary>
        public static IEnumerable<TElement> Unique<TElement>(this IEnumerable<TElement> source)
        {
            HashSet<TElement> toReturn = new HashSet<TElement>();
            HashSet<TElement> seen = new HashSet<TElement>();

            foreach (TElement element in source)
            {
                if (seen.Add(element))
                {
                    toReturn.Add(element);
                }
                else
                {
                    toReturn.Remove(element);
                }
            }

            // adding yield to get deferred execution
            foreach (TElement element in toReturn)
            {
                yield return element;
            }
        }

        /// <summary>
        /// Returns a list of items after removing the items specified in the toExclude. This method applies only to basic data types (string, int etc).
        /// </summary>
        /// <typeparam name="TElement">Type of object</typeparam>
        /// <param name="source">Hidden incase called from an IEnumerable inherited object</param>
        /// <param name="destination">List of items to be excluded</param>
        /// <returns>List of items after removing the items specified in the toExclude</returns>
        public static IEnumerable<TElement> Exclude<TElement>(this IEnumerable<TElement> source, IEnumerable<TElement> toExclude)
        {
            /*
             * the idea is to look in all the items of the source for item 0 of the toExcludelist. 
             * before entering the loop the value is added to return.
             * If the item is found in the exclude list then the item is removed from list.
             */
            HashSet<TElement> toReturn = new HashSet<TElement>();
            HashSet<TElement> seen = new HashSet<TElement>();
            foreach (TElement element in source)
            {
                //Add the item first
                toReturn.Add(element);
                foreach (var item in toExclude)
                {
                    if (item.Equals(element) == true)
                    {
                        toReturn.Remove(element);
                        break;
                    }
                }
            }
            // yield to get deferred execution
            foreach (TElement element in toReturn)
            {
                yield return element;
            }
        }

        /// <summary>
        /// Returns a list of items after removing the items specified in the toExclude. This method applies only to basic data types (string, int etc).
        /// </summary>
        /// <typeparam name="TElement">Type of object</typeparam>
        /// <param name="source">Hidden incase called from an IEnumerable inherited object</param>
        /// <param name="destination">List of items to be excluded</param>
        /// <param name="condition">A delegate that passes the two objects that need to be compared. The delegate shoul return true if object matches.</param>
        /// <returns>List of items after removing the items specified in the toExclude</returns>
        public static IEnumerable<TElement> Exclude<TElement, TKey>(this IEnumerable<TElement> source, IEnumerable<TElement> toExclude, Func<TElement,TElement,bool> condition)
        {
            /*
             * the idea is to look in all the items of the source for item 0 of the toExcludelist. 
             * before entering the loop the value is added to return.
             * If the item is found in the exclude list then the item is removed from list.
             */
            HashSet<TElement> toReturn = new HashSet<TElement>();
            HashSet<TElement> seen = new HashSet<TElement>();
            foreach (TElement element in source)
            {
                //Add the item first
                toReturn.Add(element);
                foreach (var item in toExclude)
                {
                    if (condition(element,item) == true)
                    {
                        toReturn.Remove(element);
                        break;
                    }
                }
            }
            // yield to get deferred execution
            foreach (TElement element in toReturn)
            {
                yield return element;
            }
        }
    }
}

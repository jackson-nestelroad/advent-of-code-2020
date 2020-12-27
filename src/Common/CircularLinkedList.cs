using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Common
{
    /// <summary>
    /// Implementation of a circular linked list, for fun.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class CircularLinkedList<T> : IEnumerable<T>
    {
        /// <summary>
        /// A single node in a circular linked list.
        /// Nodes double as "iterators" to move through the linked list.
        /// </summary>
        public sealed class Node
        {
            public T Value { get; set; }
            public Node Next { get; internal set; }
            public Node Previous { get; internal set; }

            internal Node(T value)
            {
                Value = value;
            }

            /// <summary>
            /// Move to the next node.
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            public static Node operator ++(Node node)
            {
                return node.Next;
            }

            /// <summary>
            /// Move to the previous node.
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            public static Node operator --(Node node)
            {
                return node.Previous;
            }

            /// <summary>
            /// Move forward a given number of steps from the current node.
            /// </summary>
            /// <param name="node"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public static Node operator +(Node node, int offset)
            {
                Node result = node;
                for (int i = 0; result != null && i < offset; ++i)
                {
                    result = result.Next;
                }
                return result;
            }

            /// <summary>
            /// Move backwards a given number of steps from the current node.
            /// </summary>
            /// <param name="node"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public static Node operator -(Node node, int offset)
            {
                Node result = node;
                for (int i = 0; result != null && i < offset; ++i)
                {
                    result = result.Previous;
                }
                return result;
            }
        }

        public Node Head { get; private set; }
        public Node Tail { get; private set; }
        public int Count { get; private set; } = 0;

        public CircularLinkedList()
        {
            Head = null;
            Tail = null;
        }

        /// <summary>
        /// Create a circular linked list from a pre-existing collection.
        /// </summary>
        /// <param name="collection"></param>
        public CircularLinkedList(IEnumerable<T> collection)
        {
            Node prev = null;
            Node curr;
            foreach (T value in collection)
            {
                curr = new Node(value);

                // First value
                if (prev == null)
                {
                    Head = curr;
                }
                else
                {
                    curr.Previous = prev;
                    prev.Next = curr;
                }

                prev = curr;
                ++Count;
            }

            // Add circular property
            if (prev != null)
            {
                Tail = prev;
                Tail.Next = Head;
                Head.Previous = Tail;
            }
        }

        /// <summary>
        /// Create a circular linked list from nodes that are already properly connected.
        /// For internal use only when splices are created.
        /// </summary>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <param name="count"></param>
        private CircularLinkedList(Node head, Node tail, int count)
        {
            Head = head;
            Tail = tail;
            Count = count;
        }

        public bool IsEmpty() => Head == null;

        private void AssertListNonEmpty()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("List cannot be empty");
            }
        }

        private static void AssertNodeNonNull(Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("Node cannot be null");
            }
        }

        private void AssertDifferentList(CircularLinkedList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("List cannot be null");
            }
            if (list == this)
            {
                throw new InvalidOperationException("Cannot insert list into itself");
            }
        }

        private void SetFirstNode(Node node)
        {
            AssertNodeNonNull(node);
            node.Previous = node;
            node.Next = node;
            Head = node;
            Tail = node;
        }

        public void Prepend(T value)
        {
            Node node = new Node(value);
            if (Head == null)
            {
                SetFirstNode(node);
            }
            else
            {
                node.Previous = Tail;
                Tail.Next = node;
                node.Next = Head;
                Head.Previous = node;
                Head = node;
            }

            ++Count;
        }

        public void Append(T value)
        {
            Node node = new Node(value);
            if (Head == null)
            {
                SetFirstNode(node);
            }
            else
            {
                node.Next = Head;
                Head.Previous = node;
                node.Previous = Tail;
                Tail.Next = node;
                Tail = node;
            }

            ++Count;
        }

        public Node GetNodeAt(int pos)
        {
            if (IsEmpty())
            {
                return null;
            }
            Node it = Head;
            for (int i = 0; i < pos; ++i, ++it) ;
            return it;
        }

        public T GetIndex(int pos)
        {
            AssertListNonEmpty();
            Node node = GetNodeAt(pos);
            return node.Value;
        }

        public void Insert(int pos, T value)
        {
            if (pos == 0 || Head == null)
            {
                Prepend(value);
            }
            else
            {
                InsertBefore(GetNodeAt(pos), value);
            }
        }

        public void InsertBefore(Node node, Node newNode)
        {
            AssertNodeNonNull(node);
            AssertNodeNonNull(newNode);

            Node previous = node.Previous;
            newNode.Next = node;
            node.Previous = newNode;
            newNode.Previous = previous;
            previous.Next = newNode;

            if (node == Head)
            {
                Head = newNode;
            }

            ++Count;
        }

        public void InsertBefore(Node node, CircularLinkedList<T> list)
        {
            AssertNodeNonNull(node);
            AssertDifferentList(list);

            Node previous = node.Previous;
            list.Tail.Next = node;
            node.Previous = list.Tail;
            list.Head.Previous = previous;
            previous.Next = list.Head;

            if (node == Head)
            {
                Head = list.Head;
            }

            Count += list.Count;
        }

        public void InsertBefore(Node node, T value)
        {
            InsertBefore(node, new Node(value));
        }

        public void InsertAfter(Node node, Node newNode)
        {
            AssertNodeNonNull(node);
            AssertNodeNonNull(newNode);

            Node next = node.Next;
            newNode.Next = next;
            next.Previous = newNode;
            newNode.Previous = node;
            node.Next = newNode;

            if (node == Tail)
            {
                Tail = newNode;
            }

            ++Count;
        }

        public void InsertAfter(Node node, CircularLinkedList<T> list)
        {
            AssertNodeNonNull(node);
            AssertDifferentList(list);

            Node next = node.Next;
            list.Tail.Next = next;
            next.Previous = list.Tail;
            list.Head.Previous = node;
            node.Next = list.Head;

            if (node == Tail)
            {
                Tail = list.Tail;
            }

            Count += list.Count;
        }

        public void InsertAfter(Node node, T value)
        {
            InsertAfter(node, new Node(value));
        }

        public void Clear()
        {
            Head = null;
            Tail = null;
            Count = 0;
        }

        public Node Find(T value)
        {
            Node it = Head;
            if (it != null)
            {
                do
                {
                    if (it.Value.Equals(value))
                    {
                        return it;
                    }
                    ++it;
                } while (it != Head);
            }
            return null;
        }

        public Node Find(Predicate<T> predicate)
        {
            Node it = Head;
            if (it != null)
            {
                do
                {
                    if (predicate.Invoke(it.Value))
                    {
                        return it;
                    }
                    ++it;
                } while (it != Head);
            }
            return null;
        }

        public Node FindLast(T value)
        {
            Node it = Tail;
            if (it != null)
            {
                do
                {
                    if (it.Value.Equals(value))
                    {
                        return it;
                    }
                } while (it != Tail);
            }
            return null;
        }

        public Node FindLast(Predicate<T> predicate)
        {
            Node it = Tail;
            if (it != null)
            {
                do
                {
                    if (predicate.Invoke(it.Value))
                    {
                        return it;
                    }
                } while (it != Tail);
            }
            return null;
        }

        public void Remove(Node node)
        {
            AssertNodeNonNull(node);

            node.Previous.Next = node.Next;
            node.Next.Previous = node.Previous;

            node.Next = null;
            node.Previous = null;

            --Count;
        }

        /// <summary>
        /// Permanently removes a set of nodes from the linked list.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public CircularLinkedList<T> Splice(Node from, int count)
        {
            if (count == 0)
            {
                return new CircularLinkedList<T>();
            }

            AssertNodeNonNull(from);

            Node splicedHead = from;
            Node splicedTail = from + (count - 1);

            splicedHead.Previous.Next = splicedTail.Next;
            splicedTail.Next.Previous = splicedHead.Previous;

            if (splicedHead == Head)
            {
                Head = splicedTail.Next;
            }
            if (splicedHead == Tail)
            {
                Tail = splicedHead.Previous;
                
                // Head must also be replaced
                if (count > 1)
                {
                    Head = splicedTail.Next;
                }
            }
            if (splicedTail == Head)
            {
                Head = splicedTail.Next;

                // Tail must also be replaced
                if (count > 1)
                {
                    Tail = splicedHead.Previous;
                }
            }
            if (splicedTail == Tail)
            {
                Tail = splicedHead.Previous;
            }

            splicedHead.Previous = splicedTail;
            splicedTail.Next = splicedHead;

            Count -= count;

            return new CircularLinkedList<T>(splicedHead, splicedTail, count);
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node it = Head;
            if (it != null)
            {
                do
                {
                    yield return it.Value;
                    ++it;
                } while (it != Head);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

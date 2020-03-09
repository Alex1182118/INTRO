using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using L1.Helpers;
using L1.Controllers;
using L1.Models;

namespace L1.Models
{
    public class BTree<T> : IEnumerable<T> where T : IComparable
    {
        private readonly int maxNode;
        private readonly int minNode;

        private BTreeNode<T> Root;

        

        public BTree(int maxNode)
        {
            this.maxNode = maxNode;
            minNode = maxNode / 2;
        }

     
        public void Insert(T newValue)
        {
            if (Root == null)
            {
                Root = new BTreeNode<T>(maxNode, null) { Keys = { [0] = newValue } };
                Root.KeyCount++;
               
                return;
            }

            var leafToInsert = Leaf(Root, newValue);
            Split(ref leafToInsert, newValue, null, null);
            
        }

        private BTreeNode<T> Leaf(BTreeNode<T> node, T newValue)
        {
           
            if (node.IsLeaf)
            {
                return node;
            }

           
            for (var i = 0; i < node.KeyCount; i++)
            {
                
                if (newValue.CompareTo(node.Keys[i]) < 0)
                {
                    return Leaf(node.Children[i], newValue);
                }
               

                if (node.KeyCount == i + 1)
                {
                    return Leaf(node.Children[i + 1], newValue);
                }

            }

            return node;
        }

        private void Split(ref BTreeNode<T> node, T newValue, BTreeNode<T> newValueLeft, BTreeNode<T> newValueRight)
        {
           
            if (node == null)
            {
                node = new BTreeNode<T>(maxNode, null);
                Root = node;
            }

            
            if (node.KeyCount != maxNode)
            {
                NotFull(ref node, newValue, newValueLeft, newValueRight);
                return;
            }

            
            var left = new BTreeNode<T>(maxNode, null);
            var right = new BTreeNode<T>(maxNode, null);

           
            var currentMedianIndex = node.GetMedianIndex();

            
            var currentNode = left;
            var currentNodeIndex = 0;

            
            var newMedian = default(T);
            var newMedianSet = false;
            var newValueInserted = false;

            
            var insertionCount = 0;

            
            for (var i = 0; i < node.KeyCount; i++)
            {

                
                if (!newMedianSet && insertionCount == currentMedianIndex)
                {
                    newMedianSet = true;

                    
                    if (!newValueInserted && newValue.CompareTo(node.Keys[i]) < 0)
                    {
                        
                        newMedian = newValue;
                        newValueInserted = true;

                        if (newValueLeft != null)
                        {
                            Child(currentNode, currentNode.KeyCount, newValueLeft);
                        }

                        
                        currentNode = right;
                        currentNodeIndex = 0;

                        if (newValueRight != null)
                        {
                            Child(currentNode, 0, newValueRight);
                        }

                        i--;
                        insertionCount++;
                        continue;
                    }

                    
                    newMedian = node.Keys[i];

                    
                    currentNode = right;
                    currentNodeIndex = 0;

                    continue;

                }

                
                if (newValueInserted || node.Keys[i].CompareTo(newValue) < 0)
                {
                    currentNode.Keys[currentNodeIndex] = node.Keys[i];
                    currentNode.KeyCount++;

                    
                    if (currentNode.Children[currentNodeIndex] == null)
                    {
                        Child(currentNode, currentNodeIndex, node.Children[i]);
                    }

                    Child(currentNode, currentNodeIndex + 1, node.Children[i + 1]);

                }
                else
                {
                    currentNode.Keys[currentNodeIndex] = newValue;
                    currentNode.KeyCount++;

                    Child(currentNode, currentNodeIndex, newValueLeft);
                    Child(currentNode, currentNodeIndex + 1, newValueRight);

                    i--;
                    newValueInserted = true;
                }

                currentNodeIndex++;
                insertionCount++;
            }

            
            if (!newValueInserted)
            {
                currentNode.Keys[currentNodeIndex] = newValue;
                currentNode.KeyCount++;

                Child(currentNode, currentNodeIndex, newValueLeft);
                Child(currentNode, currentNodeIndex + 1, newValueRight);

            }

            
            var parent = node.Parent;
            Split(ref parent, newMedian, left, right);

        }

      
       
        private void Child(BTreeNode<T> parent, int childIndex, BTreeNode<T> child)
        {
            parent.Children[childIndex] = child;

            if (child == null)
            {
                return;
            }

            child.Parent = parent;
            child.Index = childIndex;

        }
        private void insertAt<TS>(TS[] array, int index, TS newValue)
        {
            
            Array.Copy(array, index, array, index + 1, array.Length - index - 1);
            
            array[index] = newValue;
        }
        private void NotFull(ref BTreeNode<T> node, T newValue, BTreeNode<T> newValueLeft, BTreeNode<T> newValueRight)
        {
            var inserted = false;

            
            for (var i = 0; i < node.KeyCount; i++)
            {
                if (newValue.CompareTo(node.Keys[i]) >= 0)
                {
                    continue;
                }

                insertAt(node.Keys, i, newValue);
                node.KeyCount++;

               
                Child(node, i, newValueLeft);
                insertChild(node, i + 1, newValueRight);


                inserted = true;
                break;
            }

            
            if (inserted)
            {
                return;
            }

            node.Keys[node.KeyCount] = newValue;
            node.KeyCount++;

            Child(node, node.KeyCount - 1, newValueLeft);
            Child(node, node.KeyCount, newValueRight);
        }
        private void insertChild(BTreeNode<T> parent, int childIndex, BTreeNode<T> child)
        {
            insertAt(parent.Children, childIndex, child);

            if (child != null)
            {
                child.Parent = parent;
            }

            
            for (var i = childIndex; i <= parent.KeyCount; i++)
            {
                if (parent.Children[i] != null)
                {
                    parent.Children[i].Index = i;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new BTreeEnumerator<T>(Root);
        }
    }


    public abstract class BNode<T> where T : IComparable
    {
        internal int Index;
        internal T[] Keys { get; set; }
        internal int KeyCount;
        internal abstract BNode<T> GetParent();
        internal abstract BNode<T>[] GetChildren();

        internal BNode(int maxKeysPerNode)
        {
            Keys = new T[maxKeysPerNode];
        }

        internal int GetMedianIndex()
        {
            return (KeyCount / 2) + 1;
        }
    }

    public class BTreeNode<T> : BNode<T> where T : IComparable
    {

        internal BTreeNode<T> Parent { get; set; }
        internal BTreeNode<T>[] Children { get; set; }
        internal bool IsLeaf => Children[0] == null;

        internal BTreeNode(int maxKeysPerNode, BTreeNode<T> parent) : base(maxKeysPerNode)
        {

            Parent = parent;
            Children = new BTreeNode<T>[maxKeysPerNode + 1];

        }

        internal override BNode<T> GetParent()
        {
            return Parent;
        }

        internal override BNode<T>[] GetChildren()
        {
            return Children;
        }
    }

    internal class BTreeEnumerator<T> : IEnumerator<T> where T : IComparable
    {
        private readonly BTreeNode<T> root;
        private Stack<BTreeNode<T>> progress;
        private BTreeNode<T> current;
        private int index;

        internal BTreeEnumerator(BTreeNode<T> root)
        {
            this.root = root;
        }

        public bool MoveNext()
        {
            if (root == null)
            {
                return false;
            }

            if (progress == null)
            {
                current = root;
                progress = new Stack<BTreeNode<T>>(root.Children.Take(root.KeyCount + 1).Where(x => x != null));
                return current.KeyCount > 0;
            }

            if (current != null && index + 1 < current.KeyCount)
            {
                index++;
                return true;
            }

            if (progress.Count > 0)
            {
                index = 0;

                current = progress.Pop();

                foreach (var child in current.Children.Take(current.KeyCount + 1).Where(x => x != null))
                {
                    progress.Push(child);
                }

                return true;
            }

            return false;
        }

        public void Reset()
        {
            progress = null;
            current = null;
            index = 0;
        }

        object IEnumerator.Current => Current;

        public T Current
        {
            get
            {
                return current.Keys[index];
            }
        }

        public void Dispose()
        {
            progress = null;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRakeCore
{    
    /// <summary>
    /// Sparse matrix implementation that provides faster access to entire rows of data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RowOrientedSparseMatrix<T> 
    {
        class Node<T1> : IComparable //CF: not sure how to enforce that T1 has the same type as T, but it's a private nested class, so should be OK.
        {            
            public int Col { get; set; }
            public T1 Value { get; set; }

            public int CompareTo(Node<T1> other)
            {
                //CF: should we compare the values? Doesn't seem necessary in this case.
                return this.Col.CompareTo(other.Col);
            }

            public int CompareTo(object obj)
            {
                return CompareTo((Node<T1>)obj);
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public long Size { get; private set; }

        //The long parameter in the dict represents the index of the row.
        private Dictionary<long, SortedSet<Node<T>>> _rows = new Dictionary<long, SortedSet<Node<T>>>();

        public RowOrientedSparseMatrix(int w, int h)
        {
            this.Width = w;
            this.Height = h;
            this.Size = w * h;
        }

        public T[] Row(int row)
        {
            //int w = this.Width;
            //T[] items = new T[w];            
            //for (int i = 0; i  < w; i++){
            //    items[i] = default(T);
            //}
            
            //SortedSet<Node<T>> rowSet = LocateRowSet(row);
            //foreach (Node<T> x in rowSet)
            //{
            //    items[x.Col] = x.Value;
            //}

            //return items;

            SortedSet<Node<T>> rowSet = LocateRowSet(row);
            int length = rowSet.Count;
            T[] items = new T[length];
            if (length > 0)
            {
                int i = 0;
                foreach (Node<T> n in rowSet)
                {
                    items[i] = n.Value;
                    i += 1;
                }
            }
            return items;
        }

        public T this[int row, int col]
        {
            get
            {
                SortedSet<Node<T>> rowSet = LocateRowSet(row);
                if (rowSet == null) { return default(T); }
                
                //CF: I don't know the time complexity of this... SortedSet is implemented with a self-balancing red/black tree, so this
                //  *should* be O(log n), but I'm just guessing here.
                var found = rowSet.Where(x => x.Col == col).FirstOrDefault();
                if (found == null) { return default(T); }
                return found.Value;
            }
            set
            {
                SortedSet<Node<T>> rowSet = null;
                Node<T> cell = new Node<T>() { Col = col, Value = value };
                rowSet = LocateRowSet(row);
                rowSet.RemoveWhere(x => x.Col == col); //remove current entry, if it exists...
                rowSet.Add(cell);                
                _rows[row] = rowSet;
            }
        }

        private SortedSet<Node<T>> LocateRowSet(int row)
        {
            if (_rows.ContainsKey(row))
            {
                return _rows[row];
            }
            else
            {
                return new SortedSet<Node<T>>();
            }
        }
    }


    public class SparseMatrix<T>
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public long Size { get; private set; }

        private Dictionary<long, T> _cells = new Dictionary<long, T>();

        public SparseMatrix(int w, int h)
        {
            this.Width = w;
            this.Height = h;
            this.Size = w * h;
        }

        public T this[int row, int col]
        {
            get
            {
                long index = row * Width + col;
                T result;
                _cells.TryGetValue(index, out result);
                return result;
            }
            set
            {
                long index = row * Width + col;
                _cells[index] = value;
            }
        }
    }
}

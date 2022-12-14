using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Drastic.UI.Internals;

namespace Drastic.UI
{
	public abstract class TableSectionBase<T> : TableSectionBase, IList<T>, INotifyCollectionChanged where T : BindableObject
	{
		readonly ObservableCollection<T> _children = new ObservableCollection<T>();

		/// <summary>
		///     Constructs a Section without an empty header.
		/// </summary>
		protected TableSectionBase()
		{
			_children.CollectionChanged += OnChildrenChanged;
		}

		/// <summary>
		///     Constructs a Section with the specified header.
		/// </summary>
		protected TableSectionBase(string title) : base(title)
		{
			_children.CollectionChanged += OnChildrenChanged;
		}

		public void Add(T item)
		{
			_children.Add(item);
		}

		public void Clear()
		{
			_children.Clear();
		}

		public bool Contains(T item)
		{
			return _children.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_children.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _children.Count; }
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			return _children.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _children.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			return _children.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_children.Insert(index, item);
		}

		public T this[int index]
		{
			get { return _children[index]; }
			set { _children[index] = value; }
		}

		public void RemoveAt(int index)
		{
			_children.RemoveAt(index);
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add { _children.CollectionChanged += value; }
			remove { _children.CollectionChanged -= value; }
		}

		public void Add(IEnumerable<T> items)
		{
			items.ForEach(_children.Add);
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			object bc = BindingContext;
			foreach (T child in _children)
				SetInheritedBindingContext(child, bc);
		}

		void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			// We need to hook up the binding context for new items.
			if (notifyCollectionChangedEventArgs.NewItems == null)
				return;
			object bc = BindingContext;
			foreach (BindableObject item in notifyCollectionChangedEventArgs.NewItems)
			{
				SetInheritedBindingContext(item, bc);
			}
		}
	}

	public sealed class TableSection : TableSectionBase<Cell>
	{
		public TableSection()
		{
		}

		public TableSection(string title) : base(title)
		{
		}
	}
}
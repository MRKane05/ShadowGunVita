public class Queue<T>
{
	private uint _count;

	private Node<T> _head;

	private Node<T> _tail;

	private Node<T> _temp;

	public bool IsEmpty
	{
		get
		{
			return _count == 0;
		}
	}

	public uint Count
	{
		get
		{
			return _count;
		}
	}

	public T Peek()
	{
		if (IsEmpty)
		{
			return default(T);
		}
		return _head.Value;
	}

	public void Enqueue(T obj)
	{
		if (_count == 0)
		{
			_head = (_tail = new Node<T>(obj, _head));
		}
		else
		{
			_tail.Next = new Node<T>(obj, _tail.Next);
			_tail = _tail.Next;
		}
		_count++;
	}

	public T Dequeue()
	{
		if (IsEmpty)
		{
			return default(T);
		}
		_temp = _head;
		_head = _head.Next;
		_count--;
		return _temp.Value;
	}

	public void Clear()
	{
		_head = (_tail = (_temp = null));
		_count = 0u;
	}

	public override string ToString()
	{
		string text = "(";
		for (_temp = _head; _temp != null; _temp = _temp.Next)
		{
			text = text + _temp.Value.ToString() + ((_temp.Next == null) ? string.Empty : ", ");
		}
		return text + ")";
	}
}
public class Queue
{
}

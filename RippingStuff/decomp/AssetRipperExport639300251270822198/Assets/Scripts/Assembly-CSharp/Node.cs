internal class Node<T>
{
	public Node<T> Next;

	public T Value;

	public Node(T value, Node<T> next)
	{
		Next = next;
		Value = value;
	}
}

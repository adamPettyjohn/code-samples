//Adam Pettyjohn
//This class is a data structure. Users can add elements to it from both the
//front and back of it and can remove them from the front and back as well.

import java.util.*;

public class ArrayDeque<E> implements Deque<E> {
	private E[] theDeck;
	private int front;
	private int back;
	private int size;
	
	//This function constructs and empty ArrayDeque.
	@SuppressWarnings("unchecked")
	public ArrayDeque() {
		theDeck = (E[]) (new Object[10]);
		front = 0;
		back = 0;
		size = 0;
	}
	
	//This function adds the passed element to the front of the Deque. It throws
	//a NullPointerException if the passed element is null.
	public void addFirst(E element) {
		if(element == null) {
			throw new NullPointerException();
		}
		if(size < theDeck.length - 1) {
			if(size == 0) {
				theDeck[front] = element;
			} else if(front == 0) {
				theDeck[theDeck.length - 1] = element;
				front = theDeck.length - 1;
			} else {
				theDeck[front - 1] = element;
				front--;
			}
			size++;
		} else {
			expandDeck();
			addFirst(element);
		}
	}
	
	//This function adds the passed element to the back of the Deque. It throws
	//a NullPointerException if the passed element is null.
	public void addLast(E element) {
		if(element == null) {
			throw new NullPointerException();
		}
		if(size < theDeck.length - 1) {
			if(size == 0) {
				theDeck[back] = element;
			} else if(back == theDeck.length - 1) {
				theDeck[0] = element;
				back = 0;
			} else {
				theDeck[back + 1] = element;
				back++;
			}
			size++;
		} else {
			expandDeck();
			addLast(element);
		}
	}
	
	//This function expands the Deque.
	@SuppressWarnings("unchecked")
	private void expandDeck() {
		E[] largerDeck = (E[]) (new Object[theDeck.length * 2]);
		for(int i = 0; i < theDeck.length; i++) {
			largerDeck[i] = removeFirst();
			size++;
		}
		theDeck = largerDeck;
		front = 0;
		back = size - 1;
	}
	
	//This function empties the Deque.
	public void clear() {
		for(int i = 0; i < theDeck.length; i++) {
			theDeck[i] = null;
		}
		front = 0;
		back = 0;
		size = 0;
	}
	
	//This function returns whether or not the Deque is empty.
	public boolean isEmpty() {
		return size == 0;
	}
	
	//This function returns an Iterator for the Deque.
	public Iterator<E> iterator() {
		return new ArrayDequeIterator();
	}
	
	//This function returns the element at the front of the Deque. If the Deque
	//is empty it throws a NoSuchElementException.
	public E peekFirst() {
		return peek(front);
	}
	
	//This function returns the element at the back of the Deque. If the Deque
	//is empty it throws a NoSuchElementException.
	public E peekLast() {
		return peek(back);
	}
	
	//This function returns the element that exists at the passed location. If the Deque
	//is empty it throws a NoSuchElementException.
	private E peek(int index) {
		if(size == 0) {
			throw new NoSuchElementException();
		}
		return theDeck[index];
	}
	
	//This function returns the element at the front of the Deque and the removes the element.
	//If the Deque is empty it throws a NoSuchElementException.
	public E removeFirst() {
		E temp = peek(front);
		theDeck[front] = null;
		if(size == 1) {
			//do nothing to front;
		} else if(front == theDeck.length - 1) {
			front = 0;
		} else {
			front++;
		}
		size--;
		return temp;
	}
	
	//This function returns the element at the back of the Deque and the removes the element.
	//If the Deque is empty it throws a NoSuchElementException.
	public E removeLast() {
		E temp = peek(back);
		theDeck[back] = null;
		if(size == 1) {
			//do nothing to back;
		} else if(back == 0) {
			back = theDeck.length - 1;
		} else {
			back--;
		}
		size--;
		return temp;
	}
	
	//This function returns the size of the Deque.
	public int size() {
		return size;
	}
	
	//This function returns the Deque as a String.
	public String toString() {
		StringBuilder myDeckAsString = new StringBuilder();
		myDeckAsString.append("[");
		
		int current = front;
		
		if(size > 0) {
			for(int i = 0; i < size - 1; i++) {
				myDeckAsString.append(theDeck[current]);
				myDeckAsString.append(", ");
				current++;
				if(current > theDeck.length - 1) {
					current = 0;
				}
			}
			myDeckAsString.append(theDeck[current]);
		}
		
		myDeckAsString.append("]");
		
		
		return myDeckAsString.toString();
	}
	
	//This class is an Iterator for the ArrayDeque class. It allows the user to
	//iterate over the elements in the Deque.
	private class ArrayDequeIterator implements Iterator<E> {
		private int index;
		
		//This function creates a new Iterator.
		public ArrayDequeIterator() {
			index  = front;
		}
		
		//This function returns whether or not the Iterator still has elements to return.
		public boolean hasNext() {
			int next;
			if(index - 1 == theDeck.length - 1){
				next = 0;
			} else {
				next = index;
			}
			return (theDeck[next] != null && !theDeck[next].equals(front));
		}
		
		//This function returns the next element in the Deque.
		public E next() {
			E result = theDeck[index];
			if(index == theDeck.length - 1) {
				index = 0;
			} else {
				index++;
			}
			
			return result;
		}
		
		/**
		 * Removes the most recently returned element.
		 * Not supported. Throws an UnsupportedOperationException when called.
		 */
		public void remove() {
			throw new UnsupportedOperationException();
		}
	}
}

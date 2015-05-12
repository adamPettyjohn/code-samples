/*Adam Pettyjohn
March 1, 2014
The purpose of this file is to allow other files to call
 these function of Trie.*/

#ifndef TRIE_H
#define TRIE_H

typedef struct node trieNode;
struct node {
    char *nodeWord;
    trieNode *nextNodes[9];
};

typedef struct trie trieRoot;
struct trie {
    trieNode *root;
    trieNode *current;
};

void CreateTrie(trieRoot *tempTrie);
void DestroyNode(trieNode *tempNode);
void NextNode(trieRoot *tempTrie);
void AddNode(trieRoot *tempTrie, int newWord[], char *theWord, int tempSize);
void FindNode(trieRoot *tempTrie, int findWord[], int tempSize);

#endif


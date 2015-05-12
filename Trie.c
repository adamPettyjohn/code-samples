/*Adam Pettyjohn
March 1, 2014
The purpose of this file is to create a data structure
that stores words. The words in the data structure can
then be accessed using T9.*/

#include "Trie.h"
#include <stdio.h>
#include <stdlib.h>

trieNode *CreateNode();

/*CreateTrie initializes the parameters for the passed in trieRoot "tempTrie".*/
void CreateTrie(trieRoot *tempTrie) {
    tempTrie->root = CreateNode();
    tempTrie->current = tempTrie->root;
}

/*CreateNode creates and returns a trieNode. The returned trieNode has all of
 its parameters initialized by CreateNode.*/
trieNode *CreateNode() {
    int i;
    trieNode *tempNode = NULL; //the new trieNode to be returned
    
    tempNode = (trieNode *)malloc(sizeof(trieNode));
 
    if(tempNode != NULL) {
        tempNode->nodeWord = NULL;
 
        for(i = 0; i < 9; i++) {
            tempNode->nextNodes[i] = NULL;
        }
    }
 
    return tempNode;
}

/*Destroy node frees the memory allocated for the passed in trieNode "tempNode".
 In addition, it frees the memory allocated for all of the trieNodes "tempNode"
 links to.*/
void DestroyNode(trieNode *tempNode) {
    if(tempNode->nextNodes[0] != NULL) {
        DestroyNode(tempNode->nextNodes[0]);
    }
    if(tempNode->nextNodes[1] != NULL) {
        DestroyNode(tempNode->nextNodes[1]);
    }
    if(tempNode->nextNodes[2] != NULL) {
        DestroyNode(tempNode->nextNodes[2]);
    }
    if(tempNode->nextNodes[3] != NULL) {
        DestroyNode(tempNode->nextNodes[3]);
    }
    if(tempNode->nextNodes[4] != NULL) {
        DestroyNode(tempNode->nextNodes[4]);
    }
    if(tempNode->nextNodes[5] != NULL) {
        DestroyNode(tempNode->nextNodes[5]);
    }
    if(tempNode->nextNodes[6] != NULL) {
        DestroyNode(tempNode->nextNodes[6]);
    }
    if(tempNode->nextNodes[7] != NULL) {
        DestroyNode(tempNode->nextNodes[7]);
    }
    if(tempNode->nextNodes[8] != NULL) {
        DestroyNode(tempNode->nextNodes[8]);
    }
    
    free(tempNode);
}

/*NextNode takes in a trieRoot "tempTrie". Whatever the current trieNode is for
"tempTrie", NextNode sets the current trieNode to the ninth trieNode the current
 trieNode is linked to. If there is no such trieNode, NextNode prints
 "There are no more T9onyms." If the trieNode does exist, its associated word
 is printed.*/
void NextNode(trieRoot *tempTrie) {
    if(tempTrie->current->nextNodes[8] != NULL) {
        tempTrie->current = tempTrie->current->nextNodes[8];
        printf("        '%s'", tempTrie->current->nodeWord);
        printf("\n");
    } else {
        printf("        There are no more T9onyms");
        printf("\n");
    }
}

/*AddNode adds the passed in string "theWord" to the passed in trieRoot "tempTrie".
 The parameters "newWord" and "tempSize" include information for placing the
 string correctly in the trieRoot. "newWord" is the string's number code and
 "tempSize" is the length of the string.*/
void AddNode(trieRoot *tempTrie, int newWord[], char *theWord, int tempSize) {
    int added; //a check to see if the word has been added yet
    int i;
    trieNode *currentNode; //the current trieNode that is being iterated on
    
    added = 0;
    
    currentNode = tempTrie->root;
    
    for(i = 0; i < tempSize; i++) {
        if(currentNode->nextNodes[newWord[i] - 2] == NULL) {
            currentNode->nextNodes[newWord[i] - 2] = CreateNode();
        }
        
        currentNode = currentNode->nextNodes[newWord[i] - 2];
    }
    
    while(!added) {
        if(currentNode->nodeWord == NULL) {
            currentNode->nodeWord = theWord;
            added = 1;
        } else {
            if(currentNode->nextNodes[8] == NULL) {
                currentNode->nextNodes[8] = CreateNode();
            }
            
            currentNode = currentNode->nextNodes[8];
        }
    }
}

/*FindNode searches for the passed in sequence "findWord", which is of length
 "tempSize", in the passed in trieRoot "tempTrie".  If it finds the trieNode
 represented by the passed in sequence it prints out the word associated with
 that trieNode. If the trieNode is not found, it prints an appropriate message
 as to why it was not.*/
void FindNode(trieRoot *tempTrie, int findWord[], int tempSize) {
    int i;
    int type; //a check to see if the word was found
    trieNode *currentNode; //the current trieNode being iterated on
    
    currentNode = tempTrie->root;
    
    type = 1;
    
    for(i = 0; i < tempSize; i++) {
        if(currentNode->nextNodes[findWord[i] - 2] == NULL) {
            if(findWord[i] - 2 == 8) {
                printf("        There are no more T9onyms");
                printf("\n");
            } else {
                printf("        Not found in current dictionary.");
                printf("\n");
            }
            
            type = 0;
            tempTrie->current = tempTrie->root;
            break;
        } else {
            currentNode = currentNode->nextNodes[findWord[i] - 2];
        }
    }
    
    if(type == 1) {
        tempTrie->current = currentNode;
        printf("        '%s'", currentNode->nodeWord);
        printf("\n");
    }
    
}

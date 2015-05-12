/*Adam Pettyjohn
March 1, 2014
The purpose of this file is to read in a file of words.
After the words are read in, the words can be searched using T9.*/

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "Trie.h"

void AddWord(trieRoot *tempTrie, char *wordToAdd);

/*Main takes the file passed in the command line and creates a dictionary
 from the words in the file. If the file can not be opened an error is
 printed and the program terminated. After the dictionary is created,
 the user is prompted for input. Depending on the user's input, words
 can be found in the dictionary*/
int main(int argc, char** argv) {
    FILE *fp; //the file to be read from
    trieRoot test; //the trieRoot that will store the words
    int wc;
    char oneword[100]; //this stores the current word being read
    char en;
    int nc;
    int j;
    char *a; //this stores user input during the interactive phase
    int k;
    int tempLen;
    
    CreateTrie(&test);
    
    //This block reads in words from a file
    ////////////////////////////////////////
    wc = 0;
    
    fp = fopen(argv[1],"r");
    
    if(fp == NULL) {
        fputs("Error while trying to open file." , stderr);
        exit(EXIT_FAILURE);
    }
    
    while(1) {
        en = fscanf(fp,"%s",oneword);
        
        if(en == EOF) {
            break;
        }
        
        wc += 1;
    }    
    fclose(fp);
    
    char keys[wc][50];
    nc = 0;
    
    fp = fopen(argv[1],"r");
    while(1) {
        en = fscanf(fp,"%s",oneword);
        
        strcpy(keys[nc], oneword);
        
        if(en == EOF) {
            break;
        }
        
        nc += 1;
    }
    fclose(fp);
    /////////////////////////////////////
    
    for(j = 0; j < wc; j++) {
        AddWord(&test, keys[j]);
    }
    
    printf("Enter \"exit\" to quit.\n");
    
    while(1) {
        printf("Enter Key Sequence (or \"#\" for next word):\n");
        scanf("%s", a);
        
        if(strcmp(a, "exit") == 0) {
            break;
        }
        
        if(strcmp(a, "#") == 0) {
            NextNode(&test);
        } else {
            tempLen = strlen(a);

            int wordFind[tempLen];

            for(k = 0; k < tempLen; k++) {
                if((int)(a[k]) - 48 == -13) {
                    a[k] = 58;
                }
                wordFind[k] = (int)(a[k]) - 48;
            }
            
            
            FindNode(&test, wordFind, tempLen);
        }   
    }
    
    DestroyNode(test.root);
    
    return (EXIT_SUCCESS);
}

/*AddWord adds the passed in string "wordToAdd" to the passed in 
trie "tempTrie".*/
void AddWord(trieRoot *tempTrie, char *wordToAdd) {
    int length; //the length of the word
    int i;
    
    length = strlen(wordToAdd);
    
    int nums[length];
    
    for(i = 0; i < length; i++) {
        char temp = wordToAdd[i];
        if(temp == 'a' || temp == 'b' || temp == 'c') {
            nums[i] = 2;
        } else if(temp == 'd' || temp == 'e' || temp == 'f') {
            nums[i] = 3;
        } else if(temp == 'g' || temp == 'h' || temp == 'i') {
            nums[i] = 4;
        } else if(temp == 'j' || temp == 'k' || temp == 'l') {
            nums[i] = 5;
        } else if(temp == 'm' || temp == 'n' || temp == 'o') {
            nums[i] = 6;
        } else if(temp == 'p' || temp == 'q' || temp == 'r' || temp == 's') {
            nums[i] = 7;
        } else if(temp == 't' || temp == 'u' || temp == 'v') {
            nums[i] = 8;
        } else if(temp == 'w' || temp == 'x' || temp == 'y' || temp == 'z') {
            nums[i] = 9;
        }
    }
    
    AddNode(tempTrie, nums, wordToAdd, length);
}






.MODEL small
.STACK 100h
.DATA
HelloMessage DB 'Hello, world',13,10,'$'
.CODE
.startup
mov  ax,@data
mov  ds,ax                  
mov  ah,9                   
mov  dx,OFFSET HelloMessage 
int  21h                    
mov  ah,4ch                 
int  21h                    
END
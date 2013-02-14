killa
=====

Quick and Dirty Process Killer.  Watches for a sepcific process and will kill the given process and its children when requested by user.

Sample Ouput
====
        PS>  .\Killa.exe notepad
        This Is Live And It Will Kill Processes

        Enter Command(x,g): g
        0) 5840 notepad
             "C:\Windows\system32\notepad.exe"

        1) 3844 notepad
             "C:\Windows\system32\notepad.exe"

        2) 5928 notepad
             "C:\Windows\system32\notepad.exe"

        3) 5932 notepad
             "C:\Windows\system32\notepad.exe"


        Enter Index(es) To Kill or x to continue: 0,2
        Killing Children For: 5840 notepad
        Killing: 5840 notepad
        Killing Children For: 5928 notepad
        Error Killing Child Process
        Killing: 5928 notepad

        Enter Command(x,g): g
        0) 3844 notepad
             "C:\Windows\system32\notepad.exe"
        
        1) 5932 notepad
             "C:\Windows\system32\notepad.exe"
        
        
        Enter Index(es) To Kill or x to continue: 0
        Killing Children For: 3844 notepad
        Killing: 3844 notepad
        
        Enter Command(x,g): g
        0) 5932 notepad
             "C:\Windows\system32\notepad.exe"


        Enter Index(es) To Kill or x to continue: x

        Enter Command(x,g): x
        PS>

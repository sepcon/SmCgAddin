# Autosar StateMachine Code Generation
Currently, the addin was implement for generating code from State Chart in enterprise architect, we can use it for checking errors or inconsistencies in the diagram, but this feature has not implemented yet.

## How to use:
### Steps:
- Open your StateMachine diagram in EA, choose EXTENSIONS > StateMachine Code Generation > Generate Source code with naive naming.
- Select the folder for saving generated source code.
- there are 2 files will be generated: header file(.h) and source file(.c)
### Notes:
- the header file and source file will be named using the name of the diagram, so you should name the diagram meaningful and contains no spaces.
- each state in diagram should have a meaningful name and must not contain any space character.
- transition from state to state with no trigger events will be considered as Default transition according to Autosar StateMachine Lib.
- make sure the transitions using Choice Node would be draw with nicely name and conditions, it would make the conditions on transitions are collected correctly and easy to understand after code generating.
- there are a lot of rules to draw transitions from states to states, so the Addin is not able to make sure generate source code from all free style State diagram correctly.
- after you implement a function, you can renamed it by using Replace All feature on your editor/development tool.
- currently, the transition/condition functions are named with number like Action_Function1, Action_Function2... there would be a lot of nicely ways to named a function using the Events, conditions, or actions on each transition. But it needs to have much time to implement. If you are interested, let improve the Addin by doing that.


## How to install Addin:
### Prerequisites: 
- Enterprise Architect verion > 12
- Microsoft .Net Framework 4.5
### install:
- run file install.exe with admin right
- choose the folder for install the addin
- will have a message box show the installing result.
-- [IMPORTANT] make sure that Prerequisites are installed and turn off EA before install.

## Customization:
- if you want to improve or customize the addin for your specific needs, you can using Visual Studio 2013 or greater for implementing.
- the addin is developed in C# and EA automation interface (with Microsoft COM interface (Component Object Model))
- for using EA automation interface, refer: [EA Automation](https://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=web&cd=2&cad=rja&uact=8&ved=0ahUKEwjctsqykrLTAhVLKo8KHTrWDjQQFggpMAE&url=http%3A%2F%2Fwww.sparxsystems.com.au%2Fresources%2Fuser-guides%2Fautomation%2Fautomation.pdf&usg=AFQjCNFgGa4kK4Th51Y1f4G9YheOf44mXw&sig2=7jrrXmjBxEyC9zne5upqmA)
### Source code structure:
- there are 2 projects in SMCodeGenAddin solution, one for implement addin and the remaining for deploying addin.
- the OutPut dir contains the install.exe, Addin dll and the data folder contains files to store formats of output source code using by addin.
- format files structure:
-- each line is corresponding to a format (functions, transition table, hsm table and source/header file format...)
-- you can take a line to modify it by copy the line to other file and replace "\n" with new line character in your editor. After modifying, you should revert the new line character with "\n" and paste exactly to line number that you took the line previously. --> make sure that you must not remove the "{0}", "{1}" ,"{n}" away, they are placeholder to insert infos collected from the diagram. If not, the program will crash.
-- you can find the meaning of each format on each line by matching the line number with corresponding enum in DataFormat.cs.
if you want to add new lines to format file you should add new corresponding values to the enum and vice versa. :)
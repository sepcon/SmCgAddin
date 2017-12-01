import re
#from pkg_resources._vendor.pyparsing import line
from sets import Set
import sys
import glob
import argparse
import os

makeFileTemplate = '''
CC            = gcc
CXX           = g++
LINK          = g++
DEFINES       = -DENABLE_CONSOLE_LOG -DQT_QML_DEBUG
CFLAGS        = -pipe -g -Wall -W -fPIC $(DEFINES)
CXXFLAGS      = -pipe -g -std=gnu++11 -Wall -W -fPIC $(DEFINES)
INCPATH       = %INCPATH%

OBJECTS       = %OBJECTS%

TARGET        = Nothing


####### Build rules

$(TARGET):  $(OBJECTS)
	$(LINK) $(LFLAGS) -o $(TARGET) $(OBJECTS) $(OBJCOMP) $(LIBS)

####### Compile
%COMPILE%
'''




def getFileNameFromPath(filePath):
    indexOfSlash = filePath.rfind('/')
    if indexOfSlash == -1:
        return filePath
    else:
        return filePath[indexOfSlash : len(filePath)]

def getContainingDir(filePath):
    indexOfSlash = filePath.rfind('/')
    if indexOfSlash == -1:
        return ""
    else:
        return filePath[0 : indexOfSlash]



includePathsList = None
cppFilesList = Set()
headerFilesList = Set()
objectFilesList = ""

argParser = argparse.ArgumentParser('python qmake_from_make.py',
                                    description = '\n\n\tSimply conversion from makefile to qmake file for investigating source code with qtcreator')
argParser.add_argument('gnumake_file_path',
                       help = 'path to make file that wanted to convert to qmake')
argParser.add_argument('-od', '--out_dir', default = '.',
                       help = 'directory for storing Makefile file output')
argParser.add_argument('-v', "--verbose", help = 'more details about output', action = 'store_true')

args = argParser.parse_args()

makefile = open(args.gnumake_file_path, 'r')
makeContent = makefile.readlines()


for line in makeContent:
    cppMatch = re.search(r'\/[^%]+.cpp[\s]+\\', line)
    if cppMatch :
        cppFilesList.add(cppMatch.group()[0 : cppMatch.group().rfind(".cpp") + 4])


objectsList = ""
compileRules = ""
includePaths = ""


for cppFile in cppFilesList:
    start = cppFile.rfind("/") + 1
    end = len(cppFile) - 3
    objectFile = cppFile[ start : end] + "o"
    objectFilesList += " " + objectFile
    objectsList += objectFile + " \\\n\t\t"
    compileRules += objectFile + ": " + cppFile + "\n\n"

for line in makeContent:
    startId = line.find("-I");
    if (startId >= 0):
        includePaths = line[startId:len(line)]
    break


outputMakeFile = makeFileTemplate.replace("%COMPILE%", compileRules).replace("%OBJECTS%", objectsList).replace("%INCPATH%", includePaths)

makefile.close()

qmakeFilePath = args.out_dir + '/' + "Makefile"
qmakeFile = open( qmakeFilePath, 'w')
qmakeFile.write(outputMakeFile);


os.system("cd " + args.out_dir + "; touch Nothing " + objectFilesList)

qmakeFile.close()
if args.verbose:
    print '''
    Conversion succesful!
    OUTPUT: {}
    please open the file with Netbeans IDE for using it!
    '''.format(qmakeFilePath)

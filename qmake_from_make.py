import re
from sets import Set
import sys
import glob



MAKEFILE_PATH_KEY = '-m'
QMAKE_DIR_KEY = '-qm'

QMAKE_FILE_EXTENSION = ".pro"
QMAKE_FILE_TEMPLATE = 'TEMPLATE = app\nCONFIG += console c++11\nCONFIG -= app_bundle \nCONFIG -= qt\n\n\n'
QMAKE_INCLUDEPATH_KEYWORD = "INCLUDEPATH += \\\n"
QMAKE_SOURCE_KEYWORD = "SOURCES += \\\n"
HEADER = "HEADERS += \\\n"


def printHelp():
    print '''
    -----------------------------------USAGE--------------------------------------------
    SYNTAX: python qmake_from_make.py -m <path_to_makefile> [-qm <path_to_qmake_output>]
    ------------------------------------------------------------------------------------
    '''
def checkArguments(argumentsMap):
    if not argumentsMap.has_key(MAKEFILE_PATH_KEY):
        print '\nplease specify the path of makefile'
        return False

    if not argumentsMap.has_key(QMAKE_DIR_KEY):
        argumentsMap[QMAKE_DIR_KEY] = '.'

    return True

def parseArguments():
    argumentsMap = {}
    argc = len(sys.argv)
    if argc > 1:
        i = 1
        while i < argc:
            if sys.argv[i][0] == '-':
                argumentsMap[sys.argv[i]] = sys.argv[i + 1]
                i = i + 2
            else:
                i = i + 1
    return argumentsMap

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



argumentsMap = parseArguments()
if checkArguments(argumentsMap) != True:
    printHelp()
    exit(-1)





includePathsList = None
cppFilesList = Set()
headerFilesList = Set()

makefile = open(argumentsMap[MAKEFILE_PATH_KEY], 'r')
makeContent = makefile.readlines()


for line in makeContent:
    cppMatch = re.search(r'\/[^%]+.cpp[\s]+\\', line)
    if cppMatch :
        cppFilesList.add(cppMatch.group() + "\n")


for line in makeContent:
    includeMatch = re.search(r'-I.*\/ ', line)
    if includeMatch:
        includeContent = includeMatch.group()
        includePathsList = includeContent.replace("-I", " \\\n")
        # includePathsList = Set(includePathsList)
        break

for cppFile in cppFilesList:
    containingDir = getContainingDir(cppFile)
    headerFilesInSameDir = glob.glob(containingDir + "/*.h")
    for header in headerFilesInSameDir:
        headerFilesList.add(header + ' \\\n')

makefile.close()

qmakeFilePath = argumentsMap[QMAKE_DIR_KEY] + '/' + getFileNameFromPath(argumentsMap[MAKEFILE_PATH_KEY]) + '.pro'
qmakeFile = open( qmakeFilePath, 'w')
qmakeFile.write(QMAKE_FILE_TEMPLATE);
if includePathsList :
    qmakeFile.write(QMAKE_INCLUDEPATH_KEYWORD )
    qmakeFile.write(includePathsList + '\n')
    qmakeFile.write("\n\n")

qmakeFile.write(QMAKE_SOURCE_KEYWORD)
qmakeFile.writelines(cppFilesList)

qmakeFile.write("\n\n")

qmakeFile.write(HEADER)
qmakeFile.writelines(headerFilesList)

qmakeFile.close()

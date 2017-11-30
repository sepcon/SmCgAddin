import re
from sets import Set
import sys
import glob
import argparse

QMAKE_FILE_EXTENSION = ".pro"
QMAKE_FILE_TEMPLATE = 'TEMPLATE = app\nCONFIG += console c++11\nCONFIG -= app_bundle \nCONFIG -= qt\n\n\n'
QMAKE_INCLUDEPATH_KEYWORD = "INCLUDEPATH += \\\n"
QMAKE_MACROS_DEFINE_KEYWORD = "DEFINES += \\\n"
QMAKE_SOURCE_KEYWORD = "SOURCES += \\\n"
HEADER = "HEADERS += \\\n"



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
macrosList = None
cppFilesList = Set()
headerFilesList = Set()

argParser = argparse.ArgumentParser('python qmake_from_make.py',
                                    description = '\n\n\tSimply conversion from makefile to qmake file for investigating source code with qtcreator')
argParser.add_argument('make_file_path',
                       help = 'path to make file that wanted to convert to qmake')
argParser.add_argument('-qm', '--qmake_out_dir', default = '.',
                       help = 'directory for storing qmake file output')
argParser.add_argument('-v', "--verbose", help = 'more details about output', action = 'store_true')

args = argParser.parse_args()

makefile = open(args.make_file_path, 'r')
# makefile = open("/home/sepcon/Documents/git/SmCgAddin/apphmi_sds-cgi3-rnaivi_out.gnumake", 'r')
gnumakeContent = makefile.readlines()


for line in gnumakeContent:
    cppMatch = re.search(r'\/[^%]+.cpp[\s]+\\', line)
    if cppMatch :
        cppFilesList.add(cppMatch.group() + "\n")


for line in gnumakeContent:
    includeMatch = re.search(r'-I.*\/ ', line)
    if includeMatch:
        includeContent = includeMatch.group()
        includePathsList = includeContent.replace("-I", " \\\n")
        # includePathsList = Set(includePathsList)
        break

for line in gnumakeContent:
    macrosMatch = re.search(r'-D.* ', line)
    if macrosMatch:
        macrosContent = macrosMatch.group()
        macrosList = macrosContent.replace("-D", "")
        break
        
for cppFile in cppFilesList:
    containingDir = getContainingDir(cppFile)
    headerFilesInSameDir = glob.glob(containingDir + "/*.h")
    for header in headerFilesInSameDir:
        headerFilesList.add(header + ' \\\n')

makefile.close()

qmakeFilePath = args.qmake_out_dir + '/' + getFileNameFromPath(args.make_file_path) + '.pro'
qmakeFile = open( qmakeFilePath, 'w')
qmakeFile.write(QMAKE_FILE_TEMPLATE);


####################################################################################################################
# #################################Write DEFINES Section#######################################################
####################################################################################################################
if macrosList :
    qmakeFile.write(QMAKE_MACROS_DEFINE_KEYWORD )
    qmakeFile.write(macrosList)
    qmakeFile.write("\n\n")

####################################################################################################################
# #################################Write Include Path Section#######################################################
####################################################################################################################
if includePathsList :
    qmakeFile.write(QMAKE_INCLUDEPATH_KEYWORD )
    qmakeFile.write(includePathsList + '\n')
    qmakeFile.write("\n\n")
####################################################################################################################
# #################################write srouce files section#######################################################
####################################################################################################################
if len(cppFilesList) > 0:
    qmakeFile.write(QMAKE_SOURCE_KEYWORD)
    qmakeFile.writelines(cppFilesList)
    qmakeFile.write("\n\n")

####################################################################################################################
# #####################################write header file section####################################################
####################################################################################################################
if len(headerFilesList) > 0:
    qmakeFile.write(HEADER)
    qmakeFile.writelines(headerFilesList)
    qmakeFile.write("\n\n")

qmakeFile.close()
if args.verbose:
    print '''
    Conversion succesful!
    OUTPUT: {}
    please open the file with qtcreator for using it!
    '''.format(qmakeFilePath)

import re
from sets import Set
makefile = open("/home/sepcon/Documents/git/SmCgAddin/sds_adapter-rnaivi_out.gnumake", 'r')

INCLUDEPATH = "INCLUDE_PATH += \\"
SOURCE = "SOURCE += \\"

setOfInclude = None
setOfCppFiles = Set()


line = makefile.readline()
while line != '':
    cppFile = re.search(r'\/[^%]+.cpp[\s]+\\', line)
    if cppFile :
        setOfCppFiles.add(cppFile.group() + "\n")
    line = makefile.readline()

makefile.seek(0)
line = makefile.readline()
while line != '':
    includeMatch = re.search(r'-I.*\/ ', line)
    if includeMatch:
        includeContent = includeMatch.group();
        setOfInclude = includeContent.replace("-I", " \\\n")
        # setOfInclude = Set(includePaths)
        break
    else:
        line = makefile.readline()

makefile.close()

profile = open("/home/sepcon/Documents/git/SmCgAddin/sds.pro", 'w')

if setOfInclude :
    profile.write(INCLUDEPATH + "\n")
    profile.write(setOfInclude + '\n')
    profile.write("\n\n")
profile.write(SOURCE + '\n')
profile.writelines(setOfCppFiles)
profile.close()
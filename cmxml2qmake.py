import xml.etree.ElementTree as ET
import os
import os.path



OS_FILTER = "Linux"
VARIANT_FILTER = "AIVI"

QMK_FILE_EXTENSION = ".pro"
QMK_FILE_TEMPALTE_HEAD ='''
TEMPLATE = app
CONFIG += console 
CONFIG -= app_bundle 
CONFIG -= qt
'''
QMK_SOURCES = "SOURCES += "
QMK_INCLUDEPATH = "INCLUDEPATH += "
QMK_DEFINES = "DEFINES += "
SEPERATED_CHARS = "\n\n"


_SWBUILDROOT = os.environ.get("_SWBUILDROOT", "")
_SWROOT = os.environ.get("_SWROOT", "")
_SWNAVIROOT= os.environ.get("_SWNAVIROOT", "")

# _SWBUILDROOT = "Z:/views/SDS"
# _SWROOT = "Z:/views/SDS"
# _SWNAVIROOT= "Z:/views/SDS/ai_sds"

prod_xml_file = "/home/cgo1hc/samba/views/SDS/ai_sds/config/xml/ai_sds_prod.xml"
outputDir = "/home/cgo1hc/QtProjects/sds_middleware"

def getRootDir():
    indexOfConfigSubDir = prod_xml_file.find("/config/xml")
    return prod_xml_file[0:indexOfConfigSubDir]

ROOT_DIR = getRootDir()

def operatingSystemFileter(elem):
    ret = elem.get("operatingsystemfilter")
    return ( ret == None or ret == OS_FILTER)

def variantFilter(elem):
    ret = elem.get("customerprojectfilter")
    return (ret == None or ret == VARIANT_FILTER)

def writeToConsole(fileName, fileContent):
    print "===============================", fileName, "======================================="
    print fileContent
    print "===============================", fileName, "======================================="
def writeToFile(fileName, fileContent):
    filePath = outputDir + "/" + fileName + ".pro"
    writer = open(filePath, 'w')
    writer.write(fileContent)
    writer.close()

def writeContent(fileName, fileContent):
    # writeToConsole(fileName, fileContent)
    writeToFile(fileName, fileContent)

#Get a list of source files with format of
# source1.cpp \
# source2.cpp \
# ... \
# sourceN.cpp \
def formatedAdd(listStr, value):
    return listStr + "\t" + value + " \\\n"
def includeSubProject(projectContent, newSubProject):
    return projectContent + SEPERATED_CHARS + "include(" + newSubProject + ")"

def getSourceFilesList(xmlPath):
    sourceFiles = ""
    idx = xmlPath.rfind("/")
    subPath = xmlPath + "/" + xmlPath[idx + 1 : len(xmlPath)] + ".xml"
    cfgFileName = _SWROOT + subPath
    if not os.path.isfile(cfgFileName):
        cfgFileName = _SWNAVIROOT + subPath
        if not os.path.isfile(cfgFileName):
            return ""

    idx = cfgFileName.rfind("/")
    curDir = cfgFileName[0:idx + 1]
    componentDeclarations = ET.parse(cfgFileName).getroot()
    for component in componentDeclarations.iter("component"):
        for file in component.findall("file"):
            if operatingSystemFileter(file):
                absFilePath = curDir + file.get("name")
                sourceFiles = formatedAdd(sourceFiles, absFilePath)
    return sourceFiles

def qmakeFileName(productDeclaration):
    return productDeclaration.get("name") + QMK_FILE_EXTENSION

def createSubQmakeProject(productdeclaration):
    sourceFilesList = QMK_SOURCES
    includePathsList = QMK_INCLUDEPATH
    macrosList = QMK_DEFINES
    #Get the source files
    for basePath in productdeclaration.iter("basepath"):
        if(variantFilter(basePath)):
            sourceFilesList += getSourceFilesList(basePath.get("path"))

    #Get the include paths and macros define
    for compilerSettings in productdeclaration.iter("compilersettings"):
        #Get include paths
        for include in compilerSettings.iter("include"):
            if(variantFilter(include)):
                includePathsList = formatedAdd(includePathsList, include.get("path"))
        #GET MACROS
        for define in compilerSettings.iter("define"):
            if(variantFilter(define)):
                macrosList = formatedAdd(macrosList, define.get("name"))
    fileContent = (QMK_FILE_TEMPALTE_HEAD + SEPERATED_CHARS + macrosList + SEPERATED_CHARS + includePathsList + SEPERATED_CHARS + sourceFilesList)
    return fileContent.replace("$(_SWBUILDROOT)", _SWBUILDROOT).replace("$(_SWROOT)", _SWROOT)


def createQmakeProject(productGroup):
    projectFileContent = QMK_FILE_TEMPALTE_HEAD
    listOfSubProjects = ""
    for productdeclaration in productGroup.iter("productdeclaration"):
        writeContent(productdeclaration.get("name"), createSubQmakeProject(productdeclaration))
        listOfSubProjects = includeSubProject(listOfSubProjects, qmakeFileName(productdeclaration))
    projectFileContent += SEPERATED_CHARS + listOfSubProjects
    writeContent(productGroup.get("name"), projectFileContent)

def parseProjects(xmlFilePath):
    productdeclarations = ET.parse(xmlFilePath).getroot()
    for productGroup in productdeclarations.findall("productgroup"):
        createQmakeProject(productGroup)

print parseProjects(prod_xml_file)



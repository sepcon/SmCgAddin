using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using ARAddinDeploy;

namespace SMDeploy
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = "INSTALLATION: Select destination directory for installing addin";
            DialogResult result = folderBrowser.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
            {
                copyDirectory(".", folderBrowser.SelectedPath);
                try
                {
                    DeployInfo.install(folderBrowser.SelectedPath);
                    
                    MessageBox.Show("Installation sucessful!");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

        }

        static void copyDirectory(string sourcePath, string desPath)
        {
            var sourceFiles = Directory.GetFiles(sourcePath);
            string installationProjectName = getProjectName();
            foreach(var sourceFile in sourceFiles)
            {
                if(!(new FileInfo(sourceFile)).Name.Contains(installationProjectName)) //do not copy the installation files
                {
                    File.Copy(sourceFile, desPath + "\\" + (new FileInfo(sourceFile)).Name, true);
                }
            }

            var sourceDirs = Directory.GetDirectories(sourcePath);
            foreach(var sourceDir in sourceDirs)
            {
                string desDir = desPath + "\\" + (new DirectoryInfo(sourceDir)).Name;
                if (!Directory.Exists(desDir))
                {
                    Directory.CreateDirectory(desDir);
                }
                copyDirectory(sourceDir, desDir);
            }
        }

        static string getProjectName()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        }

    }
}

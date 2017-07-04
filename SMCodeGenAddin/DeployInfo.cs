using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ARAddinDeploy
{
    //TBD - Refactoring: the string constants for messaging or exception using in this file should be move to format files or define in somewhere with corresponding ID
    //and use by ID instead of hard code here!
    public class DeployInfo
    {
        public enum Result
        {
            Successful,
            Failed
        }
        public static void install(string path)
        {
            try
            {
                registerForComInterop(path);
                registerInstallationPath(path);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public static void registerInstallationPath(string path)
        {
            if(!existEAInSystem())
            {
                throw new Exception("Enterprise Architect was not installed in your system!");
            }
            try
            {
                //register addin to EA-sparxsystem
                string addInPath = getRegAddinPath();
                string addInClassName = typeof(SMCodeGenAddin.Addin).Name; //not using "AddIn" directly, because in the future, some one can rename it without any error or warning
                RegistryKey smAddinKey = createSubRegistryKey(addInPath);
                smAddinKey.SetValue(InstallationInfo.DefaultKey, string.Format("{0}.{1}", getProjectName(), addInClassName));
                smAddinKey.Close();

                //store intallation path
                RegistryKey installDirKey = createSubRegistryKey(getRegDataPath());
                installDirKey.SetValue(InstallationInfo.DataKey, path + @"\data");
                installDirKey.SetValue(InstallationInfo.DllKey, path + @"\dll");
                installDirKey.SetValue(InstallationInfo.LogPathKey, path + @"\log");

                installDirKey.Close();
            }
            catch
            {
                throw new Exception("register registry failed!");
            }
        }

        public static bool existEAInSystem()
        {
            bool ret = false;
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(RegInfo.EaSparxSystemPath);
                if(key != null)
                {
                    key.Close();
                    ret = true;
                }
            }
            catch
            {
                ret = false;
            }
            return ret;
        }
        public static string getRegAddinPath()
        {
           return string.Format(@"{0}\{1}", RegInfo.EaAddinPath, getProjectName());
        }
        public static string getProjectName()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        }
        public static string getRegDataPath()
        {
            return string.Format(@"{0}\{1}", getRegAddinPath(), "dat");
        }
        private static void registerForComInterop(string path, bool retry = false)
        {
            string regasmExe = findRegAsm();//@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\regasm.exe";
            if(!System.IO.File.Exists(regasmExe))
            {
                throw new Exception(string.Format("NOT FOUND: {0}\nMake sure you Microsoft.Net 4.5 or later was installed in your machine", regasmExe));
            }
            try
            {
                //register dll to system
                Result ret = executeCommand(regasmExe, string.Format(@"""{0}\dll\{1}.dll"" /codebase", path, getProjectName()));
                
                if (ret == Result.Failed) //assume that in the future, the message will always return successful, if not this will cause the bug
                {
                    if(!retry)
                    {
                        //unregister old version of dll
                        ret = executeCommand(regasmExe, string.Format(@"""{0}\dll\{1}.dll"" /unregister", path, getProjectName()));
                        retry = true;
                        registerForComInterop(path, retry);
                    }
                    else
                    {
                        throw new Exception(string.Format("Cannot register {0}.dll to system", getProjectName()));
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        static Result executeCommand(string exe, string arguments)
        {
            Result ret = Result.Failed;
            string commandOutput = "";
            try
            {
                var proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = exe;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.Start();
                commandOutput = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                var exitCode = proc.ExitCode;
                proc.Close();
                if(commandOutput.Contains("successful"))
                {
                    ret = Result.Successful;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return ret;
        }

        private static string findRegAsm(string path = @"C:\Windows\Microsoft.NET\Framework")
        {
            var dirs = new List<string>(System.IO.Directory.GetDirectories(path));
            dirs.Sort();   //get the latest version of regasm.exe
            dirs.Reverse();//get the latest version of regasm.exe
            foreach(var dir in dirs)
            {
                var files = System.IO.Directory.GetFiles(dir, "regasm.exe");
                if(files != null)
                {
                    foreach(var file in files)
                    {
                        if((new System.IO.FileInfo(file)).Name.ToLower() == "regasm.exe")
                        {
                            return file;
                        }
                    }
                }
            }
            return "";
        }
        public static string getInstallationPath(string key)
        {
            string path = "";
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(getRegDataPath());
                path = regKey.GetValue(key).ToString();
                regKey.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

            return path;
        }

        private static RegistryKey createSubRegistryKey(string path)
        {
            return Registry.CurrentUser.CreateSubKey(path);
        }

        public class RegInfo
        {
            private static string _eaAddinPath = @"SOFTWARE\Sparx Systems\EAAddins";
            private static string _eaSparxSystemPath = @"SOFTWARE\Sparx Systems";

            public static string EaSparxSystemPath
            {
                get { return RegInfo._eaSparxSystemPath; }
                set { RegInfo._eaSparxSystemPath = value; }
            } 
            public static string EaAddinPath
            {
                get { return RegInfo._eaAddinPath; }
                set { RegInfo._eaAddinPath = value; }
            }

        }

        public class InstallationInfo
        {
            private static string _dataKey = "data";
            private static string _dllKey = "dll";
            private static string _defaultKey = "";
            private static string _logKey = "log";
            private static string _sourceFormatFileName = "SourceFormat.dat";
            private static string _errorFormatFileName = "ErrorFormat.dat";
            private static string _eaAddinKey = "SMCodeGenAddin.Addin";

            public static string EaAddinKey
            {
                get { return InstallationInfo._eaAddinKey; }
                set { InstallationInfo._eaAddinKey = value; }
            }
            public static string ErrorFormatFileName
            {
                get { return InstallationInfo._errorFormatFileName; }
                set { InstallationInfo._errorFormatFileName = value; }
            }

            public static string SourceFormatFileName
            {
                get { return InstallationInfo._sourceFormatFileName; }
                set { InstallationInfo._sourceFormatFileName = value; }
            }
            public static string LogPathKey
            {
                get { return InstallationInfo._logKey; }
                set { InstallationInfo._logKey = value; }
            }
            public static string DefaultKey
            {
                get { return InstallationInfo._defaultKey; }
                set { InstallationInfo._defaultKey = value; }
            }

            public static string DataKey
            {
                get { return InstallationInfo._dataKey; }
                set { InstallationInfo._dataKey = value; }
            }

            public static string DllKey
            {
                get { return InstallationInfo._dllKey; }
                set { InstallationInfo._dllKey = value; }
            }

        }
    }
}

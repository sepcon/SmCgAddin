using System;
using System.IO;
using System.Collections.Generic;



namespace ARSourceGeneration
{
    public static class SourceFileWriter
    {
        public static void write(string fileName, string content)
        {
            File.WriteAllText(fileName, content, System.Text.Encoding.UTF8);
        }
    }

    public static class FileFormatReader
    {
        public static string[] read(string formatFile)
        {

            string[] formats = null;
            if (shouldReadBecauseOfFileChanged(formatFile))
            {
                if(File.Exists(formatFile))
                {
                    DateTime lastModifiedTime = System.IO.File.GetLastWriteTime(formatFile);
                    filesModifiedTime[formatFile] = lastModifiedTime;

                    string[] lines = File.ReadAllLines(formatFile);

                    formats = new string[lines.Length];

                    for (int i = 0; i < lines.Length; ++i)
                    {
                        formats[i] = lines[i].Replace("\\t", "\t").Replace("\\n", "\n").Replace("\\\"", "\"");
                    }
                }
                
            }

            return formats;
        }

        public static bool shouldReadBecauseOfFileChanged(string file)
        {
            DateTime lastModifiedTime = System.IO.File.GetLastWriteTime(file);
            return (!filesModifiedTime.ContainsKey(file)) || (filesModifiedTime[file] != lastModifiedTime);
        }

        static Dictionary<string, DateTime> filesModifiedTime = new Dictionary<string,DateTime>();
    }
}
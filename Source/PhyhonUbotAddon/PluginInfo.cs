using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PHPUbotAddons
{
    public partial class PluginInfo
    {
        public static string HashCode
        {
            get
            {
                return "3922bdf450bfffe08e15e1529b66ace4351f2470";
            }
        }
        public String ExecutePython(string compilerPath, string content, bool isFile)
        {
            try
            {
                string returnValue = string.Empty;

                if (!isFile)
                {
                    string path = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString().Replace("-", "") + ".py";
                    TextWriter writer = new StreamWriter(path);
                    writer.WriteLine(content);
                    writer.Close();
                    content = path;
                }
                //now deal as a file
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = "\"" + compilerPath + "\"";
                start.Arguments = "\"" + content + "\"";
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.CreateNoWindow = true;

                using (Process process = Process.Start(start))
                {
                    using (StreamReader errorReader = process.StandardError)
                    {
                        returnValue = errorReader.ReadToEnd();
                        if (returnValue != null && returnValue.Trim().Length > 0)
                        {
                            returnValue = string.Empty;
                        }
                    }
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        returnValue = result;
                    }
                }
                return returnValue;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

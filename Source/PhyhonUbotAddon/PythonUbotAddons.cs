using PHPUbotAddons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using UBotPlugin;

namespace PythonUbotAddons
{
    public class ExecuteFromFile : IUBotFunction
    {
        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();

        private string _returnValue = string.Empty;
        
        PluginInfo pinfo = new PluginInfo();

        public ExecuteFromFile()
        {
            this._parameters.Add(new UBotParameterDefinition("PythonFile", UBotType.String));
            this._parameters.Add(new UBotParameterDefinition("PythonCompiler", UBotType.String));
            
        }

        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {
            try
            {
                string pythonFile = Path.GetFullPath(parameters["PythonFile"]);
                string pythonCompiler = Path.GetFullPath(parameters["PythonCompiler"]);
                
                //Check for python compiler
                if (File.Exists(pythonCompiler))
                {
                    if (File.Exists(pythonFile))
                    {
                        this._returnValue = pinfo.ExecutePython(pythonCompiler, pythonFile, true);
                    }
                    else
                    {
                        this._returnValue = "Python file specified doesn't exists";
                    }
                }
                else
                {
                    this._returnValue = "Python compiler specified doesn't exists";
                }
            }
            catch (Exception exception)
            {
                this._returnValue = exception.Message;
            }
        }
        
        public void RunScript(string pythonCompiler, string pythonFile)
        {
            try
            {
                if (pythonFile.Length != 0)
                {
                    ProcessStartInfo start = new ProcessStartInfo();
                    start.FileName = "\""+ pythonCompiler + "\"";
                    start.Arguments = "\"" + pythonFile + "\"";
                    start.UseShellExecute = false;
                    start.RedirectStandardOutput = true;
                    start.RedirectStandardError = true;
                    start.CreateNoWindow = true;
                    
                    using (Process process = Process.Start(start))
                    {
                        using (StreamReader errorReader = process.StandardError) 
                        {
                            this._returnValue = errorReader.ReadToEnd();
                            if (this._returnValue != null && this._returnValue.Trim().Length > 0)
                                return;
                        }
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string result = reader.ReadToEnd();
                            this._returnValue = result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._returnValue = ex.Message;
            }
        }

        
        public string Category
        {
            get
            {
                return "Python Functions";
            }
        }

        public string FunctionName
        {
            get
            {
                return "Execute From File";
            }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get
            {
                return this._parameters;
            }
        }

        public object ReturnValue
        {
            get
            {
                return this._returnValue;
            }
        }

        public UBotType ReturnValueType
        {
            get
            {
                return UBotType.String;
            }
        }

        public UBotPlugin.UBotVersion UBotVersion
        {
            get
            {
                return UBotPlugin.UBotVersion.Standard;
            }
        }
    }

    public class ExecuteFromCode : IUBotFunction
    {
        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();

        private string _returnValue = string.Empty;

        PluginInfo pinfo = new PluginInfo();

        public ExecuteFromCode()
        {
            this._parameters.Add(new UBotParameterDefinition("PythonCode", UBotType.String));
            this._parameters.Add(new UBotParameterDefinition("PythonCompiler", UBotType.String));
        }

        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {
            try
            {
                string pythonCode = parameters["PythonCode"];
                string pythonCompiler = Path.GetFullPath(parameters["PythonCompiler"]);

                //Check for python compiler
                if (File.Exists(pythonCompiler))
                {
                    this._returnValue = pinfo.ExecutePython(pythonCompiler, pythonCode, false);
                }
                else
                {
                    this._returnValue = "Python compiler specified doesn't exists";
                }
            }
            catch (Exception ex)
            {
                this._returnValue = ex.Message;
            }
        }
        
        public void RunScript(string pythonCompiler, string pythonCode)
        {
            //Create a python file for the code
            string filePath = System.IO.Path.GetTempPath() + @"\" + Guid.NewGuid().ToString().Replace("-", "").Trim() + ".py";

            try
            {
                StreamWriter writer = File.CreateText(filePath);
                writer.WriteLine("<?php");
                writer.WriteLine(pythonCode);
                writer.WriteLine("?>");
                writer.Flush();
                writer.Close();
                if (pythonCode.Length != 0)
                {
                   
                    ProcessStartInfo start = new ProcessStartInfo();
                    start.FileName = "\"" + pythonCompiler + "\"";
                    start.Arguments = "\"" + filePath +"\"";
                    start.UseShellExecute = false;
                    start.RedirectStandardOutput = true;
                    start.RedirectStandardError = true;
                    start.CreateNoWindow = true;
                    using (Process process = Process.Start(start))
                    {
                        using (StreamReader errorReader = process.StandardError)
                        {
                            this._returnValue = errorReader.ReadToEnd();
                            if (this._returnValue != null && this._returnValue.Trim().Length > 0)
                                return;
                        }

                        using (StreamReader reader = process.StandardOutput)
                        {
                            string result = reader.ReadToEnd();
                            this._returnValue = result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._returnValue = ex.Message;
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        public string Category
        {
            get
            {
                return "Python Functions";
            }
        }

        public string FunctionName
        {
            get
            {
                return "Execute From Code";
            }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get
            {
                return this._parameters;
            }
        }

        public object ReturnValue
        {
            get
            {
                return this._returnValue;
            }
        }

        public UBotType ReturnValueType
        {
            get
            {
                return UBotType.String;
            }
        }

        public UBotPlugin.UBotVersion UBotVersion
        {
            get
            {
                return UBotPlugin.UBotVersion.Standard;
            }
        }
    }
}

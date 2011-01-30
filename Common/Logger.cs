﻿/* Copyright 2011 the OpenDMS.NET Project (http://sites.google.com/site/opendmsnet/)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;

namespace Common
{
    public class Logger
    {
        public enum LevelEnum
        {
            Normal = 0,
            Debug
        }

        private StreamWriter _writer;
        private bool _isOpen;

        //public static Logger Instance = new Logger();

        public Logger(string logName)
        {
            try
            {
                _writer = new StreamWriter(new FileStream(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + logName,
                    FileMode.Append, FileAccess.Write, FileShare.Read));
                _isOpen = true;
            }
            catch (Exception)
            {
            }

            Write(LevelEnum.Debug, "Logging started.");
        }

        public Logger(string logPath, string logName)
        {
            try
            {
                _writer = new StreamWriter(new FileStream(logPath + logName, FileMode.Append, FileAccess.Write, FileShare.Read));
                _isOpen = true;
            }
            catch (Exception)
            {
            }

            Write(LevelEnum.Debug, "Logging started.");
        }

        ~Logger()
        {
            if (_writer != null && _writer.BaseStream != null 
                && _writer.BaseStream.CanWrite)
                Close();
        }

        public void Write(LevelEnum level, string text)
        {
            if (_writer == null) return;

            lock (_writer)
            {
                _writer.WriteLine("------------------------------------------------------------------------------------------------------------------");
                _writer.WriteLine("Level: " + level.ToString());
                _writer.WriteLine("Timestamp: " + DateTime.Now.ToString());
                _writer.WriteLine("Message: " + text);
                _writer.Flush();
            }
        }

        public void Close()
        {
            if (_writer != null && _isOpen)
            {
                Write(LevelEnum.Debug, "Logging stopped.");
                _writer.Close();
                _writer.Dispose();
            }
            _isOpen = false;
        }

        public static string ExceptionToString(Exception ex)
        {
            System.Diagnostics.StackTrace stackTrace;
            string fileNames;
            int lineNumber;
            System.Reflection.MethodBase methodBase;
            string methodName;
            string output = "";
            string indent = "\t";
            Exception ie = ex;

            stackTrace = new System.Diagnostics.StackTrace(ex, true);
            fileNames = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetFileName();
            lineNumber = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetFileLineNumber();
            methodBase = stackTrace.GetFrame((stackTrace.FrameCount - 1)).GetMethod();
            methodName = methodBase.Name;

            output = "****** Exception Logging ******\r\n";
            output = "Filename: " + fileNames + "\r\n" +
                        "Line number: " + lineNumber.ToString() + "\r\n" +
                        "Method name: " + methodName + "\r\n" +
                        "ExceptionType: " + ex.GetType().Name + "\r\n" +
                        "HelpLink: " + ex.HelpLink + "\r\n" +
                        "Message: " + ex.Message + "\r\n" +
                        "Source: " + ex.Source + "\r\n" +
                        "TargetSite: " + ex.TargetSite + "\r\n" +
                        "StackTrace: " + ex.StackTrace;

            while (!((ie.InnerException == null)))
            {
                ie = ie.InnerException;
                output =    indent + "****** Exception Logging ******\r\n" +
                            indent + "Filename: " + fileNames + "\r\n" +
                            indent + "Line number: " + lineNumber.ToString() + "\r\n" +
                            indent + "Method name: " + methodName + "\r\n" +
                            indent + "ExceptionType: " + ex.GetType().Name + "\r\n" +
                            indent + "HelpLink: " + ex.HelpLink + "\r\n" +
                            indent + "Message: " + ex.Message + "\r\n" +
                            indent + "Source: " + ex.Source + "\r\n" +
                            indent + "TargetSite: " + ex.TargetSite + "\r\n" +
                            indent + "StackTrace: " + ex.StackTrace;
                indent += "\t";
            }

            return output;
        }
    }
}

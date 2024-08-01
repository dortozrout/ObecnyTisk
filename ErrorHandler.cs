using System;
using System.Diagnostics;
using System.IO;
using Form;

namespace TiskStitku
{
    static class ErrorHandler
    {
        public static event EventHandler<ErrorEventArgs> Error;
        public static void HandleError(object sourceOfEx, Exception exception)
        {
            var stackTrace = new StackTrace();
            var stackFrame = stackTrace.GetFrame(1); // Get the caller frame
            var methodBase = stackFrame.GetMethod();
            var className = methodBase.DeclaringType.Name;
            var methodName = methodBase.Name;

            //string source = $"{className}.{methodName}"; //nefunguje v .Net Framework
            string source = string.Format("{0}.{1}", className, methodName);

            ErrorForm errorForm = new ErrorForm();
            string textLog = String.Format("{0}|Vyjímka: {1}|Zdroj: {2}", DateTime.Now, exception.Message, source);
            ErrorEventArgs eventArgs = new ErrorEventArgs { LogText = textLog, Source = source, Message = exception.Message };

            Error = null;

            //if (className != "Configuration" && className != "Log")
            //  Error += Log.Zapis;
            Error += errorForm.Display;

            OnError(eventArgs);
            if (exception is FileNotFoundException)
            {
                //continue
            }
            else if(exception is DirectoryNotFoundException)
            {
                Spravce spravce = new Spravce();
                spravce.EditujKonfSoubor(true);
                spravce.Restart();
            }
            else if(exception is ArgumentException && sourceOfEx.GetType() == typeof(Spravce))
            {
                //continue
            }
            else Environment.Exit(1);
        }
        private static void OnError(ErrorEventArgs eventArgs)
        {
            Error?.Invoke(null, eventArgs);
        }
    }
}


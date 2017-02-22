using System;
using System.IO;

namespace ProductPublisher
{
    internal class Logger
    {
        private readonly string _name;

        public Logger(string name)
        {
            _name = $"{name}.txt";
        }

        public void Error(string error)
        {
            Decorate(ref error);
            Append($"Error: {error}");
            Console.Error.WriteLine(error);
        }

        public void Information(string message)
        {
            Decorate(ref message);
            Append(message);
            Console.WriteLine(message);
        }

        private static void Decorate(ref string message)
        {
            message = $"{DateTime.Now}: {message}";
        }

        private void Append(string text)
        {
            using (var writer = File.AppendText(_name))
            {
                writer.WriteLine(text);
            }
        }
    }
}
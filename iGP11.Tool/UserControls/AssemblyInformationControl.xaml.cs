using System;
using System.Collections.Generic;
using System.Reflection;

using iGP11.Library;

namespace iGP11.Tool.UserControls
{
    public partial class AssemblyInformationControl
    {
        public AssemblyInformationControl()
        {
            InitializeComponent();

            var assembly = Assembly.GetEntryAssembly().GetAssemblyInformation();
            Copyright = string.Join(Environment.NewLine, GetCompany(assembly));
            Product = assembly.Product;
            Version = assembly.DisplayVersion;
        }

        public string Copyright { get; }

        public string Product { get; }

        public string Version { get; }

        private static IEnumerable<string> GetCompany(AssemblyInformation information)
        {
            var collection = information.Copyright.Split(new[] { information.Company }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < collection.Length; i++)
            {
                var part = collection[i].Trim();
                yield return i < collection.Length - 1
                                 ? $"{part} {information.Company}"
                                 : part;
            }
        }
    }
}
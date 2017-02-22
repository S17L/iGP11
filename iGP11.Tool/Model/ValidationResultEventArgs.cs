using System;
using System.Collections.Generic;
using System.Linq;

namespace iGP11.Tool.Model
{
    public class ValidationResultEventArgs : EventArgs
    {
        private readonly IEnumerable<string> _errors;

        public ValidationResultEventArgs(IEnumerable<string> errors)
        {
            _errors = errors;
        }

        public IEnumerable<string> Errors => _errors.ToArray();
    }
}
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace iGP11.Library
{
    public class SecuredText : ISecuredText
    {
        private readonly SecureString _secureString;

        public SecuredText(string text)
        {
            _secureString = new SecureString();
            if (!text.IsNullOrEmpty())
            {
                foreach (var character in text.ToCharArray())
                {
                    _secureString.AppendChar(character);
                }
            }

            _secureString.MakeReadOnly();
        }

        public SecuredText(ref string text)
            : this(text)
        {
            text = string.Empty;
        }

        public SecuredText(SecureString text)
        {
            _secureString = text;
        }

        public string GetUnsecuredText()
        {
            var pointer = IntPtr.Zero;
            try
            {
                pointer = Marshal.SecureStringToGlobalAllocUnicode(_secureString);
                return Marshal.PtrToStringUni(pointer);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(pointer);
            }
        }

        public bool IsEmpty()
        {
            return GetUnsecuredText().IsNullOrEmpty();
        }

        public bool IsEqual(ISecuredText securedText)
        {
            return (securedText != null) && (GetUnsecuredText() == securedText.GetUnsecuredText());
        }
    }
}
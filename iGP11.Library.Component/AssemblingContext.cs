using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Library.Component
{
    public class AssemblingContext
    {
        public AssemblingContext(FormType type, ITokenReplacer tokenReplacer = null)
        {
            FormType = type;
            TokenReplacer = tokenReplacer;
        }

        public FormType FormType { get; }

        public ITokenReplacer TokenReplacer { get; }
    }
}
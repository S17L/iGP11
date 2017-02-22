namespace iGP11.Library.Component
{
    public class ConcreteComponentAssembler
    {
        private readonly ComponentAssembler _assembler;
        private readonly AssemblingContext _assemblingContext;

        public ConcreteComponentAssembler(ComponentAssembler assembler, AssemblingContext assemblingContext)
        {
            _assembler = assembler;
            _assemblingContext = assemblingContext;
        }

        public Component<TEntry> Assemble<TEntry>(TEntry item)
        {
            return _assembler.Assemble(item, _assemblingContext);
        }
    }
}
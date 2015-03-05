namespace SignalCompiler
{
    public class CompilerError
    {
        public string Message { get; set; }
        public int Position { get; set; }
        public int Line { get; set; }
    }
}

namespace SignalCompiler.Models
{
    public class Lexem
    {
        public Position Position { get; set; }
        public int Id { get; set; }


        public override string ToString()
        {
            return string.Format("{0}", Constants.GetLexem(Id));
        }
    }
}

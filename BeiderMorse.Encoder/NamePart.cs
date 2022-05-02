namespace BeiderMorse.Encoder
{
    public class NamePart
    {
        public NamePart(int index, string part)
        {
            Index = index;
            Part = part;
        }
        public int Index { get; }
        public string Part { get; }
    }
}
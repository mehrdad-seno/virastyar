namespace VirastyarWordAddin.Verifiers.Basics
{
    /// <summary>
    /// To be used as item in a list-box. Enables the user to put
    /// strings containing formula and picture control characters
    /// in a list box, while it is displayed differently.
    /// </summary>
    internal class TextValuePair
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public TextValuePair(string text, object value)
        {
            this.Text = text;
            this.Value = value;
        }

        public TextValuePair(string text)
            : this(text, text)
        {
        }

        public TextValuePair()
            : this("")
        {
        }

        public override string ToString()
        {
            return this.Text;
        }
    }
}

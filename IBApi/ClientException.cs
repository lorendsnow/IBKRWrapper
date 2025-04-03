namespace IBApi
{
    public class ClientException : Exception
    {
        public CodeMsgPair Err { get; init; }
        public string? Text { get; init; }

        public ClientException(CodeMsgPair err)
        {
            Err = err;
        }

        public ClientException(CodeMsgPair err, string text)
        {
            Err = err;
            Text = text;
        }
    }
}

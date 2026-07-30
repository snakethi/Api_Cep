namespace ApiCep.Application.Common.Exceptions
{
    public sealed class ConflictException : System.Exception
    {
        public ConflictException(string message) : base(message)
        {
        }
    }
}

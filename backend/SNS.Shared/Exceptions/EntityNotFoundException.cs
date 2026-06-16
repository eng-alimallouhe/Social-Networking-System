namespace SNS.Shared.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string massage) : base(massage) { }
    }
}

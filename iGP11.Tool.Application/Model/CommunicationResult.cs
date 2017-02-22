namespace iGP11.Tool.Application.Model
{
    public class CommunicationResult<TResult>
    {
        public CommunicationResult(bool isCompleted, TResult response)
        {
            IsCompleted = isCompleted;
            Response = response;
        }

        public bool IsCompleted { get; }

        public TResult Response { get; }
    }
}
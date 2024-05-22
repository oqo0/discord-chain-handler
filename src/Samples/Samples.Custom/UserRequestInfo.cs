namespace Samples.Custom;

internal class UserRequestInfo
{
    public DateTime FirstRequestDateTime { get; private set; }
    public int Count { get; private set; }

    public UserRequestInfo()
    {
        FirstRequestDateTime = DateTime.Now;
        Count++;
    }

    public bool IsRequestAvailable()
    {
        if (DateTime.Now - FirstRequestDateTime > TimeSpan.FromMinutes(1))
        {
            FirstRequestDateTime = DateTime.Now;
            Count = 0;
            return true;
        }

        Count++;
        
        return Count < 3;
    }
}
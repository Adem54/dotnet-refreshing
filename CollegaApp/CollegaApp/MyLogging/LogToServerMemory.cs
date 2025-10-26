namespace CollegaApp.MyLogging
{
    public class LogToServerMemory : IMyLogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"Log to ServerMemory: {message}");
            //write your logic here
        }
    }
}

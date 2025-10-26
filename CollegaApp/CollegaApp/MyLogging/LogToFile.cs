namespace CollegaApp.MyLogging
{
    public class LogToFile : IMyLogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"Logg to File: {message}");
            //write your logic here
        }
    }
}

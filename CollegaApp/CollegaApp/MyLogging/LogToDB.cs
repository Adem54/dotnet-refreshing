namespace CollegaApp.MyLogging
{
    public class LogToDB : IMyLogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"Log to DB : {message}");
            //write your logic here
        }
    }
}

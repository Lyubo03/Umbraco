namespace HighlyDeveloped.Core.ViewModels
{
    /// <summary>
    /// This will hold the twitter tweet data for rendering 
    /// the latest tweets
    /// </summary>
    public class TwitterViewModel
    {
        public string TwitterHandle { get; set; }
        public bool Error { get; set; }
        // Twitter API return data in a Json format
        public string Json { get; set; }

        //In case of an error
        public string Message { get; set; }
    }
}
namespace ArticleClientWebsite
{
    public class Article
    {
        public string id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string publisher { get; set; }
        public string publishDate { get; set; }
        public string category { get; set; }

        public string toString()
        {
            return "Title: " + title + "\nContent: " + content + "\nPublisher: " + publisher + "Publish Date: " + publishDate + "Category: " + category;
        }
    }
}

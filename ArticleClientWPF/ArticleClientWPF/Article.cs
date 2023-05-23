using System;

namespace ArticleClientWPF
{
    internal class Article
    {
        public Article(string title, string content, string publisher, string category)
        {
            this.title = title;            
            this.content = content;
            this.publisher = publisher;
            this.publishDate = DateTime.Now.ToString("dd\\.MM\\.yyyy HH\\:mm");
            this.category = category;

        }
        public string id { get; set; }
        public string title { get; set; }
        public string content { get; set; } 
        public string publisher { get; set; }
        public string publishDate { get; set; }        
        public string category { get; set; }

    }
}

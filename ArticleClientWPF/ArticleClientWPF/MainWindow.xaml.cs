using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ArticleClientWPF
{
    public partial class MainWindow : Window
    {
        //currentArticle wird verwendet, um zu wissen welchen Artikel man gerade vor sich hat / anschaut
        //Wert wird in displayArticleFromNavbar() und displayThisArticleFromNavbar() zugewiesen
        Article currentArticle = null;
        public string currentPublisher = "Anonymous";

        List<Article> articles = new List<Article>();
        List<string> comboboxitems = new List<string>();

        string baseApiURL = "http://localhost:3002/articles";
        public MainWindow()
        {
            InitializeComponent();
            displayAllArticlesInNavbar();
            getComboBoxItems();

            //Um gleich am Anfang den ersten Artikel anzuzeigen
            if (articles.Count != 0)
            {
                displayThisArticleFromNavbar(articles[0]);
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();

            Article a = new Article("Bitte Titel eingeben", "Hier ist Platz für Inhalt...", currentPublisher, "Ohne Kategorie");


            string json = JsonConvert.SerializeObject(a);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = client.PostAsync(baseApiURL, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    deleteButton.Visibility = Visibility.Hidden;
                    displayAllArticlesInNavbar();
                    changeButton_Click(sender, e);
                    getArticlesFromAPI();
                    displayThisArticleFromNavbar(articles[articles.Count - 1]);
                }
                else
                {
                    Console.WriteLine("POST FEHLER");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var Result = MessageBox.Show("Wollen Sie wirklich unwiderruflich löschen?", "Artikel löschen", MessageBoxButton.YesNo);

            if (Result == MessageBoxResult.Yes)
            {
                try
                {
                    HttpClient client = new();
                    int index = articles.IndexOf(currentArticle);
                    string URL = baseApiURL + "/" + currentArticle.id;

                    HttpResponseMessage message = await client.DeleteAsync(URL);

                    if (message.IsSuccessStatusCode)
                    {
                        displayAllArticlesInNavbar();
                        if (index > 0)
                        {
                            displayThisArticleFromNavbar(articles[index - 1]);
                        }
                        else
                        {
                            displayThisArticleFromNavbar(articles[0]);
                        }
                    }
                    else
                    {
                        Console.WriteLine("FEHLER DELETE");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void changeButton_Click(object sender, RoutedEventArgs e)
        {
            cancelButton.Visibility = Visibility.Visible;

            foreach (Button b in sidebarStackPanel.Children)
            {
                b.IsEnabled = false;
            }

            titleTextBox.IsReadOnly = false;
            titleTextBox.BorderThickness = new Thickness(1);

            contentTextBox.IsReadOnly = false;
            contentTextBox.BorderThickness = new Thickness(1);

            publisherTextBox.Text = currentPublisher;

            categoryComboBox.IsEnabled = true;

            Image i = new Image();
            i.Source = new BitmapImage(new Uri("Pictures/check.png", UriKind.RelativeOrAbsolute));
            i.Stretch = Stretch.Uniform;
            changeButton.Content = i;

            //-= um zu verhindern, dass zu viele Events auf einem Button sind
            //(weil er ev. schon ein Mal benutzt wurde und ein Event hinzugefügt wurde)
            changeButton.Click -= changeButton_Click;
            changeButton.Click += confirmChangeButton_Click;
        }

        private async void confirmChangeButton_Click(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            string URL = "";

            //aold ist der ursprüngliche, zu ändernde Artikel und wird hier deklariert, um die ID des ursprünglichen
            //auf den neuen Artikel zu übergeben
            Article aold = currentArticle;
            Article anew = new Article(titleTextBox.Text, contentTextBox.Text, currentPublisher, categoryComboBox.Text);

            if (aold != anew)
            {
                anew.id = aold.id;
                URL = baseApiURL + "/" + anew.id;

                string json = JsonConvert.SerializeObject(anew);
                HttpContent content = new StringContent(json);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                try
                {
                    HttpResponseMessage response = (await client.PutAsync(URL, content));

                    if (response.IsSuccessStatusCode)
                    {
                        titleTextBox.IsReadOnly = true;
                        titleTextBox.BorderThickness = new Thickness(0);

                        contentTextBox.IsReadOnly = true;
                        contentTextBox.BorderThickness = new Thickness(0);

                        categoryComboBox.IsEnabled = false;

                        Image i = new Image();
                        i.Source = new BitmapImage(new Uri("Pictures/edit.png", UriKind.RelativeOrAbsolute));
                        i.Stretch = Stretch.Uniform;
                        changeButton.Content = i;

                        //-= um zu verhindern, dass zu viele Events auf einem Button sind
                        //(weil er ev. schon ein Mal benutzt wurde und ein Event hinzugefügt wurde)
                        changeButton.Click -= confirmChangeButton_Click;
                        changeButton.Click += changeButton_Click;

                        displayAllArticlesInNavbar();
                        displayThisArticleFromNavbar(anew);

                        //um den deletebutton wieder zu aktivieren, der in addButton_Click deaktiviert wurde
                        deleteButton.Visibility = Visibility.Visible;

                        //um die Buttons, die die Artikel anzeigen wieder zu aktivieren, wurden
                        //in changeButton_Click deaktiviert
                        foreach (Button b in sidebarStackPanel.Children)
                        {
                            b.IsEnabled = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("FEHLER PUT\nStatus: " + response.IsSuccessStatusCode);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void displayAllArticlesInNavbar()
        {
            sidebarStackPanel.Children.Clear();

            getArticlesFromAPI();

            if (articles.Count != 0)
            {
                foreach (Article a in articles)
                {
                    Button button = new Button();
                    button.Name = "_" + a.id;
                    button.Click += new RoutedEventHandler(displayArticleFromNavbar);
                    button.BorderBrush = Brushes.Black;
                    button.BorderThickness = new Thickness(0, 0, 0, 0.3);
                    button.Background = Brushes.DarkGray;
                    TextBlock t = new();
                    t.Text = a.title;
                    t.TextWrapping = TextWrapping.Wrap;
                    t.TextAlignment = TextAlignment.Center;
                    button.Content = t;

                    sidebarStackPanel.Children.Add(button);
                }
            }
            else
            {
                Button button = new Button();
                button.Click += new RoutedEventHandler(refreshNavbar_Click);
                button.BorderBrush = Brushes.Black;
                button.BorderThickness = new Thickness(0, 0, 0, 0.3);
                button.Background = Brushes.DarkGray;
                button.Height = 25;
                Image i = new Image();
                i.Source = new BitmapImage(new Uri("Pictures/refresh.png", UriKind.RelativeOrAbsolute));
                i.Stretch = Stretch.Uniform;
                button.Content = i;

                TextBox t = new TextBox();
                t.Text = "Liste ist leer";
                t.Background = Brushes.DarkGray;
                sidebarStackPanel.Children.Add(t);

                sidebarStackPanel.Children.Add(button);
            }
        }

        private void displayArticleFromNavbar(object? sender, RoutedEventArgs e)
        {
            Button src = e.Source as Button;

            string[] id = src.Name.Split("_");

            Article a = getArticleWithId(id[1]);

            if (a != null)
            {
                //Um eine Variable zu haben, die anzeigt, welchen Articel man gerade sieht
                currentArticle = a;

                titleTextBox.Text = a.title;
                contentTextBox.Text = a.content;
                publisherTextBox.Text = a.publisher;
                publishDateTextBox.Text = a.publishDate;
                categoryComboBox.Text = a.category;
            }

        }

        private void getArticlesFromAPI()
        {
            HttpClient client = new HttpClient();

            try
            {
                string response = client.GetStringAsync(baseApiURL).Result;
                articles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Article>>(response);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Verbindung zur API konnte nicht hergestellt werden.", "Verbindungsfehler");
            }
        }

        /// <summary>
        /// Get-Method which gives you an Article with the specified Id
        /// </summary>
        /// <param>Id in string</param>
        /// <returns>Article</returns>
        private Article getArticleWithId(string id)
        {
            foreach (Article a in articles)
            {
                if (a.id == id)
                {
                    return a;
                }
            }

            return null;
        }

        private void displayThisArticleFromNavbar(Article a)
        {
            if (a != null)
            {
                //Um eine Variable zu haben, die anzeigt, welchen Articel man gerade sieht
                currentArticle = a;

                titleTextBox.Text = a.title;
                contentTextBox.Text = a.content;
                publisherTextBox.Text = a.publisher;
                publishDateTextBox.Text = a.publishDate;
                categoryComboBox.Text = a.category;
            }
            else
            {
                Console.WriteLine("Artikel nicht gefunden!");
            }
        }

        private void addKategorieButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(categoryTextBox.Text))
            {
                if (!comboboxitems.Contains(categoryTextBox.Text))
                {
                    comboboxitems.Add(categoryTextBox.Text);
                    writeComboBoxItems();
                    getComboBoxItems();
                    categoryTextBox.Clear();
                }
                else
                {
                    categoryTextBox.Text = "Schon vorhanden";
                }
            }
        }

        private void getComboBoxItems()
        {
            comboboxitems = null;
            categoryComboBox.Items.Clear();
            string content = File.ReadAllText("categories.txt");
            comboboxitems = JsonConvert.DeserializeObject<List<string>>(content);
            foreach (string s in comboboxitems)
            {
                categoryComboBox.Items.Add(s);
            }
        }

        private void writeComboBoxItems()
        {
            string jsonstring = JsonConvert.SerializeObject(comboboxitems);
            File.WriteAllText("categories.txt", jsonstring);
        }

        private void addPublisherButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(setPublisherTextBox.Text))
            {
                currentPublisher = setPublisherTextBox.Text;
                addPublisherButton.Visibility = Visibility.Hidden;
            }
            else
            {
                currentPublisher = "Anonymous";
            }
        }

        private void setPublisherTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            addPublisherButton.Visibility = Visibility.Visible;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            titleTextBox.IsReadOnly = true;
            titleTextBox.BorderThickness = new Thickness(0);

            contentTextBox.IsReadOnly = true;
            contentTextBox.BorderThickness = new Thickness(0);

            categoryComboBox.IsEnabled = false;

            displayThisArticleFromNavbar(currentArticle);

            Image i = new Image();
            i.Source = new BitmapImage(new Uri("Pictures/edit.png", UriKind.RelativeOrAbsolute));
            i.Stretch = Stretch.Uniform;
            changeButton.Content = i;

            //-= um zu verhindern, dass zu viele Events auf einem Button sind
            //(weil er ev. schon ein Mal benutzt wurde und ein Event hinzugefügt wurde)
            changeButton.Click -= confirmChangeButton_Click;
            changeButton.Click += changeButton_Click;

            displayAllArticlesInNavbar();
            displayThisArticleFromNavbar(currentArticle);

            cancelButton.Visibility = Visibility.Hidden;

            //um den deletebutton wieder zu aktivieren, der in addButton_Click deaktiviert wurde
            deleteButton.Visibility = Visibility.Visible;

            //um die Buttons, die die Artikel anzeigen wieder zu aktivieren, wurden
            //in changeButton_Click deaktiviert
            foreach (Button b in sidebarStackPanel.Children)
            {
                b.IsEnabled = true;
            }
        }

        private void refreshNavbar_Click(object sender, RoutedEventArgs e)
        {
            displayAllArticlesInNavbar();
            if(articles.Count != 0)
            {
                displayThisArticleFromNavbar(articles[0]);
            }
        }
    }
}
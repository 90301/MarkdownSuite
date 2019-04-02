using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MarkdownSuite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public List<FileSelection> Directory { get; set; } = new List<FileSelection>();

        public FileSelection EditingSelected { get; set; }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            
            //Loads the directory

            String dir = AppDomain.CurrentDomain.BaseDirectory;

            //TODO dialog box

            Console.WriteLine(dir);

            string[] filePaths = System.IO.Directory.GetFiles(dir, "*.md",
                                         SearchOption.AllDirectories);

            foreach (String filePath in filePaths)
            {
                Directory.Add(new FileSelection(filePath));

                Console.WriteLine(filePath);
            }

            fileSelection.ItemsSource = Directory;
        }

        private void EditSelected(Object sender, RoutedEventArgs e)
        {
            EditingSelected = Directory.Find(x => x.FileName.Equals(((Button)sender).Tag));
            previewTextbox.DataContext = EditingSelected;
            //previewTextbox.Text = EditingSelected.Content;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }


    public class FileSelection
    {

        public String Dir { get; set; }

        public String FileName { get; set; }

        public Boolean Selected { get; set; }

        public String Content { get; set; }

        public FileSelection(string filePath)
        {
            this.Dir = filePath;
            this.FileName = System.IO.Path.GetFileName(filePath);

            this.Content = File.ReadAllText(this.Dir);
        }

    }
}

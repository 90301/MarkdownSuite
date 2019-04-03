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

        public GeneratedDocument GenDoc { get; set; } = new GeneratedDocument();

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
            editorGrid.DataContext = EditingSelected;

            processedGrid.DataContext = this;
            //previewTextbox.DataContext = EditingSelected;
            //previewTextbox.Text = EditingSelected.Content;
        }

        private void DownBtn_Click(object sender, RoutedEventArgs e)
        {
            int index = Directory.FindIndex(x => x.Equals(EditingSelected));

            if (index < Directory.Count -1 && index != -1)
            {
                FileSelection tmp = Directory[index + 1];
                Directory[index + 1] = EditingSelected;
                Directory[index] = tmp;
            }

        }

        private void UpBtn_Click(object sender, RoutedEventArgs e)
        {
            int index = Directory.FindIndex(x => x.Equals(EditingSelected));

            if (index > 0)
            {
                FileSelection tmp = Directory[index - 1];
                Directory[index - 1] = EditingSelected;
                Directory[index] = tmp;
            }
            
        }

        private void ProcessBtn_Click(object sender, RoutedEventArgs e)
        {
            processedGrid.DataContext = this.GenDoc;

            titleEditor.DataContext = this.GenDoc;
            previewProcessedTextbox.DataContext = this.GenDoc;



            IEnumerable<FileSelection> selected = Directory.Where(x => x.Selected);
            if (selected.Count() >= 1)
            {

                GenDoc.FinalDoc = selected.Select(x => x.Content).Aggregate((x, y) => x + Environment.NewLine + Environment.NewLine + y);

                Console.WriteLine(GenDoc.FinalDoc);

                previewProcessedTextbox.Text = GenDoc.FinalDoc;
            }
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

    public class GeneratedDocument
    {
        public String FinalDoc { get; set; } = "";

        public String DocName { get; set; } = "";
    }
}

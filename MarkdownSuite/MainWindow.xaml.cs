using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

            

            processedGrid.DataContext = GenDoc;
        }

        public ObservableCollection<FileSelection> Directory { get; set; } = new ObservableCollection<FileSelection>();

        public FileSelection EditingSelected { get; set; }

        public GeneratedDocument GenDoc { get; set; } = new GeneratedDocument();

        public String Dir { get; set; }

        private void LoadDirectory_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (openFileDialog.ShowDialog() == true)
            {
                Dir = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
            } else
            {
                //Dir = AppDomain.CurrentDomain.BaseDirectory;
                return;
            }
                

            //Loads the directory

            

            //TODO dialog box

            Console.WriteLine(Dir);

            string[] filePaths = System.IO.Directory.GetFiles(Dir, "*.md",
                                         SearchOption.AllDirectories);

            foreach (String filePath in filePaths)
            {
                if (Directory.ToList().Find(x => x.Dir.Equals(filePath)) == null) {

                    Directory.Add(new FileSelection(filePath));

                    Console.WriteLine(filePath);
                }
            }

            //fileSelection.ItemsSource = Directory;

            fileSelection.DataContext = this;
        }

        private void EditSelected(Object sender, RoutedEventArgs e)
        {
            EditingSelected = Directory.ToList().Find(x => x.FileName.Equals(((Button)sender).Tag));
            editorGrid.DataContext = EditingSelected;

            processedGrid.DataContext = this;
            //previewTextbox.DataContext = EditingSelected;
            //previewTextbox.Text = EditingSelected.Content;
        }

        private void DownBtn_Click(object sender, RoutedEventArgs e)
        {
            int index = Directory.ToList().FindIndex(x => x.Equals(EditingSelected));

            if (index < Directory.Count -1 && index != -1)
            {
                FileSelection tmp = Directory[index + 1];
                Directory[index + 1] = EditingSelected;
                Directory[index] = tmp;
            }

        }

        private void UpBtn_Click(object sender, RoutedEventArgs e)
        {
            int index = Directory.ToList().FindIndex(x => x.Equals(EditingSelected));

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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            GenDoc.FileLocation = System.IO.Path.Combine(Dir , GenDoc.DocName + ".md");

            System.IO.File.WriteAllText(GenDoc.FileLocation, GenDoc.FinalDoc);



            // create pdf

            string processName = "pandoc.exe";
            string arguments = GenDoc.FileLocation + " -o " + System.IO.Path.Combine(Dir , GenDoc.DocName + ".pdf");
            
            var psi = new ProcessStartInfo
            {
                FileName = processName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };

            var process = new Process { StartInfo = psi };

            Console.WriteLine(processName + " " + arguments);

            process.Start();
            process.WaitForExit();
            
            
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            Directory.ToList().ForEach(x => x.Refresh());
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

        internal void Refresh()
        {
            this.Content = File.ReadAllText(this.Dir);
        }
    }

    public class GeneratedDocument
    {
        public String FinalDoc { get; set; } = "preview";

        public String DocName { get; set; } = "Doc_" + System.DateTime.Now.ToFileTime();

        public String FileLocation { get; set; }
    }
}

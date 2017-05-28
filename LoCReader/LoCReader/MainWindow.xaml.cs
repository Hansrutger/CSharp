using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.IO;
using System.Windows.Forms;

namespace LoCReader
{
    public partial class MainWindow : Window
    {
        private int counter = 0;

        public MainWindow()
        {
            InitializeComponent();

            this.Title = "Line of Code Reader";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            resLb.Content = "";
            statusLb.Content = "";
            directoryTxt.Text = ""; // set default path here if you wish to have one

            extensionTxt.Focus();
            extensionTxt.CaretIndex = extensionTxt.Text.Length;
        }

        private void browseBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = directoryTxt.Text;

            DialogResult res = fbd.ShowDialog();

            if (res.ToString() == "OK")
            {
                directoryTxt.Text = fbd.SelectedPath;
            }
        }

        private async void runBtn_Click(object sender, RoutedEventArgs e)
        {
            string maindir = directoryTxt.Text;

            statusLb.Content = "Processing...";

            counter = 0;
            await Task.Factory.StartNew(() =>
            {
                CountInDirectory(maindir);
                Directories(maindir);
            });

            if (counter == 1)
            {
                statusLb.Content = "Found " + counter + " line of code!";
            }
            else
            {
                statusLb.Content = "Found " + counter + " lines of code!";
            }
        }

        private void Directories(string currentDirectory)
        {
            string[] subdirectories = Directory.GetDirectories(currentDirectory);
            if (subdirectories.Length != 0)
            {
                foreach (string subdir in subdirectories)
                {
                    CountInDirectory(subdir);
                    Directories(subdir);
                }
            }
        }

        private void CountInDirectory(string directory)
        {
            string[] filesInDir = Directory.GetFiles(directory);
            foreach (string filename in filesInDir)
            {
                bool found = false;
                foreach (string listboxtext in extensionListBox.Items)
                {
                    if (filename.EndsWith(listboxtext))
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    string path = System.IO.Path.Combine(directory, filename);
                    System.Diagnostics.Debug.WriteLine("Reading... " + filename);
                    using (StreamReader file = new StreamReader(path))
                    {
                        string line;
                        while ((line = file.ReadLine()) != null)
                        {
                            counter++;
                        }
                    }
                }
            }
        }

        private void addExtensionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (extensionTxt.Text.Length != 0)
            {
                extensionListBox.Items.Add(extensionTxt.Text);
                extensionTxt.Text = "";
            }
        }

        private void extensionListBox_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (extensionListBox.SelectedIndex != -1)
            {
                extensionListBox.Items.RemoveAt(extensionListBox.SelectedIndex);
            }
        }

        private void extensionTxt_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                addExtensionBtn_Click(sender, e);
            }
        }
    }
}

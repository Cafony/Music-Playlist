using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Music_Playlist
{
    public partial class Form1 : Form
    {
        List<string> mp3List = new List<string>();
        BindingSource _bindingSource = new BindingSource();
        string _playlistDefaultPath = System.IO.Path.Combine(Application.StartupPath, "playlist.txt");


        public Form1()
        {
            InitializeComponent();
            _bindingSource.DataSource = mp3List; 
            listBox1.DataSource = _bindingSource;
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            // Use FolderBrowserDialog to select a folder
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected folder path
                    string selectedFolder = folderDialog.SelectedPath;

                    // Get all .mp3 files in the selected folder
                    string[] mp3Files = Directory.GetFiles(selectedFolder, "*.mp3");

                    // Add each .mp3 file to the mp3List
                    foreach (var file in mp3Files)
                    {
                        mp3List.Add(file);
                    }

                    //Refresh the binding to update listBox1
                    _bindingSource.ResetBindings(false);
                    //DisplayOnlyNames();
                }
            }

        }

        public void DisplayOnlyNames()
        {
            // Display only the file names in listBox1
            List<string> fileNames = new List<string>();

            foreach (var file in mp3List)
            {
                fileNames.Add(Path.GetFileName(file));  // Get only the file name
            }

            // Set the data source to the file names for display in listBox1
            _bindingSource.DataSource = fileNames;

            // Refresh the binding to update listBox1
            _bindingSource.ResetBindings(false);
        }

        public void CreatePlaylistFile()
        {
            // Check if the file exists, if not, create an empty file
            if (!System.IO.File.Exists(_playlistDefaultPath))
            {
                // Create an empty file
                using (System.IO.FileStream fs = System.IO.File.Create(_playlistDefaultPath)) ;
            }
        }

        private void LoadListToListbox()
        {
            string[] lines = File.ReadAllLines(_playlistDefaultPath);

            foreach(var file in lines)
            {
                if (!string.IsNullOrWhiteSpace(file))
                {
                    mp3List.Add(file.Trim());
                }
            }

            //DisplayOnlyNames() ;
            

        }

        private void SaveToTxt()
        {
            CreatePlaylistFile();
            // Write the mp3List to the file
            using (StreamWriter writer = new StreamWriter(_playlistDefaultPath))
            {
                foreach (string mp3File in mp3List)
                {
                    writer.WriteLine(mp3File);  // Write each mp3 file path to the file
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveToTxt();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {

            // Check if any item is selected in listBox1
            if (listBox1.SelectedItems.Count > 0)
            {
                // Create a list to hold the items to remove
                List<string> itemsToRemove = new List<string>();

                // Collect the selected items from listBox1
                foreach (var selectedItem in listBox1.SelectedItems)
                {
                    string selectedItemName = selectedItem.ToString();
                    // Find the corresponding item in mp3List
                    string itemToRemove = mp3List.FirstOrDefault(mp3 => Path.GetFileName(mp3).Equals(selectedItemName, StringComparison.OrdinalIgnoreCase));
                    if (itemToRemove != null)
                    {
                        itemsToRemove.Add(itemToRemove);
                    }
                }

                // Remove each item from mp3List
                foreach (var item in itemsToRemove)
                {
                    mp3List.Remove(item);
                }

                // Refresh the binding to update listBox1
                DisplayOnlyNames();
            }


            else
            {
                // Show a message if no item is selected
                MessageBox.Show("Please select an item to find and remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #region FORM CLOSE OPEN
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadListToListbox();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveToTxt();
        }
        #endregion

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
         
            // Get the search term from textBox1
            string searchTerm = textBox1.Text.Trim();

            // Create a filtered list based on the search term
            if (string.IsNullOrEmpty(searchTerm))
            {
                // If the search term is empty, set the original list as the DataSource
                _bindingSource.DataSource = mp3List.Select(mp3 => Path.GetFileName(mp3)).ToList();
            }
            else
            {
                // Filter the mp3List based on the search term
                var filteredList = mp3List
                    .Where(mp3 => Path.GetFileName(mp3).IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Select(mp3 => Path.GetFileName(mp3))
                    .ToList();

                // Set the filtered list as the DataSource
                _bindingSource.DataSource = filteredList;
            }
            _bindingSource.ResetBindings(false);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelMusicName.Text=listBox1.SelectedItem.ToString();
        }

        // This will display only names on ListBox
        private void listBox1_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value=Path.GetFileName(e.ListItem.ToString());    
        }


    }
}

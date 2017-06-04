using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace MonitorMe
{
    public partial class MonitorMeForm : Form
    {
        private Thread oThread = null;
        private FileMonitor fileMonitor = new FileMonitor();
        public static MySql.Data.MySqlClient.MySqlConnection conn = null;

        public MonitorMeForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Make connection
            string myConnectionString;

            myConnectionString = "server=" + ConfigurationManager.AppSettings["ServerName"] + ";uid=" + ConfigurationManager.AppSettings["UserName"] + ";" +
                "pwd=" + ConfigurationManager.AppSettings["Password"] + ";database=" + ConfigurationManager.AppSettings["Database"] + ";";

            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myConnectionString;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show("Unable to connect to database : \n\n" + ex.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text Files|*.txt";
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                // Remove old thread
                try
                {
                    fileMonitor.fileName = openFileDialog1.FileName;

                    if (oThread != null && oThread.IsAlive)
                    {
                        if (fileMonitor.watcher != null)
                        {
                            fileMonitor.watcher.EnableRaisingEvents = false;
                        }
                        oThread.Abort();
                    }

                    // Run thread
                    oThread = new Thread(new ThreadStart(fileMonitor.moniorFile));

                    oThread.Start();
                }
                catch (IOException)
                {
                }
            }
        }

        private void reportBtn_Click(object sender, EventArgs e)
        {
            errorLb.Text = "";

            if (monthComboBox.SelectedIndex == -1)
            {
                errorLb.Text = "Select a month";
                return;
            }

            MySqlCommand cmd = new MySqlCommand();
            MySqlDataReader reader;

            cmd.CommandText = "Select Artist, Song_Name, count(Song_Name) as Count from songs where Year(now()) = Year(Date) and Month(Date) = " + (monthComboBox.SelectedIndex + 1) + " group by Song_Name;";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            conn.Open();
            
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine(reader["Artist"] + ", " + reader["Song_Name"] + ", " + reader["Count"]);
            }

            conn.Close();

            cmd.CommandText = "Select Header_Name from header;";

            conn.Open();

            reader = cmd.ExecuteReader();

            if(reader.Read())
            {
                Console.WriteLine(reader["Header_Name"]);
            }

            conn.Close();

            // Generate Report
        }
    }

    public class FileMonitor
    {
        public string fileName;
        public FileSystemWatcher watcher = null;
        private DateTime lastRead = DateTime.MinValue;
        public FileMonitor(string fileName)
        {
            this.fileName = fileName;
        }

        public FileMonitor()
        {
        }

        public void moniorFile()
        {
            // Create a new FileSystemWatcher and set its properties.
            watcher = new FileSystemWatcher();

            //watcher.Path = System.IO.Path.GetDirectoryName(this.fileName);
            //watcher.Filter System.IO.Path.GetFileName(this.fileName);

            watcher.Path = Path.GetDirectoryName(this.fileName);
            watcher.Filter = Path.GetFileName(this.fileName);

            watcher.NotifyFilter = NotifyFilters.LastWrite;

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            while (true) ;
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.fileName);
            if (lastWriteTime != lastRead)
            {
                // Specify what is done when a file is changed.
                Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
                lastRead = lastWriteTime;

                // Read from file and store into database

                FileStream fs = new FileStream(this.fileName,
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.ReadWrite);
                var sr = new StreamReader(fs, Encoding.UTF8);

                int lineNumber = 0;
                string line, artist = null, song = null;

                // Assuming that maximum there can be only two lines
                while ((line = sr.ReadLine()) != null)
                {
                    if (lineNumber == 0)
                    {
                        lineNumber++;
                        artist = line;
                    }
                    else
                    {
                        lineNumber = 0;
                        song = line;
                    }
                }

                // Insert into mysql database
                if (MonitorMeForm.conn != null)
                {
                    MonitorMeForm.conn.Open();
                    MySqlCommand command = MonitorMeForm.conn.CreateCommand();
                    command.CommandText = "insert into songs (Date, Time, Artist, Song_Name) values (now(), now(), '" + artist + "', '" + song + "');";
                    command.ExecuteNonQuery();
                    MonitorMeForm.conn.Close();
                }
            }
        }
    }
}

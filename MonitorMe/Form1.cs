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
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;

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
            errorLbl.Text = "";

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
            errorLbl.Text = "";

            if (monthComboBox.SelectedIndex == -1)
            {
                errorLbl.Text = "Select a month";
                return;
            }

            MySqlCommand cmd = new MySqlCommand();
            MySqlDataReader reader;

            cmd.CommandText = "Select Artist, Song_Name, count(Song_Name) as Count from songs where Year(now()) = Year(Date) and Month(Date) = " + (monthComboBox.SelectedIndex + 1) + " group by Song_Name;";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            conn.Open();

            reader = cmd.ExecuteReader();

            List<Dictionary<string, string>> songs = new List<Dictionary<string, string>>();

            while (reader.Read())
            {
                Dictionary<string, string> song = new Dictionary<string, string>();

                song.Add("Artist", (string)reader["Artist"]);
                song.Add("Song_Name", (string)reader["Song_Name"]);
                song.Add("Count", ((Int64)reader["Count"]) + "");

                songs.Add(song);
            }

            conn.Close();

            cmd.CommandText = "Select Header_Name from header;";

            conn.Open();

            reader = cmd.ExecuteReader();
            string header = "";

            if (reader.Read())
            {
                header = (string)reader["Header_Name"];
            }

            conn.Close();

            string month = DateTime.Now.ToString("MMMM");
            string year = DateTime.Now.ToString("yyyy");

            Boolean areReportGenerated = this.generateReports(header, year, month, songs);

            if (areReportGenerated)
            {
                errorLbl.Text = "Reports Generated Successfully";
            }
            else
            {
                errorLbl.Text = "Unable to generate reports";
            }

        }

        private Boolean generateReports(string header, string year, string month, List<Dictionary<string, string>> songs)
        {
            try
            {
                DataTable table = new DataTable();

                table.Columns.Add("Artist", typeof(string));
                table.Columns.Add("Song", typeof(string));
                table.Columns.Add("Count", typeof(string));

                foreach (Dictionary<string, string> song in songs)
                {
                    table.Rows.Add(song["Artist"], song["Song_Name"], song["Count"]);
                }

                string responsePath = "";
                string filename = "report-" + DateTime.Now.ToString("yyyy-MM-dd HHmmssfff");
                filename = Path.Combine(responsePath, filename);
                //Directory.CreateDirectory(responsePath);

                using (var stream = new FileStream(filename + ".xlsx", FileMode.Create, FileAccess.Write, FileShare.None, 0x2000, false))
                {
                    using (ExcelPackage pck = new ExcelPackage(stream))
                    {
                        ExcelWorksheet ws1 = pck.Workbook.Worksheets.Add("Report");

                        ws1.Cells[1, 1].Value = header;
                        ws1.Cells[1, 2].Value = month;
                        ws1.Cells[1, 3].Value = year;

                        ws1.Cells["A3"].LoadFromDataTable(table, true);
                        pck.Save();
                    }
                }

                System.IO.FileStream fs = new FileStream(filename + ".pdf", FileMode.Create);

                // Create an instance of the document class which represents the PDF document itself.
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                // Create an instance to the PDF file by creating an instance of the PDF 
                // Writer class using the document and the filestrem in the constructor.
                PdfWriter writer = PdfWriter.GetInstance(document, fs);

                // Open the document to enable you to write to the document
                document.Open();
                // Add a simple and wellknown phrase to the document in a flow layout manner

                Paragraph heading = new Paragraph(header);
                heading.Alignment = Element.ALIGN_CENTER;

                document.Add(heading);

                Paragraph yearMonth = new Paragraph("Month of " + month + " " + year + "\n");
                yearMonth.Alignment = Element.ALIGN_CENTER;
                yearMonth.SpacingAfter = 20;

                document.Add(yearMonth);

                PdfPTable pdfTable = new PdfPTable(3);
                pdfTable.HeaderRows = 1;

                pdfTable.AddCell(new Phrase("Artist", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
                pdfTable.AddCell(new Phrase("Song Name", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
                pdfTable.AddCell(new Phrase("Count", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));

                foreach (Dictionary<string, string> song in songs)
                {
                    pdfTable.AddCell(new Phrase(song["Artist"]));
                    pdfTable.AddCell(new Phrase(song["Song_Name"]));
                    pdfTable.AddCell(new Phrase(song["Count"]));
                }

                document.Add(pdfTable);

                // Close the document
                document.Close();
                // Close the writer instance
                writer.Close();
                // Always close open filehandles explicity
                fs.Close();

            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        private void label4_Click(object sender, EventArgs e)
        {

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

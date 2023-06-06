using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Windows.Forms.DataVisualization.Charting;

namespace Graph
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void databtn_Click(object sender, EventArgs e)
        {
            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source= C:\\Users\\lccmcd7\\Desktop\\OpcProj\\Opc2\\Opc2\\DWSWindCatcher101.db; Version = 3; New = True; Compress = True; ");
            // Open the connection:
            using (var connection = new SQLiteConnection(sqlite_conn))
            {
                using (var command = new SQLiteCommand(connection))
                {                   
                    command.Connection.Open();
                    SQLiteDataAdapter sda = new SQLiteDataAdapter("Select * From WindData", sqlite_conn); 

                    DataTable dt;
                    using (dt = new DataTable())
                    {
                        sda.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                   
                }
            }

            chart1.Series[0] = new Series();
            chart1.Series[0].XValueMember = dataGridView1.Columns[1].DataPropertyName;
            chart1.Series[0].YValueMembers = dataGridView1.Columns[0].DataPropertyName;
            chart1.DataSource = dataGridView1.DataSource;

        }

        private void button1_Click(object sender, EventArgs e) {

            SQLiteConnection sqlite_conn;
            var end = dateTimePicker1.Value.ToString("dd/MM/yyyy hh:mm:ss"); // willl need to switch
            var start = dateTimePicker2.Value.ToString("dd/MM/yyyy hh:mm:ss"); //hh:mm:ss was origional
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source= C:\\Users\\lccmcd7\\Desktop\\OpcProj\\Opc2\\Opc2\\DWSWindCatcher101.db; Version = 3; New = True; Compress = True; ");
            // Open the connection:
            using (var connection = new SQLiteConnection(sqlite_conn))
            {
                using (var command = new SQLiteCommand(connection))
                {
                    command.Connection.Open(); 

                    // command.Parameters.AddWithValue("@param1", start);
                    // command.Parameters.AddWithValue("@param2", end);
                    command.CommandText = $"Select * from WindData Where ActTime BETWEEN Convert(DateTime,{start}) AND Convert(DateTime,{end})";
                   // command.ExecuteNonQuery();
       
                    SQLiteDataAdapter sda = new SQLiteDataAdapter(command.CommandText, sqlite_conn);


                    DataTable dt;
                    using (dt = new DataTable())
                    {
                        sda.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }

                }
            }

            chart1.Series[0] = new Series();
            chart1.Series[0].XValueMember = dataGridView1.Columns[1].DataPropertyName;
            chart1.Series[0].YValueMembers = dataGridView1.Columns[0].DataPropertyName;
            chart1.DataSource = dataGridView1.DataSource;
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "dd/MM/yyyy hh:mm:ss";
            //dateTimePicker2.CustomFormat = "dd/MM/yyyy";
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd/MM/yyyy hh:mm:ss";
            //dateTimePicker2.CustomFormat = "dd/MM/yyyy";
        }
    }
}

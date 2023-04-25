using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseWinFormsConnector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // Credentials
        static string server = "db4free.net";
        static string database = "mikhail_db";
        static string userid = "mikhail";
        static string password = "S8nluvzu";

        static async Task<string[]> MakeQuery(string query)
        {
            using (MySqlDatabaseConnector connector =
                new MySqlDatabaseConnector(server, database, userid, password))
            {
                try
                {
                    string[] response = await connector.SendQuery(query);
                    return response;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                return new string[] { };
            }
        }

        async Task AddCharacter()
        {
            string name = nameTextBox.Text;
            if (name.Length == 0)
            {
                throw new Exception("Имя не может быть пустым!");
            }
            string className = classTextBox.Text;
            if (className.Length == 0)
            {
                throw new Exception("Имя класса не может быть пустым!");
            }
            string[] currentClass = await MakeQuery(
                "SELECT Id " +
                "FROM CharactersClass " +
                @"WHERE Name = """ + className + @""";"
                );
            if (currentClass.Length == 0)
            {
                _ = await MakeQuery("INSERT INTO CharactersClass(Name) " +
                    @"VALUES (""" + className + @""");"
                    );
                currentClass = await MakeQuery(
                "SELECT Id " +
                "FROM CharactersClass " +
                @"WHERE Name = """ + className + @""";"
                );
            }
            _ = await MakeQuery(
                "INSERT INTO Characters(Name, CharactersClassId) " +
                @"VALUES (""" + name + @""", " + $"{currentClass[0]});"
                );
            nameTextBox.Text = "";
            classTextBox.Text = "";
            outputListBox.Items.Clear();
            outputListBox.Items.Add($"Персонаж {name} ({className}) добавлен успешно!");
        }

        async Task ListCharacters() 
        {
            string query =
                @"SELECT c.NAME AS ""Character Name"", cc.NAME AS ""Class Name"" " +
                "FROM Characters AS c, CharactersClass AS cc " +
                "WHERE c.CharactersClassId = cc.Id;";
            string[] response = await MakeQuery(query);
            foreach (var line in response)
            {
                outputListBox.Items.Add(line);
                Console.WriteLine(line);
            }
        }

        private void listAllButton_Click(object sender, EventArgs e)
        {
            outputListBox.Items.Clear();
            ListCharacters();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            try
            {
                AddCharacter();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

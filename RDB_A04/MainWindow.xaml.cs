/* FILE           : MainWindow.xaml.cs
 * PROJECT        : Assignment 04
 * PROGRAMMER     : Devin Caron & Cole Spehar
 * FIRST VERSION  : 2020-12-08
 * DESCRIPTION    : This program is a small math quiz game that gets the questions
 *                  from a MySql database and stores scores in a MySql leaderboard 
 *                  table.
 *                  This file is the starting page where the user enters their name
 *                  and is then sent to the Test page.
 */


using System;
using System.Collections.Generic;
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
using MySql.Data.MySqlClient;

namespace RDB_A04
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            uName.Text = connString.UserName;

            // initialize the connection string (change if uid or pwd is different)
            connString.myConnectionString = "server=localhost;uid=root;pwd=root;database=RDBA04";
        }

        // class to store the connection string and user name throughout the program
        public static class connString
        {
            public static string myConnectionString { get; set; }

            public static string UserName { get; set; }
        }

        //	FUNCTION    : startBtn_Click
        //	DESCRIPTION : This function is called when the 
        //                the start button is clicked and verifies
        //                if the user has entered a name then inserts
        //                the name into the leaderboard mysql table and
        //                starts the game by opening the test page.
        //	PARAMETERS  : object sender, RoutedEventArgs e
        //	RETURNS     : NONE
        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            // verify user entered a name
            if (uName.Text != "")
            {
                Leaderboard.CanPlay.play = true;

                // clear error
                errorLbl.Content = "";

                // save name
                string name = uName.Text;

                // create MySqlConnection
                MySqlConnection conn = new MySqlConnection();
                conn.ConnectionString = connString.myConnectionString;
                // start connection
                conn.Open();

                // see if the name is already in the leaderboard
                string sql = "select UserName from leaderboard where UserName='" + name + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                // if name is not in the leaderboard, insert the name
                if (rdr.HasRows == false)
                {
                    rdr.Close();
                    sql = $"insert into leaderboard(UserName, Score) Values('{name}', 0 );";
                    cmd = new MySqlCommand(sql, conn);
                    rdr = cmd.ExecuteReader();
                }
                // close connection
                conn.Close();

                // store users name in class variable
                connString.UserName = name;

                // open Test page
                Test testPage = new Test();
                testPage.Show();

                this.Close();
            }
            else // if the user didn't enter a name
            {
                // display error
                errorLbl.Content = "Please enter a name";
            }
        }

        //	FUNCTION    : leaderBoard_Click
        //	DESCRIPTION : This function is called when the 
        //                the leaderboard button is clicked and opens
        //                the leaderboard page.
        //	PARAMETERS  : object sender, RoutedEventArgs e
        //	RETURNS     : NONE
        private void leaderBoard_Click(object sender, RoutedEventArgs e)
        {
            // disable play again button
            Leaderboard.CanPlay.play = false;

            // open leaderboard page
            Leaderboard leaderboard = new Leaderboard();
            leaderboard.Show();
        }
    }
}

/* FILE           : Leaderboard.xaml.cs
 * PROJECT        : Assignment 04
 * PROGRAMMER     : Devin Caron & Cole Spehar
 * FIRST VERSION  : 2020-12-08
 * DESCRIPTION    : This program is a small math quiz game that gets the questions
 *                  from a MySql database and stores scores in a MySql leaderboard 
 *                  table.
 *                  This file is the leaderboard page which displays the top 10 scores
 *                  from the leaderboard MySql database table.
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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace RDB_A04
{
    /// <summary>
    /// Interaction logic for Leaderboard.xaml
    /// </summary>
    public partial class Leaderboard : Window
    {
        // class used to store if the play again button should be enabled
        public static class CanPlay
        {
            public static bool play { get; set; }
        }

        public Leaderboard()
        { 
            InitializeComponent();

            // disable button if false
            if (CanPlay.play == false)
            {
                playAgainBtn.IsEnabled = false;
            }

            // display user's score
            finalScoreLbl.Content = Test.question.scoreCount;

            // start MySqlConnection
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = MainWindow.connString.myConnectionString;
            // open connection
            conn.Open();

            int i = 0;
            // display names from leaderboard sorted by highest score descending to listbox
            string sql = "select UserName from leaderboard order by Score DESC;";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                i++;
                nameBox.Items.Add(rdr["UserName"]);
                if(i == 10)
                {
                    i = 0;
                    break;
                }
            }
            rdr.Close();

            // displays scores descending to listbox
            sql = "select Score from leaderboard order by Score DESC;";
            cmd = new MySqlCommand(sql, conn);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                i++;
                scoreBox.Items.Add(rdr["Score"]);
                if (i == 10)
                {
                    break;
                }
            }
            rdr.Close();
            conn.Close();
        }

        //	FUNCTION    : PlayAgain_Click
        //	DESCRIPTION : This function is called when the play
        //                again button is pressed and will open
        //                the MainWindow page.
        //	PARAMETERS  : NONE
        //	RETURNS     : NONE
        private void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            // open mainwindow
            MainWindow start = new MainWindow();
            start.Show();
            // close leaderboard
            this.Close();
        }
    }
}

/* FILE           : Test.xaml.cs
 * PROJECT        : Assignment 04
 * PROGRAMMER     : Devin Caron & Cole Spehar
 * FIRST VERSION  : 2020-12-08
 * DESCRIPTION    : This program is a small math quiz game that gets the questions
 *                  from a MySql database and stores scores in a MySql leaderboard 
 *                  table.
 *                  This file is the testing page that keeps track of the user's
 *                  score and displays the leaderboard page after quiz is complete.
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
using System.Windows.Threading;

namespace RDB_A04
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : Window
    {
        // question class to store the current question, score and correct answer
        public static class question
        {
            public static int questionID { get; set; }

            public static int scoreCount { get; set; }

            public static int correctAnswer { get; set; }
        }

        public Test()
        {
            InitializeComponent();

            OnStart();
        }


        //	FUNCTION    : OnStart
        //	DESCRIPTION : This function is called on the start
        //                of the test page and it sets up all the
        //                default variables.
        //	PARAMETERS  : NONE
        //	RETURNS     : NONE
        private void OnStart()
        {
            // display Question in label
            questionLbl.Content = "Question ";

            // disable submit button
            questionBtn.IsEnabled = false;

            question.scoreCount = 0;
            // start at questionID 1
            question.questionID = 1;
            // display question number
            questionID.Content = question.questionID;

            // call function to insert the multiple choice answers
            InsertAnswers(question.questionID);

            // create DispatcherTimer to run threaded and count down the time
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += dtTicker;
            dt.Start();
        }

        // create increment variable for the DispatcherTimer
        private int increment = 21;

        //	FUNCTION    : dtTicker
        //	DESCRIPTION : This function is threaded through
        //                the DispatcherTimer and increments the
        //                timer down from 20. If the timer hits 0
        //                it starts the next question.
        //	PARAMETERS  : object sender, RoutedEventArgs e
        //	RETURNS     : NONE
        private void dtTicker(object sender, EventArgs e)
        {
            // display timer
            TimerLabel.Content = increment.ToString();
            // count down
            increment--;
            TimerLabel.Content = increment.ToString();

            // ran out of time, start next question
            if (increment == 0)
            {
                CompleteQuestion();
            }
        }

        //	FUNCTION    : SubmitButton_Click
        //	DESCRIPTION : This function is called when the submit
        //                button is pressed. If it's the final question
        //                submit the score to the leaderboard database and
        //                open the leaderboard if not it will call the completeAnswer
        //                function to insert the new answers.
        //	PARAMETERS  : object sender, RoutedEventArgs e
        //	RETURNS     : NONE
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // if question is 10 then complete the game
            if (question.questionID >= 10)
            {
                // start connection
                MySqlConnection conn = new MySqlConnection();
                conn.ConnectionString = MainWindow.connString.myConnectionString;
                conn.Open();

                int score = 0;

                // get the last score saved to that name in leaderboard
                string query = $"select Score from leaderboard where UserName = '{MainWindow.connString.UserName}';";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    var sc = rdr["Score"];
                    score = Convert.ToInt32(sc);
                }
                rdr.Close();

                // if their new score is higher than their highest score in the leaderboard update leaderboard score
                if (question.scoreCount > score)
                {
                    query = $"update leaderboard set Score = {question.scoreCount} where UserName = '{MainWindow.connString.UserName}';";
                    cmd = new MySqlCommand(query, conn);
                    rdr = cmd.ExecuteReader();
                    rdr.Close();
                }

                // close connection
                conn.Close();

                // open leaderboard page
                Leaderboard leaderBoard = new Leaderboard();
                leaderBoard.Show();

                // reset variables
                OnStart();

                // close test page
                this.Close();
            }
            else
            {
                // insert next question and add score
                CompleteQuestion();
            }
        }

        //	FUNCTION    : CompleteQuestion()
        //	DESCRIPTION : This function increases the user's score
        //                if they got the question right and then
        //                calls the insertAnswers function to start
        //                the next question.
        //	PARAMETERS  : NONE
        //	RETURNS     : NONE
        private void CompleteQuestion()
        {
            // if the question is right and there is still time on the clock
            if (Convert.ToInt32(questionDrop.SelectedItem) == question.correctAnswer && increment > 0)
            {
                // add to the user's score
                question.scoreCount += increment;
                // display updated score
                scoreLbl.Content = question.scoreCount;
            }
            // go to the next question
            question.questionID++;
            // display new question number
            questionID.Content = question.questionID;
            // insert the multiple choice answers
            InsertAnswers(question.questionID);
            // disable the submit button
            questionBtn.IsEnabled = false;
            // reset the timer
            increment = 21;
        }

        //	FUNCTION    : InsertAnswers()
        //	DESCRIPTION : This function inserts the answer and
        //                potential answers in a random order to
        //                the drop down box.
        //	PARAMETERS  : int questionID
        //	RETURNS     : NONE
        private void InsertAnswers(int questionID)
        {
            // clear the old items
            questionDrop.Items.Clear();

            // get MySqlConnection
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = MainWindow.connString.myConnectionString;
            // open connection
            conn.Open();

            // get the questions from the questionID
            string query = $"select Question from Questions where QuestionID = {question.questionID};";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                string question = "";
                var ques = rdr["Question"];
                question = ques.ToString();
                questionText.Content = question;
            }
            rdr.Close();

            // randomize a list with numbers 1-4
            var random = new Random();
            List<int> numberList = new List<int>();
            while (numberList.Count < 4)
            {
                int rnd = random.Next(1, 5);
                if (!numberList.Contains(rnd)) numberList.Add(rnd);
            }

            // randomize the inserting of the combo box
            for (var i = 0; i < numberList.Count; i++)
            {
                if (numberList[i] == 1)
                {
                    query = $"select PotentialAnswer1 from Answers where QuestionID = {question.questionID};";
                    cmd = new MySqlCommand(query, conn);
                    rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        int Ans = 0;
                        var Answer = rdr["PotentialAnswer1"];
                        Ans = Convert.ToInt32(Answer);
                        questionDrop.Items.Add(Ans);
                    }
                    rdr.Close();
                }
                else if (numberList[i] == 2)
                {
                    query = $"select PotentialAnswer2 from Answers where QuestionID = {question.questionID};";
                    cmd = new MySqlCommand(query, conn);
                    rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        int Ans = 0;
                        var Answer = rdr["PotentialAnswer2"];
                        Ans = Convert.ToInt32(Answer);
                        questionDrop.Items.Add(Ans);
                    }
                    rdr.Close();
                }
                else if (numberList[i] == 3)
                {
                    query = $"select PotentialAnswer3 from Answers where QuestionID = {question.questionID};";
                    cmd = new MySqlCommand(query, conn);
                    rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        int Ans = 0;
                        var Answer = rdr["PotentialAnswer3"];
                        Ans = Convert.ToInt32(Answer);
                        questionDrop.Items.Add(Ans);
                    }
                    rdr.Close();
                }
                else if (numberList[i] == 4)
                {
                    query = $"select ActualAnswer from Answers where QuestionID = {question.questionID};";
                    cmd = new MySqlCommand(query, conn);
                    rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        int Ans = 0;
                        var Answer = rdr["ActualAnswer"];
                        Ans = Convert.ToInt32(Answer);
                        questionDrop.Items.Add(Ans);
                        question.correctAnswer = Ans;
                    }
                    rdr.Close();
                }
            }
            // close connection
            conn.Close();
        }

        //	FUNCTION    : questionDrop_SelectionChanged()
        //	DESCRIPTION : This function enables the submit button
        //                when the user picks a answer.
        //	PARAMETERS  : object sender, SelectionChangedEventArgs e
        //	RETURNS     : NONE
        private void questionDrop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            questionBtn.IsEnabled = true;
        }
    }
}

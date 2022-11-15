using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddEmUp
{
    public class Winner
    {

        private bool haveTightScore = false;

        private Dictionary<string, int> cardValues;

        private string file;

        public Winner(string file)
        {
            // Cards and Suits values
            cardValues = new Dictionary<string, int>()
            {
                {"J", 11 },
                {"Q", 12 },
                {"K", 13 },
                {"A", 1 },
                {"S", 4 },
                {"H", 3 },
                {"D", 2 },
                {"C", 1 }
            };

            // Assign args received from the user.
            this.file = file;
        }

        /// <summary>
        /// Creates a dictionary mapping of users and their scores.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetUserCards()
        {
            Dictionary<string, string> userCards = new Dictionary<string, string>();

            try
            {
                // Read a file.
                using (StreamReader sr = new StreamReader(file))
                {
                    string fileData = sr.ReadToEnd();

                    // Get a single file line.
                    string[] userAndScore = fileData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                    foreach (string element in userAndScore)
                    {
                        // Split a line.
                        string[] nameAndCards = element.Split(':');
                        // Add name and score.
                        userCards.Add(nameAndCards[0], nameAndCards[1]);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return userCards;
        }

        /// <summary>
        /// Returns the result of GetAllScores method.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetScores()
        {
            return GetAllScores(GetUserCards());
        }

        /// <summary>
        /// Returns computer user scores.
        /// </summary>
        /// <param name="userCards"></param>
        /// <returns></returns>
        private Dictionary<string, int> GetAllScores(Dictionary<string, string> userCards)
        {
            Dictionary<string, int> userScores = new Dictionary<string, int>();
            try
            {
                foreach (var item in userCards)
                {
                    string usersame = item.Key;
                    int score = AssignIndividualScore(item.Value);
                    userScores.Add(usersame, score);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return userScores;
        }

        /// <summary>
        /// Splits card and suit into an array of characters.
        /// </summary>
        /// <param name="userCards"></param>
        /// <returns></returns>
        private int AssignIndividualScore(string userCards)
        {
            int score = 0;
            string[] cards = userCards.Split(',');

            foreach (string card in cards)
            {
                int result = getCardScore(card);
                score += result;
            }

            return score;
        }

        /// <summary>
        /// Gets a value assigned to a card or suit
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        private int getCardScore(string card)
        {
            string cardOrSuit = haveTightScore ? card[1].ToString() : card[0].ToString();

            var isNumeric = int.TryParse(cardOrSuit, out int output);

            int cardScore = isNumeric ? output : LookupCardNumber(cardOrSuit);

            return cardScore;
        }

        /// <summary>
        /// Looks up a card/suit value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private int LookupCardNumber(string key)
        {
            if (cardValues.ContainsKey(key))
            {
                return cardValues[key];
            }

            return 0;
        }

        /// <summary>
        /// Gets a list of users with a tied score
        /// </summary>
        /// <param name="userScores"></param>
        /// <returns></returns>
        private List<string> DetectTie(Dictionary<string, int> userScores)
        {
            Dictionary<int, int> scores = new Dictionary<int, int>();

            List<string> usernames = new List<string>();

            foreach (var userScore in userScores)
            {
                /*
                 * User scores are assigned as dictionary keys with a value that is 1 initially.
                 * This value gets incremented when a certain score exists more than once.
                 * */
                if (scores.ContainsKey(userScore.Value))
                {
                    scores[userScore.Value]++;
                }
                else
                {
                    scores.Add(userScore.Value, 1);
                }
            }

            /* A key (as a score) that have a value of more than one is used to lookup users who share that particular score. 
             * These users are stored in a separate list (usernames).
             */
            foreach (var userScore in userScores)
            {
                if (scores.ContainsKey(userScore.Value))
                {
                    if (scores[userScore.Value] > 1)
                    {
                        usernames.Add(userScore.Key);
                    }
                }
            }

            return usernames;
        }

        /// <summary>
        /// Calculates the scores of users with a tied score using suits.
        /// </summary>
        /// <param name="usernames"></param>
        /// <returns></returns>
        private Dictionary<string, int> BreakTie(List<string> usernames)
        {
            haveTightScore = true;
            Dictionary<string, string> userScores = new Dictionary<string, string>();

            foreach (var username in usernames)
            {
                if (GetUserCards().ContainsKey(username))
                {
                    // Lookup key (user).
                    var key = GetUserCards().Keys.FirstOrDefault(k => k == username);
                    // Lookup value (user score).
                    var value = GetUserCards()[key];

                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    {
                        userScores.Add(key, value);
                    }
                }
            }

            // Recompute the scores of users with a tied score.
            Dictionary<string, int> results = GetAllScores(userScores);

            return results;
        }

        /// <summary>
        /// Recomputes the score of users who had a tie score to see if it can be broken.
        /// </summary>
        /// <param name="initialScores"></param>
        /// <param name="brokenTieScores"></param>
        /// <returns></returns>
        private Dictionary<string, int> ReComputeScore(Dictionary<string, int> initialScores, Dictionary<string, int> brokenUserTieScores)
        {
            Dictionary<string, int> newScore = new Dictionary<string, int>();
            
            foreach (var userScore in initialScores)
            {
                int score;
                string key;

                /* Find user and assign their suit score as a new score
                 * If the new score is not equal to the initial score, the new score is assigned, else the initial score remains.
                 * N:B - SHOULD THESE SCORES BE ADDED TOGETHER? IF YES, LOOK AT THE COMMENTED LINE OF CODE (CAN UNCOMMENT IT IF YOU COMMENT OUT THE CODE MARKED <<CAN COMMENT OUT>>)
                 */
                if (brokenUserTieScores.ContainsKey(userScore.Key))
                {
                    key = userScore.Key;

                    // <<CAN COMMENT OUT>>
                    if (initialScores[userScore.Key] != brokenUserTieScores[userScore.Key])
                    {
                        score = brokenUserTieScores[userScore.Key];
                    }
                    else
                    {
                        score = initialScores[userScore.Key];
                    }

                     // score = initialScores[userScore.Key] + brokenUserTieScores[userScore.Key];
                    
                }
                else
                {
                    // User with no tied scores are assigned their initial scores
                    score = userScore.Value;
                    key = userScore.Key;
                }

                newScore.Add(key, score);
            }

            return newScore;
        }

        /// <summary>
        /// Processes the users score by:
        /// Checking if ties exists.
        /// Call a method responsible for formatting the score results.
        /// </summary>
        /// <param name="winner"></param>
        /// <returns></returns>
        public StringBuilder ProcessUserScores(Winner winner)
        {
            // Get user scores.
            var scores = winner.GetScores();

            // Check that there is a tied score.
            var usersWithTightScore = winner.DetectTie(scores);

            StringBuilder results;

            if (usersWithTightScore.Count > 0)
            {
                // Break tie.
                var brokenUserTieScores = winner.BreakTie(usersWithTightScore);
                // Recompute new scores.
                var newUserScores = winner.ReComputeScore(scores, brokenUserTieScores);
                // Format new score.

                // PROBLEM IS HERE
                results = winner.OutputScore(newUserScores, winner);
            }
            else
            {
                // No tie was detected. Format new score.
                results = winner.OutputScore(scores, winner);
            }

            return results;
        }

        /// <summary>
        /// Formats users score.
        /// </summary>
        /// <param name="scores"></param>
        /// <param name="winner"></param>
        /// <returns></returns>
        private StringBuilder OutputScore(Dictionary<string, int> scores, Winner winner)
        {
            StringBuilder results = new StringBuilder("");

            var tiedScores = winner.DetectTie(scores);

            // Where user if found to have no tied score with another user.
            if (tiedScores.Count > 0)
            {
                foreach (var score in scores)
                {
                    if (!tiedScores.Contains(score.Key))
                    {
                        results.Append($"{score.Key} : {score.Value}\n");
                    }
                }
                // Where user if found to have a tied score with another user.
                int counter = 1;
                foreach (var score in scores)
                {
                    if (counter >= tiedScores.Count)
                    {
                        break;
                    }
                    if (tiedScores.Contains(score.Key))
                    {
                       var userList = scores.Keys.Where((user) => scores[user] == score.Value);

                        string users = string.Join(',', userList.ToArray());
                        results.Append($"{users} : {score.Value}\n");
                        counter += 2;
                    }
                }
            }
            else
            {
                foreach (var score in scores)
                {
                    results.Append($"{score.Key} : {score.Value}\n");
                }
            }

            return results;
        }

        /// <summary>
        /// Writes final users score to an output file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="results"></param>
        public void WriteToOutputFile(String filename, string results)
        {
            File.WriteAllText(filename, results);
            Console.WriteLine("SUCCESS: Output file generated successfully");
        }

    }
}

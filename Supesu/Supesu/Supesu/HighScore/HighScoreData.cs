using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Supesu.HighScore
{
    public class HighScores
    {
        public static readonly String fileName = "Highscores.lst";
        public static string playerName = "Player1";

        [Serializable]
        public struct HighScoreData
        {
            public String[] playerName; // A name the user gets to set himself after a game has been finished.
            public int[] score;
            public int[] level; // How far the user made it?

            public int count;

            public HighScoreData(int count)
            {
                playerName = new string[count];
                score = new int[count];
                level = new int[count];

                this.count = count;
            }
        }

        public static void SaveHighScores(HighScoreData data, String fileName)
        {
            // Open the file, creating it if necessary
            FileStream stream = File.Open(fileName, FileMode.OpenOrCreate);
            try
            {
                // Convert the object to XML data and put it in the stream
                XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
                //TODO: This will crash the game incase the format of the Highscore.lst file is incorrect.
                serializer.Serialize(stream, data);
            }
            finally
            {
                // Close the file
                stream.Close();
            }
        }

        public static HighScoreData LoadHighScores(String fileName)
        {
            HighScoreData data;

            // Open the file
            FileStream stream = File.Open(fileName, FileMode.OpenOrCreate,
            FileAccess.Read);
            try
            {
                // Read the data from the file
                XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
                data = (HighScoreData)serializer.Deserialize(stream);
                
            }
            finally
            {
                // Close the file
                stream.Close();
            }

            return (data);
        }

        public static void SaveHighscoreToFile()
        {
            HighScores.HighScoreData data = HighScores.LoadHighScores(HighScores.fileName);

            //HighScores.HighScoreData data = new HighScores.HighScoreData(oldData.count + 1);

            //for (int i = 0; i < oldData.count; i++)
            //{
            //    data.playerName[i] = oldData.playerName[i];
            //    data.score[i] = oldData.score[i];
            //    data.level[i] = oldData.level[i];
            //}
            data.count += 1;

            Array.Resize<String>(ref data.playerName, data.count);
            Array.Resize<int>(ref data.score, data.count);
            Array.Resize<int>(ref data.level, data.count);

            int index = -1;
            for (int i = 0; i < data.count; i++)
            {
                if (InGameScreen.playerScore > data.score[i])
                {
                    index = i;
                    break;
                }
            }

            //Sort the values based on score
            if (index > -1)
            {
                for (int i = data.count - 1; i >= index + 1; i--)
                {
                    data.playerName[i] = data.playerName[i - 1];
                    data.score[i] = data.score[i - 1];
                    data.level[i] = data.level[i - 1];
                }
                data.playerName[index] = playerName; //TODO: Let the player set his own name.
                data.score[index] = InGameScreen.playerScore;
                data.level[index] = (int)InGameScreen.level;
            }
            else
            {
                data.playerName[data.count - 1] = playerName; //TODO: Let the player set his own name.
                data.score[data.count - 1] = InGameScreen.playerScore;
                data.level[data.count - 1] = (int)InGameScreen.level;
            }

            HighScores.SaveHighScores(data, HighScores.fileName);
        }
    }
}

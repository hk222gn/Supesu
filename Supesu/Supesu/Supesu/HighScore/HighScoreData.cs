﻿using Microsoft.Xna.Framework;
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
        //TODO: Make highscores sort automatically on the highest score.

        public static readonly String fileName = "Highscores.lst";

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
            HighScores.HighScoreData oldData = HighScores.LoadHighScores(HighScores.fileName);

            HighScores.HighScoreData data = new HighScores.HighScoreData(oldData.count + 1);

            for (int i = 0; i < oldData.count; i++)
            {
                data.playerName[i] = oldData.playerName[i];
                data.score[i] = oldData.score[i];
                data.level[i] = oldData.level[i];
            }

            data.playerName[data.count - 1] = "Player1"; //TODO: Let the player set his own name.
            data.score[data.count - 1] = InGameScreen.playerScore;
            data.level[data.count - 1] = (int)InGameScreen.level;

            HighScores.SaveHighScores(data, HighScores.fileName);
        }
    }
}

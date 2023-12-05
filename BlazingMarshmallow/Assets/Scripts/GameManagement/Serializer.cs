using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Serializer
{
    public static void SaveLeaderboard(LeaderboardSave leaderboard, string level)
    {
        BinaryFormatter bf = new BinaryFormatter();

        string path = Application.persistentDataPath + "/" + level + ".blazingmarshmallow";
        FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);

        bf.Serialize(stream, leaderboard);
        stream.Close();

        Debug.Log("Saved data to: " + path);
    }

    public static LeaderboardSave LoadLeaderboard(string level)
    {
        string path = Application.persistentDataPath + "/" + level + ".blazingmarshmallow";
        if(File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            LeaderboardSave save = bf.Deserialize(stream) as LeaderboardSave;

            stream.Close();
            Debug.Log("Succesfully read save data.");

            return save;
        }
        else
        {
            Debug.Log("Failed to read save data.");
            return null;
        }
    }
}

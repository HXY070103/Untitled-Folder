using UnityEngine;
using System.IO;
using System;

public class FileDataHandler
{
    private string fullPath;

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        fullPath = Path.Combine(dataDirPath, dataFileName);
    }

    public void SaveData(GameData data)
    {
        try
        {
            // Creaste directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //convert the data to json format
            string dataToSave = JsonUtility.ToJson(data, true);

            //open/create a new file
            using(FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                //write the josn text to the file
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToSave);
                }
            }
        }
        catch(Exception e) {
            Debug.LogError("Error saving data: " + e.Message);
        }
    }

    public GameData LoadData()
    {
        GameData loadData = null;

        //check if the save file exists
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                //open the file
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    //read the file's text content
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                //convert the json text into a GameData object
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading data: " + e.Message);
            }
        }

        return loadData;
    }

    public void Delete()
    {
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}

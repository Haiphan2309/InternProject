using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GDC.Managers;

namespace GDC.Managers
{
    static public class SaveLoadSystem
    {
        static BinaryFormatter formatter = new BinaryFormatter();

        //public string folderName;
        //public string fileName;
        // Start is called before the first frame update


        // Update is called once per frame
        static public void SaveData(GameDataOrigin gameDataOrigin)
        {
#if UNITY_ANDROID
            BinaryFormatter formatter = new BinaryFormatter();

            string path = Application.persistentDataPath + "/dataPoint.fun";
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, gameDataOrigin);
            stream.Close();
#else
            string folderName = "SaveData";
            string fileName = "Data";
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            FileStream dataFile = File.Create(folderName + "/" + fileName + ".bin"); //"forderName/fileName.bin"

            formatter.Serialize(dataFile, gameDataOrigin);
            dataFile.Close();
            //print("Save data to: " + Directory.GetCurrentDirectory().ToString() + "/" + folderName + "/" + fileName + ".bin");
            //print("Value1 and Value2: " + gameData.sliderValue1 + " " + gameData.sliderValue2);
#endif
        }

        static public void LoadData(out GameDataOrigin gameDataOrigin)
        {
#if UNITY_ANDROID
            string path = Application.persistentDataPath + "/dataPoint.fun";
            gameDataOrigin = new GameDataOrigin();
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                FileStream stream = new FileStream(path, FileMode.Open);

                gameDataOrigin = (GameDataOrigin)formatter.Deserialize(stream);
                stream.Close();

                SaveLoadManager.Instance.GameDataOrigin = gameDataOrigin;
            }
#else
            string folderName = "SaveData";
            string fileName = "Data";
            if (!Directory.Exists(folderName))
            {
                Logger.Log("Don't have data to load");
                gameDataOrigin = new GameDataOrigin();
                return;
            }

            FileStream dataFile = File.Open(folderName + "/" + fileName + ".bin", FileMode.Open);
            gameDataOrigin = (GameDataOrigin)formatter.Deserialize(dataFile);
       
            //SaveLoadManager.Instance.GameDataOrigin = gameDataOrigin;
            dataFile.Close();
#endif
        }
    }
}

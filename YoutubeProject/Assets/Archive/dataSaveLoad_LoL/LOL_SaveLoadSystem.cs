using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class LOL_SaveLoadSystem : MonoBehaviour
{
    public ChampionData[] data;

    static string path;

    // Start is called before the first frame update
    void Awake()
    {
        path = Application.persistentDataPath + "/Save.mybinary";

        Debug.Log("persistant path : " + Application.persistentDataPath);

        // 우선 persistant 쪽에 데이터없으면 자동으로 넣어줌 
        if (File.Exists(path) == false)
        {
            string localDataArchivePath = Application.dataPath + "/Archive/dataSaveLoad_LoL/Resources/Save.mybinary";

            // 유니티 프로젝트안에 내가 저장해둔거잇는데 이것도 없으면 
            // 해당 파일업스면 깃으로 리버트 ㄱㄱ 하셈 
            if (File.Exists(localDataArchivePath) == false)
            {
                Debug.LogError("do revert save fileeeee that already existed!!!!!!!@@");
            }
            else
            {
                Debug.Log("copied data from project folder to " + Application.persistentDataPath);
                File.Copy(localDataArchivePath, path);
            }
        }

        data = Load();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Save(ChampionData[] data)
    {
        var formatter = new BinaryFormatter();

        using (var stream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(stream, data);
            stream.Close();
        }
    }

    static public ChampionData[] Load()
    {
        if (File.Exists(path) == false)
        {
            Debug.LogError("Save Data First~~");
            return null;
        }

        var formatter = new BinaryFormatter();

        using (var stream = new FileStream(path, FileMode.Open))
        {
            var result = formatter.Deserialize(stream) as ChampionData[];
            return result;
        }

        return null;
    }
}

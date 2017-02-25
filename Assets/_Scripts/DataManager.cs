using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    static private DataManager m_this;

    private List<float> m_recs = new List<float>();

    void Awake()
    {
        if (m_this == null)
        {
            m_this = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public List<float> UpdateRecords (float recTime) // Called at end of level by game manager
    {
        LoadRecords(SceneManager.GetActiveScene().buildIndex);

        Debug.Log("adding record " + recTime.ToString());

        m_recs.Add(recTime);

        Debug.Log("after add record");
        int i = 0;
        foreach (float num in m_recs)
        {
            Debug.Log("m_recs[" + i.ToString() + "] == " + num.ToString());
            i++;
        }
        i = 0;

        m_recs.Sort();

        Debug.Log(System.Environment.NewLine + "after list sort");
        foreach (float num in m_recs)
        {
            Debug.Log("m_recs[" + i.ToString() + "] == " + num.ToString());
            i++;
        }
        i = 0;

        m_recs.RemoveAt(m_recs.Count - 1);

        Debug.Log(System.Environment.NewLine + "after remove item");
        foreach (float num in m_recs)
        {
            Debug.Log("m_recs[" + i.ToString() + "] == " + num.ToString());
            i++;
        }
        i = 0;

        SaveRecords(SceneManager.GetActiveScene().buildIndex);

        return m_recs;
    }

    public List<float> LoadRecords(int i, float defaultTime = 120.0f) // Called on game load by leveldetail panels
    {
        FileStream file;
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/Level" + i.ToString() + "Records.dat"))
        {
            file = File.Open(Application.persistentDataPath + "/Level" + i.ToString() + "Records.dat", FileMode.Open);
            m_recs = (List<float>)bf.Deserialize(file);
        }
        else
        {   
            SaveRecords(i, defaultTime);            
            return m_recs;
        }

        file.Close();
        return m_recs;
    }

    public void SaveRecords(int i, float defaultTime = 120.0f)
    {
        FileStream file;
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/Level" + i.ToString() + "Records.dat"))
        {
            file = File.Open(Application.persistentDataPath + "/Level" + i.ToString() + "Records.dat", FileMode.Open);
            //m_recs = (List<float>)bf.Deserialize(file);
        }
        else
        {
            for (int j = 0; j < 5; j++)
            {
                m_recs.Add(defaultTime + 10.0f * j);
            }

            file = File.Create(Application.persistentDataPath + "/Level" + i.ToString() + "Records.dat");
            //bf.Serialize(file, m_recs);
            //file.Close();
            //return;
            //....
        }

        bf.Serialize(file, m_recs);
        file.Close();
        return;
    }
}

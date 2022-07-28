using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    #region Factions
    public static void SaveFaction (Faction faction)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/"+faction.faction.ToString()+".savedFiles";
        FileStream stream = new FileStream(path, FileMode.Create);

        FactionData data = new FactionData(faction);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static FactionData loadFaction(Faction faction)
    {
        string path = Application.persistentDataPath + "/" + faction.faction.ToString() + ".savedFiles";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            FactionData data = formatter.Deserialize(stream) as FactionData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("SaveFile not found in " + path);
            return null;
        }
    }

    #endregion
    #region Armies
    public static void SaveArmy(Army army)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + army.name.ToString() + ".savedFiles";
        FileStream stream = new FileStream(path, FileMode.Create);

        ArmyData data = new ArmyData(army);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static ArmyData loadArmy(Army army)
    {
        string path = Application.persistentDataPath + "/" + army.name.ToString() + ".savedFiles";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            ArmyData data = formatter.Deserialize(stream) as ArmyData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("SaveFile not found in " + path);
            return null;
        }
    }

    #endregion
    #region Merchants
    public static void SaveMerchant(Merchant merchant)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + merchant.name.ToString() + ".savedFiles";
        FileStream stream = new FileStream(path, FileMode.Create);

        MerchantData data = new MerchantData(merchant);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static MerchantData loadMerchant(Merchant merchant)
    {
        string path = Application.persistentDataPath + "/" + merchant.name.ToString() + ".savedFiles";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            MerchantData data = formatter.Deserialize(stream) as MerchantData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("SaveFile not found in " + path);
            return null;
        }
    }

    #endregion
}

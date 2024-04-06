using Godot;
using System.Collections.Generic;

public partial class MyMethods : Node
{
    public static Godot.Collections.Dictionary LoadJson(string FilePath)
    {
        FileAccess JsonFile = FileAccess.Open(FilePath, FileAccess.ModeFlags.Read);
        if (JsonFile == null)
        {
            GD.PrintErr("Failed to load map data file: " + FilePath);
            return null;
        }

        Json JsonLoader = new Json();
        Error Error = JsonLoader.Parse(JsonFile.GetAsText());
        if (Error != Error.Ok)
        {
            GD.PrintErr(Error);
            return null;
        }

        return (Godot.Collections.Dictionary)JsonLoader.Data;
    }

    public static Dictionary<string, List<string>> LoadCsv(string FilePath)
    {
        Dictionary<string, List<string>> RetVal = new Dictionary<string, List<string>>();

        FileAccess CsvFile = FileAccess.Open(FilePath, FileAccess.ModeFlags.Read);
        if (CsvFile == null)
        {
            GD.PushError("Failed to open csv file: ", FilePath);
            return null;
        }

        string[] Headers = CsvFile.GetCsvLine();
        Dictionary<int, string> IndexToHeader = new Dictionary<int, string>();
        for(int i = 0; i < Headers.Length; i++)
        {
            IndexToHeader.Add(i, Headers[i]);
            RetVal.Add(Headers[i], new List<string>());
        }

        while(!CsvFile.EofReached())
        {
            string[] Fields = CsvFile.GetCsvLine();
            if (Fields.Length != 0 && Fields[0] == "")
            {
                continue;
            }

            for(int i = 0; i < Fields.Length; i++)
            {
                string Key = IndexToHeader.GetValueOrDefault(i);
                RetVal.GetValueOrDefault(Key).Add(Fields[i]);
            }
        }

        CsvFile.Close();

        return RetVal;
    }

    // Each line of the txt file as an List's element
    public static List<string> LoadTxtToList(string FilePath)
    {
        List<string> RetVal = new List<string>();

        FileAccess TxtFile = FileAccess.Open(FilePath, FileAccess.ModeFlags.Read);
        if (TxtFile == null)
        {
            GD.PushError("Failed to open txt file: ", FilePath);
            return null;
        }

        while(TxtFile.EofReached() == false)
        {
            RetVal.Add(TxtFile.GetLine());
        }

        return RetVal;
    }
}

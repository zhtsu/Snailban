using Godot;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.IO;

public partial class MyMethods : Node
{
    public static Godot.Collections.Dictionary LoadJson(string FilePath)
    {
        if (!File.Exists(FilePath))
        {
            GD.PrintErr("Failed to load map data! FilePath: " + FilePath);
            return null;
        }

        string FileData = null;
        try
        {
            FileData = File.ReadAllText(FilePath);
        }
        catch(System.Exception Err)
        {
            GD.PrintErr(Err);
        }

        Json JsonLoader = new Json();
        Error Error = JsonLoader.Parse(FileData);
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

        using (TextFieldParser Parser = new TextFieldParser(FilePath))
        {
            Parser.TextFieldType = FieldType.Delimited;
            Parser.SetDelimiters(",");

            Dictionary<int, string> IndexToHeader = new Dictionary<int, string>();
            string[] Headers = Parser.ReadFields();
            for(int i = 0; i < Headers.Length; i++)
            {
                IndexToHeader.Add(i, Headers[i]);
                RetVal.Add(Headers[i], new List<string>());
            }

            while(!Parser.EndOfData)
            {
                string[] Fields = Parser.ReadFields();
                for(int i = 0; i < Fields.Length; i++)
                {
                    string Key = IndexToHeader.GetValueOrDefault(i);
                    RetVal.GetValueOrDefault(Key).Add(Fields[i]);
                }
            }
        }

        return RetVal;
    }

    // Each line of the txt file as an List's element
    public static List<string> LoadTxtToList(string FilePath)
    {
        List<string> RetVal = new List<string>();

        using(StreamReader Reader = new StreamReader(FilePath))
        {
            string Line;
            while((Line = Reader.ReadLine()) != null)
            {
                RetVal.Add(Line);
            }
        }

        return RetVal;
    }
}

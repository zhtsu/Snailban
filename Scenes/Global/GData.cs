using System;
using System.Collections.Generic;
using Godot;

public partial class GData : Node
{
    public static Dictionary<int, string> ElementDict = new Dictionary<int, string>();
    public static Dictionary<int, FMapData> MapDataDict = new Dictionary<int, FMapData>();

    public override void _Ready()
	{
        LoadMapData();
        LoadElementData();
	}

    public void LoadMapData()
    {
        List<string> MapFiles = Methods.LoadTxtToList(Paths.GenMapDataPath("all.maps"));
        foreach(string MapFile in MapFiles)
        {
            FMapData MapData = LoadMapDataFromPath(Paths.GenMapDataPath(MapFile));
            MapDataDict.Add(MapData.Id, MapData);
        }
    }

    public void LoadElementData()
    {
        Dictionary<string, List<string>> Dict = Methods.LoadCsv(Paths.GenDataPath("element_table.csv"));
        
        List<string> Ids = Dict["ID"];
        List<string> Tscns = Dict["TSCN"];
        if (Ids.Count != Tscns.Count)
        {
            GD.PrintErr("CSV file format error!");
            return;
        }

        for (int i = 0; i < Ids.Count; i++)
        {
            ElementDict.Add(Int32.Parse(Ids[i]), Tscns[i]);
        }
    }

    public FMapData LoadMapDataFromPath(string FilePath)
    {
        Godot.Collections.Dictionary MapDataDict = Methods.LoadJson(FilePath);
        
        FMapData RetVal = new FMapData();
        RetVal.Id = (int)MapDataDict["id"];
        RetVal.Name = (string)MapDataDict["name"];
        RetVal.Row = (int)MapDataDict["row"];
        RetVal.Column = (int)MapDataDict["column"];
        RetVal.TileWidth = (int)MapDataDict["tile_width"];
        RetVal.TileHeight = (int)MapDataDict["tile_height"];
        RetVal.LayerCount = (int)MapDataDict["layer_count"];
        Godot.Collections.Array LayersArray = (Godot.Collections.Array)MapDataDict["layers"];
        RetVal.Layers = new int[LayersArray.Count, RetVal.Row, RetVal.Column];

        for (int i = 0; i < LayersArray.Count; i++)
        {
            Godot.Collections.Array Rows = (Godot.Collections.Array)LayersArray[i];
            for (int j = 0; j < Rows.Count; j++)
            {
                Godot.Collections.Array Columns = (Godot.Collections.Array)Rows[j];
                for (int k = 0; k < Columns.Count; k++)
                {
                    RetVal.Layers[i, j, k] = (int)Columns[k];
                }
            }
        }

        return RetVal;
    }
}

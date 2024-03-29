using Godot;
using System;
using System.Collections.Generic;

public partial class DialogSystem : Node
{
    private DialogBox MyDialogBox;
    private Dictionary<int, List<DialogueBean>> DialoguesDict;

    public override void _Ready()
    {
        DialoguesDict = new Dictionary<int, List<DialogueBean>>();
        LoadDialoguesDict();

        PackedScene DialogBoxScene = (PackedScene)GD.Load("res://Scenes/UI/DialogBox.tscn");
        MyDialogBox = (DialogBox)DialogBoxScene.Instantiate();
        AddChild(MyDialogBox);
    }

    public void StartDialogues(int DialoguesId)
    {
        if (DialoguesDict.TryGetValue(DialoguesId, out List<DialogueBean> MyDialogBeans) == false)
        {
            GD.PushWarning("Invalid dialogues id! id: ", DialoguesId);
        }
        
    }

    public void LoadDialoguesDict()
    {

    }
}

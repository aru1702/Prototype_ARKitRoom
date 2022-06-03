using UnityEngine;

public class MyOrigin
{
    public string name { get; set; }
    public string parent { get; set; }
    public Vector3 position { get; set; }
    public Vector3 euler_rotation { get; set; }
    public string comment { get; set; }

    public MyOrigin(string name, string parent, Vector3 position, Vector3 euler_rotation, string comment = "")
    {
        this.name = name;
        this.parent = parent;
        this.position = position;
        this.euler_rotation = euler_rotation;
        this.comment = comment;
    }

    public MyOrigin(string name, string parent)
    {
        this.name = name;
        this.parent = parent;
        position = Vector3.zero;
        euler_rotation = Vector3.zero;
        comment = "";
    }

    public override string ToString()
    {
        string returnStr = "Name: " + name + "\n" +
                            "Parent: " + parent + "\n" +
                            "Position: " + position.ToString() + "\n" +
                            "Euler rotation: " + euler_rotation.ToString() + "\n" +
                            "Comment: " + comment;
        return returnStr;
    }
}

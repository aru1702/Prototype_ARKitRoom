using UnityEngine;

public class Things
{
    public struct Position
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Position(float x, float y, float z) { X = x; Y = y; Z = z; }
        public Position(Vector3 xyz) { X = xyz.x; Y = xyz.y; Z = xyz.z; }

        public Vector3 GetPosition() { return new Vector3(X, Y, Z); }
    }

    // use transform.Rotate(new Vector3(x,y,z))
    public struct Rotation
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Rotation(float x, float y, float z) { X = x; Y = y; Z = z; }
        public Rotation(Vector3 xyz) { X = xyz.x; Y = xyz.y; Z = xyz.z; }

        public Vector3 GetRotation() { return new Vector3(X, Y, Z); }
    }

    public struct Scale
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Scale(float x, float y, float z) { X = x; Y = y; Z = z; }
        public Scale(Vector3 xyz) { X = xyz.x; Y = xyz.y; Z = xyz.z; }

        public Vector3 GetScale() { return new Vector3(X, Y, Z); }
    }

    public string name { get; set; }
    public string parent { get; set; }
    public Position position { get; set; }
    public Rotation rotation { get; set; }
    public Scale scale { get; set; }
    public bool render { get; set; }

    public Things(string name, string parent, Position position, Rotation rotation, Scale scale, bool render = false)
    {
        this.name = name;
        this.parent = parent;
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
        this.render = render;
    }

    public override string ToString()
    {
        return "Name: " + name + "\n" +
                "Parent: " + parent + "\n" +
                "Position: " + position.GetPosition() + "\n" +
                "Rotation in Euler angle: " + rotation.GetRotation() + "\n" +
                "Scale: " + scale.GetScale() + "\n" +
                "Render? " + render;
    }
}

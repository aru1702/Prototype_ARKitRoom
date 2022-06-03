public class MyObject
{
    public string name { get; set; }
    public string parent { get; set; }
    public float length { get; set; }
    public float height { get; set; }
    public float width { get; set; }
    public string origin_type { get; set; }
    public string origin_descriptor { get; set; }
    public string prefab_type { get; set; }
    public string prefab_special { get; set; }
    public string comment { get; set; }

    public struct OriginType
    {
        public static string EDGE = "EDGE";
        public static string PLANECENTER = "PLANECENTER";
        public static string DIMENSIONCENTER = "DIMENSIONTCENTER";
    }

    public struct OriginDescriptor
    {
        public static string LUF = "LUF";
        public static string LUB = "LUB";
        public static string LDF = "LDF";
        public static string LDB = "LDB";
        public static string RUF = "RUF";
        public static string RUB = "RUB";
        public static string RDF = "RDF";
        public static string RDB = "RDB";

        public static string L = "L";
        public static string R = "R";
        public static string U = "U";
        public static string D = "D";
        public static string F = "F";
        public static string B = "B";
    }

    public struct PrefabType
    {
        public static string CUBE = "CUBE";
        public static string SPHERE = "SPHERE";
        public static string CYLINDER = "CYLINDER";
        public static string SPECIAL = "SPECIAL";
    }

    public struct MyObject_LHW
    {
        public float L { get; set; }
        public float H { get; set; }
        public float W { get; set; }
        public MyObject_LHW(float l, float h, float w) { L = l; H = h; W = w; }

        public MyObject_LHW Zero() { return new MyObject_LHW(0, 0, 0); }
    }

    public MyObject(string name, string parent, float length, float height, float width, string origin_type, string origin_descriptor, string prefab_type, string prefab_special, string comment = "")
    {
        this.name = name;
        this.parent = parent;
        this.length = length;
        this.height = height;
        this.width = width;
        this.origin_type = origin_type;
        this.origin_descriptor = origin_descriptor;
        this.prefab_type = prefab_type;
        this.prefab_special = prefab_special;
        this.comment = comment;
    }

    public MyObject(string name, string parent, float length, float height, float width)
    {
        this.name = name;
        this.parent = parent;
        this.length = length;
        this.height = height;
        this.width = width;
        prefab_type = PrefabType.CUBE;
        prefab_special = "none";
        origin_type = OriginType.DIMENSIONCENTER;
        origin_descriptor = "none";
        comment = "";
    }

    public override string ToString()
    {
        string returnStr = "Name: " + name + "\n" +
                            "Parent: " + parent + "\n" +
                            "Dimension (L,H,W): " + length + "," + height + "," + width + "," + "\n" +
                            "Origin: " + origin_type + " with " + origin_descriptor + "\n" +
                            "Prefab: " + prefab_type + " which if special: " + prefab_special + "\n" +
                            "Comment: " + comment;
        return returnStr;
    }
}
